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

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nomad.SourceGenerator
{
    /// <summary>
    /// 
    /// </summary>
    [Generator]
    public class EngineBaseGenerator : ISourceGenerator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxReceiver is not SyntaxReceiver receiver)
            {
                return;
            }

            // Detect engine by referenced assemblies
            bool isGodot = context.Compilation.ReferencedAssemblyNames.Any(r => r.Name.Contains("GodotSharp"));
            bool isUnity = context.Compilation.ReferencedAssemblyNames.Any(r => r.Name.Contains("UnityEngine"));

            if (!isGodot && !isUnity)
            {
                return; // No engine assemblies found – nothing to generate
            }

            foreach (var classDecl in receiver.CandidateClasses)
            {
                var model = context.Compilation.GetSemanticModel(classDecl.SyntaxTree);
				if (model.GetDeclaredSymbol(classDecl) is not INamedTypeSymbol classSymbol)
                {
                    continue;
                }

				var attribute = classSymbol.GetAttributes().FirstOrDefault(attr => attr.AttributeClass?.Name == "EngineBaseClassAttribute");
                if (attribute == null)
                {
                    continue;
                }

                // Extract base type names from attribute constructor arguments
                if (attribute.ConstructorArguments.Length < 2)
                {
                    continue;
                }
                string godotBase = attribute.ConstructorArguments[0].Value as string;
                string unityBase = attribute.ConstructorArguments[1].Value as string;
                string baseTypeName = isGodot ? godotBase : unityBase;
                if (string.IsNullOrEmpty(baseTypeName))
                {
                    continue;
                }

                // Resolve the base type symbol to check for lifecycle support
                INamedTypeSymbol baseTypeSymbol = null;
                if (isGodot)
                {
                    baseTypeSymbol = context.Compilation.GetTypeByMetadataName($"Godot.{baseTypeName}");
                    // If not found, you could try other namespaces (e.g., Godot.Collections) – but for Node types it's usually Godot.
                }
                else if (isUnity)
                {
                    baseTypeSymbol = context.Compilation.GetTypeByMetadataName($"UnityEngine.{baseTypeName}");
                }

                bool supportsLifecycle = false;
                if (baseTypeSymbol != null)
                {
                    if (isGodot)
                    {
                        var nodeSymbol = context.Compilation.GetTypeByMetadataName("Godot.Node");
                        if (nodeSymbol != null)
                        {
                            supportsLifecycle = InheritsFrom(baseTypeSymbol, nodeSymbol);
                        }
                    }
                    else // Unity
                    {
                        var monoBehaviourSymbol = context.Compilation.GetTypeByMetadataName("UnityEngine.MonoBehaviour");
                        if (monoBehaviourSymbol != null)
                        {
                            supportsLifecycle = InheritsFrom(baseTypeSymbol, monoBehaviourSymbol);
                        }
                    }
                }

                string namespaceName = classSymbol.ContainingNamespace.ToDisplayString();
                string className = classSymbol.Name;

                // Generate the partial class
                string source = GeneratePartialClass(namespaceName, className, baseTypeName, isGodot, supportsLifecycle);
                string hintName = $"{className}.{(isGodot ? "Godot" : "Unity")}.g.cs";
                context.AddSource(hintName, SourceText.From(source, Encoding.UTF8));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="derived"></param>
        /// <param name="baseType"></param>
        /// <returns></returns>
        private static bool InheritsFrom(INamedTypeSymbol derived, INamedTypeSymbol baseType)
        {
            var current = derived.BaseType;
            while (current != null)
            {
                if (SymbolEqualityComparer.Default.Equals(current, baseType))
                {
                    return true;
                }
                current = current.BaseType;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="namespaceName"></param>
        /// <param name="className"></param>
        /// <param name="baseType"></param>
        /// <param name="isGodot"></param>
        /// <param name="supportsLifecycle"></param>
        /// <returns></returns>
        private static string GeneratePartialClass(string namespaceName, string className, string baseType, bool isGodot, bool supportsLifecycle)
        {
            var sb = new StringBuilder();
            sb.AppendLine("// <auto-generated/>");
            if (isGodot)
            {
                sb.AppendLine("using Godot;");
            }
            else
            {
                sb.AppendLine("using UnityEngine;");
            }
            sb.AppendLine();
            sb.AppendLine($"namespace {namespaceName}");
            sb.AppendLine("{");
            sb.AppendLine($"    public abstract partial class {className} : {baseType}");
            sb.AppendLine("    {");

            if (supportsLifecycle)
            {
                if (isGodot)
                {
                    sb.AppendLine("        public sealed override void _Ready() => Init();");
                    sb.AppendLine("        public sealed override void _ExitTree() => Shutdown();");
                    sb.AppendLine("        public sealed override void _Process(double delta) => Update((float)delta);");
                    sb.AppendLine("        public sealed override void _PhysicsProcess(double delta) => FixedUpdate((float)delta);");
                }
                else // Unity
                {
                    sb.AppendLine("        private void Awake() => Init();");
                    sb.AppendLine("        private void OnDestroy() => Shutdown();");
                    sb.AppendLine("        private void Update() => Update(UnityEngine.Time.deltaTime);");
                    sb.AppendLine("        private void FixedUpdate() => FixedUpdate(UnityEngine.Time.fixedDeltaTime);");
                }
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        private class SyntaxReceiver : ISyntaxReceiver
        {
            public readonly List<ClassDeclarationSyntax> CandidateClasses = new List<ClassDeclarationSyntax>();
            
            /// <summary>
            /// 
            /// </summary>
            /// <param name="syntaxNode"></param>
            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (syntaxNode is ClassDeclarationSyntax cds && cds.AttributeLists.Count > 0)
                {
                    CandidateClasses.Add(cds);
                }
            }
        }
    }
}