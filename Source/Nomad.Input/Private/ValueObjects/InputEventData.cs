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

using System.Runtime.InteropServices;
using Nomad.Core.Input;

namespace Nomad.Input.Private.ValueObjects {
	[StructLayout( LayoutKind.Explicit, Pack = 16, Size = 32 )]
	internal readonly struct InputEventData {
		[FieldOffset( 0 )] public readonly InputType Type;

		[FieldOffset( 4 )] public readonly KeyboardEvent Keyboard;
		[FieldOffset( 4 )] public readonly MouseButtonEvent MouseButton;
		[FieldOffset( 4 )] public readonly MouseMotionEvent MouseMotion;

		/*
		===============
		InputEventData
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="keyboard"></param>
		public InputEventData( KeyboardEvent keyboard ) {
			Type = InputType.Keyboard;
			Keyboard = keyboard;
			MouseButton = default;
			MouseMotion = default;
		}

		/*
		===============
		InputEventData
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="mouseButton"></param>
		public InputEventData( MouseButtonEvent mouseButton ) {
			Type = InputType.MouseButton;
			Keyboard = default;
			MouseButton = mouseButton;
			MouseMotion = default;
		}

		/*
		===============
		InputEventData
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="mouseMotion"></param>
		public InputEventData( MouseMotionEvent mouseMotion ) {
			Type = InputType.MouseMotion;
			Keyboard = default;
			MouseButton = default;
			MouseMotion = mouseMotion;
		}
	};
};