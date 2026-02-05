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

namespace Nomad.Save.Private.ValueObjects {
	/*
	===================================================================================
	
	Checksum
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	public readonly struct Checksum {
		public static readonly Checksum Empty = new Checksum( 0 );

		public readonly ulong Value;

		/*
		===============
		Checksum
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		public Checksum( ulong value ) {
			Value = value;
		}

		/*
		===============
		Compute
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public static Checksum Compute( ReadOnlySpan<byte> data ) {
			return new Checksum();
		}
	};
};
