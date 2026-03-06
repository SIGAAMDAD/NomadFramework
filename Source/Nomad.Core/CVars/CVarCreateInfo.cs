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

namespace Nomad.Core.CVars
{
    /// <summary>
    /// Creation information for a <see cref="ICVar"/>.
    /// </summary>
    /// <typeparam name="T">The internal value type of the CVar.</typeparam>
    public record CVarCreateInfo<T>
    {
        /// <summary>
        /// The CVar's name.
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// The CVar's default/starting value. After reset/restart, the CVar's value will be assigned to this.
        /// </summary>
        public T DefaultValue { get; init; }

        /// <summary>
        /// A short description of the CVar.
        /// </summary>
        public string Description { get; init; }

        /// <summary>
        /// The CVar's assigned group.
        /// </summary>
        /// <remarks>
        /// This is optional, if <see langword="null"/>, the group is set to "Default".
        /// </remarks>
        public string Group { get; init; }

        /// <summary>
        /// The CVar's permissions/runtime rules.
        /// </summary>
        /// <remarks>
        /// This is optional, defaults to <see cref="CVarFlags.None"/>, simply meaning "no rules".
        /// </remarks>
        public CVarFlags Flags { get; init; } = CVarFlags.None;

        /// <summary>
        /// A validator callback for approving value changes.
        /// </summary>
        /// <remarks>
        /// This is optional, but highly recommended to set this if you want extra safety/are exposing the CVar to
        /// modification via external processes.
        /// </remarks>
        public Func<T, bool>? Validator { get; init; } = null;
    }
}
