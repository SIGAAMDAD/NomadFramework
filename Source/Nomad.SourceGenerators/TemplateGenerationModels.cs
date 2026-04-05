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

using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Nomad.SourceGenerators
{
    /// <summary>
    /// Identifies which engine-specific wrapper variant the generator should produce.
    /// </summary>
    internal enum EngineProjectKind
    {
        Unknown,
        Godot,
        Unity
    }

    /// <summary>
    /// Represents the complete set of data required to render a generated engine wrapper.
    /// </summary>
    internal sealed class TemplateGenerationModel
    {
        public TemplateGenerationModel(
            string generatedNamespace,
            string generatedClassName,
            Accessibility accessibility,
            ImmutableArray<string> classDocumentationLines,
            string baseTypeName,
            string contractTypeName,
            ImmutableArray<string> additionalImplementedContractNames,
            ImmutableArray<TemplateConstantDefinition> constantImplementations,
            ImmutableArray<PropertyImplementation> propertyImplementations,
            ImmutableArray<EventImplementation> eventImplementations,
            ImmutableArray<MethodImplementation> methodImplementations,
            bool requiresSpacingThemeConstant,
            bool usesGameObjectAdapter,
            bool isAsset,
            bool baseInheritsUnityObject)
        {
            GeneratedNamespace = generatedNamespace;
            GeneratedClassName = generatedClassName;
            Accessibility = accessibility;
            ClassDocumentationLines = classDocumentationLines;
            BaseTypeName = baseTypeName;
            ContractTypeName = contractTypeName;
            AdditionalImplementedContractNames = additionalImplementedContractNames;
            ConstantImplementations = constantImplementations;
            PropertyImplementations = propertyImplementations;
            EventImplementations = eventImplementations;
            MethodImplementations = methodImplementations;
            RequiresSpacingThemeConstant = requiresSpacingThemeConstant;
            UsesGameObjectAdapter = usesGameObjectAdapter;
            IsAsset = isAsset;
            BaseInheritsUnityObject = baseInheritsUnityObject;
        }

        public string GeneratedNamespace { get; }

        public string GeneratedClassName { get; }

        public Accessibility Accessibility { get; }

        public ImmutableArray<string> ClassDocumentationLines { get; }

        public string BaseTypeName { get; }

        public string ContractTypeName { get; }

        public ImmutableArray<string> AdditionalImplementedContractNames { get; }

        public ImmutableArray<TemplateConstantDefinition> ConstantImplementations { get; }

        public ImmutableArray<PropertyImplementation> PropertyImplementations { get; }

        public ImmutableArray<EventImplementation> EventImplementations { get; }

        public ImmutableArray<MethodImplementation> MethodImplementations { get; }

        public bool RequiresSpacingThemeConstant { get; }

        public bool UsesGameObjectAdapter { get; }

        public bool IsAsset { get; }

        public bool BaseInheritsUnityObject { get; }
    }

    /// <summary>
    /// Represents a generated source file and the diagnostics discovered while creating it.
    /// </summary>
    internal sealed class GeneratedSource
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

    /// <summary>
    /// Stores template metadata for a generated property member.
    /// </summary>
    internal sealed class TemplatePropertyDefinition
    {
        public TemplatePropertyDefinition(
            string name,
            string? typeName,
            string? godotGetterExpression,
            string? unityGetterExpression,
            string? godotSetterExpression,
            string? unitySetterExpression,
            string? documentation,
            bool isReadOnly)
        {
            Name = name;
            TypeName = typeName;
            GodotGetterExpression = godotGetterExpression;
            UnityGetterExpression = unityGetterExpression;
            GodotSetterExpression = godotSetterExpression;
            UnitySetterExpression = unitySetterExpression;
            Documentation = documentation;
            IsReadOnly = isReadOnly;
        }

        public string Name { get; }

        public string? TypeName { get; }

        public string? GodotGetterExpression { get; }

        public string? UnityGetterExpression { get; }

        public string? GodotSetterExpression { get; }

        public string? UnitySetterExpression { get; }

        public string? Documentation { get; }

        public bool IsReadOnly { get; }
    }

    /// <summary>
    /// Stores template metadata for a generated method member.
    /// </summary>
    internal sealed class TemplateMethodDefinition
    {
        public TemplateMethodDefinition(string name, string? godotMethodName, string? unityMethodName, string? documentation)
        {
            Name = name;
            GodotMethodName = godotMethodName;
            UnityMethodName = unityMethodName;
            Documentation = documentation;
        }

        public string Name { get; }

        public string? GodotMethodName { get; }

        public string? UnityMethodName { get; }

        public string? Documentation { get; }
    }

    /// <summary>
    /// Stores template metadata for a generated constant or static readonly field member.
    /// </summary>
    internal sealed class TemplateConstantMetadata
    {
        public TemplateConstantMetadata(
            string name,
            string? typeName,
            string? valueExpression,
            string? godotValueExpression,
            string? unityValueExpression,
            bool? isConstant,
            string? documentation)
        {
            Name = name;
            TypeName = typeName;
            ValueExpression = valueExpression;
            GodotValueExpression = godotValueExpression;
            UnityValueExpression = unityValueExpression;
            IsConstant = isConstant;
            Documentation = documentation;
        }

        public string Name { get; }

        public string? TypeName { get; }

        public string? ValueExpression { get; }

        public string? GodotValueExpression { get; }

        public string? UnityValueExpression { get; }

        public bool? IsConstant { get; }

        public string? Documentation { get; }
    }

    /// <summary>
    /// Describes a static readonly field or constant to be emitted in the generated wrapper.
    /// </summary>
    internal sealed class TemplateConstantDefinition
    {
        public TemplateConstantDefinition(
            string name,
            string typeName,
            string valueExpression,
            bool isConstant,
            bool requiresNewKeyword,
            ImmutableArray<string> documentationLines)
        {
            Name = name;
            TypeName = typeName;
            ValueExpression = valueExpression;
            IsConstant = isConstant;
            RequiresNewKeyword = requiresNewKeyword;
            DocumentationLines = documentationLines;
        }

        public string Name { get; }
        public string TypeName { get; }
        public string ValueExpression { get; }
        public bool IsConstant { get; }
        public bool RequiresNewKeyword { get; }
        public ImmutableArray<string> DocumentationLines { get; }
    }
    /// <summary>
    /// Stores type-conversion metadata used when translating between agnostic and engine-native types.
    /// </summary>
    internal sealed class TemplateTypeConversionDefinition
    {
        public TemplateTypeConversionDefinition(
            string agnosticTypeName,
            string? godotToAgnosticExpression,
            string? agnosticToGodotExpression,
            string? unityToAgnosticExpression,
            string? agnosticToUnityExpression)
        {
            AgnosticTypeName = agnosticTypeName;
            GodotToAgnosticExpression = godotToAgnosticExpression;
            AgnosticToGodotExpression = agnosticToGodotExpression;
            UnityToAgnosticExpression = unityToAgnosticExpression;
            AgnosticToUnityExpression = agnosticToUnityExpression;
        }

        public string AgnosticTypeName { get; }

        public string? GodotToAgnosticExpression { get; }

        public string? AgnosticToGodotExpression { get; }

        public string? UnityToAgnosticExpression { get; }

        public string? AgnosticToUnityExpression { get; }
    }

    /// <summary>
    /// Stores template metadata for a generated event member.
    /// </summary>
    internal sealed class TemplateEventDefinition
    {
        public TemplateEventDefinition(
            string name,
            string? payloadTypeName,
            string? documentation,
            string? godotHookExpression,
            string? unityHookExpression)
        {
            Name = name;
            PayloadTypeName = payloadTypeName;
            Documentation = documentation;
            GodotHookExpression = godotHookExpression;
            UnityHookExpression = unityHookExpression;
        }

        public string Name { get; }

        public string? PayloadTypeName { get; }

        public string? Documentation { get; }

        public string? GodotHookExpression { get; }

        public string? UnityHookExpression { get; }
    }

    /// <summary>
    /// Describes how a property should be emitted into a generated engine wrapper.
    /// </summary>
    internal sealed class PropertyImplementation
    {
        public PropertyImplementation(
            PropertyImplementationKind kind,
            string propertyTypeName,
            string propertyName,
            string? getterExpression,
            string? setterExpression,
            bool requiresNewKeyword,
            ImmutableArray<string> documentationLines,
            string? backingFieldDeclaration)
        {
            Kind = kind;
            PropertyTypeName = propertyTypeName;
            PropertyName = propertyName;
            GetterExpression = getterExpression;
            SetterExpression = setterExpression;
            RequiresNewKeyword = requiresNewKeyword;
            DocumentationLines = documentationLines;
            BackingFieldDeclaration = backingFieldDeclaration;
        }

        public PropertyImplementationKind Kind { get; }

        public string PropertyTypeName { get; }

        public string PropertyName { get; }

        public string? GetterExpression { get; }

        public string? SetterExpression { get; }

        public bool RequiresNewKeyword { get; }

        public ImmutableArray<string> DocumentationLines { get; }

        public string? BackingFieldDeclaration { get; }
    }

    /// <summary>
    /// Describes how an event member should be emitted into a generated engine wrapper.
    /// </summary>
    internal sealed class EventImplementation
    {
        public EventImplementation(
            string eventName,
            string payloadTypeName,
            string fieldName,
            string registryEventName,
            ImmutableArray<string> hookStatements,
            bool requiresNewKeyword,
            ImmutableArray<string> documentationLines)
        {
            EventName = eventName;
            PayloadTypeName = payloadTypeName;
            FieldName = fieldName;
            RegistryEventName = registryEventName;
            HookStatements = hookStatements;
            RequiresNewKeyword = requiresNewKeyword;
            DocumentationLines = documentationLines;
        }

        public string EventName { get; }

        public string PayloadTypeName { get; }

        public string FieldName { get; }

        public string RegistryEventName { get; }

        public ImmutableArray<string> HookStatements { get; }

        public bool RequiresNewKeyword { get; }

        public ImmutableArray<string> DocumentationLines { get; }
    }

    /// <summary>
    /// Describes how a method should be emitted into a generated engine wrapper.
    /// </summary>
    internal sealed class MethodImplementation
    {
        public MethodImplementation(
            string signature,
            ImmutableArray<string> constraints,
            string body,
            bool isBound,
            ImmutableArray<string> documentationLines)
        {
            Signature = signature;
            Constraints = constraints;
            Body = body;
            IsBound = isBound;
            DocumentationLines = documentationLines;
        }

        public string Signature { get; }

        public ImmutableArray<string> Constraints { get; }

        public string Body { get; }

        public bool IsBound { get; }

        public ImmutableArray<string> DocumentationLines { get; }
    }

    /// <summary>
    /// Identifies whether a property binding was generated successfully or emitted as a throwing stub.
    /// </summary>
    internal enum PropertyImplementationKind
    {
        Generated,
        ThrowingStub
    }
}
