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

using System.Runtime.CompilerServices;
using Nomad.Core.FileSystem.Streams;

namespace Nomad.Save.ValueObjects
{
    /// <summary>
    /// Represents the serialized game version in integer format. This only supports a Major, Minor, and Patch version, all 32-bit unsigned integers.
    /// </summary>
    /// <remarks>
    /// The first two bits are for the major version, next 3 bits are the minor, the rest is dedicated to the patch number.
    /// </remarks>

    public readonly struct GameVersion
    {
        /// <summary>
        /// 
        /// </summary>
        public readonly uint Major { get; }

        /// <summary>
        /// 
        /// </summary>
        public readonly uint Minor { get; }

        /// <summary>
        /// 
        /// </summary>
        public readonly uint Patch { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="major"></param>
        /// <param name="minor"></param>
        /// <param name="patch"></param>
        public GameVersion(uint major, uint minor, uint patch)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong ToInt()
        {
            ulong majorPart = (ulong)Major * 10_000_000UL;
            ulong minorPart = (ulong)Minor * 10_000UL;
            ulong patchPart = Patch;

            return majorPart + minorPart + patchPart;
        }

        /// <summary>
        /// Writes the GameVersion to disk.
        /// </summary>
        /// <param name="writer"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Serialize(IWriteStream writer)
        {
            writer.WriteUInt32(Major);
            writer.WriteUInt32(Minor);
            writer.WriteUInt32(Patch);
        }

        /// <summary>
        /// Reads the GameVersion from disk.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static GameVersion Deserialize(IReadStream reader)
        {
            uint major = reader.ReadUInt32();
            uint minor = reader.ReadUInt32();
            uint patch = reader.ReadUInt32();

            return new GameVersion(
                major: major,
                minor: minor,
                patch: patch
            );
        }
    };
};
