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
    internal enum EngineProjectKind
    {
        Unknown,
        Godot,
        Unity
    }

    internal sealed class TemplateGenerationModel
    {
        public TemplateGenerationModel(
            string generatedNamespace,
            string generatedClassName,
            Accessibility accessibility,
            string baseTypeName,
            string contractTypeName,
            ImmutableArray<string> additionalImplementedContractNames,
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
            BaseTypeName = baseTypeName;
            ContractTypeName = contractTypeName;
            AdditionalImplementedContractNames = additionalImplementedContractNames;
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

        public string BaseTypeName { get; }

        public string ContractTypeName { get; }

        public ImmutableArray<string> AdditionalImplementedContractNames { get; }

        public ImmutableArray<PropertyImplementation> PropertyImplementations { get; }

        public ImmutableArray<EventImplementation> EventImplementations { get; }

        public ImmutableArray<MethodImplementation> MethodImplementations { get; }

        public bool RequiresSpacingThemeConstant { get; }

        public bool UsesGameObjectAdapter { get; }

        public bool IsAsset { get; }

        public bool BaseInheritsUnityObject { get; }
    }

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

    internal sealed class TemplatePropertyDefinition
    {
        public TemplatePropertyDefinition(
            string name,
            string? typeName,
            string? godotGetterExpression,
            string? unityGetterExpression,
            string? godotSetterExpression,
            string? unitySetterExpression,
            bool isReadOnly)
        {
            Name = name;
            TypeName = typeName;
            GodotGetterExpression = godotGetterExpression;
            UnityGetterExpression = unityGetterExpression;
            GodotSetterExpression = godotSetterExpression;
            UnitySetterExpression = unitySetterExpression;
            IsReadOnly = isReadOnly;
        }

        public string Name { get; }

        public string? TypeName { get; }

        public string? GodotGetterExpression { get; }

        public string? UnityGetterExpression { get; }

        public string? GodotSetterExpression { get; }

        public string? UnitySetterExpression { get; }

        public bool IsReadOnly { get; }
    }

    internal sealed class TemplateMethodDefinition
    {
        public TemplateMethodDefinition(string name, string? godotMethodName, string? unityMethodName)
        {
            Name = name;
            GodotMethodName = godotMethodName;
            UnityMethodName = unityMethodName;
        }

        public string Name { get; }

        public string? GodotMethodName { get; }

        public string? UnityMethodName { get; }
    }

    internal sealed class TemplateTypeConversionDefinition
    {
        public TemplateTypeConversionDefinition(
            string agnosticTypeName,
            string? godotTypeName,
            string? godotToAgnosticExpression,
            string? agnosticToGodotExpression,
            string? unityTypeName,
            string? unityToAgnosticExpression,
            string? agnosticToUnityExpression)
        {
            AgnosticTypeName = agnosticTypeName;
            GodotTypeName = godotTypeName;
            GodotToAgnosticExpression = godotToAgnosticExpression;
            AgnosticToGodotExpression = agnosticToGodotExpression;
            UnityTypeName = unityTypeName;
            UnityToAgnosticExpression = unityToAgnosticExpression;
            AgnosticToUnityExpression = agnosticToUnityExpression;
        }

        public string AgnosticTypeName { get; }

        public string? GodotTypeName { get; }

        public string? GodotToAgnosticExpression { get; }

        public string? AgnosticToGodotExpression { get; }

        public string? UnityTypeName { get; }

        public string? UnityToAgnosticExpression { get; }

        public string? AgnosticToUnityExpression { get; }
    }

    internal sealed class TemplateEventDefinition
    {
        public TemplateEventDefinition(string name, string? payloadTypeName)
        {
            Name = name;
            PayloadTypeName = payloadTypeName;
        }

        public string Name { get; }

        public string? PayloadTypeName { get; }
    }

    internal sealed class PropertyImplementation
    {
        public PropertyImplementation(
            PropertyImplementationKind kind,
            string propertyTypeName,
            string propertyName,
            string? getterExpression,
            string? setterExpression,
            bool requiresNewKeyword,
            string? backingFieldDeclaration)
        {
            Kind = kind;
            PropertyTypeName = propertyTypeName;
            PropertyName = propertyName;
            GetterExpression = getterExpression;
            SetterExpression = setterExpression;
            RequiresNewKeyword = requiresNewKeyword;
            BackingFieldDeclaration = backingFieldDeclaration;
        }

        public PropertyImplementationKind Kind { get; }

        public string PropertyTypeName { get; }

        public string PropertyName { get; }

        public string? GetterExpression { get; }

        public string? SetterExpression { get; }

        public bool RequiresNewKeyword { get; }

        public string? BackingFieldDeclaration { get; }
    }

    internal sealed class EventImplementation
    {
        public EventImplementation(
            string eventName,
            string payloadTypeName,
            string fieldName,
            string registryEventName,
            ImmutableArray<string> hookStatements,
            bool requiresNewKeyword)
        {
            EventName = eventName;
            PayloadTypeName = payloadTypeName;
            FieldName = fieldName;
            RegistryEventName = registryEventName;
            HookStatements = hookStatements;
            RequiresNewKeyword = requiresNewKeyword;
        }

        public string EventName { get; }

        public string PayloadTypeName { get; }

        public string FieldName { get; }

        public string RegistryEventName { get; }

        public ImmutableArray<string> HookStatements { get; }

        public bool RequiresNewKeyword { get; }
    }

    internal sealed class MethodImplementation
    {
        public MethodImplementation(string signature, ImmutableArray<string> constraints, string body, bool isBound)
        {
            Signature = signature;
            Constraints = constraints;
            Body = body;
            IsBound = isBound;
        }

        public string Signature { get; }

        public ImmutableArray<string> Constraints { get; }

        public string Body { get; }

        public bool IsBound { get; }
    }

    internal enum PropertyImplementationKind
    {
        Generated,
        ThrowingStub
    }
}
