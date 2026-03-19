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

namespace Nomad.EngineTemplates.Attributes
{
    /// <summary>
    /// Represents a template generated property for the EAT
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    internal class TemplateProperty : Attribute
    {
        /// <summary>
        /// The property's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The property's data type.
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Literal source expression copied into the generated getter for Godot builds.
        /// Use <c>{{value}}</c> to reference the engine-side value when binding an engine property.
        /// </summary>
        public string GodotGetterExpression { get; set; } = null;

        /// <summary>
        /// Literal source expression copied into the generated getter for Unity builds.
        /// Use <c>{{value}}</c> to reference the engine-side value when binding an engine property.
        /// </summary>
        public string UnityGetterExpression { get; set; } = null;

        /// <summary>
        /// Literal source expression copied into the generated setter for Godot builds.
        /// Use <c>{{value}}</c> to reference the incoming agnostic value.
        /// </summary>
        public string GodotSetterExpression { get; set; } = null;

        /// <summary>
        /// Literal source expression copied into the generated setter for Unity builds.
        /// Use <c>{{value}}</c> to reference the incoming agnostic value.
        /// </summary>
        public string UnitySetterExpression { get; set; } = null;

        /// <summary>
        /// Indicates the generated fallback backing field should be readonly.
        /// </summary>
        public bool IsReadOnly { get; set; } = false;

        /// <summary>
        /// The documentation for the generated property.
        /// </summary>
        public string Documentation { get; set; } = null;
    }
}
