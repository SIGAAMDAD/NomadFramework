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
	/// 
	/// </summary>
	public sealed class PackedBitSet
	{
		private readonly ulong[] _words;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="bitCount"></param>
		public PackedBitSet(int bitCount)
		{
			RangeGuard.ThrowIfNegativeOrZero(bitCount, nameof(bitCount));
			_words = new ulong[(bitCount + 63) >> 6];
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public bool Get(int index)
		{
			int word = index >> 6;
			int bit = index & 63;
			return (_words[word] & (1UL << bit)) != 0;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <param name="value"></param>
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
		/// Clears the bitset
		/// </summary>
		public void Clear()
		{
			Array.Clear(_words, 0, _words.Length);
		}
	}
}