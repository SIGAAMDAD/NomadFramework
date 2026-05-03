/*
===========================================================================
The Nomad MPLv2 Source Code
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
    /// A memory-efficient data structure for storing and manipulating a set of boolean values.
    /// </summary>
    /// <remarks>
    /// This class uses an array of 64-bit unsigned integers to store bits, allowing efficient storage and manipulation of large bitsets.
    /// Each ulong can store 64 boolean values, making this structure ideal for scenarios where you need to track many boolean flags with minimal memory overhead.
    /// </remarks>
    public sealed class PackedBitSet
    {
        private readonly ulong[] _words;

        /// <summary>
        /// Initializes a new instance of the <see cref="PackedBitSet"/> class with the specified bit capacity.
        /// </summary>
        /// <param name="bitCount">The number of bits to store. Must be positive.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="bitCount"/> is negative or zero.</exception>
        public PackedBitSet(int bitCount)
        {
            RangeGuard.ThrowIfNegativeOrZero(bitCount, nameof(bitCount));
            _words = new ulong[(bitCount + 63) >> 6];
        }

        /// <summary>
        /// Gets the value of the bit at the specified index.
        /// </summary>
        /// <param name="index">The index of the bit to retrieve.</param>
        /// <returns>True if the bit is set; otherwise, false.</returns>
        public bool Get(int index)
        {
            int word = index >> 6;
            int bit = index & 63;
            return (_words[word] & (1UL << bit)) != 0;
        }

        /// <summary>
        /// Sets or clears the bit at the specified index.
        /// </summary>
        /// <param name="index">The index of the bit to modify.</param>
        /// <param name="value">True to set the bit; false to clear it.</param>
        public void Set(int index, bool value)
        {
            int word = index >> 6;
            int bit = index & 63;
            ulong mask = 1UL << bit;

            if (value)
            {
                _words[word] |= mask;
            }
            else
            {
                _words[word] &= ~mask;
            }
        }

        /// <summary>
        /// Clears all bits in the bitset, setting them to false.
        /// </summary>
        public void Clear()
        {
            Array.Clear(_words, 0, _words.Length);
        }
    }
}
