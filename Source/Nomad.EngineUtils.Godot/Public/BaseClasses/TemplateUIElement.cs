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
using Nomad.Core.UI;
using Nomad.Core.Events;

namespace Nomad.EngineUtils.BaseClasses
{
    /// <summary>
    /// Declares a UI element type.
    /// </summary>
    [TemplateClass(Contract = typeof(IUIElement))]
    [TemplateBaseClass]
    [TemplateEvent(Name = "Focused", PayloadType = typeof(EmptyEventArgs))]
    [TemplateEvent(Name = "Unfocused", PayloadType = typeof(EmptyEventArgs))]
    [TemplateProperty(Name = "Position", Type = typeof(System.Numerics.Vector2), ToEngineMethod = "ToGodot", FromEngineMethod = "ToSystem")]
    [TemplateProperty(Name = "Scale", Type = typeof(System.Numerics.Vector2), ToEngineMethod = "ToGodot", FromEngineMethod = "ToSystem")]
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class TemplateUIElement : Attribute
    {
    }
}
