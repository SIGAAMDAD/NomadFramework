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

namespace Nomad.Core.FileSystem
{
    /// <summary>
    /// Defines the type of a data stream.
    /// </summary>
    public enum StreamType : byte
    {
        /// <summary>
        /// A memory stream.
        /// </summary>
        Memory,

        /// <summary>
        /// A memory file stream (the file is stored in RAM).
        /// </summary>
        MemoryFile,

        /// <summary>
        /// A standard file stream.
        /// </summary>
        File,

        /// <summary>
        /// A network stream.
        /// </summary>
        Network,

        /// <summary>
        /// A memory-mapped file stream.
        /// </summary>
        Virtual,

        /// <summary>
        /// Indicates the number of stream types.
        /// </summary>
        Count
    }
}
