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

namespace Nomad.Core.ServiceRegistry
{
    /// <summary>
    /// The lifetime of a registered service.
    /// </summary>
    public enum ServiceLifetime : byte
    {
        /// <summary>
        /// One instance per container.
        /// </summary>
        Singleton,

        /// <summary>
        /// New instance each time.
        /// </summary>
        Transient,

        /// <summary>
        /// One instance per scope.
        /// </summary>
        Scoped,

        /// <summary>
        /// One instance per thread.
        /// </summary>
        Thread,

        /// <summary>
        /// New instance each time the scene is changed.
        /// </summary>
        Scene,

        /// <summary>
        /// The total number of ServiceLifetime values.
        /// </summary>
        Count
    }
}
