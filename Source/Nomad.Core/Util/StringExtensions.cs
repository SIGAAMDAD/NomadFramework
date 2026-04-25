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
using Nomad.Core.Compatibility.Guards;

namespace Nomad.Core.Util
{
    /// <summary>
    /// Provides extension methods for string operations.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Computes a 32-bit hash of a filename in a case-insensitive manner using the XxHash32 algorithm.
        /// </summary>
        /// <remarks>
        /// This method is useful for creating normalized hashes of filenames that are consistent regardless of case differences.
        /// All uppercase letters are converted to lowercase before hashing. Use the optional seed parameter for domain-specific hashing.
        /// </remarks>
        /// <param name="filename">The filename to hash. Must not be null or empty.</param>
        /// <param name="seed">An optional seed value for the hash function. Defaults to 0.</param>
        /// <returns>A 32-bit hash value of the filename.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="filename"/> is null or empty.</exception>
        public static uint HashFileName(this string filename, int seed = 0)
        {
            ArgumentGuard.ThrowIfNullOrEmpty(filename);

            int length = filename.Length;
            Span<byte> hash = stackalloc byte[length];
            for (int i = 0; i < length; i++)
            {
                char c = filename[i];
                hash[i] = (byte)(c >= 'A' && c <= 'Z' ? c + 32 : c);
            }

            return System.IO.Hashing.XxHash32.HashToUInt32(hash, seed);
        }
    }
}
