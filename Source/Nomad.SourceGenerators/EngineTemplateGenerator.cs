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
using System.Xml.Linq;
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
        private const string TemplateClassMetadataName = "Nomad.EngineTemplates.Attributes.TemplateClass";
        private const string TemplatePropertyMetadataName = "Nomad.EngineTemplates.Attributes.TemplateProperty";
        private const string TemplateEventMetadataName = "Nomad.EngineTemplates.Attributes.TemplateEvent";
        private const string TemplateMethodMetadataName = "Nomad.EngineTemplates.Attributes.TemplateMethod";
        private const string TemplateConstantMetadataName = "Nomad.EngineTemplates.Attributes.TemplateConstant";
        private const string TemplateTypeConversionMetadataName = "Nomad.EngineTemplates.Attributes.TemplateTypeConversion";
        private const string TemplateBaseClassMetadataName = "Nomad.EngineTemplates.Attributes.TemplateBaseClass";
        private const string TemplateNamespaceMetadataName = "Nomad.EngineTemplates.Attributes.TemplateNamespace";
        private const string GameObjectMetadataName = "Nomad.Core.Scene.GameObjects.IGameObject";
        private const string GameEventMetadataName = "Nomad.Core.Events.IGameEvent`1";
        private const string ValuePlaceholder = "{{value}}";
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
            messageFormat: "Template class '{0}' must specify TemplateClass.{1} for {2} generation",
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

        private static readonly DiagnosticDescriptor InvalidBaseTypeDiagnostic = new(
            id: "NOMADSG006",
            title: "Configured engine base type could not be resolved",
            messageFormat: "Template class '{0}' configured base type '{1}', but that type could not be resolved in the current {2} compilation",
            category: "Nomad.SourceGenerators",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        private static readonly DiagnosticDescriptor UnboundMethodDiagnostic = new(
            id: "NOMADSG007",
            title: "Method binding could not be inferred",
            messageFormat: "Could not infer an engine-side implementation for method '{0}' on template '{1}'. A throwing stub was generated.",
            category: "Nomad.SourceGenerators",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        private static readonly DiagnosticDescriptor UnboundConstantDiagnostic = new(
            id: "NOMADSG008",
            title: "Constant binding could not be inferred",
            messageFormat: "Could not infer a generated value for constant '{0}' on template '{1}'. The member was skipped.",
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
                    SourceText.From(generated.Source, System.Text.Encoding.UTF8));
            });
        }

        private static GeneratedSource? CreateGeneratedSource(GeneratorAttributeSyntaxContext context)
        {
            if (context.TargetSymbol is not INamedTypeSymbol templateSymbol)
            {
                return null;
            }

            var templateClassAttribute = FindAttribute(templateSymbol, TemplateClassMetadataName);
            if (templateClassAttribute is null)
            {
                return null;
            }

            if (FindAttribute(templateSymbol, TemplateBaseClassMetadataName) is not null)
            {
                return null;
            }

            var compilation = context.SemanticModel.Compilation;
            var engineProjectKind = DetectEngineProjectKind(compilation);
            if (engineProjectKind == EngineProjectKind.Unknown)
            {
                return null;
            }

            var diagnostics = new List<Diagnostic>();
            var location = templateSymbol.Locations.FirstOrDefault();

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

            var configuredBaseTypeName = ResolveConfiguredBaseTypeName(templateClassAttribute, engineProjectKind);
            if (string.IsNullOrWhiteSpace(configuredBaseTypeName))
            {
                diagnostics.Add(Diagnostic.Create(
                    MissingBaseTypeDiagnostic,
                    location,
                    templateSymbol.Name,
                    engineProjectKind == EngineProjectKind.Godot ? "GodotBase" : "UnityBase",
                    engineProjectKind.ToString()));
                return GeneratedSource.Invalid(diagnostics);
            }

            var baseTypeSymbol = ResolveBaseTypeSymbol(compilation, configuredBaseTypeName!);
            if (baseTypeSymbol is null)
            {
                diagnostics.Add(Diagnostic.Create(
                    InvalidBaseTypeDiagnostic,
                    location,
                    templateSymbol.Name,
                    configuredBaseTypeName,
                    engineProjectKind.ToString()));
                return GeneratedSource.Invalid(diagnostics);
            }

            var templateProperties = new Dictionary<string, TemplatePropertyDefinition>(StringComparer.Ordinal);
            var templateEvents = new Dictionary<string, TemplateEventDefinition>(StringComparer.Ordinal);
            var templateMethods = new Dictionary<string, TemplateMethodDefinition>(StringComparer.Ordinal);
            var templateConstants = new Dictionary<string, TemplateConstantMetadata>(StringComparer.Ordinal);
            var templateTypeConversions = new Dictionary<string, TemplateTypeConversionDefinition>(StringComparer.Ordinal);
            CollectTemplateMetadata(
                templateSymbol,
                templateProperties,
                templateEvents,
                templateMethods,
                templateConstants,
                templateTypeConversions);

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
                .Select(baseTemplateContract => baseTemplateContract.ToDisplayString(TypeDisplayFormat))
                .ToImmutableArray();

            var usesGameObjectAdapter = interfaceHierarchy.Any(static interfaceSymbol =>
                interfaceSymbol.ToDisplayString() == GameObjectMetadataName);

            var isAsset = GetNamedBooleanArgument(templateClassAttribute, "IsAsset");
            var baseInheritsUnityObject = engineProjectKind == EngineProjectKind.Unity &&
                                         InheritsFrom(baseTypeSymbol, "UnityEngine.Object");

            var constantImplementations = new List<TemplateConstantDefinition>();
            var propertyImplementations = new List<PropertyImplementation>();
            var eventImplementations = new List<EventImplementation>();
            var methodImplementations = new List<MethodImplementation>();
            var matchedTemplateMembers = new HashSet<string>(StringComparer.Ordinal);
            var matchedTemplateConstants = new HashSet<string>(StringComparer.Ordinal);
            var seenProperties = new HashSet<string>(StringComparer.Ordinal);
            var seenConstants = new HashSet<string>(StringComparer.Ordinal);
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
                    if (member is IFieldSymbol fieldSymbol &&
                        fieldSymbol.IsStatic &&
                        (fieldSymbol.IsReadOnly || fieldSymbol.IsConst))
                    {
                        if (!seenConstants.Add(fieldSymbol.Name))
                        {
                            continue;
                        }

                        templateConstants.TryGetValue(fieldSymbol.Name, out var templateConstant);
                        if (TryCreateConstantImplementation(
                                fieldSymbol,
                                templateConstant,
                                engineProjectKind,
                                baseTypeSymbol,
                                out var constantImplementation))
                        {
                            constantImplementations.Add(constantImplementation);
                            matchedTemplateConstants.Add(fieldSymbol.Name);
                        }
                        else
                        {
                            diagnostics.Add(Diagnostic.Create(
                                UnboundConstantDiagnostic,
                                location,
                                fieldSymbol.Name,
                                templateSymbol.Name));
                        }

                        continue;
                    }

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
                            propertySymbol,
                            templateProperties,
                            templateTypeConversions,
                            compilation,
                            engineProjectKind,
                            baseTypeSymbol,
                            usesGameObjectAdapter,
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

                    var methodImplementation = CreateMethodImplementation(
                        methodSymbol,
                        templateMethods,
                        templateTypeConversions,
                        compilation,
                        engineProjectKind,
                        baseTypeSymbol,
                        usesGameObjectAdapter);

                    if (methodImplementation.IsBound)
                    {
                        matchedTemplateMembers.Add(methodSymbol.Name);
                    }
                    else
                    {
                        diagnostics.Add(Diagnostic.Create(
                            UnboundMethodDiagnostic,
                            location,
                            methodSymbol.Name,
                            templateSymbol.Name));
                    }

                    methodImplementations.Add(methodImplementation);
                }
            }

            foreach (var templateConstant in templateConstants.Values)
            {
                if (matchedTemplateConstants.Contains(templateConstant.Name))
                {
                    continue;
                }

                if (TryCreateConstantImplementation(
                        fieldSymbol: null,
                        templateConstant,
                        engineProjectKind,
                        baseTypeSymbol,
                        out var constantImplementation))
                {
                    constantImplementations.Add(constantImplementation);
                }
                else
                {
                    diagnostics.Add(Diagnostic.Create(
                        UnboundConstantDiagnostic,
                        location,
                        templateConstant.Name,
                        templateSymbol.Name));
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

                eventImplementations.Add(CreateEventImplementation(eventPropertySymbol, templateEvent, engineProjectKind, baseTypeSymbol));
                matchedTemplateMembers.Add(templateEvent.Name);
            }

            foreach (var eventProperty in eventPropertiesByName.Values)
            {
                if (matchedTemplateMembers.Contains(eventProperty.Name))
                {
                    continue;
                }

                var propertyImplementation = CreateThrowingPropertyImplementation(
                    eventProperty,
                    BaseTypeHasInstanceMember(baseTypeSymbol, eventProperty.Name));
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

            foreach (var templateMethod in templateMethods.Values)
            {
                if (!matchedTemplateMembers.Contains(templateMethod.Name))
                {
                    diagnostics.Add(Diagnostic.Create(
                        UnknownTemplateMemberDiagnostic,
                        location,
                        templateMethod.Name,
                        templateSymbol.Name));
                }
            }

            var generatedNamespace = ResolveGeneratedNamespace(contractSymbol, templateSymbol);
            var generatedClassName = ResolveGeneratedClassName(templateSymbol, contractSymbol);
            var classDocumentationLines = GetDocumentationLines(
                templateSymbol,
                GetNamedStringArgument(templateClassAttribute, "Documentation"));
            if (classDocumentationLines.IsDefaultOrEmpty)
            {
                classDocumentationLines = GetDocumentationLines(contractSymbol);
            }

            var model = new TemplateGenerationModel(
                generatedNamespace,
                generatedClassName,
                templateSymbol.DeclaredAccessibility,
                classDocumentationLines,
                NormalizeTypeName(configuredBaseTypeName!),
                contractSymbol.ToDisplayString(TypeDisplayFormat),
                additionalImplementedContracts,
                constantImplementations.ToImmutableArray(),
                propertyImplementations.ToImmutableArray(),
                eventImplementations.ToImmutableArray(),
                methodImplementations.ToImmutableArray(),
                requiresSpacingThemeConstant,
                usesGameObjectAdapter,
                isAsset,
                baseInheritsUnityObject);

            var renderer = CreateRenderer(engineProjectKind);
            var generatedSource = renderer.Render(model);

            return new GeneratedSource(
                hintName: generatedNamespace.Replace('.', '_') + "_" + generatedClassName + ".g.cs",
                source: generatedSource,
                diagnostics: diagnostics.ToImmutableArray(),
                canGenerate: true);
        }

        private static IEngineTemplateRenderer CreateRenderer(EngineProjectKind engineProjectKind)
            => engineProjectKind switch
            {
                EngineProjectKind.Godot => new GodotTemplateRenderer(),
                EngineProjectKind.Unity => new UnityTemplateRenderer(),
                _ => throw new InvalidOperationException("Unsupported engine project kind.")
            };

        private static EngineProjectKind DetectEngineProjectKind(Compilation compilation)
        {
            var hasGodot = compilation.GetTypeByMetadataName("Godot.Node") is not null;
            var hasUnity = compilation.GetTypeByMetadataName("UnityEngine.Object") is not null;

            if (hasGodot && !hasUnity)
            {
                return EngineProjectKind.Godot;
            }

            if (hasUnity && !hasGodot)
            {
                return EngineProjectKind.Unity;
            }

            return EngineProjectKind.Unknown;
        }

        private static string? ResolveConfiguredBaseTypeName(AttributeData templateClassAttribute, EngineProjectKind engineProjectKind)
            => engineProjectKind switch
            {
                EngineProjectKind.Godot => GetNamedStringArgument(templateClassAttribute, "GodotBase"),
                EngineProjectKind.Unity => GetNamedStringArgument(templateClassAttribute, "UnityBase"),
                _ => null
            };

        private static INamedTypeSymbol? ResolveBaseTypeSymbol(Compilation compilation, string configuredBaseTypeName)
            => compilation.GetTypeByMetadataName(configuredBaseTypeName.Trim().Replace("global::", string.Empty));

        private static void CollectTemplateMetadata(
            INamedTypeSymbol templateSymbol,
            Dictionary<string, TemplatePropertyDefinition> templateProperties,
            Dictionary<string, TemplateEventDefinition> templateEvents,
            Dictionary<string, TemplateMethodDefinition> templateMethods,
            Dictionary<string, TemplateConstantMetadata> templateConstants,
            Dictionary<string, TemplateTypeConversionDefinition> templateTypeConversions)
        {
            CollectTemplateMetadataFromAttributes(
                templateSymbol.GetAttributes(),
                templateProperties,
                templateEvents,
                templateMethods,
                templateConstants,
                templateTypeConversions,
                new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default));
        }

        private static void CollectTemplateMetadataFromAttributes(
            ImmutableArray<AttributeData> attributes,
            Dictionary<string, TemplatePropertyDefinition> templateProperties,
            Dictionary<string, TemplateEventDefinition> templateEvents,
            Dictionary<string, TemplateMethodDefinition> templateMethods,
            Dictionary<string, TemplateConstantMetadata> templateConstants,
            Dictionary<string, TemplateTypeConversionDefinition> templateTypeConversions,
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
                            GetNamedTypeArgument(attribute, "Type")?.ToDisplayString(TypeDisplayFormat),
                            GetNamedStringArgument(attribute, "GodotGetterExpression") ??
                            GetNamedStringArgument(attribute, "FromGodotMethod"),
                            GetNamedStringArgument(attribute, "UnityGetterExpression") ??
                            GetNamedStringArgument(attribute, "FromUnityMethod"),
                            GetNamedStringArgument(attribute, "GodotSetterExpression") ??
                            GetNamedStringArgument(attribute, "ToGodotMethod"),
                            GetNamedStringArgument(attribute, "UnitySetterExpression") ??
                            GetNamedStringArgument(attribute, "ToUnityMethod"),
                            GetNamedStringArgument(attribute, "Documentation"),
                            GetNamedBooleanArgument(attribute, "IsReadOnly"));
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
                            payloadType?.ToDisplayString(TypeDisplayFormat),
                            GetNamedStringArgument(attribute, "Documentation"));
                    }

                    continue;
                }

                if (metadataName == TemplateMethodMetadataName)
                {
                    var name = GetNamedStringArgument(attribute, "Name");
                    if (!string.IsNullOrEmpty(name))
                    {
                        templateMethods[name!] = new TemplateMethodDefinition(
                            name!,
                            GetNamedStringArgument(attribute, "GodotMethodName"),
                            GetNamedStringArgument(attribute, "UnityMethodName"),
                            GetNamedStringArgument(attribute, "Documentation"));
                    }

                    continue;
                }

                if (metadataName == TemplateConstantMetadataName)
                {
                    var name = GetNamedStringArgument(attribute, "Name");
                    if (!string.IsNullOrEmpty(name))
                    {
                        templateConstants[name!] = new TemplateConstantMetadata(
                            name!,
                            GetNamedTypeArgument(attribute, "Type")?.ToDisplayString(TypeDisplayFormat),
                            GetNamedStringArgument(attribute, "Value"),
                            GetNamedStringArgument(attribute, "GodotValue"),
                            GetNamedStringArgument(attribute, "UnityValue"),
                            GetNamedBooleanArgumentOrNull(attribute, "IsConstant"),
                            GetNamedStringArgument(attribute, "Documentation"));
                    }

                    continue;
                }

                if (metadataName == TemplateTypeConversionMetadataName)
                {
                    var agnosticTypeName = GetNamedTypeArgument(attribute, "AgnosticType")?.ToDisplayString(TypeDisplayFormat);
                    if (!string.IsNullOrWhiteSpace(agnosticTypeName))
                    {
                        templateTypeConversions[agnosticTypeName!] = new TemplateTypeConversionDefinition(
                            agnosticTypeName!,
                            GetNamedStringArgument(attribute, "GodotToAgnosticExpression"),
                            GetNamedStringArgument(attribute, "AgnosticToGodotExpression"),
                            GetNamedStringArgument(attribute, "UnityToAgnosticExpression"),
                            GetNamedStringArgument(attribute, "AgnosticToUnityExpression"));
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
                    templateMethods,
                    templateConstants,
                    templateTypeConversions,
                    visitedAttributeTypes);
            }
        }

        private static bool TryCreateConstantImplementation(
            IFieldSymbol? fieldSymbol,
            TemplateConstantMetadata? templateConstant,
            EngineProjectKind engineProjectKind,
            INamedTypeSymbol? baseTypeSymbol,
            out TemplateConstantDefinition constantImplementation)
        {
            var constantName = templateConstant?.Name ?? fieldSymbol?.Name;
            if (string.IsNullOrWhiteSpace(constantName))
            {
                constantImplementation = null!;
                return false;
            }

            var typeName = templateConstant?.TypeName ?? fieldSymbol?.Type.ToDisplayString(TypeDisplayFormat);
            if (string.IsNullOrWhiteSpace(typeName))
            {
                constantImplementation = null!;
                return false;
            }

            var valueExpression = ResolveConfiguredConstantValue(templateConstant, engineProjectKind) ??
                                  ResolveFieldInitializerExpression(fieldSymbol);
            if (string.IsNullOrWhiteSpace(valueExpression))
            {
                constantImplementation = null!;
                return false;
            }

            var documentationLines = !string.IsNullOrWhiteSpace(templateConstant?.Documentation)
                ? CreateSummaryDocumentationLines(templateConstant!.Documentation!)
                : fieldSymbol is not null
                    ? GetDocumentationLines(fieldSymbol)
                    : ImmutableArray<string>.Empty;

            constantImplementation = new TemplateConstantDefinition(
                constantName!,
                NormalizeTypeName(typeName!),
                valueExpression,
                templateConstant?.IsConstant ?? fieldSymbol?.IsConst ?? false,
                BaseTypeHasMember(baseTypeSymbol, constantName!),
                documentationLines);
            return true;
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
            IPropertySymbol propertySymbol,
            IReadOnlyDictionary<string, TemplatePropertyDefinition> templateProperties,
            IReadOnlyDictionary<string, TemplateTypeConversionDefinition> templateTypeConversions,
            Compilation compilation,
            EngineProjectKind engineProjectKind,
            INamedTypeSymbol baseTypeSymbol,
            bool usesGameObjectAdapter,
            out bool requiresSpacingThemeConstant)
        {
            requiresSpacingThemeConstant = false;
            var requiresNewKeyword = BaseTypeHasInstanceMember(baseTypeSymbol, propertySymbol.Name);

            if (usesGameObjectAdapter &&
                propertySymbol.ContainingType.ToDisplayString() == GameObjectMetadataName)
            {
                return CreateGameObjectPropertyImplementation(propertySymbol, engineProjectKind, requiresNewKeyword);
            }

            templateProperties.TryGetValue(propertySymbol.Name, out var templateProperty);
            var getterTemplate = ResolveConfiguredGetterExpression(templateProperty, engineProjectKind);
            var setterTemplate = ResolveConfiguredSetterExpression(templateProperty, engineProjectKind);
            var configuredGetterExpression = getterTemplate;
            var configuredSetterExpression = string.IsNullOrWhiteSpace(setterTemplate)
                ? null
                : ApplyValueTemplate(setterTemplate!, "value");
            requiresSpacingThemeConstant = ReferencesSpacingThemeConstant(configuredGetterExpression) ||
                                           ReferencesSpacingThemeConstant(configuredSetterExpression);

            var engineMember = FindEngineProperty(baseTypeSymbol, propertySymbol.Name);
            if (engineMember is not null)
            {
                var engineAccessExpression = "base." + engineMember.Name;
                configuredGetterExpression = string.IsNullOrWhiteSpace(getterTemplate)
                    ? null
                    : ApplyValueTemplate(getterTemplate!, engineAccessExpression);
                configuredSetterExpression = string.IsNullOrWhiteSpace(setterTemplate)
                    ? null
                    : ApplyValueTemplate(setterTemplate!, "value");

                return new PropertyImplementation(
                    PropertyImplementationKind.Generated,
                    propertySymbol.Type.ToDisplayString(TypeDisplayFormat),
                    propertySymbol.Name,
                    propertySymbol.GetMethod is null
                        ? null
                        : configuredGetterExpression ?? BuildGetterExpression(
                            propertySymbol,
                            engineAccessExpression,
                            engineMember.Type,
                            compilation,
                            engineProjectKind,
                            templateTypeConversions),
                    propertySymbol.SetMethod is null
                        ? null
                        : configuredSetterExpression ?? BuildSetterExpression(
                            propertySymbol,
                            engineAccessExpression,
                            engineMember.Type,
                            compilation,
                            engineProjectKind,
                            templateTypeConversions),
                    requiresNewKeyword,
                    documentationLines: GetDocumentationLines(propertySymbol, templateProperty?.Documentation),
                    backingFieldDeclaration: null
                );
            }

            if (HasCompleteConfiguredExpressions(propertySymbol, configuredGetterExpression, configuredSetterExpression))
            {
                return new PropertyImplementation(
                    PropertyImplementationKind.Generated,
                    propertySymbol.Type.ToDisplayString(TypeDisplayFormat),
                    propertySymbol.Name,
                    propertySymbol.GetMethod is null ? null : configuredGetterExpression,
                    propertySymbol.SetMethod is null ? null : configuredSetterExpression,
                    requiresNewKeyword,
                    documentationLines: GetDocumentationLines(propertySymbol, templateProperty?.Documentation),
                    backingFieldDeclaration: null
                );
            }

            return CreateFieldBackedPropertyImplementation(propertySymbol, templateProperty, requiresNewKeyword);
        }

        private static PropertyImplementation CreateGameObjectPropertyImplementation(
            IPropertySymbol propertySymbol,
            EngineProjectKind engineProjectKind,
            bool requiresNewKeyword)
        {
            var getterExpression = propertySymbol.Name switch
            {
                "Children" => "_impl.Children",
                "Parent" => "_impl.Parent",
                "Enabled" when engineProjectKind == EngineProjectKind.Godot => "ProcessMode != global::Godot.Node.ProcessModeEnum.Disabled",
                _ => "_impl." + propertySymbol.Name
            };

            var setterExpression = propertySymbol.SetMethod is null
                ? null
                : propertySymbol.Name switch
                {
                    "Parent" => "_impl.Parent = value",
                    "Enabled" when engineProjectKind == EngineProjectKind.Godot
                        => "ProcessMode = value ? global::Godot.Node.ProcessModeEnum.Inherit : global::Godot.Node.ProcessModeEnum.Disabled",
                    _ => "_impl." + propertySymbol.Name + " = value"
                };

            return new PropertyImplementation(
                PropertyImplementationKind.Generated,
                propertySymbol.Type.ToDisplayString(TypeDisplayFormat),
                propertySymbol.Name,
                propertySymbol.GetMethod is null ? null : getterExpression,
                setterExpression,
                requiresNewKeyword,
                documentationLines: GetDocumentationLines(propertySymbol),
                backingFieldDeclaration: null
            );
        }

        private static string? ResolveConfiguredGetterExpression(
            TemplatePropertyDefinition? templateProperty,
            EngineProjectKind engineProjectKind)
        {
            if (templateProperty is null)
            {
                return null;
            }

            return engineProjectKind switch
            {
                EngineProjectKind.Godot => templateProperty.GodotGetterExpression,
                EngineProjectKind.Unity => templateProperty.UnityGetterExpression,
                _ => null
            };
        }

        private static string? ResolveConfiguredSetterExpression(
            TemplatePropertyDefinition? templateProperty,
            EngineProjectKind engineProjectKind)
        {
            if (templateProperty is null)
            {
                return null;
            }

            return engineProjectKind switch
            {
                EngineProjectKind.Godot => templateProperty.GodotSetterExpression,
                EngineProjectKind.Unity => templateProperty.UnitySetterExpression,
                _ => null
            };
        }

        private static bool HasCompleteConfiguredExpressions(
            IPropertySymbol propertySymbol,
            string? getterExpression,
            string? setterExpression)
        {
            if (propertySymbol.GetMethod is not null && string.IsNullOrWhiteSpace(getterExpression))
            {
                return false;
            }

            if (propertySymbol.SetMethod is not null && string.IsNullOrWhiteSpace(setterExpression))
            {
                return false;
            }

            return !string.IsNullOrWhiteSpace(getterExpression) || !string.IsNullOrWhiteSpace(setterExpression);
        }

        private static bool ReferencesSpacingThemeConstant(string? expression)
            => !string.IsNullOrWhiteSpace(expression) &&
               expression!.IndexOf("SeparationThemeConstantName", StringComparison.Ordinal) >= 0;

        private static PropertyImplementation CreateFieldBackedPropertyImplementation(
            IPropertySymbol propertySymbol,
            TemplatePropertyDefinition? templateProperty,
            bool requiresNewKeyword)
        {
            var fieldName = "_" + ToCamelCase(propertySymbol.Name);
            var fieldTypeName = propertySymbol.Type.ToDisplayString(TypeDisplayFormat);
            var isReadOnly = templateProperty?.IsReadOnly == true;
            var backingFieldDeclaration = "private " +
                                          (isReadOnly ? "readonly " : string.Empty) +
                                          fieldTypeName +
                                          " " +
                                          fieldName +
                                          ";";

            string? setterExpression = null;
            if (propertySymbol.SetMethod is not null)
            {
                setterExpression = isReadOnly
                    ? "throw new global::System.NotSupportedException(\"This generated property is read-only.\")"
                    : fieldName + " = value";
            }

            return new PropertyImplementation(
                PropertyImplementationKind.Generated,
                propertySymbol.Type.ToDisplayString(TypeDisplayFormat),
                propertySymbol.Name,
                propertySymbol.GetMethod is null ? null : fieldName,
                setterExpression,
                requiresNewKeyword,
                documentationLines: GetDocumentationLines(propertySymbol, templateProperty?.Documentation),
                backingFieldDeclaration
            );
        }

        private static PropertyImplementation CreateThrowingPropertyImplementation(IPropertySymbol propertySymbol, bool requiresNewKeyword)
            => new(
                PropertyImplementationKind.ThrowingStub,
                propertySymbol.Type.ToDisplayString(TypeDisplayFormat),
                propertySymbol.Name,
                propertySymbol.GetMethod is null
                    ? null
                    : "throw new global::System.NotSupportedException(\"The engine binding for this property was not generated.\")",
                propertySymbol.SetMethod is null
                    ? null
                    : "throw new global::System.NotSupportedException(\"The engine binding for this property was not generated.\")",
                requiresNewKeyword,
                documentationLines: GetDocumentationLines(propertySymbol),
                backingFieldDeclaration: null
            );

        private static EventImplementation CreateEventImplementation(
            IPropertySymbol propertySymbol,
            TemplateEventDefinition templateEvent,
            EngineProjectKind engineProjectKind,
            INamedTypeSymbol baseTypeSymbol)
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

            return new EventImplementation(
                propertySymbol.Name,
                payloadTypeName,
                "_" + ToCamelCase(propertySymbol.Name),
                BuildRegistryEventName(propertySymbol),
                BuildEventHookStatements(engineProjectKind, templateEvent.Name, "_" + ToCamelCase(propertySymbol.Name), payloadTypeName),
                BaseTypeHasInstanceMember(baseTypeSymbol, propertySymbol.Name),
                GetDocumentationLines(propertySymbol, templateEvent.Documentation)
            );
        }

        private static MethodImplementation CreateMethodImplementation(
            IMethodSymbol methodSymbol,
            IReadOnlyDictionary<string, TemplateMethodDefinition> templateMethods,
            IReadOnlyDictionary<string, TemplateTypeConversionDefinition> templateTypeConversions,
            Compilation compilation,
            EngineProjectKind engineProjectKind,
            INamedTypeSymbol baseTypeSymbol,
            bool usesGameObjectAdapter)
        {
            var returnTypeName = methodSymbol.ReturnType.ToDisplayString(TypeDisplayFormat);
            var typeParameters = methodSymbol.TypeParameters.Length == 0
                ? string.Empty
                : "<" + string.Join(", ", methodSymbol.TypeParameters.Select(typeParameter => typeParameter.Name)) + ">";

            var parameters = string.Join(", ", methodSymbol.Parameters.Select(FormatParameter));
            var constraints = BuildConstraintClauses(methodSymbol);
            templateMethods.TryGetValue(methodSymbol.Name, out var templateMethod);

            if (usesGameObjectAdapter &&
                methodSymbol.ContainingType.ToDisplayString() == GameObjectMetadataName)
            {
                var callArguments = string.Join(", ", methodSymbol.Parameters.Select(parameter => parameter.Name));
                var invocation = "_impl." + methodSymbol.Name + typeParameters + "(" + callArguments + ")";
                return new MethodImplementation(
                    signature: "public " + returnTypeName + " " + methodSymbol.Name + typeParameters + "(" + parameters + ")",
                    constraints: constraints,
                    body: methodSymbol.ReturnsVoid ? invocation + ";" : "return " + invocation + ";",
                    isBound: true,
                    documentationLines: GetDocumentationLines(methodSymbol, templateMethod?.Documentation));
            }

            var engineMethodName = ResolveConfiguredMethodName(templateMethod, engineProjectKind) ?? methodSymbol.Name;
            var engineMethod = FindEngineMethod(
                baseTypeSymbol,
                engineMethodName,
                methodSymbol,
                compilation,
                engineProjectKind,
                templateTypeConversions
            );

            if (engineMethod is not null)
            {
                var invocationArguments = string.Join(", ", methodSymbol.Parameters
                    .Select(
                        (parameter, index) => BuildMethodArgument(
                            parameter,
                            engineMethod.Parameters[index],
                            compilation,
                            engineProjectKind,
                            templateTypeConversions
                        )
                    )
                );
                var invocation = "base." + engineMethod.Name + typeParameters + "(" + invocationArguments + ")";
                var body = methodSymbol.ReturnsVoid
                    ? invocation + ";"
                    : "return " + ConvertExpression(
                        invocation,
                        engineMethod.ReturnType,
                        methodSymbol.ReturnType,
                        compilation,
                        engineProjectKind,
                        templateTypeConversions
                    ) + ";";

                return new MethodImplementation(
                    signature: "public " + returnTypeName + " " + methodSymbol.Name + typeParameters + "(" + parameters + ")",
                    constraints: constraints,
                    body: body,
                    isBound: true,
                    documentationLines: GetDocumentationLines(methodSymbol, templateMethod?.Documentation)
                );
            }

            return new MethodImplementation(
                signature: "public " + returnTypeName + " " + methodSymbol.Name + typeParameters + "(" + parameters + ")",
                constraints: constraints,
                body: "throw new global::System.NotSupportedException(\"The engine binding for this method was not generated.\");",
                isBound: false,
                documentationLines: GetDocumentationLines(methodSymbol, templateMethod?.Documentation)
            );
        }

        private static string? ResolveConfiguredMethodName(
            TemplateMethodDefinition? templateMethod,
            EngineProjectKind engineProjectKind)
        {
            if (templateMethod is null)
            {
                return null;
            }

            return engineProjectKind switch
            {
                EngineProjectKind.Godot => templateMethod.GodotMethodName,
                EngineProjectKind.Unity => templateMethod.UnityMethodName,
                _ => null
            };
        }

        private static IMethodSymbol? FindEngineMethod(
            INamedTypeSymbol? typeSymbol,
            string methodName,
            IMethodSymbol contractMethod,
            Compilation compilation,
            EngineProjectKind engineProjectKind,
            IReadOnlyDictionary<string, TemplateTypeConversionDefinition> templateTypeConversions)
        {
            var current = typeSymbol;
            while (current is not null)
            {
                foreach (var member in current.GetMembers(methodName))
                {
                    if (member is not IMethodSymbol methodSymbol ||
                        methodSymbol.MethodKind != MethodKind.Ordinary ||
                        methodSymbol.IsStatic)
                    {
                        continue;
                    }

                    if (CanBindMethod(
                            contractMethod,
                            methodSymbol,
                            compilation,
                            engineProjectKind,
                            templateTypeConversions))
                    {
                        return methodSymbol;
                    }
                }

                current = current.BaseType;
            }

            return null;
        }

        private static bool CanBindMethod(
            IMethodSymbol contractMethod,
            IMethodSymbol engineMethod,
            Compilation compilation,
            EngineProjectKind engineProjectKind,
            IReadOnlyDictionary<string, TemplateTypeConversionDefinition> templateTypeConversions)
        {
            if (contractMethod.TypeParameters.Length != engineMethod.TypeParameters.Length ||
                contractMethod.Parameters.Length != engineMethod.Parameters.Length)
            {
                return false;
            }

            for (var index = 0; index < contractMethod.Parameters.Length; index++)
            {
                var contractParameter = contractMethod.Parameters[index];
                var engineParameter = engineMethod.Parameters[index];

                if (contractParameter.RefKind != engineParameter.RefKind)
                {
                    return false;
                }

                if (contractParameter.RefKind != RefKind.None)
                {
                    if (!SymbolEqualityComparer.Default.Equals(contractParameter.Type, engineParameter.Type))
                    {
                        return false;
                    }

                    continue;
                }

                if (!CanConvertType(
                        contractParameter.Type,
                        engineParameter.Type,
                        compilation,
                        engineProjectKind,
                        templateTypeConversions))
                {
                    return false;
                }
            }

            if (contractMethod.ReturnsVoid)
            {
                return engineMethod.ReturnsVoid;
            }

            if (engineMethod.ReturnsVoid)
            {
                return false;
            }

            return CanConvertType(
                engineMethod.ReturnType,
                contractMethod.ReturnType,
                compilation,
                engineProjectKind,
                templateTypeConversions
            );
        }

        private static string BuildMethodArgument(
            IParameterSymbol contractParameter,
            IParameterSymbol engineParameter,
            Compilation compilation,
            EngineProjectKind engineProjectKind,
            IReadOnlyDictionary<string, TemplateTypeConversionDefinition> templateTypeConversions)
        {
            var prefix = contractParameter.RefKind switch
            {
                RefKind.Ref => "ref ",
                RefKind.Out => "out ",
                RefKind.In => "in ",
                _ => string.Empty
            };

            if (contractParameter.RefKind != RefKind.None)
            {
                return prefix + contractParameter.Name;
            }

            return prefix + ConvertExpression(
                contractParameter.Name,
                contractParameter.Type,
                engineParameter.Type,
                compilation,
                engineProjectKind,
                templateTypeConversions
            );
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

        private static bool BaseTypeHasMember(INamedTypeSymbol? typeSymbol, string memberName)
        {
            var current = typeSymbol;
            while (current is not null)
            {
                if (current.GetMembers(memberName).Length > 0)
                {
                    return true;
                }

                current = current.BaseType;
            }

            return false;
        }

        private static bool BaseTypeHasInstanceMember(INamedTypeSymbol? typeSymbol, string memberName)
        {
            var current = typeSymbol;
            while (current is not null)
            {
                foreach (var member in current.GetMembers(memberName))
                {
                    if (!member.IsStatic)
                    {
                        return true;
                    }
                }

                current = current.BaseType;
            }

            return false;
        }

        private static bool InheritsFrom(ITypeSymbol? typeSymbol, string metadataName)
        {
            var current = typeSymbol;
            while (current is not null)
            {
                if (current.ToDisplayString() == metadataName)
                {
                    return true;
                }

                current = current.BaseType;
            }

            return false;
        }

        private static string BuildGetterExpression(
            IPropertySymbol interfaceProperty,
            string engineAccessExpression,
            ITypeSymbol engineMemberType,
            Compilation compilation,
            EngineProjectKind engineProjectKind,
            IReadOnlyDictionary<string, TemplateTypeConversionDefinition> templateTypeConversions)
        {
            return ConvertExpression(
                engineAccessExpression,
                engineMemberType,
                interfaceProperty.Type,
                compilation,
                engineProjectKind,
                templateTypeConversions
            );
        }

        private static string BuildSetterExpression(
            IPropertySymbol interfaceProperty,
            string engineAccessExpression,
            ITypeSymbol engineMemberType,
            Compilation compilation,
            EngineProjectKind engineProjectKind,
            IReadOnlyDictionary<string, TemplateTypeConversionDefinition> templateTypeConversions)
        {
            return engineAccessExpression + " = " + ConvertExpression(
                "value",
                interfaceProperty.Type,
                engineMemberType,
                compilation,
                engineProjectKind,
                templateTypeConversions
            );
        }

        private static string ConvertExpression(
            string expression,
            ITypeSymbol sourceType,
            ITypeSymbol destinationType,
            Compilation compilation,
            EngineProjectKind engineProjectKind,
            IReadOnlyDictionary<string, TemplateTypeConversionDefinition> templateTypeConversions)
        {
            if (SymbolEqualityComparer.Default.Equals(sourceType, destinationType))
            {
                return expression;
            }

            if (TryGetTemplateConversionExpression(
                    sourceType,
                    destinationType,
                    engineProjectKind,
                    templateTypeConversions,
                    expression,
                    out var convertedExpression))
            {
                return convertedExpression;
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

        private static bool CanConvertType(
            ITypeSymbol sourceType,
            ITypeSymbol destinationType,
            Compilation compilation,
            EngineProjectKind engineProjectKind,
            IReadOnlyDictionary<string, TemplateTypeConversionDefinition> templateTypeConversions)
        {
            if (SymbolEqualityComparer.Default.Equals(sourceType, destinationType))
            {
                return true;
            }

            if (TryGetTemplateConversionExpression(
                    sourceType,
                    destinationType,
                    engineProjectKind,
                    templateTypeConversions,
                    "value",
                    out _))
            {
                return true;
            }

            if (compilation is not CSharpCompilation csharpCompilation)
            {
                return false;
            }

            return csharpCompilation.ClassifyConversion(sourceType, destinationType).Exists;
        }

        private static bool TryGetTemplateConversionExpression(
            ITypeSymbol sourceType,
            ITypeSymbol destinationType,
            EngineProjectKind engineProjectKind,
            IReadOnlyDictionary<string, TemplateTypeConversionDefinition> templateTypeConversions,
            string expression,
            out string convertedExpression)
        {
            convertedExpression = string.Empty;

            var sourceTypeName = sourceType.ToDisplayString(TypeDisplayFormat);
            var destinationTypeName = destinationType.ToDisplayString(TypeDisplayFormat);

            foreach (var conversion in templateTypeConversions.Values)
            {
                var fromEngineExpression = engineProjectKind switch
                {
                    EngineProjectKind.Godot => conversion.GodotToAgnosticExpression,
                    EngineProjectKind.Unity => conversion.UnityToAgnosticExpression,
                    _ => null
                };

                var toEngineExpression = engineProjectKind switch
                {
                    EngineProjectKind.Godot => conversion.AgnosticToGodotExpression,
                    EngineProjectKind.Unity => conversion.AgnosticToUnityExpression,
                    _ => null
                };

                if (conversion.AgnosticTypeName == destinationTypeName &&
                    !string.IsNullOrWhiteSpace(fromEngineExpression))
                {
                    convertedExpression = ApplyValueTemplate(fromEngineExpression!, expression);
                    return true;
                }

                if (conversion.AgnosticTypeName == sourceTypeName &&
                    !string.IsNullOrWhiteSpace(toEngineExpression))
                {
                    convertedExpression = ApplyValueTemplate(toEngineExpression!, expression);
                    return true;
                }
            }

            return false;
        }

        private static string ApplyValueTemplate(string template, string valueExpression)
        {
            return template.Replace(ValuePlaceholder, valueExpression);
        }

        private static ImmutableArray<string> GetDocumentationLines(ISymbol symbol, string? summaryOverride = null)
        {
            if (!string.IsNullOrWhiteSpace(summaryOverride))
            {
                return CreateSummaryDocumentationLines(summaryOverride!);
            }

            var xml = symbol.GetDocumentationCommentXml(expandIncludes: true, cancellationToken: default);
            if (string.IsNullOrWhiteSpace(xml))
            {
                return ImmutableArray<string>.Empty;
            }

            try
            {
                var member = XElement.Parse(xml);
                if (!HasMeaningfulDocumentation(member))
                {
                    return ImmutableArray<string>.Empty;
                }

                var documentationLines = ImmutableArray.CreateBuilder<string>();
                foreach (var node in member.Nodes())
                {
                    AppendDocumentationNodeLines(node, documentationLines);
                }

                return documentationLines.ToImmutable();
            }
            catch
            {
                return ImmutableArray<string>.Empty;
            }
        }

        private static ImmutableArray<string> CreateSummaryDocumentationLines(string summary)
        {
            var summaryLines = summary
                .Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
                .Select(line => line.Trim())
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line => global::System.Security.SecurityElement.Escape(line) ?? string.Empty)
                .ToImmutableArray();

            if (summaryLines.IsDefaultOrEmpty)
            {
                return ImmutableArray<string>.Empty;
            }

            var documentationLines = ImmutableArray.CreateBuilder<string>();
            documentationLines.Add("<summary>");
            documentationLines.AddRange(summaryLines);
            documentationLines.Add("</summary>");
            return documentationLines.ToImmutable();
        }

        private static bool HasMeaningfulDocumentation(XElement member)
        {
            foreach (var child in member.Elements())
            {
                if (!string.IsNullOrWhiteSpace(child.Value))
                {
                    return true;
                }

                foreach (var descendant in child.DescendantsAndSelf())
                {
                    if (descendant.HasAttributes)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static void AppendDocumentationNodeLines(XNode node, ImmutableArray<string>.Builder documentationLines)
        {
            if (node is XText textNode && string.IsNullOrWhiteSpace(textNode.Value))
            {
                return;
            }

            var renderedNode = node.ToString(SaveOptions.None);
            var nodeLines = renderedNode
                .Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
                .Select(line => line.Trim())
                .Where(line => !string.IsNullOrWhiteSpace(line));

            documentationLines.AddRange(nodeLines);
        }

        private static ImmutableArray<string> BuildEventHookStatements(
            EngineProjectKind engineProjectKind,
            string eventName,
            string fieldName,
            string payloadTypeName)
        {
            if (engineProjectKind != EngineProjectKind.Godot)
            {
                return ImmutableArray<string>.Empty;
            }

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

            if (eventName == "ValueSet" || eventName == "ValueChanged")
            {
                if (payloadTypeName == "float" || payloadTypeName == "global::System.Single")
                {
                    return ImmutableArray.Create("ValueChanged += (value) => " + fieldName + ".Publish((float)value);");
                }

                return ImmutableArray.Create("ValueChanged += (value) => " + fieldName + ".Publish(value);");
            }

            if (eventName == "DisplayStateChanged" || eventName == "VisibilityChanged")
            {
                return ImmutableArray.Create("VisibilityChanged += () => " + fieldName + ".Publish(base.Visible);");
            }

            return ImmutableArray<string>.Empty;
        }

        private static string BuildRegistryEventName(IPropertySymbol propertySymbol)
            => "Nomad." + TrimInterfacePrefix(propertySymbol.ContainingType.Name) + propertySymbol.Name;

        private static string ResolveGeneratedNamespace(INamedTypeSymbol contractSymbol, INamedTypeSymbol templateSymbol)
        {
            var templateNamespaceAttribute = FindAttribute(templateSymbol, TemplateNamespaceMetadataName);
            var configuredNamespace = templateNamespaceAttribute is null
                ? null
                : GetNamedStringArgument(templateNamespaceAttribute, "Name");

            if (!string.IsNullOrWhiteSpace(configuredNamespace))
            {
                var trimmedNamespace = configuredNamespace!.Trim().Trim('.');
                if (trimmedNamespace.Equals("Nomad", StringComparison.Ordinal) ||
                    trimmedNamespace.StartsWith("Nomad.", StringComparison.Ordinal))
                {
                    return trimmedNamespace;
                }

                return "Nomad." + trimmedNamespace;
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

        private static bool? GetNamedBooleanArgumentOrNull(AttributeData attribute, string argumentName)
        {
            foreach (var namedArgument in attribute.NamedArguments)
            {
                if (namedArgument.Key == argumentName &&
                    namedArgument.Value.Kind == TypedConstantKind.Primitive &&
                    namedArgument.Value.Value is bool boolValue)
                {
                    return boolValue;
                }
            }

            return null;
        }

        private static bool GetNamedBooleanArgument(AttributeData attribute, string argumentName)
        {
            foreach (var namedArgument in attribute.NamedArguments)
            {
                if (namedArgument.Key == argumentName &&
                    namedArgument.Value.Kind == TypedConstantKind.Primitive &&
                    namedArgument.Value.Value is bool boolValue)
                {
                    return boolValue;
                }
            }

            return false;
        }

        private static string? ResolveConfiguredConstantValue(
            TemplateConstantMetadata? templateConstant,
            EngineProjectKind engineProjectKind)
        {
            if (templateConstant is null)
            {
                return null;
            }

            var engineSpecificValue = engineProjectKind switch
            {
                EngineProjectKind.Godot => templateConstant.GodotValueExpression,
                EngineProjectKind.Unity => templateConstant.UnityValueExpression,
                _ => null
            };

            return !string.IsNullOrWhiteSpace(engineSpecificValue)
                ? engineSpecificValue
                : templateConstant.ValueExpression;
        }

        private static string? ResolveFieldInitializerExpression(IFieldSymbol? fieldSymbol)
        {
            if (fieldSymbol is null)
            {
                return null;
            }

            foreach (var syntaxReference in fieldSymbol.DeclaringSyntaxReferences)
            {
                if (syntaxReference.GetSyntax() is VariableDeclaratorSyntax variableDeclarator &&
                    variableDeclarator.Initializer is not null)
                {
                    return variableDeclarator.Initializer.Value.ToString();
                }
            }

            return fieldSymbol.ContainingType.ToDisplayString(TypeDisplayFormat) + "." + fieldSymbol.Name;
        }

        private static string FormatParameter(IParameterSymbol parameter)
        {
            var builder = new System.Text.StringBuilder();

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

        private static string NormalizeTypeName(string typeName)
        {
            var trimmed = typeName.Trim();
            return trimmed.StartsWith("global::", StringComparison.Ordinal)
                ? trimmed
                : "global::" + trimmed;
        }
    }
}
