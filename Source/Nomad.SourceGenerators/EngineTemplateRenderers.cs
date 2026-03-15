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
using Microsoft.CodeAnalysis;

namespace Nomad.SourceGenerators
{
    internal interface IEngineTemplateRenderer
    {
        string Render(TemplateGenerationModel model);
    }

    internal abstract class EngineTemplateRendererBase : IEngineTemplateRenderer
    {
        protected const string EventNamespace = "Nomad.EngineUtils";

        public abstract string Render(TemplateGenerationModel model);

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

        protected static void AppendNamespaceStart(StringBuilder builder, TemplateGenerationModel model)
        {
            builder.AppendLine("namespace " + model.GeneratedNamespace);
            builder.AppendLine("{");
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

        protected static void AppendNamespaceEnd(StringBuilder builder)
        {
            builder.AppendLine("    }");
            builder.AppendLine("}");
            builder.AppendLine("#nullable disable");
        }

        protected static void AppendPropertyMembers(StringBuilder builder, TemplateGenerationModel model)
        {
            foreach (var propertyImplementation in model.PropertyImplementations)
            {
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
                    builder.AppendLine("            get => " + propertyImplementation.GetterExpression + ";");
                }

                if (propertyImplementation.SetterExpression is not null)
                {
                    builder.AppendLine("            set => " + propertyImplementation.SetterExpression + ";");
                }

                builder.AppendLine("        }");

                if (propertyImplementation.BackingFieldDeclaration is not null)
                {
                    builder.AppendLine("        " + propertyImplementation.BackingFieldDeclaration);
                }

                builder.AppendLine();
            }
        }

        protected static void AppendEventMembers(StringBuilder builder, TemplateGenerationModel model)
        {
            foreach (var eventImplementation in model.EventImplementations)
            {
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

        protected static void AppendMethodMembers(StringBuilder builder, TemplateGenerationModel model)
        {
            foreach (var methodImplementation in model.MethodImplementations)
            {
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

        protected static void AppendEventDisposal(StringBuilder builder, TemplateGenerationModel model, string indent)
        {
            foreach (var eventImplementation in model.EventImplementations)
            {
                builder.Append(indent);
                builder.Append(eventImplementation.FieldName);
                builder.AppendLine("?.Dispose();");
            }
        }

        protected static void AppendGameObjectLifecycleHooks(StringBuilder builder)
        {
            builder.AppendLine("        protected virtual void OnInit()");
            builder.AppendLine("        {");
            builder.AppendLine("        }");
            builder.AppendLine();
            builder.AppendLine("        protected virtual void OnShutdown()");
            builder.AppendLine("        {");
            builder.AppendLine("        }");
            builder.AppendLine();
            builder.AppendLine("        protected virtual void OnUpdate(float delta)");
            builder.AppendLine("        {");
            builder.AppendLine("        }");
            builder.AppendLine();
            builder.AppendLine("        protected virtual void OnPhysicsUpdate(float delta)");
            builder.AppendLine("        {");
            builder.AppendLine("        }");
            builder.AppendLine();
        }

        protected static void AppendAssetDisposeHook(StringBuilder builder)
        {
            builder.AppendLine("        protected virtual void OnDispose()");
            builder.AppendLine("        {");
            builder.AppendLine("        }");
            builder.AppendLine();
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
    }

    internal sealed class GodotTemplateRenderer : EngineTemplateRendererBase
    {
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

            AppendPropertyMembers(builder, model);

            if (model.UsesGameObjectAdapter)
            {
                builder.AppendLine("        private readonly global::Nomad.EngineUtils.GodotGameObject _impl;");
                builder.AppendLine();
            }

            AppendEventMembers(builder, model);

            if (model.UsesGameObjectAdapter)
            {
                builder.AppendLine("        public " + model.GeneratedClassName + "()");
                builder.AppendLine("        {");
                builder.AppendLine("            _impl = new global::Nomad.EngineUtils.GodotGameObject(this);");
                builder.AppendLine("        }");
                builder.AppendLine();

                builder.AppendLine("        public sealed override void _Ready()");
                builder.AppendLine("        {");
                builder.AppendLine("            base._Ready();");
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

                builder.AppendLine("        public sealed override void _Process(double delta)");
                builder.AppendLine("        {");
                builder.AppendLine("            base._Process(delta);");
                builder.AppendLine();
                builder.AppendLine("            float deltaTime = (float)delta;");
                builder.AppendLine("            _impl.OnUpdate(deltaTime);");
                builder.AppendLine("            OnUpdate(deltaTime);");
                builder.AppendLine("        }");
                builder.AppendLine();

                builder.AppendLine("        public sealed override void _PhysicsProcess(double delta)");
                builder.AppendLine("        {");
                builder.AppendLine("            base._PhysicsProcess(delta);");
                builder.AppendLine();
                builder.AppendLine("            float deltaTime = (float)delta;");
                builder.AppendLine("            _impl.OnPhysicsUpdate(deltaTime);");
                builder.AppendLine("            OnPhysicsUpdate(deltaTime);");
                builder.AppendLine("        }");
                builder.AppendLine();

                builder.AppendLine("        public sealed override void _ExitTree()");
                builder.AppendLine("        {");
                builder.AppendLine("            base._ExitTree();");
                builder.AppendLine();
                builder.AppendLine("            _impl.OnShutdown();");
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

    internal sealed class UnityTemplateRenderer : EngineTemplateRendererBase
    {
        public override string Render(TemplateGenerationModel model)
        {
            var builder = new StringBuilder();
            AppendFileHeader(builder);
            AppendNamespaceStart(builder, model);

            AppendPropertyMembers(builder, model);

            if (model.UsesGameObjectAdapter)
            {
                builder.AppendLine("        private global::Nomad.EngineUtils.UnityGameObject _impl = default!;");
                builder.AppendLine();
            }

            AppendEventMembers(builder, model);

            if (model.UsesGameObjectAdapter)
            {
                builder.AppendLine("        private void Awake()");
                builder.AppendLine("        {");
                builder.AppendLine("            _impl = new global::Nomad.EngineUtils.UnityGameObject(gameObject);");
                if (model.EventImplementations.Length > 0)
                {
                    builder.AppendLine();
                    AppendEventInitializationWithInterpolation(builder, model, "            ");
                }

                builder.AppendLine("        }");
                builder.AppendLine();

                builder.AppendLine("        private void Start()");
                builder.AppendLine("        {");
                builder.AppendLine("            _impl.OnInit();");
                builder.AppendLine("            OnInit();");
                builder.AppendLine("        }");
                builder.AppendLine();

                builder.AppendLine("        private void Update()");
                builder.AppendLine("        {");
                builder.AppendLine("            float deltaTime = global::UnityEngine.Time.deltaTime;");
                builder.AppendLine("            _impl.OnUpdate(deltaTime);");
                builder.AppendLine("            OnUpdate(deltaTime);");
                builder.AppendLine("        }");
                builder.AppendLine();

                builder.AppendLine("        private void FixedUpdate()");
                builder.AppendLine("        {");
                builder.AppendLine("            float deltaTime = global::UnityEngine.Time.fixedDeltaTime;");
                builder.AppendLine("            _impl.OnPhysicsUpdate(deltaTime);");
                builder.AppendLine("            OnPhysicsUpdate(deltaTime);");
                builder.AppendLine("        }");
                builder.AppendLine();

                builder.AppendLine("        private void OnDestroy()");
                builder.AppendLine("        {");
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
