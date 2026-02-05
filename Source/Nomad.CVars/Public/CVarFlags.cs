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

namespace Nomad.CVars
{
    /// <summary>
    /// Various flags that can be applied to a <see cref="CVar"/>.
    /// </summary>
    [Flags]
    public enum CVarFlags : uint
    {
        /// <summary>
        /// The default state for a CVar to be in, nothing fancy happens here.
        /// </summary>
        None = 0,

        /// <summary>
        /// Cannot change after initialization.
        /// </summary>
        ReadOnly = 1 << 0,

        /// <summary>
        /// Created in the console by the user.
        /// </summary>
        UserCreated = 1 << 2,

        /// <summary>
        /// Saves the CVar's value to the configuration file (usually settings.ini).
        /// </summary>
        Archive = 1 << 3,

        /// <summary>
        /// Hidden from console auto-completion.
        /// </summary>
        Hidden = 1 << 4,

        /// <summary>
        /// Can only be modified in debug/developer mode.
        /// </summary>
        Developer = 1 << 5,

        /// <summary>
        /// Cannot change from the console at all.
        /// </summary>
        Init = 1 << 6
    }
}
