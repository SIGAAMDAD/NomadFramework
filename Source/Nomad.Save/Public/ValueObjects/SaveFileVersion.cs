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

namespace Nomad.Save.ValueObjects
{
    /// <summary>
    /// 
    /// </summary>
    public readonly struct SaveFileVersion
    {
        /// <summary>
        /// 
        /// </summary>
        public readonly uint Major;

        /// <summary>
        /// 
        /// </summary>
        public readonly uint Minor;

        /// <summary>
        /// 
        /// </summary>
        public readonly ulong Patch;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="major"></param>
        /// <param name="minor"></param>
        /// <param name="patch"></param>
        public SaveFileVersion(uint major, uint minor, ulong patch)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
        }
    }
}
