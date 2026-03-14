/*
===========================================================================
The Nomad Framework
Copyright (C) 2025-2026 Noah Van Til

This Source Code Form is subject to the terms of the Mozilla Public
License, v2. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.

This software is provided "as is", without warranty of any kind,
express or implied, including but not limited to the warranties
of merchantability, fitness for a particular purpose and noninfringement.
===========================================================================
*/

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Nomad.SourceGenerators
{
    /// <summary>
    /// Generates engine wrapper classes from template definitions declared with <c>TemplateClass</c>.
    /// </summary>
    [Generator(LanguageNames.CSharp)]
    public sealed class EngineTemplateGenerator : IIncrementalGenerator
    {
        private const string TemplateClassMetadataName = "Nomad.EngineUtils.TemplateClass";
        private const string TemplatePropertyMetadataName = "Nomad.EngineUtils.TemplateProperty";
        private const string TemplateEventMetadataName = "Nomad.EngineUtils.TemplateEvent";
        private const string TemplateBaseClassMetadataName = "Nomad.EngineUtils.TemplateBaseClass";
        private const string TemplateNamespaceMetadataName = "Nomad.EngineUtils.TemplateNamespace";
        private const string GameObjectMetadataName = "Nomad.Core.EngineUtils.IGameObject";
        private const string GameEventMetadataName = "Nomad.Core.Events.IGameEvent`1";
        private static readonly SymbolDisplayFormat TypeDisplayFormat = new(
            globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included,
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
            miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes |
                                  SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier |
                                  SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers);

        private static readonly DiagnosticDescriptor MissingContractDiagnostic = new(
            id: "NOMADSG001",
            title: "Template contract is missing",
            messageFormat: "Template class '{0}' must specify TemplateClass.Contract",
            category: "Nomad.SourceGenerators",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        private static readonly DiagnosticDescriptor InvalidContractDiagnostic = new(
            id: "NOMADSG002",
            title: "Template contract must be an interface",
            messageFormat: "Template class '{0}' uses contract '{1}', but the contract must be an interface",
            category: "Nomad.SourceGenerators",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        private static readonly DiagnosticDescriptor MissingBaseTypeDiagnostic = new(
            id: "NOMADSG003",
            title: "Template base type is missing",
            messageFormat: "Template class '{0}' must inherit from a concrete engine type",
            category: "Nomad.SourceGenerators",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        private static readonly DiagnosticDescriptor UnboundPropertyDiagnostic = new(
            id: "NOMADSG004",
            title: "Property binding could not be inferred",
            messageFormat: "Could not infer an engine-side implementation for property '{0}' on template '{1}'. A throwing stub was generated.",
            category: "Nomad.SourceGenerators",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        private static readonly DiagnosticDescriptor UnknownTemplateMemberDiagnostic = new(
            id: "NOMADSG005",
            title: "Template member does not exist on the contract hierarchy",
            messageFormat: "Template member '{0}' was declared on template '{1}', but no matching member exists on the contract hierarchy",
            category: "Nomad.SourceGenerators",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        /// <inheritdoc />
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var templates = context.SyntaxProvider.ForAttributeWithMetadataName(
                TemplateClassMetadataName,
                static (node, _) => node is ClassDeclarationSyntax,
                static (syntaxContext, _) => CreateGeneratedSource(syntaxContext))
                .Where(static generated => generated is not null);

            context.RegisterSourceOutput(templates, static (productionContext, generated) =>
            {
                if (generated is null)
                {
                    return;
                }

                foreach (var diagnostic in generated.Diagnostics)
                {
                    productionContext.ReportDiagnostic(diagnostic);
                }

                if (!generated.CanGenerate)
                {
                    return;
                }

                productionContext.AddSource(
                    generated.HintName,
                    SourceText.From(generated.Source, Encoding.UTF8));
            });
        }

        private static GeneratedSource? CreateGeneratedSource(GeneratorAttributeSyntaxContext context)
        {
            if (context.TargetSymbol is not INamedTypeSymbol templateSymbol)
            {
                return null;
            }

            var diagnostics = new List<Diagnostic>();
            var location = templateSymbol.Locations.FirstOrDefault();

            var templateClassAttribute = FindAttribute(templateSymbol, TemplateClassMetadataName);
            if (templateClassAttribute is null)
            {
                return null;
            }

            if (FindAttribute(templateSymbol, TemplateBaseClassMetadataName) is not null)
            {
                return null;
            }

            var contractSymbol = GetNamedTypeArgument(templateClassAttribute, "Contract");
            if (contractSymbol is null)
            {
                diagnostics.Add(Diagnostic.Create(MissingContractDiagnostic, location, templateSymbol.Name));
                return GeneratedSource.Invalid(diagnostics);
            }

            if (contractSymbol.TypeKind != TypeKind.Interface)
            {
                diagnostics.Add(Diagnostic.Create(
                    InvalidContractDiagnostic,
                    location,
                    templateSymbol.Name,
                    contractSymbol.ToDisplayString(TypeDisplayFormat)));
                return GeneratedSource.Invalid(diagnostics);
            }

            if (templateSymbol.BaseType is null || templateSymbol.BaseType.SpecialType == SpecialType.System_Object)
            {
                diagnostics.Add(Diagnostic.Create(MissingBaseTypeDiagnostic, location, templateSymbol.Name));
                return GeneratedSource.Invalid(diagnostics);
            }

            var templateProperties = new Dictionary<string, TemplatePropertyDefinition>(StringComparer.Ordinal);
            var templateEvents = new Dictionary<string, TemplateEventDefinition>(StringComparer.Ordinal);
            CollectTemplateMetadata(templateSymbol, templateProperties, templateEvents);

            var interfaceHierarchy = new List<INamedTypeSymbol>();
            var visitedInterfaces = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
            CollectInterfaceHierarchy(contractSymbol, interfaceHierarchy, visitedInterfaces);

            var baseTemplateContracts = new List<INamedTypeSymbol>();
            CollectBaseTemplateContracts(
                templateSymbol.GetAttributes(),
                baseTemplateContracts,
                new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default),
                new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default));

            foreach (var baseTemplateContract in baseTemplateContracts)
            {
                CollectInterfaceHierarchy(baseTemplateContract, interfaceHierarchy, visitedInterfaces);
            }

            var additionalImplementedContracts = baseTemplateContracts
                .Where(baseTemplateContract =>
                    !SymbolEqualityComparer.Default.Equals(baseTemplateContract, contractSymbol) &&
                    !contractSymbol.AllInterfaces.Contains(baseTemplateContract, SymbolEqualityComparer.Default))
                .ToList();

            var propertyImplementations = new List<PropertyImplementation>();
            var eventImplementations = new List<EventImplementation>();
            var methodImplementations = new List<MethodImplementation>();
            var matchedTemplateMembers = new HashSet<string>(StringComparer.Ordinal);
            var seenProperties = new HashSet<string>(StringComparer.Ordinal);
            var seenMethods = new HashSet<string>(StringComparer.Ordinal);
            var seenEvents = new HashSet<string>(StringComparer.Ordinal);
            var eventPropertiesByName = new Dictionary<string, IPropertySymbol>(StringComparer.Ordinal);

            bool requiresSpacingThemeConstant = false;

            foreach (var interfaceSymbol in interfaceHierarchy)
            {
                if (interfaceSymbol.ToDisplayString() == "System.IDisposable")
                {
                    continue;
                }

                foreach (var member in interfaceSymbol.GetMembers())
                {
                    if (member is IPropertySymbol propertySymbol)
                    {
                        var propertyKey = interfaceSymbol.ToDisplayString() + "." + propertySymbol.Name;
                        if (!seenProperties.Add(propertyKey))
                        {
                            continue;
                        }

                        if (templateEvents.ContainsKey(propertySymbol.Name))
                        {
                            if (!eventPropertiesByName.ContainsKey(propertySymbol.Name))
                            {
                                eventPropertiesByName[propertySymbol.Name] = propertySymbol;
                            }

                            continue;
                        }

                        if (TryGetGameEventPayloadType(propertySymbol.Type, out _))
                        {
                            if (!eventPropertiesByName.ContainsKey(propertySymbol.Name))
                            {
                                eventPropertiesByName[propertySymbol.Name] = propertySymbol;
                            }

                            continue;
                        }

                        var propertyImplementation = CreatePropertyImplementation(
                            templateSymbol,
                            propertySymbol,
                            templateProperties,
                            context.SemanticModel.Compilation,
                            out var propertyRequiresSpacingThemeConstant);

                        requiresSpacingThemeConstant |= propertyRequiresSpacingThemeConstant;

                        if (propertyImplementation.Kind == PropertyImplementationKind.Generated)
                        {
                            matchedTemplateMembers.Add(propertySymbol.Name);
                        }
                        else
                        {
                            diagnostics.Add(Diagnostic.Create(
                                UnboundPropertyDiagnostic,
                                location,
                                propertySymbol.Name,
                                templateSymbol.Name));
                        }

                        propertyImplementations.Add(propertyImplementation);
                        continue;
                    }

                    if (member is not IMethodSymbol methodSymbol || methodSymbol.MethodKind != MethodKind.Ordinary)
                    {
                        continue;
                    }

                    if (methodSymbol.Name == "Dispose" && methodSymbol.Parameters.Length == 0)
                    {
                        continue;
                    }

                    var methodKey = methodSymbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
                    if (!seenMethods.Add(methodKey))
                    {
                        continue;
                    }

                    methodImplementations.Add(CreateMethodImplementation(methodSymbol));
                }
            }

            foreach (var templateEvent in templateEvents.Values)
            {
                if (!eventPropertiesByName.TryGetValue(templateEvent.Name, out var eventPropertySymbol))
                {
                    continue;
                }

                if (!seenEvents.Add(templateEvent.Name))
                {
                    continue;
                }

                eventImplementations.Add(CreateEventImplementation(eventPropertySymbol, templateEvent));
                matchedTemplateMembers.Add(templateEvent.Name);
            }

            foreach (var eventProperty in eventPropertiesByName.Values)
            {
                if (matchedTemplateMembers.Contains(eventProperty.Name))
                {
                    continue;
                }

                var propertyImplementation = CreateThrowingPropertyImplementation(eventProperty);
                propertyImplementations.Add(propertyImplementation);

                diagnostics.Add(Diagnostic.Create(
                    UnboundPropertyDiagnostic,
                    location,
                    eventProperty.Name,
                    templateSymbol.Name));
            }

            foreach (var templateProperty in templateProperties.Values)
            {
                if (!matchedTemplateMembers.Contains(templateProperty.Name))
                {
                    diagnostics.Add(Diagnostic.Create(
                        UnknownTemplateMemberDiagnostic,
                        location,
                        templateProperty.Name,
                        templateSymbol.Name));
                }
            }

            foreach (var templateEvent in templateEvents.Values)
            {
                if (!matchedTemplateMembers.Contains(templateEvent.Name))
                {
                    diagnostics.Add(Diagnostic.Create(
                        UnknownTemplateMemberDiagnostic,
                        location,
                        templateEvent.Name,
                        templateSymbol.Name));
                }
            }

            var generatedNamespace = ResolveGeneratedNamespace(contractSymbol, templateSymbol);
            var generatedClassName = ResolveGeneratedClassName(templateSymbol, contractSymbol);
            var generatedSource = RenderClass(
                generatedNamespace,
                generatedClassName,
                templateSymbol.DeclaredAccessibility,
                templateSymbol.BaseType,
                contractSymbol,
                additionalImplementedContracts,
                propertyImplementations,
                eventImplementations,
                methodImplementations,
                requiresSpacingThemeConstant);

            return new GeneratedSource(
                hintName: generatedNamespace.Replace('.', '_') + "_" + generatedClassName + ".g.cs",
                source: generatedSource,
                diagnostics: diagnostics.ToImmutableArray(),
                canGenerate: true);
        }

        private static void CollectTemplateMetadata(
            INamedTypeSymbol templateSymbol,
            Dictionary<string, TemplatePropertyDefinition> templateProperties,
            Dictionary<string, TemplateEventDefinition> templateEvents)
        {
            CollectTemplateMetadataFromAttributes(
                templateSymbol.GetAttributes(),
                templateProperties,
                templateEvents,
                new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default));
        }

        private static void CollectTemplateMetadataFromAttributes(
            ImmutableArray<AttributeData> attributes,
            Dictionary<string, TemplatePropertyDefinition> templateProperties,
            Dictionary<string, TemplateEventDefinition> templateEvents,
            HashSet<INamedTypeSymbol> visitedAttributeTypes)
        {
            foreach (var attribute in attributes)
            {
                var attributeClass = attribute.AttributeClass;
                if (attributeClass is null)
                {
                    continue;
                }

                var metadataName = attributeClass.ToDisplayString();
                if (metadataName == TemplatePropertyMetadataName)
                {
                    var name = GetNamedStringArgument(attribute, "Name");
                    if (!string.IsNullOrEmpty(name))
                    {
                        templateProperties[name!] = new TemplatePropertyDefinition(
                            name!,
                            GetNamedStringArgument(attribute, "FromEngineMethod"),
                            GetNamedStringArgument(attribute, "ToEngineMethod"));
                    }

                    continue;
                }

                if (metadataName == TemplateEventMetadataName)
                {
                    var name = GetNamedStringArgument(attribute, "Name");
                    var payloadType = GetNamedTypeArgument(attribute, "PayloadType");
                    if (!string.IsNullOrEmpty(name))
                    {
                        templateEvents[name!] = new TemplateEventDefinition(
                            name!,
                            payloadType?.ToDisplayString(TypeDisplayFormat));
                    }

                    continue;
                }

                if (metadataName == TemplateClassMetadataName || metadataName == "System.AttributeUsageAttribute")
                {
                    continue;
                }

                if (!visitedAttributeTypes.Add(attributeClass))
                {
                    continue;
                }

                CollectTemplateMetadataFromAttributes(
                    attributeClass.GetAttributes(),
                    templateProperties,
                    templateEvents,
                    visitedAttributeTypes);
            }
        }

        private static void CollectInterfaceHierarchy(
            INamedTypeSymbol interfaceSymbol,
            List<INamedTypeSymbol> orderedInterfaces,
            HashSet<INamedTypeSymbol> visitedInterfaces)
        {
            if (!visitedInterfaces.Add(interfaceSymbol))
            {
                return;
            }

            orderedInterfaces.Add(interfaceSymbol);

            foreach (var inheritedInterface in interfaceSymbol.Interfaces)
            {
                CollectInterfaceHierarchy(inheritedInterface, orderedInterfaces, visitedInterfaces);
            }
        }

        private static void CollectBaseTemplateContracts(
            ImmutableArray<AttributeData> attributes,
            List<INamedTypeSymbol> baseTemplateContracts,
            HashSet<INamedTypeSymbol> visitedAttributeTypes,
            HashSet<INamedTypeSymbol> visitedContracts)
        {
            foreach (var attribute in attributes)
            {
                var attributeClass = attribute.AttributeClass;
                if (attributeClass is null || !visitedAttributeTypes.Add(attributeClass))
                {
                    continue;
                }

                if (FindAttribute(attributeClass, TemplateBaseClassMetadataName) is not null)
                {
                    var templateClassAttribute = FindAttribute(attributeClass, TemplateClassMetadataName);
                    var contractSymbol = templateClassAttribute is null
                        ? null
                        : GetNamedTypeArgument(templateClassAttribute, "Contract");

                    if (contractSymbol is not null &&
                        contractSymbol.TypeKind == TypeKind.Interface &&
                        visitedContracts.Add(contractSymbol))
                    {
                        baseTemplateContracts.Add(contractSymbol);
                    }
                }

                CollectBaseTemplateContracts(
                    attributeClass.GetAttributes(),
                    baseTemplateContracts,
                    visitedAttributeTypes,
                    visitedContracts);
            }
        }

        private static PropertyImplementation CreatePropertyImplementation(
            INamedTypeSymbol templateSymbol,
            IPropertySymbol propertySymbol,
            IReadOnlyDictionary<string, TemplatePropertyDefinition> templateProperties,
            Compilation compilation,
            out bool requiresSpacingThemeConstant)
        {
            requiresSpacingThemeConstant = false;

            var interfaceTypeName = propertySymbol.ContainingType.ToDisplayString(TypeDisplayFormat);
            var propertyTypeName = propertySymbol.Type.ToDisplayString(TypeDisplayFormat);

            if (propertySymbol.ContainingType.ToDisplayString() == GameObjectMetadataName)
            {
                return new PropertyImplementation(
                    PropertyImplementationKind.Generated,
                    interfaceTypeName,
                    propertyTypeName,
                    propertySymbol.Name,
                    getterExpression: "_impl." + propertySymbol.Name,
                    setterExpression: propertySymbol.SetMethod is null ? null : "_impl." + propertySymbol.Name + " = value");
            }

            if (TryCreateSpecialPropertyImplementation(propertySymbol, out var specialImplementation, out requiresSpacingThemeConstant))
            {
                return specialImplementation;
            }

            templateProperties.TryGetValue(propertySymbol.Name, out var templateProperty);

            var engineMember = FindEngineProperty(templateSymbol.BaseType, propertySymbol.Name);
            if (engineMember is not null)
            {
                return new PropertyImplementation(
                    PropertyImplementationKind.Generated,
                    interfaceTypeName,
                    propertyTypeName,
                    propertySymbol.Name,
                    getterExpression: BuildGetterExpression(
                        propertySymbol,
                        engineMember.Name,
                        engineMember.Type,
                        templateProperty?.FromEngineMethod,
                        compilation),
                    setterExpression: propertySymbol.SetMethod is null ? null : BuildSetterExpression(
                        propertySymbol,
                        engineMember.Name,
                        engineMember.Type,
                        templateProperty?.ToEngineMethod,
                        compilation));
            }

            return CreateThrowingPropertyImplementation(propertySymbol);
        }

        private static bool TryCreateSpecialPropertyImplementation(
            IPropertySymbol propertySymbol,
            out PropertyImplementation implementation,
            out bool requiresSpacingThemeConstant)
        {
            implementation = default!;
            requiresSpacingThemeConstant = false;

            var interfaceTypeName = propertySymbol.ContainingType.ToDisplayString(TypeDisplayFormat);
            var propertyTypeName = propertySymbol.Type.ToDisplayString(TypeDisplayFormat);

            if (propertySymbol.Name == "Color")
            {
                implementation = new PropertyImplementation(
                    PropertyImplementationKind.Generated,
                    interfaceTypeName,
                    propertyTypeName,
                    propertySymbol.Name,
                    getterExpression: "Modulate.ToSystem()",
                    setterExpression: propertySymbol.SetMethod is null ? null : "Modulate = value.ToGodot()");
                return true;
            }

            if (propertySymbol.Name == "Alignment")
            {
                implementation = new PropertyImplementation(
                    PropertyImplementationKind.Generated,
                    interfaceTypeName,
                    propertyTypeName,
                    propertySymbol.Name,
                    getterExpression: "HorizontalAlignment.ToNomad()",
                    setterExpression: propertySymbol.SetMethod is null ? null : "HorizontalAlignment = value.ToGodot()");
                return true;
            }

            if (propertySymbol.Name == "Spacing")
            {
                implementation = new PropertyImplementation(
                    PropertyImplementationKind.Generated,
                    interfaceTypeName,
                    propertyTypeName,
                    propertySymbol.Name,
                    getterExpression: "GetThemeConstant(SeparationThemeConstantName)",
                    setterExpression: propertySymbol.SetMethod is null ? null : "AddThemeConstantOverride(SeparationThemeConstantName, (int)value)");
                requiresSpacingThemeConstant = true;
                return true;
            }

            return false;
        }

        private static PropertyImplementation CreateThrowingPropertyImplementation(IPropertySymbol propertySymbol)
        {
            var interfaceTypeName = propertySymbol.ContainingType.ToDisplayString(TypeDisplayFormat);
            var propertyTypeName = propertySymbol.Type.ToDisplayString(TypeDisplayFormat);

            return new PropertyImplementation(
                PropertyImplementationKind.ThrowingStub,
                interfaceTypeName,
                propertyTypeName,
                propertySymbol.Name,
                getterExpression: "throw new global::System.NotSupportedException(\"The engine binding for this property was not generated.\")",
                setterExpression: propertySymbol.SetMethod is null ? null : "throw new global::System.NotSupportedException(\"The engine binding for this property was not generated.\")");
        }

        private static EventImplementation CreateEventImplementation(
            IPropertySymbol propertySymbol,
            TemplateEventDefinition templateEvent)
        {
            string payloadTypeName;
            if (TryGetGameEventPayloadType(propertySymbol.Type, out var payloadType))
            {
                payloadTypeName = payloadType.ToDisplayString(TypeDisplayFormat);
            }
            else if (!string.IsNullOrWhiteSpace(templateEvent.PayloadTypeName))
            {
                payloadTypeName = templateEvent.PayloadTypeName!;
            }
            else
            {
                payloadTypeName = "global::System.Object";
            }

            var fieldName = "_" + ToCamelCase(propertySymbol.Name);
            var interfaceTypeName = propertySymbol.ContainingType.ToDisplayString(TypeDisplayFormat);

            var registryKeyConstantName = string.Concat(
                "Nomad.EngineUtils.Constants.Events.",
                ToScreamingSnakeCase(TrimInterfacePrefix(propertySymbol.ContainingType.Name)),
                "_",
                ToScreamingSnakeCase(propertySymbol.Name));

            return new EventImplementation(
                interfaceTypeName,
                propertySymbol.Name,
                payloadTypeName,
                fieldName,
                registryKeyConstantName,
                BuildEventHookStatements(templateEvent.Name, fieldName, payloadTypeName));
        }

        private static MethodImplementation CreateMethodImplementation(IMethodSymbol methodSymbol)
        {
            var returnTypeName = methodSymbol.ReturnType.ToDisplayString(TypeDisplayFormat);
            var typeParameters = methodSymbol.TypeParameters.Length == 0
                ? string.Empty
                : "<" + string.Join(", ", methodSymbol.TypeParameters.Select(typeParameter => typeParameter.Name)) + ">";

            var parameters = string.Join(", ", methodSymbol.Parameters.Select(FormatParameter));
            var constraints = BuildConstraintClauses(methodSymbol);
            var callArguments = string.Join(", ", methodSymbol.Parameters.Select(parameter => parameter.Name));
            var invocation = "_impl." + methodSymbol.Name + typeParameters + "(" + callArguments + ")";

            return new MethodImplementation(
                signature: "public " + returnTypeName + " " + methodSymbol.Name + typeParameters + "(" + parameters + ")",
                constraints: constraints,
                body: methodSymbol.ReturnsVoid ? invocation + ";" : "return " + invocation + ";");
        }

        private static bool TryGetGameEventPayloadType(ITypeSymbol typeSymbol, out ITypeSymbol payloadType)
        {
            if (typeSymbol is INamedTypeSymbol namedType &&
                namedType.Name == "IGameEvent" &&
                namedType.Arity == 1 &&
                namedType.ContainingNamespace.ToDisplayString() == "Nomad.Core.Events")
            {
                payloadType = namedType.TypeArguments[0];
                return true;
            }

            payloadType = null!;
            return false;
        }

        private static IPropertySymbol? FindEngineProperty(INamedTypeSymbol? typeSymbol, string propertyName)
        {
            var current = typeSymbol;
            while (current is not null)
            {
                foreach (var member in current.GetMembers(propertyName))
                {
                    if (member is IPropertySymbol propertySymbol && !propertySymbol.IsStatic)
                    {
                        return propertySymbol;
                    }
                }

                current = current.BaseType;
            }

            return null;
        }

        private static string BuildGetterExpression(
            IPropertySymbol interfaceProperty,
            string engineMemberName,
            ITypeSymbol engineMemberType,
            string? fromEngineMethod,
            Compilation compilation)
        {
            if (!string.IsNullOrEmpty(fromEngineMethod))
            {
                return engineMemberName + "." + fromEngineMethod + "()";
            }

            return ConvertExpression(engineMemberName, engineMemberType, interfaceProperty.Type, compilation);
        }

        private static string BuildSetterExpression(
            IPropertySymbol interfaceProperty,
            string engineMemberName,
            ITypeSymbol engineMemberType,
            string? toEngineMethod,
            Compilation compilation)
        {
            if (!string.IsNullOrEmpty(toEngineMethod))
            {
                return engineMemberName + " = value." + toEngineMethod + "()";
            }

            return engineMemberName + " = " + ConvertExpression("value", interfaceProperty.Type, engineMemberType, compilation);
        }

        private static string ConvertExpression(
            string expression,
            ITypeSymbol sourceType,
            ITypeSymbol destinationType,
            Compilation compilation)
        {
            if (SymbolEqualityComparer.Default.Equals(sourceType, destinationType))
            {
                return expression;
            }

            if (compilation is not CSharpCompilation csharpCompilation)
            {
                return expression;
            }

            var conversion = csharpCompilation.ClassifyConversion(sourceType, destinationType);
            if (conversion.IsImplicit)
            {
                return expression;
            }

            if (conversion.Exists)
            {
                return "(" + destinationType.ToDisplayString(TypeDisplayFormat) + ")" + expression;
            }

            return expression;
        }

        private static ImmutableArray<string> BuildEventHookStatements(string eventName, string fieldName, string payloadTypeName)
        {
            if (eventName == "Focused")
            {
                return ImmutableArray.Create(
                    "FocusEntered += () => " + fieldName + ".Publish(default);",
                    "MouseEntered += () => " + fieldName + ".Publish(default);");
            }

            if (eventName == "Unfocused")
            {
                return ImmutableArray.Create(
                    "FocusExited += () => " + fieldName + ".Publish(default);",
                    "MouseExited += () => " + fieldName + ".Publish(default);");
            }

            if (eventName == "Clicked")
            {
                return ImmutableArray.Create("Pressed += () => " + fieldName + ".Publish(default);");
            }

            if (eventName == "ValueChanged")
            {
                if (payloadTypeName == "float" || payloadTypeName == "global::System.Single")
                {
                    return ImmutableArray.Create("ValueChanged += (value) => " + fieldName + ".Publish((float)value);");
                }

                return ImmutableArray.Create("ValueChanged += (value) => " + fieldName + ".Publish(value);");
            }

            return ImmutableArray<string>.Empty;
        }

        private static string RenderClass(
            string generatedNamespace,
            string generatedClassName,
            Accessibility accessibility,
            INamedTypeSymbol baseType,
            INamedTypeSymbol contractType,
            IReadOnlyList<INamedTypeSymbol> additionalImplementedContracts,
            IReadOnlyList<PropertyImplementation> propertyImplementations,
            IReadOnlyList<EventImplementation> eventImplementations,
            IReadOnlyList<MethodImplementation> methodImplementations,
            bool requiresSpacingThemeConstant)
        {
            var builder = new StringBuilder();
            builder.AppendLine("/*");
            builder.AppendLine("===========================================================================");
            builder.AppendLine("The Nomad Framework");
            builder.AppendLine("Copyright (C) 2025-2026 Noah Van Til");
            builder.AppendLine();
            builder.AppendLine("This Source Code Form is subject to the terms of the Mozilla Public");
            builder.AppendLine("License, v2. If a copy of the MPL was not distributed with this");
            builder.AppendLine("file, You can obtain one at https://mozilla.org/MPL/2.0/.");
            builder.AppendLine();
            builder.AppendLine("This software is provided \"as is\", without warranty of any kind,");
            builder.AppendLine("express or implied, including but not limited to the warranties");
            builder.AppendLine("of merchantability, fitness for a particular purpose and noninfringement.");
            builder.AppendLine("===========================================================================");
            builder.AppendLine("*/");
            builder.AppendLine();
            builder.AppendLine("namespace " + generatedNamespace);
            builder.AppendLine("{");
            builder.AppendLine("    /// <summary>");
            builder.AppendLine("    ///");
            builder.AppendLine("    /// </summary>");
            builder.Append("    ");
            builder.Append(FormatAccessibility(accessibility));
            builder.Append(" partial class ");
            builder.Append(generatedClassName);
            builder.Append(" : ");
            builder.Append(baseType.ToDisplayString(TypeDisplayFormat));
            builder.Append(", ");
            builder.Append(contractType.ToDisplayString(TypeDisplayFormat));

            foreach (var additionalImplementedContract in additionalImplementedContracts)
            {
                builder.Append(", ");
                builder.Append(additionalImplementedContract.ToDisplayString(TypeDisplayFormat));
            }

            builder.AppendLine();
            builder.AppendLine("    {");

            if (requiresSpacingThemeConstant)
            {
                builder.AppendLine("        private static readonly global::Godot.StringName SeparationThemeConstantName = \"separation\";");
                builder.AppendLine();
            }

            foreach (var propertyImplementation in propertyImplementations)
            {
                builder.AppendLine("        /// <summary>");
                builder.AppendLine("        ///");
                builder.AppendLine("        /// </summary>");
                builder.AppendLine("        " + propertyImplementation.PropertyTypeName + " " + propertyImplementation.InterfaceTypeName + "." + propertyImplementation.PropertyName);
                builder.AppendLine("        {");
                builder.AppendLine("            get => " + propertyImplementation.GetterExpression + ";");

                if (propertyImplementation.SetterExpression is not null)
                {
                    builder.AppendLine("            set => " + propertyImplementation.SetterExpression + ";");
                }

                builder.AppendLine("        }");
                builder.AppendLine();
            }

            builder.AppendLine("        private readonly global::Nomad.EngineUtils.GodotGameObject _impl;");
            builder.AppendLine();

            foreach (var eventImplementation in eventImplementations)
            {
                builder.AppendLine("        /// <summary>");
                builder.AppendLine("        ///");
                builder.AppendLine("        /// </summary>");
                builder.AppendLine("        global::Nomad.Core.Events.IGameEvent<" + eventImplementation.PayloadTypeName + "> " + eventImplementation.InterfaceTypeName + "." + eventImplementation.EventName + " => " + eventImplementation.FieldName + ";");
                builder.AppendLine("        private global::Nomad.Core.Events.IGameEvent<" + eventImplementation.PayloadTypeName + "> " + eventImplementation.FieldName + ";");
                builder.AppendLine();
            }

            builder.AppendLine("        /// <summary>");
            builder.AppendLine("        ///");
            builder.AppendLine("        /// </summary>");
            builder.AppendLine("        public " + generatedClassName + "()");
            builder.AppendLine("        {");
            builder.AppendLine("            _impl = new global::Nomad.EngineUtils.GodotGameObject(this);");
            builder.AppendLine("        }");
            builder.AppendLine();

            builder.AppendLine("        /// <summary>");
            builder.AppendLine("        ///");
            builder.AppendLine("        /// </summary>");
            builder.AppendLine("        public sealed override void _Ready()");
            builder.AppendLine("        {");
            builder.AppendLine("            base._Ready();");
            builder.AppendLine();

            foreach (var eventImplementation in eventImplementations)
            {
                builder.AppendLine("            " + eventImplementation.FieldName + " = global::Nomad.Events.Globals.GameEventRegistry.GetEvent<" + eventImplementation.PayloadTypeName + ">($\"{GetHashCode()}:{" + eventImplementation.RegistryKeyConstantName + "}\", global::Nomad.EngineUtils.Constants.Events.NAMESPACE);");
            }

            if (eventImplementations.Count > 0)
            {
                builder.AppendLine();
            }

            foreach (var eventImplementation in eventImplementations)
            {
                foreach (var hookStatement in eventImplementation.HookStatements)
                {
                    builder.AppendLine("            " + hookStatement);
                }
            }

            if (eventImplementations.Count > 0)
            {
                builder.AppendLine();
            }

            builder.AppendLine("            _impl.OnInit();");
            builder.AppendLine("            OnInit();");
            builder.AppendLine("        }");
            builder.AppendLine();

            builder.AppendLine("        /// <summary>");
            builder.AppendLine("        ///");
            builder.AppendLine("        /// </summary>");
            builder.AppendLine("        /// <param name=\"delta\"></param>");
            builder.AppendLine("        public sealed override void _Process(double delta)");
            builder.AppendLine("        {");
            builder.AppendLine("            base._Process(delta);");
            builder.AppendLine();
            builder.AppendLine("            float deltaTime = (float)delta;");
            builder.AppendLine("            _impl.OnUpdate(deltaTime);");
            builder.AppendLine("            OnUpdate(deltaTime);");
            builder.AppendLine("        }");
            builder.AppendLine();

            builder.AppendLine("        /// <summary>");
            builder.AppendLine("        ///");
            builder.AppendLine("        /// </summary>");
            builder.AppendLine("        /// <param name=\"delta\"></param>");
            builder.AppendLine("        public sealed override void _PhysicsProcess(double delta)");
            builder.AppendLine("        {");
            builder.AppendLine("            base._PhysicsProcess(delta);");
            builder.AppendLine();
            builder.AppendLine("            float deltaTime = (float)delta;");
            builder.AppendLine("            _impl.OnPhysicsUpdate(deltaTime);");
            builder.AppendLine("            OnPhysicsUpdate(deltaTime);");
            builder.AppendLine("        }");
            builder.AppendLine();

            builder.AppendLine("        /// <summary>");
            builder.AppendLine("        ///");
            builder.AppendLine("        /// </summary>");
            builder.AppendLine("        public sealed override void _ExitTree()");
            builder.AppendLine("        {");
            builder.AppendLine("            base._ExitTree();");
            builder.AppendLine();
            builder.AppendLine("            _impl.OnShutdown();");
            builder.AppendLine("            OnShutdown();");

            if (eventImplementations.Count > 0)
            {
                builder.AppendLine();

                foreach (var eventImplementation in eventImplementations)
                {
                    builder.AppendLine("            " + eventImplementation.FieldName + "?.Dispose();");
                }
            }

            builder.AppendLine("        }");
            builder.AppendLine();

            builder.AppendLine("        /// <summary>");
            builder.AppendLine("        ///");
            builder.AppendLine("        /// </summary>");
            builder.AppendLine("        protected virtual void OnInit()");
            builder.AppendLine("        {");
            builder.AppendLine("        }");
            builder.AppendLine();

            builder.AppendLine("        /// <summary>");
            builder.AppendLine("        ///");
            builder.AppendLine("        /// </summary>");
            builder.AppendLine("        protected virtual void OnShutdown()");
            builder.AppendLine("        {");
            builder.AppendLine("        }");
            builder.AppendLine();

            builder.AppendLine("        /// <summary>");
            builder.AppendLine("        ///");
            builder.AppendLine("        /// </summary>");
            builder.AppendLine("        /// <param name=\"delta\"></param>");
            builder.AppendLine("        protected virtual void OnUpdate(float delta)");
            builder.AppendLine("        {");
            builder.AppendLine("        }");
            builder.AppendLine();

            builder.AppendLine("        /// <summary>");
            builder.AppendLine("        ///");
            builder.AppendLine("        /// </summary>");
            builder.AppendLine("        /// <param name=\"delta\"></param>");
            builder.AppendLine("        protected virtual void OnPhysicsUpdate(float delta)");
            builder.AppendLine("        {");
            builder.AppendLine("        }");
            builder.AppendLine();

            foreach (var methodImplementation in methodImplementations)
            {
                builder.AppendLine("        /// <summary>");
                builder.AppendLine("        ///");
                builder.AppendLine("        /// </summary>");
                builder.AppendLine("        " + methodImplementation.Signature);

                foreach (var constraintClause in methodImplementation.Constraints)
                {
                    builder.AppendLine("            " + constraintClause);
                }

                builder.AppendLine("        {");
                builder.AppendLine("            " + methodImplementation.Body);
                builder.AppendLine("        }");
                builder.AppendLine();
            }

            builder.AppendLine("    }");
            builder.AppendLine("}");

            return builder.ToString();
        }

        private static string ResolveGeneratedNamespace(INamedTypeSymbol contractSymbol, INamedTypeSymbol templateSymbol)
        {
            var templateNamespaceAttribute = FindAttribute(templateSymbol, TemplateNamespaceMetadataName);
            var configuredNamespace = templateNamespaceAttribute is null
                ? null
                : GetNamedStringArgument(templateNamespaceAttribute, "Name");

            if (!string.IsNullOrWhiteSpace(configuredNamespace))
            {
                var trimmedNamespace = configuredNamespace!.Trim().Trim('.');
                if (trimmedNamespace.Equals("Nomad.EngineUtils", StringComparison.Ordinal) ||
                    trimmedNamespace.StartsWith("Nomad.EngineUtils.", StringComparison.Ordinal))
                {
                    return trimmedNamespace;
                }

                return "Nomad.EngineUtils." + trimmedNamespace;
            }

            var contractNamespace = contractSymbol.ContainingNamespace.ToDisplayString();
            if (contractNamespace.IndexOf(".Core.", StringComparison.Ordinal) >= 0)
            {
                return contractNamespace.Replace(".Core.", ".");
            }

            if (contractNamespace.StartsWith("Nomad.Core.", StringComparison.Ordinal))
            {
                return "Nomad." + contractNamespace.Substring("Nomad.Core.".Length);
            }

            var templateNamespace = templateSymbol.ContainingNamespace.ToDisplayString();
            if (templateNamespace.EndsWith(".Templates", StringComparison.Ordinal))
            {
                return templateNamespace.Substring(0, templateNamespace.Length - ".Templates".Length);
            }

            return templateNamespace;
        }

        private static string ResolveGeneratedClassName(INamedTypeSymbol templateSymbol, INamedTypeSymbol contractSymbol)
        {
            if (templateSymbol.Name.StartsWith("Engine", StringComparison.Ordinal))
            {
                return templateSymbol.Name;
            }

            return "Engine" + TrimInterfacePrefix(contractSymbol.Name);
        }

        private static AttributeData? FindAttribute(ISymbol symbol, string metadataName)
            => symbol.GetAttributes()
                .FirstOrDefault(attribute => attribute.AttributeClass?.ToDisplayString() == metadataName);

        private static INamedTypeSymbol? GetNamedTypeArgument(AttributeData attribute, string argumentName)
        {
            foreach (var namedArgument in attribute.NamedArguments)
            {
                if (namedArgument.Key == argumentName &&
                    namedArgument.Value.Kind == TypedConstantKind.Type &&
                    namedArgument.Value.Value is INamedTypeSymbol typeSymbol)
                {
                    return typeSymbol;
                }
            }

            return null;
        }

        private static string? GetNamedStringArgument(AttributeData attribute, string argumentName)
        {
            foreach (var namedArgument in attribute.NamedArguments)
            {
                if (namedArgument.Key == argumentName &&
                    namedArgument.Value.Kind == TypedConstantKind.Primitive &&
                    namedArgument.Value.Value is string stringValue)
                {
                    return stringValue;
                }
            }

            return null;
        }

        private static string FormatAccessibility(Accessibility accessibility)
            => accessibility switch
            {
                Accessibility.Public => "public",
                Accessibility.Internal => "internal",
                Accessibility.Private => "private",
                Accessibility.Protected => "protected",
                Accessibility.ProtectedAndInternal => "private protected",
                Accessibility.ProtectedOrInternal => "protected internal",
                _ => "internal"
            };

        private static string FormatParameter(IParameterSymbol parameter)
        {
            var builder = new StringBuilder();

            if (parameter.IsParams)
            {
                builder.Append("params ");
            }

            if (parameter.RefKind == RefKind.Ref)
            {
                builder.Append("ref ");
            }
            else if (parameter.RefKind == RefKind.Out)
            {
                builder.Append("out ");
            }
            else if (parameter.RefKind == RefKind.In)
            {
                builder.Append("in ");
            }

            builder.Append(parameter.Type.ToDisplayString(TypeDisplayFormat));
            builder.Append(' ');
            builder.Append(parameter.Name);

            if (parameter.HasExplicitDefaultValue)
            {
                builder.Append(" = ");
                builder.Append(FormatDefaultValue(parameter.ExplicitDefaultValue));
            }

            return builder.ToString();
        }

        private static string FormatDefaultValue(object? defaultValue)
        {
            if (defaultValue is null)
            {
                return "null";
            }

            return defaultValue switch
            {
                string stringValue => "\"" + stringValue.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"",
                char charValue => "'" + charValue.ToString().Replace("\\", "\\\\").Replace("'", "\\'") + "'",
                bool boolValue => boolValue ? "true" : "false",
                float floatValue => floatValue.ToString(CultureInfo.InvariantCulture) + "F",
                double doubleValue => doubleValue.ToString(CultureInfo.InvariantCulture) + "D",
                decimal decimalValue => decimalValue.ToString(CultureInfo.InvariantCulture) + "M",
                _ => Convert.ToString(defaultValue, CultureInfo.InvariantCulture) ?? "default"
            };
        }

        private static ImmutableArray<string> BuildConstraintClauses(IMethodSymbol methodSymbol)
        {
            var builder = ImmutableArray.CreateBuilder<string>();

            foreach (var typeParameter in methodSymbol.TypeParameters)
            {
                var constraints = new List<string>();

                if (typeParameter.HasReferenceTypeConstraint)
                {
                    constraints.Add("class");
                }

                if (typeParameter.HasValueTypeConstraint)
                {
                    constraints.Add("struct");
                }

                if (typeParameter.HasUnmanagedTypeConstraint)
                {
                    constraints.Add("unmanaged");
                }

                if (typeParameter.HasNotNullConstraint)
                {
                    constraints.Add("notnull");
                }

                foreach (var constraintType in typeParameter.ConstraintTypes)
                {
                    constraints.Add(constraintType.ToDisplayString(TypeDisplayFormat));
                }

                if (typeParameter.HasConstructorConstraint)
                {
                    constraints.Add("new()");
                }

                if (constraints.Count > 0)
                {
                    builder.Add("where " + typeParameter.Name + " : " + string.Join(", ", constraints));
                }
            }

            return builder.ToImmutable();
        }

        private static string TrimInterfacePrefix(string interfaceName)
        {
            if (interfaceName.Length > 1 &&
                interfaceName[0] == 'I' &&
                char.IsUpper(interfaceName[1]))
            {
                return interfaceName.Substring(1);
            }

            return interfaceName;
        }

        private static string ToCamelCase(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            if (value.Length == 1)
            {
                return char.ToLowerInvariant(value[0]).ToString();
            }

            return char.ToLowerInvariant(value[0]) + value.Substring(1);
        }

        private static string ToScreamingSnakeCase(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            var builder = new StringBuilder(value.Length * 2);

            for (int i = 0; i < value.Length; i++)
            {
                var current = value[i];

                if (i > 0 && char.IsUpper(current))
                {
                    var previous = value[i - 1];
                    var nextIsLower = i + 1 < value.Length && char.IsLower(value[i + 1]);
                    if (char.IsLower(previous) || nextIsLower)
                    {
                        builder.Append('_');
                    }
                }

                builder.Append(char.ToUpperInvariant(current));
            }

            return builder.ToString();
        }

        private sealed class GeneratedSource
        {
            public GeneratedSource(string hintName, string source, ImmutableArray<Diagnostic> diagnostics, bool canGenerate)
            {
                HintName = hintName;
                Source = source;
                Diagnostics = diagnostics;
                CanGenerate = canGenerate;
            }

            public string HintName { get; }

            public string Source { get; }

            public ImmutableArray<Diagnostic> Diagnostics { get; }

            public bool CanGenerate { get; }

            public static GeneratedSource Invalid(IEnumerable<Diagnostic> diagnostics)
                => new(string.Empty, string.Empty, diagnostics.ToImmutableArray(), false);
        }

        private sealed class TemplatePropertyDefinition
        {
            public TemplatePropertyDefinition(string name, string? fromEngineMethod, string? toEngineMethod)
            {
                Name = name;
                FromEngineMethod = fromEngineMethod;
                ToEngineMethod = toEngineMethod;
            }

            public string Name { get; }

            public string? FromEngineMethod { get; }

            public string? ToEngineMethod { get; }
        }

        private sealed class TemplateEventDefinition
        {
            public TemplateEventDefinition(string name, string? payloadTypeName)
            {
                Name = name;
                PayloadTypeName = payloadTypeName;
            }

            public string Name { get; }

            public string? PayloadTypeName { get; }
        }

        private sealed class PropertyImplementation
        {
            public PropertyImplementation(
                PropertyImplementationKind kind,
                string interfaceTypeName,
                string propertyTypeName,
                string propertyName,
                string getterExpression,
                string? setterExpression)
            {
                Kind = kind;
                InterfaceTypeName = interfaceTypeName;
                PropertyTypeName = propertyTypeName;
                PropertyName = propertyName;
                GetterExpression = getterExpression;
                SetterExpression = setterExpression;
            }

            public PropertyImplementationKind Kind { get; }

            public string InterfaceTypeName { get; }

            public string PropertyTypeName { get; }

            public string PropertyName { get; }

            public string GetterExpression { get; }

            public string? SetterExpression { get; }
        }

        private sealed class EventImplementation
        {
            public EventImplementation(
                string interfaceTypeName,
                string eventName,
                string payloadTypeName,
                string fieldName,
                string registryKeyConstantName,
                ImmutableArray<string> hookStatements)
            {
                InterfaceTypeName = interfaceTypeName;
                EventName = eventName;
                PayloadTypeName = payloadTypeName;
                FieldName = fieldName;
                RegistryKeyConstantName = registryKeyConstantName;
                HookStatements = hookStatements;
            }

            public string InterfaceTypeName { get; }

            public string EventName { get; }

            public string PayloadTypeName { get; }

            public string FieldName { get; }

            public string RegistryKeyConstantName { get; }

            public ImmutableArray<string> HookStatements { get; }
        }

        private sealed class MethodImplementation
        {
            public MethodImplementation(string signature, ImmutableArray<string> constraints, string body)
            {
                Signature = signature;
                Constraints = constraints;
                Body = body;
            }

            public string Signature { get; }

            public ImmutableArray<string> Constraints { get; }

            public string Body { get; }
        }

        private enum PropertyImplementationKind
        {
            Generated,
            ThrowingStub
        }
    }
}
