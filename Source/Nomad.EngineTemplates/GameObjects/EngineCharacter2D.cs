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

using Nomad.Core.Scene.GameObjects;
using Nomad.EngineTemplates.Attributes;
using Nomad.EngineTemplates.BaseClasses;

namespace Nomad.EngineTemplates.GameObjects
{
    /// <summary>
    ///
    /// </summary>
    [TemplateClass(Contract = typeof(ICharacter2D), GodotBase = "Godot.CharacterBody2D")]
    [TemplateNamespace(Name = "GameObjects")]
    [TemplateObject2D]
    public class EngineCharacter2D
    {
    }
}
