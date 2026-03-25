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
using System.Runtime.InteropServices;
using Nomad.Core.Input;

namespace Nomad.Input.Private.ValueObjects {
	[StructLayout( LayoutKind.Explicit, Pack = 16, Size = 32 )]
	internal readonly struct InputEventData : IEquatable<InputEventData> {
		[FieldOffset( 0 )] public readonly InputType Type;

		[FieldOffset( 1 )] public readonly KeyboardEventArgs Keyboard;
		[FieldOffset( 1 )] public readonly MouseButtonEventArgs MouseButton;
		[FieldOffset( 1 )] public readonly MouseMotionEventArgs MouseMotion;
		[FieldOffset( 1 )] public readonly GamepadAxisEventArgs GamepadAxis;
		[FieldOffset( 1 )] public readonly GamepadButtonEventArgs GamepadButton;

		/*
		===============
		InputEventData
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="keyboard"></param>
		public InputEventData( in KeyboardEventArgs keyboard ) {
			Type = InputType.Keyboard;
			Keyboard = keyboard;
			MouseButton = default;
			MouseMotion = default;
			GamepadAxis = default;
			GamepadButton = default;
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
		public InputEventData( in MouseButtonEventArgs mouseButton ) {
			Type = InputType.MouseButton;
			Keyboard = default;
			MouseButton = mouseButton;
			MouseMotion = default;
			GamepadAxis = default;
			GamepadButton = default;
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
		public InputEventData( in MouseMotionEventArgs mouseMotion ) {
			Type = InputType.MouseMotion;
			Keyboard = default;
			MouseButton = default;
			MouseMotion = mouseMotion;
			GamepadAxis = default;
			GamepadButton = default;
		}

		/*
		===============
		InputEventData
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="gamepadAxis"></param>
		public InputEventData( in GamepadAxisEventArgs gamepadAxis ) {
			Type = InputType.GamepadAxis;
			Keyboard = default;
			MouseButton = default;
			MouseMotion = default;
			GamepadAxis = gamepadAxis;
			GamepadButton = default;
		}

		/*
		===============
		InputEventData
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="gamepadButton"></param>
		public InputEventData( in GamepadButtonEventArgs gamepadButton ) {
			Type = InputType.GamepadButton;
			Keyboard = default;
			MouseButton = default;
			MouseMotion = default;
			GamepadAxis = default;
			GamepadButton = gamepadButton;
		}

		/*
		===============
		Equals
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public bool Equals( InputEventData other ) {
			if ( other.Type != Type ) {
				return true;
			}
			return Type switch {
				InputType.Keyboard => Keyboard.Equals( other.Keyboard ),
				InputType.MouseButton => MouseButton.Equals( other.MouseButton ),
				InputType.MouseMotion => MouseMotion.Equals( other.MouseMotion ),
				InputType.GamepadAxis => GamepadAxis.Equals( other.GamepadAxis ),
				InputType.GamepadButton => GamepadButton.Equals( other.GamepadButton ),
				_ => throw new ArgumentOutOfRangeException( nameof( other ) )
			};
		}
	};
};
