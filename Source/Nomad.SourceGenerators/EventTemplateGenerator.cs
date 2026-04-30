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

namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// According to the principal lead developer on the C# Language Team, Jared Parsons, this is not included
    /// in any version of the framework older than .NET v5, hence it must be defined manually here when we
    /// compile for unity.
    /// </summary>
    /// <remarks>
    /// From stackoverflow post https://stackoverflow.com/questions/64749385/predefined-type-system-runtime-compilerservices-isexternalinit-is-not-defined
    /// </remarks>
    public static class IsExternalInit { }
}

namespace Nomad.SourceGenerators
{
	[Generator]
	public sealed class EventNameGenerator : IIncrementalGenerator
	{
		private const string EventAttributeMetadataName = "Nomad.Core.Events.Event";

		private static readonly DiagnosticDescriptor EventTypeMustBePartial = new(
			id: "NOMEVENT001",
			title: "Event type must be partial",
			messageFormat: "Event type '{0}' must be partial so event metadata can be generated",
			category: "Nomad.Events",
			DiagnosticSeverity.Error,
			isEnabledByDefault: true);

		private static readonly DiagnosticDescriptor EventNameMustNotBeEmpty = new(
			id: "NOMEVENT002",
			title: "Event name must not be empty",
			messageFormat: "Event type '{0}' must declare a non-empty event name",
			category: "Nomad.Events",
			DiagnosticSeverity.Error,
			isEnabledByDefault: true);
		
		private static readonly DiagnosticDescriptor EventNameSpaceMustNotBeEmpty = new(
			id: "NOMEVENT003",
			title: "Event namespace must not be empty",
			messageFormat: "Event type '{0}' must declare a non-empty event namespace",
			category: "Nomad.Events",
			DiagnosticSeverity.Error,
			isEnabledByDefault: true);

		public void Initialize(IncrementalGeneratorInitializationContext context)
		{
			IncrementalValuesProvider<EventDeclaration?> eventDeclarations =
				context.SyntaxProvider.ForAttributeWithMetadataName(
					fullyQualifiedMetadataName: EventAttributeMetadataName,
					predicate: static (node, _) => node is StructDeclarationSyntax,
					transform: static (ctx, _) => GetEventDeclaration(ctx));

			IncrementalValuesProvider<EventDeclaration> validEventDeclarations =
				eventDeclarations.Where(static declaration => declaration is not null)!;

			context.RegisterSourceOutput(
				validEventDeclarations,
				static (ctx, declaration) => Execute(ctx, declaration));
		}

		private static EventDeclaration? GetEventDeclaration(GeneratorAttributeSyntaxContext context)
		{
			if (context.TargetNode is not StructDeclarationSyntax structDeclaration)
			{
				return null;
			}

			if (context.TargetSymbol is not INamedTypeSymbol typeSymbol)
			{
				return null;
			}

			string? eventName = null;
			string? eventNameSpace = null;

			foreach (AttributeData attribute in context.Attributes)
			{
				if (attribute.AttributeClass?.ToDisplayString() != EventAttributeMetadataName)
				{
					continue;
				}

				if (attribute.ConstructorArguments.Length >= 1)
				{
					eventName = attribute.ConstructorArguments[0].Value as string;
				}
				if (attribute.ConstructorArguments.Length >= 2)
				{
					eventNameSpace = attribute.ConstructorArguments[1].Value as string;
				}

				break;
			}

			string namespaceName = typeSymbol.ContainingNamespace.IsGlobalNamespace
				? string.Empty
				: typeSymbol.ContainingNamespace.ToDisplayString();

			bool isPartial = structDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword);
			bool isReadonly = structDeclaration.Modifiers.Any(SyntaxKind.ReadOnlyKeyword);

			string accessibility = typeSymbol.DeclaredAccessibility switch
			{
				Accessibility.Public => "public",
				Accessibility.Internal => "internal",
				Accessibility.Private => "private",
				Accessibility.Protected => "protected",
				Accessibility.ProtectedAndInternal => "private protected",
				Accessibility.ProtectedOrInternal => "protected internal",
				_ => "internal"
			};

			return new EventDeclaration(
				NamespaceName: namespaceName,
				TypeName: typeSymbol.Name,
				EventName: eventName ?? string.Empty,
				EventNameSpace: eventNameSpace ?? string.Empty,
				Accessibility: accessibility,
				IsPartial: isPartial,
				IsReadonly: isReadonly,
				Location: structDeclaration.Identifier.GetLocation());
		}

		private static void Execute(SourceProductionContext context, EventDeclaration declaration)
		{
			if (!declaration.IsPartial)
			{
				context.ReportDiagnostic(Diagnostic.Create(
					EventTypeMustBePartial,
					declaration.Location,
					declaration.TypeName));

				return;
			}

			if (string.IsNullOrWhiteSpace(declaration.EventName))
			{
				context.ReportDiagnostic(Diagnostic.Create(
					EventNameMustNotBeEmpty,
					declaration.Location,
					declaration.TypeName));

				return;
			}

			if (string.IsNullOrWhiteSpace(declaration.EventNameSpace))
			{
				context.ReportDiagnostic(Diagnostic.Create(
					EventNameSpaceMustNotBeEmpty,
					declaration.Location,
					declaration.TypeName));

				return;
			}

			string source = GenerateEventMetadataSource(declaration);

			context.AddSource(
				$"{declaration.TypeName}.EventMetadata.g.cs",
				SourceText.From(source, Encoding.UTF8));
		}

		private static string GenerateEventMetadataSource(EventDeclaration declaration)
		{
			string escapedEventName = SymbolDisplay.FormatLiteral(declaration.EventName, quote: true);
			string escapedEventNameSpace = SymbolDisplay.FormatLiteral(declaration.EventNameSpace, quote: true);

			string readonlyModifier = declaration.IsReadonly ? "readonly " : string.Empty;

			var source = new StringBuilder();

			source.AppendLine("// <auto-generated />");
			source.AppendLine("#nullable enable");
			source.AppendLine();

			if (!string.IsNullOrWhiteSpace(declaration.NamespaceName))
			{
				source.Append("namespace ");
				source.AppendLine(declaration.NamespaceName);
				source.AppendLine("{");
			}

			string indent = string.IsNullOrWhiteSpace(declaration.NamespaceName) ? string.Empty : "    ";

			source.Append(indent);
			source.Append(declaration.Accessibility);
			source.Append(' ');
			source.Append(readonlyModifier);
			source.Append("partial struct ");
			source.AppendLine(declaration.TypeName);

			source.Append(indent);
			source.AppendLine("{");

			source.Append(indent);
			source.Append("    public const string Name = ");
			source.Append(escapedEventName);
			source.AppendLine(";");

			source.Append(indent);
			source.Append("    public const string NameSpace = ");
			source.Append(escapedEventNameSpace);
			source.AppendLine(";");

			source.Append(indent);
			source.AppendLine("}");

			if (!string.IsNullOrWhiteSpace(declaration.NamespaceName))
			{
				source.AppendLine("}");
			}

			return source.ToString();
		}

		private sealed record EventDeclaration(
			string NamespaceName,
			string TypeName,
			string EventName,
			string EventNameSpace,
			string Accessibility,
			bool IsPartial,
			bool IsReadonly,
			Location Location);
	}
}