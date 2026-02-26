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

using System.IO;
using Nomad.Core.Util;

namespace Nomad.Core.FileSystem.Configs
{
    /// <summary>
    /// 
    /// </summary>
    public record MemoryWriteConfig : MemoryStreamConfig, IWriteConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public override StreamType Type => StreamType.Memory;

        /// <summary>
        /// The starting capacity of the internal memory region.
        /// </summary>
        public long InitialCapacity { get; init; }

        /// <summary>
        /// Dictates if the internal buffer is allowed to resize. If <c>true</c>, the internal memory region cannot be reallocated, if <c>false</c>, its dynamic.
        /// </summary>
        public bool FixedSize { get; init; }

        /// <summary>
        /// 
        /// </summary>
        public bool Append { get; init; }
    }
}