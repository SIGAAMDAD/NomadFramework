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
    ///
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class TemplateClass : Attribute
    {
        /// <summary>
        /// The inheriting contract.
        /// </summary>
        public Type Contract { get; set; }

		/// <summary>
		/// Indicates this is a managed asset class.
		/// </summary>
		public bool IsAsset { get; set; } = false;

        /// <summary>
        /// The base godot class to inherit from.
        /// </summary>
        public string GodotBase { get; set; }

        /// <summary>
        /// The unity class to structure the behavior around.
        /// </summary>
        public string UnityBase { get; set; }
    }
}
