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
    /// 
    /// </summary>
    public readonly struct WriteConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public readonly int Length { get; }

        /// <summary>
        /// 
        /// </summary>
        public readonly StreamType Type { get; }

        /// <summary>
        /// 
        /// </summary>
        public readonly bool Append { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="append"></param>
        public WriteConfig(StreamType type, bool append = false)
        {
            Type = type;
            Append = append;
            Length = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="length"></param>
        /// <param name="append"></param>
        public WriteConfig(StreamType type, int length, bool append = false)
        {
            Type = type;
            Append = append;
            Length = length;
        }
    }
}