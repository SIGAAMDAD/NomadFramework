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

namespace Nomad.EngineTemplates.Attributes
{
    /// <summary>
    /// Maps an agnostic contract method to a concrete engine-side method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class TemplateMethod : Attribute
    {
        /// <summary>
        /// The contract method name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Optional Godot engine method name override.
        /// </summary>
        public string GodotMethodName { get; set; } = null;

        /// <summary>
        /// Optional Unity engine method name override.
        /// </summary>
        public string UnityMethodName { get; set; } = null;
    }
}
