/*
===========================================================================
The Nomad Framework
Copyright (C) 2025 Noah Van Til

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
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    [TemplateTypeConversion(
        AgnosticType = typeof(Vector2),
        GodotType = typeof(global::Godot.Vector2),
        AgnosticToGodotExpression = "new global::Godot.Vector2({{value}}.X, {{value}}.Y)",
        GodotToAgnosticExpression = "new global::System.Numerics.Vector2({{value}}.X, {{value}}.Y)")]
    [TemplateProperty(Name = "Scale", Type = typeof(Vector2))]
    public class TemplateScaleProperty : Attribute
    {
    }
}
