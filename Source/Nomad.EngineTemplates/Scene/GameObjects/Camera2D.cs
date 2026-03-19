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

using System.Numerics;
using Nomad.Core.Scene.GameObjects;
using Nomad.EngineTemplates.Attributes;
using Nomad.EngineTemplates.Attributes.Properties;

namespace Nomad.EngineTemplates.Scene.GameObjects
{
	/// <summary>
	/// Declares the engine template for 2D camera objects.
	/// </summary>
	[TemplateClass(Contract = typeof(ICamera2D), GodotBase = "Godot.Camera2D")]
	[TemplatePositionProperty]
	[TemplateScaleProperty]
	[TemplateRotationProperty]
	[TemplateTypeConversion(
        AgnosticType = typeof(Vector2),
        AgnosticToGodotExpression = "new global::Godot.Vector2({{value}}.X, {{value}}.Y)",
        GodotToAgnosticExpression = "new global::System.Numerics.Vector2({{value}}.X, {{value}}.Y)",
		AgnosticToUnityExpression = "new global::UnityEngine.Vector2({{value}}.X, {{value}}.Y)",
		UnityToAgnosticExpression = "new global::System.Numerics.Vector2({{value}}.X, {{value}}.Y)"
	)]
	[TemplateProperty(Name = "Zoom", Type = typeof(Vector2))]
	internal class Camera2D
	{
	}
}
