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

using Nomad.Core.Engine.Assets;
using Nomad.EngineTemplates.Attributes;
using System;

namespace Nomad.EngineTemplates.Assets
{
    /// <summary>
    ///
    /// </summary>
	[TemplateClass(Contract = typeof(IMaterial), IsAsset = true, GodotBase = "Godot.Material")]
	[TemplateNamespace(Name = "Assets")]
    public class EngineMaterial
    {
    }
}
