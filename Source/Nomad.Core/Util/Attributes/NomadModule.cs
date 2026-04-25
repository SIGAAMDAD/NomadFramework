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

namespace Nomad.Core.Util.Attributes
{
    /// <summary>
    /// An assembly-level attribute that marks and provides metadata about a Nomad framework module.
    /// </summary>
    /// <remarks>
    /// Apply this attribute to an assembly's AssemblyInfo to register it as a Nomad module and document its version and build information.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
    public sealed class NomadModule : Attribute
    {
        /// <summary>
        /// Gets or sets the name of the module.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the major version number of the module.
        /// </summary>
        public int VersionMajor { get; set; }

        /// <summary>
        /// Gets or sets the minor version number of the module.
        /// </summary>
        public int VersionMinor { get; set; }

        /// <summary>
        /// Gets or sets the patch version number of the module.
        /// </summary>
        public int VersionPatch { get; set; }

        /// <summary>
        /// Gets a unique identifier for this build of the module.
        /// </summary>
        public Guid BuildId { get; } = Guid.NewGuid();

        /// <summary>
        /// Gets the date and time when the module was compiled.
        /// </summary>
        public DateTime CompileTime { get; } = DateTime.Now;
    }
}
