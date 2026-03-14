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

namespace Nomad.EngineUtils
{
    /// <summary>
    ///
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class TemplateProperty : Attribute
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
        /// The method for translating from an engine-specific type to an engine agnostic type.
        /// </summary>
        public string FromEngineMethod { get; set; } = null;

        /// <summary>
        /// The method for translating from an engine-agnostic type to an engine-specific type.
        /// </summary>
        public string ToEngineMethod { get; set; } = null;
    }
}
