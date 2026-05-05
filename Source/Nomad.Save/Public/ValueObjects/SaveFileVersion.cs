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
using System.Runtime.CompilerServices;
using Nomad.Core.FileSystem.Streams;
using Nomad.Core.Compatibility.Guards;

namespace Nomad.Save.ValueObjects
{
    /// <summary>
    /// 
    /// </summary>
    public readonly struct SaveFileVersion : IEquatable<SaveFileVersion>
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
        public readonly ulong Patch { get; }

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

        /// <summary>
        /// Writes the version data to a <see cref="IWriteStream"/>.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        internal void Serialize(IWriteStream stream)
        {
            ArgumentGuard.ThrowIfNull(stream, nameof(stream));
            stream.WriteUInt32(Major);
            stream.WriteUInt32(Minor);
            stream.WriteUInt64(Patch);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        internal static SaveFileVersion Deserialize(IReadStream stream)
        {
            ArgumentGuard.ThrowIfNull(stream, nameof(stream));
            return new SaveFileVersion(
                stream.ReadUInt32(),
                stream.ReadUInt32(),
                stream.ReadUInt64()
            );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(SaveFileVersion other)
        {
            return Major == other.Major && Minor == other.Minor && Patch == other.Patch;
        }
    }
}
