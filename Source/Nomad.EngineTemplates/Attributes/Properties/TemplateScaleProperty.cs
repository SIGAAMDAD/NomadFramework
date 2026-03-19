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
using System.Numerics;

namespace Nomad.EngineTemplates.Attributes.Properties
{
    /// <summary>
    /// Declares the shared template metadata for scale properties.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    [TemplateTypeConversion(
        AgnosticType = typeof(Vector2),
        AgnosticToGodotExpression = "new global::Godot.Vector2({{value}}.X, {{value}}.Y)",
        GodotToAgnosticExpression = "new global::System.Numerics.Vector2({{value}}.X, {{value}}.Y)",
        AgnosticToUnityExpression = "new global::UnityEngine.Vector2({{value}}.X, {{value}}.Y)",
        UnityToAgnosticExpression = "new global::System.Numerics.Vector2({{value}}.x, {{value}}.y)")]
    [TemplateProperty(
        Name = "Scale",
        Type = typeof(Vector2),
        Documentation = "Represents a GameObject's size relative to the viewport.",
        UnityGetterExpression = "new global::System.Numerics.Vector2(transform.localScale.x, transform.localScale.y)",
        UnitySetterExpression = "transform.localScale = new global::UnityEngine.Vector3(value.X, value.Y, transform.localScale.z)")]
    internal class TemplateScaleProperty : Attribute
    {
    }
}
