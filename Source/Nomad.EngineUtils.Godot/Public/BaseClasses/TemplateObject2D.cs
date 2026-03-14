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
using Nomad.Core.Scene.GameObjects;

namespace Nomad.EngineUtils.BaseClasses
{
    /// <summary>
    ///
    /// </summary>
    [TemplateClass(Contract = typeof(IObject2D))]
    [TemplateBaseClass]
    [TemplateEvent(Name = "VisibilityChanged", PayloadType = typeof(bool))]
    [TemplateProperty(Name = "Position", Type = typeof(System.Numerics.Vector2), ToEngineMethod = "ToGodot", FromEngineMethod = "ToSystem")]
    [TemplateProperty(Name = "Scale", Type = typeof(System.Numerics.Vector2), ToEngineMethod = "ToGodot", FromEngineMethod = "ToSystem")]
    [TemplateProperty(Name = "Rotation", Type = typeof(float))]
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class TemplateObject2D : Attribute
    {
    }
}
