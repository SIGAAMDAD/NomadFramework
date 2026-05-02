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

using System.Text;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Nomad.SourceGenerators.EngineTemplates
{
    /// <summary>
    /// Defines the contract for rendering a fully-populated template generation model into source code.
    /// </summary>
    internal interface IEngineTemplateRenderer
    {
        string Render(TemplateGenerationModel model);
    }

    /// <summary>
    /// Provides shared rendering helpers for engine-specific template renderers.
    /// </summary>
    internal abstract class EngineTemplateRendererBase : IEngineTemplateRenderer
    {
        protected const string EventNamespace = "Nomad.EngineUtils";

        public abstract string Render(TemplateGenerationModel model);

        /// <summary>
        /// Appends the standard MPL license header and nullable enable directive.
        /// </summary>
        protected static void AppendFileHeader(StringBuilder builder)
        {
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
            builder.AppendLine("#nullable enable");
            builder.AppendLine();
        }

        /// <summary>
        /// Appends the namespace opening and class declaration with documentation, base types, and contracts.
        /// </summary>
        protected static void AppendNamespaceStart(StringBuilder builder, TemplateGenerationModel model)
        {
            builder.AppendLine("namespace " + model.GeneratedNamespace);
            builder.AppendLine("{");
            AppendDocumentationComment(builder, "    ", model.ClassDocumentationLines);
            builder.Append("    ");
            builder.Append(FormatAccessibility(model.Accessibility));
            builder.Append(" partial class ");
            builder.Append(model.GeneratedClassName);
            builder.Append(" : ");
            builder.Append(model.BaseTypeName);
            builder.Append(", ");
            builder.Append(model.ContractTypeName);

            foreach (var additionalImplementedContractName in model.AdditionalImplementedContractNames)
            {
                builder.Append(", ");
                builder.Append(additionalImplementedContractName);
            }

            builder.AppendLine();
            builder.AppendLine("    {");
        }

        /// <summary>
        /// Appends the closing braces for namespace and class, and disables nullable.
        /// </summary>
        protected static void AppendNamespaceEnd(StringBuilder builder)
        {
            builder.AppendLine("    }");
            builder.AppendLine("}");
            builder.AppendLine("#nullable disable");
        }

        /// <summary>
        /// Appends property members with their getter/setter expressions and optional backing fields.
        /// </summary>
        protected static void AppendPropertyMembers(StringBuilder builder, TemplateGenerationModel model)
        {
            foreach (var propertyImplementation in model.PropertyImplementations)
            {
                AppendDocumentationComment(builder, "        ", propertyImplementation.DocumentationLines);
                builder.Append("        public ");
                if (propertyImplementation.RequiresNewKeyword)
                {
                    builder.Append("new ");
                }

                builder.Append(propertyImplementation.PropertyTypeName);
                builder.Append(' ');
                builder.Append(propertyImplementation.PropertyName);
                builder.AppendLine();
                builder.AppendLine("        {");

                if (propertyImplementation.GetterExpression is not null)
                {
                    AppendAccessor(builder, "get", propertyImplementation.GetterExpression);
                }

                if (propertyImplementation.SetterExpression is not null)
                {
                    AppendAccessor(builder, "set", propertyImplementation.SetterExpression);
                }

                builder.AppendLine("        }");

                if (propertyImplementation.BackingFieldDeclaration is not null)
                {
                    builder.AppendLine("        " + propertyImplementation.BackingFieldDeclaration);
                }

                builder.AppendLine();
            }
        }

        private static void AppendAccessor(StringBuilder builder, string accessorKeyword, string accessorExpression)
        {
            if (!RequiresBlockAccessor(accessorExpression))
            {
                builder.AppendLine("            " + accessorKeyword + " => " + accessorExpression + ";");
                return;
            }

            builder.AppendLine("            " + accessorKeyword);
            builder.AppendLine("            {");

            foreach (var line in accessorExpression.Split(new[] { "\r\n", "\n" }, System.StringSplitOptions.None))
            {
                if (line.Length == 0)
                {
                    builder.AppendLine();
                    continue;
                }

                builder.AppendLine("                " + line);
            }

            builder.AppendLine("            }");
        }

        private static bool RequiresBlockAccessor(string accessorExpression)
        {
            var trimmedExpression = accessorExpression.TrimStart();
            return trimmedExpression.StartsWith("if ", System.StringComparison.Ordinal) ||
                   trimmedExpression.StartsWith("if(", System.StringComparison.Ordinal) ||
                   trimmedExpression.StartsWith("switch ", System.StringComparison.Ordinal) ||
                   trimmedExpression.StartsWith("switch(", System.StringComparison.Ordinal) ||
                   trimmedExpression.StartsWith("for ", System.StringComparison.Ordinal) ||
                   trimmedExpression.StartsWith("for(", System.StringComparison.Ordinal) ||
                   trimmedExpression.StartsWith("foreach ", System.StringComparison.Ordinal) ||
                   trimmedExpression.StartsWith("while ", System.StringComparison.Ordinal) ||
                   trimmedExpression.StartsWith("while(", System.StringComparison.Ordinal) ||
                   trimmedExpression.StartsWith("{", System.StringComparison.Ordinal);
        }

        /// <summary>
        /// Appends constant and static readonly field members.
        /// </summary>
        protected static void AppendConstantMembers(StringBuilder builder, TemplateGenerationModel model)
        {
            foreach (var constantImplementation in model.ConstantImplementations)
            {
                AppendDocumentationComment(builder, "        ", constantImplementation.DocumentationLines);
                builder.Append("        public ");
                if (constantImplementation.RequiresNewKeyword)
                {
                    builder.Append("new ");
                }

                builder.Append(constantImplementation.IsConstant ? "const " : "static readonly ");
                builder.Append(constantImplementation.TypeName);
                builder.Append(' ');
                builder.Append(constantImplementation.Name);
                builder.Append(" = ");
                builder.Append(constantImplementation.ValueExpression);
                builder.AppendLine(";");
                builder.AppendLine();
            }
        }

        /// <summary>
        /// Appends event members and their backing fields.
        /// </summary>
        protected static void AppendEventMembers(StringBuilder builder, TemplateGenerationModel model)
        {
            foreach (var eventImplementation in model.EventImplementations)
            {
                AppendDocumentationComment(builder, "        ", eventImplementation.DocumentationLines);
                builder.Append("        public ");
                if (eventImplementation.RequiresNewKeyword)
                {
                    builder.Append("new ");
                }

                builder.Append("global::Nomad.Core.Events.IGameEvent<");
                builder.Append(eventImplementation.PayloadTypeName);
                builder.Append("> ");
                builder.Append(eventImplementation.EventName);
                builder.Append(" => ");
                builder.Append(eventImplementation.FieldName);
                builder.AppendLine(";");

                builder.Append("        private global::Nomad.Core.Events.IGameEvent<");
                builder.Append(eventImplementation.PayloadTypeName);
                builder.Append("> ");
                builder.Append(eventImplementation.FieldName);
                builder.AppendLine(" = default!;");
                builder.AppendLine();
            }
        }

        /// <summary>
        /// Appends method members with their signatures, constraints, and bodies.
        /// </summary>
        protected static void AppendMethodMembers(StringBuilder builder, TemplateGenerationModel model)
        {
            foreach (var methodImplementation in model.MethodImplementations)
            {
                AppendDocumentationComment(builder, "        ", methodImplementation.DocumentationLines);
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
        }

        /// <summary>
        /// Appends event initialization statements using the game event registry (constant string form).
        /// </summary>
        protected static void AppendEventInitialization(StringBuilder builder, TemplateGenerationModel model, string indent)
        {
            foreach (var eventImplementation in model.EventImplementations)
            {
                builder.Append(indent);
                builder.Append(eventImplementation.FieldName);
                builder.Append(" = global::Nomad.Events.Globals.GameEventRegistry.GetEvent<");
                builder.Append(eventImplementation.PayloadTypeName);
                builder.Append(">(\"");
                builder.Append("{GetHashCode()}:");
                builder.Append(eventImplementation.RegistryEventName);
                builder.Append("\", \"");
                builder.Append(EventNamespace);
                builder.AppendLine("\");");
            }
        }

        /// <summary>
        /// Appends event initialization statements using interpolated strings for dynamic parts.
        /// </summary>
        protected static void AppendEventInitializationWithInterpolation(StringBuilder builder, TemplateGenerationModel model, string indent)
        {
            foreach (var eventImplementation in model.EventImplementations)
            {
                builder.Append(indent);
                builder.Append(eventImplementation.FieldName);
                builder.Append(" = global::Nomad.Events.Globals.GameEventRegistry.GetEvent<");
                builder.Append(eventImplementation.PayloadTypeName);
                builder.Append(">($\"");
                builder.Append("{GetHashCode()}:");
                builder.Append(eventImplementation.RegistryEventName);
                builder.Append("\", \"");
                builder.Append(EventNamespace);
                builder.AppendLine("\");");
            }
        }

        /// <summary>
        /// Appends event hook statements (e.g., subscribing to engine events).
        /// </summary>
        protected static void AppendEventHooks(StringBuilder builder, TemplateGenerationModel model, string indent)
        {
            foreach (var eventImplementation in model.EventImplementations)
            {
                foreach (var hookStatement in eventImplementation.HookStatements)
                {
                    builder.Append(indent);
                    builder.AppendLine(hookStatement);
                }
            }
        }

        /// <summary>
        /// Appends event disposal statements to release event resources.
        /// </summary>
        protected static void AppendEventDisposal(StringBuilder builder, TemplateGenerationModel model, string indent)
        {
            foreach (var eventImplementation in model.EventImplementations)
            {
                builder.Append(indent);
                builder.Append(eventImplementation.FieldName);
                builder.AppendLine("?.Dispose();");
            }
        }

        /// <summary>
        /// Appends the standard GameObject lifecycle hook methods (OnInit, OnShutdown, OnUpdate, OnPhysicsUpdate).
        /// </summary>
        protected static void AppendGameObjectLifecycleHooks(StringBuilder builder)
        {
            AppendSummaryDocumentation(builder, "        ", "Invoked after the generated engine object has completed initialization.");
            builder.AppendLine("        protected virtual void OnInit()");
            builder.AppendLine("        {");
            builder.AppendLine("            _impl.OnInit();");
            builder.AppendLine("        }");
            builder.AppendLine();
            AppendSummaryDocumentation(builder, "        ", "Invoked when the generated engine object is shutting down.");
            builder.AppendLine("        protected virtual void OnShutdown()");
            builder.AppendLine("        {");
            builder.AppendLine("            _impl.OnShutdown();");
            builder.AppendLine("        }");
            builder.AppendLine();
            AppendSummaryDocumentation(builder, "        ", "Invoked once per frame by the generated engine wrapper.");
            builder.AppendLine("        protected virtual void OnUpdate(float delta)");
            builder.AppendLine("        {");
            builder.AppendLine("            _impl.OnUpdate(delta);");
            builder.AppendLine("        }");
            builder.AppendLine();
            AppendSummaryDocumentation(builder, "        ", "Invoked once per physics tick by the generated engine wrapper.");
            builder.AppendLine("        protected virtual void OnPhysicsUpdate(float delta)");
            builder.AppendLine("        {");
            builder.AppendLine("            _impl.OnPhysicsUpdate(delta);");
            builder.AppendLine("        }");
            builder.AppendLine();
        }

        /// <summary>
        /// Appends the OnDispose hook for asset wrappers.
        /// </summary>
        protected static void AppendAssetDisposeHook(StringBuilder builder)
        {
            AppendSummaryDocumentation(builder, "        ", "Invoked before the generated asset wrapper releases engine resources.");
            builder.AppendLine("        protected virtual void OnDispose()");
            builder.AppendLine("        {");
            builder.AppendLine("        }");
            builder.AppendLine();
        }

        /// <summary>
        /// Appends an XML documentation comment composed of multiple lines.
        /// </summary>
        protected static void AppendDocumentationComment(
            StringBuilder builder,
            string indent,
            ImmutableArray<string> documentationLines)
        {
            if (documentationLines.IsDefaultOrEmpty)
            {
                return;
            }

            foreach (var documentationLine in documentationLines)
            {
                builder.Append(indent);
                builder.Append("/// ");
                builder.AppendLine(documentationLine);
            }
        }

        /// <summary>
        /// Appends a simple <summary> XML documentation comment.
        /// </summary>
        protected static void AppendSummaryDocumentation(StringBuilder builder, string indent, string summary)
        {
            builder.Append(indent);
            builder.AppendLine("/// <summary>");
            builder.Append(indent);
            builder.Append("/// ");
            builder.AppendLine(summary);
            builder.Append(indent);
            builder.AppendLine("/// </summary>");
        }

        /// <summary>
        /// Converts Roslyn Accessibility to a valid C# accessibility keyword.
        /// </summary>
        private static string FormatAccessibility(Accessibility accessibility)
            => accessibility switch
            {
                Accessibility.Public => "public",
                Accessibility.Internal => "public",
                Accessibility.Private => "private",
                Accessibility.Protected => "protected",
                Accessibility.ProtectedAndInternal => "private protected",
                Accessibility.ProtectedOrInternal => "protected internal",
                _ => "internal"
            };
    }

    /// <summary>
    /// Renders engine wrapper source for Godot projects.
    /// </summary>
    internal sealed class GodotTemplateRenderer : EngineTemplateRendererBase
    {
        /// <summary>
        /// Renders a complete Godot engine wrapper class using the provided generation model.
        /// </summary>
        public override string Render(TemplateGenerationModel model)
        {
            var builder = new StringBuilder();
            AppendFileHeader(builder);
            AppendNamespaceStart(builder, model);

            if (model.RequiresSpacingThemeConstant)
            {
                builder.AppendLine("        private static readonly global::Godot.StringName SeparationThemeConstantName = \"separation\";");
                builder.AppendLine();
            }

            AppendConstantMembers(builder, model);
            AppendPropertyMembers(builder, model);

            if (model.UsesGameObjectAdapter)
            {
                builder.AppendLine("        private readonly global::Nomad.EngineUtils.GodotGameObject _impl;");
                builder.AppendLine("        private bool _isDisposed;");
                builder.AppendLine();
            }

            AppendEventMembers(builder, model);

            if (model.UsesGameObjectAdapter)
            {
                AppendSummaryDocumentation(builder, "        ", "Initializes the generated engine wrapper.");
                builder.AppendLine("        public " + model.GeneratedClassName + "()");
                builder.AppendLine("        {");
                builder.AppendLine("            _impl = new global::Nomad.EngineUtils.GodotGameObject(this);");
                builder.AppendLine("            Ready += () => _Ready();");
                builder.AppendLine("            TreeExited += () => _ExitTree();");
                builder.AppendLine("        }");
                builder.AppendLine();

                AppendSummaryDocumentation(builder, "        ", "Initializes generated events and invokes startup hooks.");
                builder.AppendLine("        public sealed override void _Ready()");
                builder.AppendLine("        {");
                builder.AppendLine("            base._Ready();");
                builder.AppendLine();
                builder.AppendLine("            GetTree().ProcessFrame += () => _Process(GetProcessDeltaTime());");
                builder.AppendLine("            GetTree().PhysicsFrame += () => _PhysicsProcess(GetPhysicsProcessDeltaTime());");
                builder.AppendLine();

                AppendEventInitializationWithInterpolation(builder, model, "            ");
                if (model.EventImplementations.Length > 0)
                {
                    builder.AppendLine();
                }

                AppendEventHooks(builder, model, "            ");
                if (model.EventImplementations.Length > 0)
                {
                    builder.AppendLine();
                }

                builder.AppendLine("            _impl.OnInit();");
                builder.AppendLine("            OnInit();");
                builder.AppendLine("        }");
                builder.AppendLine();

                AppendSummaryDocumentation(builder, "        ", "Invokes the generated frame update pipeline.");
                builder.AppendLine("        public sealed override void _Process(double delta)");
                builder.AppendLine("        {");
                builder.AppendLine("            base._Process(delta);");
                builder.AppendLine();
                builder.AppendLine("            float deltaTime = (float)delta;");
                builder.AppendLine("            _impl.OnUpdate(deltaTime);");
                builder.AppendLine("            OnUpdate(deltaTime);");
                builder.AppendLine("        }");
                builder.AppendLine();

                AppendSummaryDocumentation(builder, "        ", "Invokes the generated physics update pipeline.");
                builder.AppendLine("        public sealed override void _PhysicsProcess(double delta)");
                builder.AppendLine("        {");
                builder.AppendLine("            base._PhysicsProcess(delta);");
                builder.AppendLine();
                builder.AppendLine("            float deltaTime = (float)delta;");
                builder.AppendLine("            _impl.OnPhysicsUpdate(deltaTime);");
                builder.AppendLine("            OnPhysicsUpdate(deltaTime);");
                builder.AppendLine("        }");
                builder.AppendLine();

                AppendSummaryDocumentation(builder, "        ", "Invokes shutdown hooks and disposes generated events.");
                builder.AppendLine("        public sealed override void _ExitTree()");
                builder.AppendLine("        {");
                builder.AppendLine("            base._ExitTree();");
                builder.AppendLine();
                builder.AppendLine("            DisposeCore();");
                builder.AppendLine("        }");
                builder.AppendLine();

                AppendSummaryDocumentation(builder, "        ", "Releases generated engine resources for the wrapper.");
                builder.AppendLine("        public new void Dispose()");
                builder.AppendLine("        {");
                builder.AppendLine("            DisposeCore();");
                builder.AppendLine("            base.Dispose();");
                builder.AppendLine("        }");
                builder.AppendLine();

                AppendSummaryDocumentation(builder, "        ", "Performs one-time disposal for the generated engine wrapper.");
                builder.AppendLine("        private void DisposeCore()");
                builder.AppendLine("        {");
                builder.AppendLine("            if (_isDisposed)");
                builder.AppendLine("            {");
                builder.AppendLine("                return;");
                builder.AppendLine("            }");
                builder.AppendLine();
                builder.AppendLine("            _isDisposed = true;");
                builder.AppendLine();
                builder.AppendLine("            _impl.OnShutdown();");
                builder.AppendLine("            OnShutdown();");
                if (model.EventImplementations.Length > 0)
                {
                    builder.AppendLine();
//                    AppendEventDisposal(builder, model, "            ");
                }

                builder.AppendLine("        }");
                builder.AppendLine();

                AppendGameObjectLifecycleHooks(builder);
            }
            else if (model.IsAsset)
            {
                AppendSummaryDocumentation(builder, "        ", "Disposes the generated asset wrapper and releases generated event bindings.");
                builder.AppendLine("        public new void Dispose()");
                builder.AppendLine("        {");
                builder.AppendLine("            OnDispose();");
                if (model.EventImplementations.Length > 0)
                {
                    builder.AppendLine();
                    AppendEventDisposal(builder, model, "            ");
                }

                builder.AppendLine("            base.Dispose();");
                builder.AppendLine("        }");
                builder.AppendLine();

                AppendAssetDisposeHook(builder);
            }

            AppendMethodMembers(builder, model);
            AppendNamespaceEnd(builder);
            return builder.ToString();
        }
    }

    /// <summary>
    /// Renders engine wrapper source for Unity projects.
    /// </summary>
    internal sealed class UnityTemplateRenderer : EngineTemplateRendererBase
    {
        /// <summary>
        /// Renders a complete Unity engine wrapper class using the provided generation model.
        /// </summary>
        public override string Render(TemplateGenerationModel model)
        {
            var builder = new StringBuilder();
            AppendFileHeader(builder);
            AppendNamespaceStart(builder, model);

            AppendConstantMembers(builder, model);
            AppendPropertyMembers(builder, model);

            if (model.UsesGameObjectAdapter)
            {
                builder.AppendLine("        private global::Nomad.EngineUtils.UnityGameObject _impl = default!;");
                builder.AppendLine("        private bool _isDisposed;");
                builder.AppendLine();
                builder.AppendLine("        private TUnityComponent? GetUnityComponent<TUnityComponent>()");
                builder.AppendLine("            where TUnityComponent : global::UnityEngine.Component");
                builder.AppendLine("        {");
                builder.AppendLine("            return gameObject.GetComponent<TUnityComponent>();");
                builder.AppendLine("        }");
                builder.AppendLine();
            }

            AppendEventMembers(builder, model);

            if (model.UsesGameObjectAdapter)
            {
                AppendSummaryDocumentation(builder, "        ", "Creates the generated engine adapter and initializes generated events.");
                builder.AppendLine("        private void Awake()");
                builder.AppendLine("        {");
                builder.AppendLine("            _impl = new global::Nomad.EngineUtils.UnityGameObject(gameObject);");
                if (model.EventImplementations.Length > 0)
                {
                    builder.AppendLine();
                    AppendEventInitializationWithInterpolation(builder, model, "            ");
                    builder.AppendLine();
                    AppendEventHooks(builder, model, "            ");
                }

                builder.AppendLine("        }");
                builder.AppendLine();

                AppendSummaryDocumentation(builder, "        ", "Invokes startup hooks for the generated engine wrapper.");
                builder.AppendLine("        private void Start()");
                builder.AppendLine("        {");
                builder.AppendLine("            _impl.OnInit();");
                builder.AppendLine("            OnInit();");
                builder.AppendLine("        }");
                builder.AppendLine();

                AppendSummaryDocumentation(builder, "        ", "Invokes the generated frame update pipeline.");
                builder.AppendLine("        private void Update()");
                builder.AppendLine("        {");
                builder.AppendLine("            float deltaTime = global::UnityEngine.Time.deltaTime;");
                builder.AppendLine("            _impl.OnUpdate(deltaTime);");
                builder.AppendLine("            OnUpdate(deltaTime);");
                builder.AppendLine("        }");
                builder.AppendLine();

                AppendSummaryDocumentation(builder, "        ", "Invokes the generated physics update pipeline.");
                builder.AppendLine("        private void FixedUpdate()");
                builder.AppendLine("        {");
                builder.AppendLine("            float deltaTime = global::UnityEngine.Time.fixedDeltaTime;");
                builder.AppendLine("            _impl.OnPhysicsUpdate(deltaTime);");
                builder.AppendLine("            OnPhysicsUpdate(deltaTime);");
                builder.AppendLine("        }");
                builder.AppendLine();

                AppendSummaryDocumentation(builder, "        ", "Invokes shutdown hooks and releases generated event bindings.");
                builder.AppendLine("        private void OnDestroy()");
                builder.AppendLine("        {");
                builder.AppendLine("            DisposeCore();");
                builder.AppendLine("        }");
                builder.AppendLine();

                AppendSummaryDocumentation(builder, "        ", "Releases generated engine resources for the wrapper.");
                builder.AppendLine("        public void Dispose()");
                builder.AppendLine("        {");
                builder.AppendLine("            DisposeCore();");
                builder.AppendLine("            global::UnityEngine.Object.Destroy(this);");
                builder.AppendLine("        }");
                builder.AppendLine();

                AppendSummaryDocumentation(builder, "        ", "Performs one-time disposal for the generated engine wrapper.");
                builder.AppendLine("        private void DisposeCore()");
                builder.AppendLine("        {");
                builder.AppendLine("            if (_isDisposed)");
                builder.AppendLine("            {");
                builder.AppendLine("                return;");
                builder.AppendLine("            }");
                builder.AppendLine();
                builder.AppendLine("            _isDisposed = true;");
                builder.AppendLine();
                builder.AppendLine("            if (_impl != null)");
                builder.AppendLine("            {");
                builder.AppendLine("                _impl.OnShutdown();");
                builder.AppendLine("                _impl.Dispose();");
                builder.AppendLine("            }");
                builder.AppendLine();
                builder.AppendLine("            OnShutdown();");
                if (model.EventImplementations.Length > 0)
                {
                    builder.AppendLine();
                    AppendEventDisposal(builder, model, "            ");
                }

                builder.AppendLine("        }");
                builder.AppendLine();

                AppendGameObjectLifecycleHooks(builder);
            }
            else if (model.IsAsset)
            {
                AppendSummaryDocumentation(builder, "        ", "Disposes the generated asset wrapper and releases generated event bindings.");
                builder.AppendLine("        public void Dispose()");
                builder.AppendLine("        {");
                builder.AppendLine("            OnDispose();");
                if (model.EventImplementations.Length > 0)
                {
                    builder.AppendLine();
                    AppendEventDisposal(builder, model, "            ");
                }

                if (model.BaseInheritsUnityObject)
                {
                    builder.AppendLine();
                    builder.AppendLine("            global::UnityEngine.Object.Destroy(this);");
                }

                builder.AppendLine("        }");
                builder.AppendLine();

                AppendAssetDisposeHook(builder);
            }

            AppendMethodMembers(builder, model);
            AppendNamespaceEnd(builder);
            return builder.ToString();
        }
    }
}
