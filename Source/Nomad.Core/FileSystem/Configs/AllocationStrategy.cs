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

namespace Nomad.Core.FileSystem.Configs
{
    /// <summary>
    /// 
    /// </summary>
    public enum AllocationStrategy : byte
    {
        /// <summary>
        /// Allocates the buffer utilizing ArrayPool and a memory copy.
        /// </summary>
        Pooled,

        /// <summary>
        /// Allocates the buffer utilizing operator new[] and a memory copy.
        /// </summary>
        Standard,

        /// <summary>
        /// Allocates the buffer utilizing File.ReadAllBytes.
        /// </summary>
        FromFile,

        /// <summary>
        /// 
        /// </summary>
        Count
    }
}