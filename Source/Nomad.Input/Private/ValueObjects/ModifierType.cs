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

namespace Nomad.Input.Private.ValueObjects {
	/// <summary>
	/// 
	/// </summary>
	[Flags]
	public enum ModifierType : uint {
		/// <summary>
		/// 
		/// </summary>
		Coordinates3D = 1 << 0,

		/// <summary>
		/// 
		/// </summary>
		Directional8Way = 1 << 1,

		/// <summary>
		/// 
		/// </summary>
		CanvasCoordinates = 1 << 2,

		/// <summary>
		/// 
		/// </summary>
		Curve = 1 << 3,

		/// <summary>
		/// 
		/// </summary>
		Deadzone = 1 << 4,

		/// <summary>
		/// 
		/// </summary>
		InputSwizzle = 1 << 5,

		/// <summary>
		/// 
		/// </summary>
		Magnitude = 1 << 6,

		/// <summary>
		/// 
		/// </summary>
		MapRange = 1 << 7,

		/// <summary>
		/// 
		/// </summary>
		Negate = 1 << 8,

		/// <summary>
		/// 
		/// </summary>
		Normalize = 1 << 9,

		/// <summary>
		/// 
		/// </summary>
		PositveNegative = 1 << 10,

		/// <summary>
		/// 
		/// </summary>
		Scale = 1 << 11,

		/// <summary>
		/// 
		/// </summary>
		VirtualCursor = 1 << 12,

		/// <summary>
		/// 
		/// </summary>
		WindowRelative = 1 << 13
	}
}
