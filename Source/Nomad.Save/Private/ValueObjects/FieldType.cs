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

namespace Nomad.Save.Private.ValueObjects {
	/// <summary>
	/// A save field's type.
	/// </summary>
	public enum FieldType : byte {
		/// <summary>
		/// 
		/// </summary>
		Int8,

		/// <summary>
		/// 
		/// </summary>
		Int16,

		/// <summary>
		/// 
		/// </summary>
		Int32,

		/// <summary>
		/// 
		/// </summary>
		Int64,

		/// <summary>
		/// 
		/// </summary>
		UInt8,
		
		/// <summary>
		/// 
		/// </summary>
		UInt16,

		/// <summary>
		/// 
		/// </summary>
		UInt32,

		/// <summary>
		/// 
		/// </summary>
		UInt64,

		/// <summary>
		/// 
		/// </summary>
		Float,

		/// <summary>
		/// 
		/// </summary>
		Double,
		
		/// <summary>
		/// 
		/// </summary>
		Boolean,

		/// <summary>
		/// 
		/// </summary>
		String,

		/// <summary>
		/// 
		/// </summary>
		Count
	};
};
