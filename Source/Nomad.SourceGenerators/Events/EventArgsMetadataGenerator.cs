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
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Nomad.SourceGenerators.Events
{
	[Generator]
	public sealed class EventArgsMetadataGenerator : IIncrementalGenerator
	{
		private const string EventAttributeMetadataName = "Nomad.Core.Events.EventAttribute";
		private const string EventPayloadAttributeMetadataName = "Nomad.Core.Events.EventPayloadAttribute";

		private static readonly DiagnosticDescriptor EventPropertyMustReturnGameEvent = new(
			id: "NOMEVENT001",
			title: "Event declaration must return IGameEvent<TEventArgs>",
			messageFormat: "Event declaration property '{0}' must return IGameEvent<TEventArgs>",
			category: "Nomad.Events",
			defaultSeverity: DiagnosticSeverity.Error,
			isEnabledByDefault: true);

		private static readonly DiagnosticDescriptor EventPayloadTypeMissing = new(
			id: "NOMEVENT002",
			title: "Event payload type is missing",
			messageFormat: "Event payload '{0}' on event '{1}' must provide either typeof(TConcrete) or a TypeName string",
			category: "Nomad.Events",
			defaultSeverity: DiagnosticSeverity.Error,
			isEnabledByDefault: true);

		private static readonly DiagnosticDescriptor EventPayloadNameMissing = new(
			id: "NOMEVENT003",
			title: "Event payload name is missing",
			messageFormat: "Event payload on event '{0}' must provide a non-empty payload name",
			category: "Nomad.Events",
			defaultSeverity: DiagnosticSeverity.Error,
			isEnabledByDefault: true);

		private static readonly DiagnosticDescriptor EventNamespaceMissing = new(
			id: "NOMEVENT004",
			title: "Event namespace is missing",
			messageFormat: "Event declaration '{0}' must provide a non-empty event namespace",
			category: "Nomad.Events",
			defaultSeverity: DiagnosticSeverity.Error,
			isEnabledByDefault: true);

		private static readonly DiagnosticDescriptor EventArgsNameMismatch = new(
			id: "NOMEVENT005",
			title: "Event args type name does not match generated type name",
			messageFormat: "Event declaration '{0}' returns '{1}', but the generator will emit '{2}' from the property name",
			category: "Nomad.Events",
			defaultSeverity: DiagnosticSeverity.Warning,
			isEnabledByDefault: true);

		private static readonly DiagnosticDescriptor EventPayloadDuplicateOrder = new(
			id: "NOMEVENT006",
			title: "Duplicate event payload order",
			messageFormat: "Event declaration '{0}' has multiple payloads using Order = {1}: {2}",
			category: "Nomad.Events",
			defaultSeverity: DiagnosticSeverity.Error,
			isEnabledByDefault: true);

		private static readonly DiagnosticDescriptor EventPayloadDuplicateName = new(
			id: "NOMEVENT007",
			title: "Duplicate event payload name",
			messageFormat: "Event declaration '{0}' has multiple payloads named '{1}'",
			category: "Nomad.Events",
			defaultSeverity: DiagnosticSeverity.Error,
			isEnabledByDefault: true);

		private static readonly DiagnosticDescriptor EventPayloadDuplicateDeclaration = new(
			id: "NOMEVENT008",
			title: "Duplicate event payload declaration",
			messageFormat: "Event declaration '{0}' has duplicate payload declaration '{1}' of type '{2}'",
			category: "Nomad.Events",
			defaultSeverity: DiagnosticSeverity.Error,
			isEnabledByDefault: true);

		public void Initialize(IncrementalGeneratorInitializationContext context)
		{
			IncrementalValuesProvider<EventDeclaration?> eventDeclarations =
				context.SyntaxProvider.ForAttributeWithMetadataName(
					fullyQualifiedMetadataName: EventAttributeMetadataName,
					predicate: static (syntaxNode, _) => syntaxNode is PropertyDeclarationSyntax,
					transform: static (syntaxContext, cancellationToken) =>
					{
						cancellationToken.ThrowIfCancellationRequested();

						if (syntaxContext.TargetSymbol is not IPropertySymbol propertySymbol)
						{
							return null;
						}

						AttributeData? eventAttribute = syntaxContext.Attributes.FirstOrDefault();
						if (eventAttribute is null)
						{
							return null;
						}

						return CreateEventDeclaration(propertySymbol, eventAttribute);
					});

			IncrementalValueProvider<ImmutableArray<EventDeclaration>> collectedDeclarations =
				eventDeclarations
					.Where(static declaration => declaration is not null)
					.Select(static (declaration, _) => declaration!)
					.Collect();

			context.RegisterSourceOutput(collectedDeclarations, static (sourceProductionContext, declarations) =>
			{
				foreach (EventDeclaration declaration in declarations)
				{
					ReportDiagnostics(sourceProductionContext, declaration);

					if (!declaration.CanGenerate)
					{
						continue;
					}

					sourceProductionContext.AddSource(
						$"{declaration.GeneratedTypeFileName}.g.cs",
						SourceText.From(GenerateEventArgsSource(declaration), Encoding.UTF8));
				}
			});
		}

		private static EventDeclaration CreateEventDeclaration(IPropertySymbol propertySymbol, AttributeData eventAttribute)
		{
			string eventName = propertySymbol.Name;
			string generatedTypeName = ReadEventPayloadNameOverride(eventAttribute) ?? eventName + "EventArgs";
			Location? location = propertySymbol.Locations.FirstOrDefault();
			string containingNamespace = GetNamespaceName(propertySymbol);
			string? eventNamespace = ReadEventNamespace(eventAttribute);
			bool hasValidEventNamespace = !string.IsNullOrWhiteSpace(eventNamespace);

			INamedTypeSymbol? declaredEventArgsType = TryGetGameEventArgsType(propertySymbol.Type);
			bool returnsGameEvent = declaredEventArgsType is not null;
			string? declaredEventArgsTypeName = declaredEventArgsType?.Name;
			bool eventArgsNameMatches = declaredEventArgsTypeName is null || declaredEventArgsTypeName == generatedTypeName;

			ImmutableArray<string> syntaxTypeParameterNames = GetEventArgsTypeParameterNamesFromSyntax(propertySymbol);
			ImmutableArray<ITypeParameterSymbol> typeParameterSymbols = GetGeneratedTypeParameters(
				declaredEventArgsType,
				propertySymbol,
				syntaxTypeParameterNames);

			ImmutableArray<string> typeParameters = typeParameterSymbols
				.Select(static parameter => parameter.Name)
				.Concat(syntaxTypeParameterNames)
				.Where(static name => !string.IsNullOrWhiteSpace(name))
				.Distinct(StringComparer.Ordinal)
				.ToImmutableArray();

			ImmutableArray<ITypeParameterSymbol> constraintTypeParameters = typeParameters
				.Select(name => FindTypeParameterByName(propertySymbol, name))
				.Where(static parameter => parameter is not null)
				.Select(static parameter => parameter!)
				.GroupBy(static parameter => parameter.Name, StringComparer.Ordinal)
				.Select(static group => group.First())
				.ToImmutableArray();

			ImmutableArray<string> constraints = constraintTypeParameters
				.SelectMany(GetTypeParameterConstraintClauses)
				.ToImmutableArray();

			ImmutableArray<EventPayloadDeclaration> payloads = propertySymbol
				.GetAttributes()
				.Where(static attribute => IsAttribute(attribute, EventPayloadAttributeMetadataName))
				.Select((attribute, index) => CreatePayloadDeclaration(eventName, attribute, index))
				.OrderBy(static payload => payload.Order)
				.ThenBy(static payload => payload.SourceIndex)
				.ToImmutableArray();

			return new EventDeclaration(
				ContainingNamespace: containingNamespace,
				EventNamespace: eventNamespace ?? string.Empty,
				EventName: eventName,
				GeneratedTypeName: generatedTypeName,
				GeneratedTypeFileName: CreateHintSafeFileName(containingNamespace, generatedTypeName),
				DeclaredEventArgsTypeName: declaredEventArgsTypeName ?? string.Empty,
				TypeParameters: typeParameters,
				ConstraintClauses: constraints,
				Payloads: payloads,
				ReturnsGameEvent: returnsGameEvent,
				HasValidEventNamespace: hasValidEventNamespace,
				EventArgsNameMatches: eventArgsNameMatches,
				Location: location);
		}

		private static EventPayloadDeclaration CreatePayloadDeclaration(string eventName, AttributeData attribute, int sourceIndex)
		{
			string payloadName = string.Empty;
			string typeName = string.Empty;
			int order = 0;
			Location? location = attribute.ApplicationSyntaxReference?.GetSyntax().GetLocation();

			if (attribute.ConstructorArguments.Length >= 1)
			{
				payloadName = attribute.ConstructorArguments[0].Value as string ?? string.Empty;
			}

			if (attribute.ConstructorArguments.Length >= 2)
			{
				TypedConstant typeArgument = attribute.ConstructorArguments[1];

				if (typeArgument.Kind == TypedConstantKind.Type && typeArgument.Value is ITypeSymbol typeSymbol)
				{
					typeName = typeSymbol.ToDisplayString(FullyQualifiedTypeFormat);
				}
				else if (typeArgument.Kind == TypedConstantKind.Primitive && typeArgument.Value is string explicitTypeName)
				{
					typeName = explicitTypeName.Trim();
				}
			}

			foreach (KeyValuePair<string, TypedConstant> namedArgument in attribute.NamedArguments)
			{
				if (namedArgument.Key == "Order" && namedArgument.Value.Value is int orderValue)
				{
					order = orderValue;
				}
				else if (namedArgument.Key == "TypeName" && namedArgument.Value.Value is string explicitTypeName)
				{
					typeName = explicitTypeName.Trim();
				}
			}

			return new EventPayloadDeclaration(
				EventName: eventName,
				Name: payloadName,
				TypeName: typeName,
				Order: order,
				SourceIndex: sourceIndex,
				Location: location);
		}

		private static void ReportDiagnostics(SourceProductionContext context, EventDeclaration declaration)
		{
			if (!declaration.ReturnsGameEvent)
			{
				context.ReportDiagnostic(Diagnostic.Create(
					EventPropertyMustReturnGameEvent,
					declaration.Location,
					declaration.EventName));
			}

			if (!declaration.HasValidEventNamespace)
			{
				context.ReportDiagnostic(Diagnostic.Create(
					EventNamespaceMissing,
					declaration.Location,
					declaration.EventName));
			}

			if (!declaration.EventArgsNameMatches)
			{
				context.ReportDiagnostic(Diagnostic.Create(
					EventArgsNameMismatch,
					declaration.Location,
					declaration.EventName,
					declaration.DeclaredEventArgsTypeName,
					declaration.GeneratedTypeName));
			}

			foreach (EventPayloadDeclaration payload in declaration.Payloads)
			{
				if (string.IsNullOrWhiteSpace(payload.Name))
				{
					context.ReportDiagnostic(Diagnostic.Create(
						EventPayloadNameMissing,
						payload.Location ?? declaration.Location,
						declaration.EventName));
				}

				if (string.IsNullOrWhiteSpace(payload.TypeName))
				{
					context.ReportDiagnostic(Diagnostic.Create(
						EventPayloadTypeMissing,
						payload.Location ?? declaration.Location,
						payload.Name,
						declaration.EventName));
				}
			}

			ReportDuplicatePayloadDiagnostics(context, declaration);
		}

		private static bool HasDuplicatePayloadErrors(ImmutableArray<EventPayloadDeclaration> payloads)
		{
			bool hasDuplicateOrder = payloads
				.GroupBy(static payload => payload.Order)
				.Any(static group => group.Count() > 1);

			if (hasDuplicateOrder)
			{
				return true;
			}

			bool hasDuplicateName = payloads
				.Where(static payload => !string.IsNullOrWhiteSpace(payload.Name))
				.GroupBy(static payload => payload.Name, StringComparer.Ordinal)
				.Any(static group => group.Count() > 1);

			if (hasDuplicateName)
			{
				return true;
			}

			return payloads
				.Where(static payload => !string.IsNullOrWhiteSpace(payload.Name) && !string.IsNullOrWhiteSpace(payload.TypeName))
				.GroupBy(static payload => payload.Name + "" + payload.TypeName, StringComparer.Ordinal)
				.Any(static group => group.Count() > 1);
		}

		private static void ReportDuplicatePayloadDiagnostics(SourceProductionContext context, EventDeclaration declaration)
		{
			foreach (IGrouping<int, EventPayloadDeclaration> orderGroup in declaration.Payloads.GroupBy(static payload => payload.Order))
			{
				EventPayloadDeclaration[] duplicates = orderGroup.ToArray();

				if (duplicates.Length <= 1)
				{
					continue;
				}

				context.ReportDiagnostic(Diagnostic.Create(
					EventPayloadDuplicateOrder,
					duplicates[1].Location ?? declaration.Location,
					declaration.EventName,
					orderGroup.Key,
					string.Join(", ", duplicates.Select(static payload => $"'{payload.Name}'"))));
			}

			foreach (IGrouping<string, EventPayloadDeclaration> nameGroup in declaration.Payloads
				.Where(static payload => !string.IsNullOrWhiteSpace(payload.Name))
				.GroupBy(static payload => payload.Name, StringComparer.Ordinal))
			{
				EventPayloadDeclaration[] duplicates = nameGroup.ToArray();

				if (duplicates.Length <= 1)
				{
					continue;
				}

				context.ReportDiagnostic(Diagnostic.Create(
					EventPayloadDuplicateName,
					duplicates[1].Location ?? declaration.Location,
					declaration.EventName,
					nameGroup.Key));
			}

			foreach (IGrouping<string, EventPayloadDeclaration> declarationGroup in declaration.Payloads
				.Where(static payload => !string.IsNullOrWhiteSpace(payload.Name) && !string.IsNullOrWhiteSpace(payload.TypeName))
				.GroupBy(static payload => payload.Name + "" + payload.TypeName, StringComparer.Ordinal))
			{
				EventPayloadDeclaration[] duplicates = declarationGroup.ToArray();

				if (duplicates.Length <= 1)
				{
					continue;
				}

				context.ReportDiagnostic(Diagnostic.Create(
					EventPayloadDuplicateDeclaration,
					duplicates[1].Location ?? declaration.Location,
					declaration.EventName,
					duplicates[0].Name,
					duplicates[0].TypeName));
			}
		}

		private static string GenerateEventArgsSource(EventDeclaration declaration)
		{
			StringBuilder builder = new StringBuilder();
			string eventNameLiteral = SymbolDisplay.FormatLiteral(declaration.EventName, quote: true);
			string eventNamespaceLiteral = SymbolDisplay.FormatLiteral(declaration.EventNamespace, quote: true);
			ImmutableArray<string> emittedTypeParameters = declaration.TypeParameters
				.Where(static parameter => !string.IsNullOrWhiteSpace(parameter))
				.ToImmutableArray();
			string typeParameters = emittedTypeParameters.Length == 0
				? string.Empty
				: "<" + string.Join(", ", emittedTypeParameters) + ">";

			builder.AppendLine("// <auto-generated />");
			builder.AppendLine("#nullable enable");
			builder.AppendLine();

			if (!string.IsNullOrWhiteSpace(declaration.EventNamespace))
			{
				builder.Append("namespace ");
				builder.Append(declaration.EventNamespace);
				builder.AppendLine();
				builder.AppendLine("{");
			}

			builder.Append("\tpublic readonly partial struct ");
			builder.Append(declaration.GeneratedTypeName);
			builder.AppendLine(typeParameters);

			foreach (string constraint in declaration.ConstraintClauses)
			{
				builder.Append("\t\t");
				builder.AppendLine(constraint);
			}

			builder.AppendLine("\t{");
			builder.Append("\t\tpublic const string Name = ");
			builder.Append(eventNameLiteral);
			builder.AppendLine(";");

			builder.Append("\t\tpublic const string NameSpace = ");
			builder.Append(eventNamespaceLiteral);
			builder.AppendLine(";");

			if (declaration.Payloads.Length > 0)
			{
				builder.AppendLine();
			}

			foreach (EventPayloadDeclaration payload in declaration.Payloads)
			{
				builder.Append("\t\tpublic readonly ");
				builder.Append(payload.TypeName);
				builder.Append(' ');
				builder.Append(payload.Name);
				builder.AppendLine(";");
			}

			if (declaration.Payloads.Length > 0)
			{
				builder.AppendLine();
				builder.Append("\t\tpublic ");
				builder.Append(declaration.GeneratedTypeName);
				builder.Append('(');

				for (int i = 0; i < declaration.Payloads.Length; i++)
				{
					EventPayloadDeclaration payload = declaration.Payloads[i];

					if (i > 0)
					{
						builder.Append(", ");
					}

					builder.Append(payload.TypeName);
					builder.Append(' ');
					builder.Append(ToParameterName(payload.Name));
				}

				builder.AppendLine(")");
				builder.AppendLine("\t\t{");

				foreach (EventPayloadDeclaration payload in declaration.Payloads)
				{
					builder.Append("\t\t\t");
					builder.Append(payload.Name);
					builder.Append(" = ");
					builder.Append(ToParameterName(payload.Name));
					builder.AppendLine(";");
				}

				builder.AppendLine("\t\t}");
			}

			builder.AppendLine("\t}");

			if (!string.IsNullOrWhiteSpace(declaration.ContainingNamespace))
			{
				builder.AppendLine("}");
			}

			return builder.ToString();
		}

		private static string? ReadEventNamespace(AttributeData eventAttribute)
		{
			if (eventAttribute.ConstructorArguments.Length >= 1)
			{
				return eventAttribute.ConstructorArguments[0].Value as string;
			}

			foreach (KeyValuePair<string, TypedConstant> namedArgument in eventAttribute.NamedArguments)
			{
				if ((namedArgument.Key == "NameSpace" || namedArgument.Key == "Namespace") && namedArgument.Value.Value is string nameSpace)
				{
					return nameSpace;
				}
			}

			return null;
		}

		private static string? ReadEventPayloadNameOverride(AttributeData eventAttribute)
		{
			foreach (KeyValuePair<string, TypedConstant> namedArgument in eventAttribute.NamedArguments)
			{
				if ((namedArgument.Key == "PayloadName" || namedArgument.Key == "EventPayloadName" || namedArgument.Key == "EventArgsName") && namedArgument.Value.Value is string payloadName)
				{
					payloadName = payloadName.Trim();

					if (payloadName.Length == 0)
					{
						return null;
					}

					int genericStart = payloadName.IndexOf('<');
					if (genericStart >= 0)
					{
						payloadName = payloadName.Substring(0, genericStart).Trim();
					}

					return payloadName.Length == 0 ? null : payloadName;
				}
			}

			return null;
		}

		private static ImmutableArray<ITypeParameterSymbol> GetGeneratedTypeParameters(
			INamedTypeSymbol? declaredEventArgsType,
			IPropertySymbol propertySymbol,
			ImmutableArray<string> syntaxTypeParameterNames)
		{
			ImmutableArray<ITypeParameterSymbol>.Builder builder = ImmutableArray.CreateBuilder<ITypeParameterSymbol>();

			if (declaredEventArgsType is not null)
			{
				foreach (ITypeSymbol typeArgument in declaredEventArgsType.TypeArguments)
				{
					if (typeArgument is ITypeParameterSymbol typeParameter && !string.IsNullOrWhiteSpace(typeParameter.Name))
					{
						builder.Add(typeParameter);
					}
				}

				foreach (ITypeParameterSymbol typeParameter in declaredEventArgsType.OriginalDefinition.TypeParameters)
				{
					if (!string.IsNullOrWhiteSpace(typeParameter.Name))
					{
						builder.Add(typeParameter);
					}
				}
			}

			foreach (string typeParameterName in syntaxTypeParameterNames)
			{
				ITypeParameterSymbol? typeParameter = FindTypeParameterByName(propertySymbol, typeParameterName);

				if (typeParameter is not null)
				{
					builder.Add(typeParameter);
				}
			}

			return builder
				.Where(static parameter => !string.IsNullOrWhiteSpace(parameter.Name))
				.GroupBy(static parameter => parameter.Name, StringComparer.Ordinal)
				.Select(static group => group.First())
				.ToImmutableArray();
		}

		private static ImmutableArray<string> GetEventArgsTypeParameterNamesFromSyntax(IPropertySymbol propertySymbol)
		{
			foreach (SyntaxReference syntaxReference in propertySymbol.DeclaringSyntaxReferences)
			{
				if (syntaxReference.GetSyntax() is not PropertyDeclarationSyntax propertyDeclaration)
				{
					continue;
				}

				GenericNameSyntax? gameEventTypeSyntax = FindGenericName(propertyDeclaration.Type, "IGameEvent");

				if (gameEventTypeSyntax is null || gameEventTypeSyntax.TypeArgumentList.Arguments.Count != 1)
				{
					continue;
				}

				TypeSyntax eventArgsTypeSyntax = gameEventTypeSyntax.TypeArgumentList.Arguments[0];

				if (eventArgsTypeSyntax is QualifiedNameSyntax qualifiedNameSyntax)
				{
					eventArgsTypeSyntax = qualifiedNameSyntax.Right;
				}
				else if (eventArgsTypeSyntax is AliasQualifiedNameSyntax aliasQualifiedNameSyntax)
				{
					eventArgsTypeSyntax = aliasQualifiedNameSyntax.Name;
				}

				if (eventArgsTypeSyntax is not GenericNameSyntax genericEventArgsSyntax)
				{
					continue;
				}

				return genericEventArgsSyntax.TypeArgumentList.Arguments
					.Select(static argument => argument.ToString().Trim())
					.Where(static name => !string.IsNullOrWhiteSpace(name))
					.Distinct(StringComparer.Ordinal)
					.ToImmutableArray();
			}

			return ImmutableArray<string>.Empty;
		}

		private static GenericNameSyntax? FindGenericName(TypeSyntax typeSyntax, string name)
		{
			if (typeSyntax is GenericNameSyntax genericNameSyntax && genericNameSyntax.Identifier.ValueText == name)
			{
				return genericNameSyntax;
			}

			if (typeSyntax is QualifiedNameSyntax qualifiedNameSyntax)
			{
				return FindGenericName(qualifiedNameSyntax.Right, name) ?? FindGenericName(qualifiedNameSyntax.Left, name);
			}

			if (typeSyntax is AliasQualifiedNameSyntax aliasQualifiedNameSyntax)
			{
				return FindGenericName(aliasQualifiedNameSyntax.Name, name);
			}

			if (typeSyntax is NullableTypeSyntax nullableTypeSyntax)
			{
				return FindGenericName(nullableTypeSyntax.ElementType, name);
			}

			return null;
		}

		private static ITypeParameterSymbol? FindTypeParameterByName(IPropertySymbol propertySymbol, string typeParameterName)
		{
			if (string.IsNullOrWhiteSpace(typeParameterName))
			{
				return null;
			}

			for (INamedTypeSymbol? type = propertySymbol.ContainingType; type is not null; type = type.ContainingType)
			{
				foreach (ITypeParameterSymbol typeParameter in type.TypeParameters)
				{
					if (typeParameter.Name == typeParameterName)
					{
						return typeParameter;
					}
				}
			}

			if (propertySymbol.GetMethod is not null)
			{
				foreach (ITypeParameterSymbol typeParameter in propertySymbol.GetMethod.TypeParameters)
				{
					if (typeParameter.Name == typeParameterName)
					{
						return typeParameter;
					}
				}
			}

			return null;
		}

		private static INamedTypeSymbol? TryGetGameEventArgsType(ITypeSymbol propertyType)
		{
			if (propertyType is not INamedTypeSymbol namedType)
			{
				return null;
			}

			if (namedType.Name == "IGameEvent" && namedType.TypeArguments.Length == 1 && namedType.TypeArguments[0] is INamedTypeSymbol directArgsType)
			{
				return directArgsType;
			}

			foreach (INamedTypeSymbol interfaceType in namedType.AllInterfaces)
			{
				if (interfaceType.Name == "IGameEvent" && interfaceType.TypeArguments.Length == 1 && interfaceType.TypeArguments[0] is INamedTypeSymbol interfaceArgsType)
				{
					return interfaceArgsType;
				}
			}

			return null;
		}

		private static IEnumerable<string> GetTypeParameterConstraintClauses(ITypeParameterSymbol typeParameter)
		{
			List<string> constraints = new List<string>();

			if (typeParameter.HasNotNullConstraint)
			{
				constraints.Add("notnull");
			}

			if (typeParameter.HasUnmanagedTypeConstraint)
			{
				constraints.Add("unmanaged");
			}
			else if (typeParameter.HasValueTypeConstraint)
			{
				constraints.Add("struct");
			}
			else if (typeParameter.HasReferenceTypeConstraint)
			{
				constraints.Add(typeParameter.ReferenceTypeConstraintNullableAnnotation == NullableAnnotation.Annotated ? "class?" : "class");
			}

			foreach (ITypeSymbol constraintType in typeParameter.ConstraintTypes)
			{
				constraints.Add(constraintType.ToDisplayString(FullyQualifiedTypeFormat));
			}

			if (typeParameter.HasConstructorConstraint)
			{
				constraints.Add("new()");
			}

			if (constraints.Count == 0)
			{
				yield break;
			}

			yield return $"where {typeParameter.Name} : {string.Join(", ", constraints)}";
		}

		private static string GetNamespaceName(ISymbol symbol)
		{
			INamespaceSymbol? namespaceSymbol = symbol.ContainingNamespace;

			if (namespaceSymbol is null || namespaceSymbol.IsGlobalNamespace)
			{
				return string.Empty;
			}

			return namespaceSymbol.ToDisplayString();
		}

		private static bool IsAttribute(AttributeData attribute, string metadataName)
		{
			return string.Equals(attribute.AttributeClass?.ToDisplayString(), metadataName, StringComparison.Ordinal);
		}

		private static string ToParameterName(string memberName)
		{
			if (string.IsNullOrWhiteSpace(memberName))
			{
				return "value";
			}

			if (memberName.Length == 1)
			{
				return char.ToLowerInvariant(memberName[0]).ToString();
			}

			return char.ToLowerInvariant(memberName[0]) + memberName.Substring(1);
		}

		private static string CreateHintSafeFileName(string namespaceName, string typeName)
		{
			string fullName = string.IsNullOrWhiteSpace(namespaceName)
				? typeName
				: namespaceName + "." + typeName;

			StringBuilder builder = new StringBuilder(fullName.Length);

			foreach (char value in fullName)
			{
				builder.Append(char.IsLetterOrDigit(value) || value == '_' || value == '.' ? value : '_');
			}

			return builder.ToString();
		}

		private static readonly SymbolDisplayFormat FullyQualifiedTypeFormat = new(
			globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included,
			typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
			genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
			miscellaneousOptions:
				SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers |
				SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);

		private sealed record EventDeclaration(
			string ContainingNamespace,
			string EventNamespace,
			string EventName,
			string GeneratedTypeName,
			string GeneratedTypeFileName,
			string DeclaredEventArgsTypeName,
			ImmutableArray<string> TypeParameters,
			ImmutableArray<string> ConstraintClauses,
			ImmutableArray<EventPayloadDeclaration> Payloads,
			bool ReturnsGameEvent,
			bool HasValidEventNamespace,
			bool EventArgsNameMatches,
			Location? Location)
		{
			public bool CanGenerate =>
				ReturnsGameEvent &&
				HasValidEventNamespace &&
				Payloads.All(static payload => !string.IsNullOrWhiteSpace(payload.Name) && !string.IsNullOrWhiteSpace(payload.TypeName)) &&
				!HasDuplicatePayloadErrors(Payloads);
		}

		private sealed record EventPayloadDeclaration(
			string EventName,
			string Name,
			string TypeName,
			int Order,
			int SourceIndex,
			Location? Location);
	}
}