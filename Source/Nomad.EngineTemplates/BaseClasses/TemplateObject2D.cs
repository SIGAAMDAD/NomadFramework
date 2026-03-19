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
using Nomad.EngineTemplates.Attributes;
using Nomad.EngineTemplates.Attributes.Events;
using Nomad.EngineTemplates.Attributes.Properties;

namespace Nomad.EngineTemplates.BaseClasses
{
    /// <summary>
    /// Declares the shared template metadata for 2D object engine wrappers.
    /// </summary>
    [TemplateClass(Contract = typeof(IObject2D))]
    [TemplateBaseClass]
    [TemplateDisplayStateChangedEvent]
    [TemplatePositionProperty]
    [TemplateScaleProperty]
    [TemplateRotationProperty]
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    internal class TemplateObject2D : Attribute
    {
    }
}
