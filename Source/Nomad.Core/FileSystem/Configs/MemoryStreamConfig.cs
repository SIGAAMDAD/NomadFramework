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

using Nomad.Core.Memory.Buffers;

namespace Nomad.Core.FileSystem.Configs
{
    /// <summary>
    /// 
    /// </summary>
    public abstract record MemoryStreamConfig : StreamConfig
    {
        /// <summary>
        /// The buffer to operate on at first.
        /// </summary>
        public IBufferHandle? Buffer { get; init; }

        /// <summary>
        /// The maximum amount of bytes that can be allocated into a memory file before we start
        /// streaming directly to I/O DMA.
        /// </summary>
        public long? MaxCapacity { get; init; }

        /// <summary>
        /// Tells the framework how to allocate the internal memory buffer.
        /// </summary>
        public AllocationStrategy Strategy { get; init; }
    }
}