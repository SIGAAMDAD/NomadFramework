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

namespace Nomad.Core.Logger
{
    /// <summary>
    ///
    /// </summary>
    public enum LogLevel : byte
    {
        /// <summary>
        /// Prints in red, indicates something went wrong.
        /// </summary>
        Error,

        /// <summary>
        /// Prints in yellow, indicates something is not right but not critical.
        /// </summary>
        Warning,

        /// <summary>
        /// Prints in white, for generic information.
        /// </summary>
        Info,

        /// <summary>
        /// Prints in blue, for state/memory tracking.
        /// </summary>
        Debug,

        Count
    }
}

