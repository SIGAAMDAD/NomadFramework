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
using System.Drawing;

namespace Nomad.EngineTemplates.Attributes.Properties
{
    /// <summary>
    /// Declares the shared template metadata for color properties.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    [TemplateTypeConversion(
        AgnosticType = typeof(Color),
        AgnosticToGodotExpression = "new global::Godot.Color({{value}}.R, {{value}}.G, {{value}}.B, {{value}}.A)",
        GodotToAgnosticExpression = "global::System.Drawing.Color.FromArgb({{value}}.A8, {{value}}.R8, {{value}}.G8, {{value}}.B8)",
        AgnosticToUnityExpression = "new global::UnityEngine.Color({{value}}.R / 255f, {{value}}.G / 255f, {{value}}.B / 255f, {{value}}.A / 255f)",
        UnityToAgnosticExpression = "global::System.Drawing.Color.FromArgb((int)({{value}}.a * 255f), (int)({{value}}.r * 255f), (int)({{value}}.g * 255f), (int)({{value}}.b * 255f))")]
    [TemplateProperty(Name = "Color", Type = typeof(Color), Documentation = "Represents a GameObject's color.")]
    internal class TemplateColorProperty : Attribute
    {
    }
}
