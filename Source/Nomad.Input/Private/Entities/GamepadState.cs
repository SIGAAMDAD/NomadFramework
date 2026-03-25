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
using System.Numerics;
using Nomad.Core.Util;

namespace Nomad.Input.Private.Entities {
	/*
	===================================================================================
	
	GamepadState
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	internal sealed class GamepadState {
		public PackedBitSet Buttons { get; }
		public float[] Axis1D { get; }
		public Vector2[] Axis2D { get; }

		/*
		===============
		GamepadState
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="buttonCount"></param>
		/// <param name="axis1DCount"></param>
		/// <param name="axis2DCount"></param>
		public GamepadState( int buttonCount, int axis1DCount, int axis2DCount ) {
			Buttons = new PackedBitSet( buttonCount );
			Axis1D = new float[ axis1DCount ];
			Axis2D = new Vector2[ axis2DCount ];
		}

		public void Clear() {
			Buttons.Clear();
			Array.Clear( Axis1D, 0, Axis1D.Length );
			Array.Clear( Axis2D, 0, Axis2D.Length );
		}
	};
};