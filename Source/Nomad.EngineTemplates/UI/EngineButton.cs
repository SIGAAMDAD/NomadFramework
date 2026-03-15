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

using Nomad.Core.UI;
using Nomad.Core.Events;
using Nomad.EngineTemplates.BaseClasses;
using Nomad.EngineTemplates.Attributes;

namespace Nomad.EngineTemplates.UI
{
    /// <summary>
    /// Represents an
    /// </summary>
    [TemplateClass(Contract = typeof(IButton), GodotBase = "Godot.Button")]
    [TemplateNamespace(Name = "UI")]
    [TemplateUIElement]
    [TemplateEvent(Name = "Clicked", PayloadType = typeof(EmptyEventArgs))]
    public class EngineButton
    {
    }
}
