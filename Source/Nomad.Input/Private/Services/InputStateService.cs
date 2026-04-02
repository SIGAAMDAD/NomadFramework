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

using System.Numerics;
using System.Runtime.CompilerServices;
using Nomad.Input.ValueObjects;

namespace Nomad.Input.Private.Services {
	/*
	===================================================================================
	
	InputStateService
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>

	internal sealed class InputStateService {
		private const int DEVICE_SLOT_COUNT = (int)InputDeviceSlot.Count;
		private const int CONTROL_COUNT = (int)InputControlId.Count;

		private const int WORDS_PER_DEVICE = 4;

		private readonly ulong[] _pressedBits = new ulong[DEVICE_SLOT_COUNT * WORDS_PER_DEVICE];

		private readonly float[] _axis1D = new float[DEVICE_SLOT_COUNT * CONTROL_COUNT];
		private readonly Vector2[] _axis2D = new Vector2[DEVICE_SLOT_COUNT * CONTROL_COUNT];

		public Vector2 MouseDelta => _mouseDelta;
		private Vector2 _mouseDelta;

		public Vector2 MousePosition => _mousePosition;
		private Vector2 _mousePosition;

		/*
		===============
		IsPressed
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="slot"></param>
		/// <param name="control"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public bool IsPressed( InputDeviceSlot slot, InputControlId control ) {
			int controlIndex = (int)control;
			int baseWord = ((int)slot * WORDS_PER_DEVICE) + (controlIndex >> 6);
			ulong mask = 1UL << (controlIndex & 63);
			return (_pressedBits[baseWord] & mask) != 0UL;
		}

		/*
		===============
		SetPressed
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="slot"></param>
		/// <param name="control"></param>
		/// <param name="pressed"></param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void SetPressed( InputDeviceSlot slot, InputControlId control, bool pressed ) {
			if ( pressed ) {
				SetPressedBit( slot, control );
			} else {
				ClearPressedBit( slot, control );
			}
		}

		/*
		===============
		TryGetAxis1D
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="slot"></param>
		/// <param name="control"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool TryGetAxis1D( InputDeviceSlot slot, InputControlId control, out float value ) {
			if ( !IsAxis1DControl( control ) ) {
				value = default;
				return false;
			}
			value = _axis1D[GetFlatIndex( slot, control )];
			return true;
		}

		/*
		===============
		TryGetAxis2D
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="slot"></param>
		/// <param name="control"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool TryGetAxis2D( InputDeviceSlot slot, InputControlId control, out Vector2 value ) {
			if ( !IsAxis2DControl( control ) ) {
				value = default;
				return false;
			}
			value = _axis2D[GetFlatIndex( slot, control )];
			return true;
		}

		/*
		===============
		SetAxis1D
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="slot"></param>
		/// <param name="control"></param>
		/// <param name="value"></param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void SetAxis1D( InputDeviceSlot slot, InputControlId control, float value ) {
			_axis1D[GetFlatIndex( slot, control )] = value;
		}

		/*
		===============
		SetAxis2D
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="slot"></param>
		/// <param name="control"></param>
		/// <param name="value"></param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void SetAxis2D( InputDeviceSlot slot, InputControlId control, Vector2 value ) {
			_axis2D[GetFlatIndex( slot, control )] = value;
		}

		/*
		===============
		SetMousePosition
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void SetMousePosition( Vector2 value ) {
			_mousePosition = value;
			_axis2D[GetFlatIndex( InputDeviceSlot.Mouse, InputControlId.Position )] = value;
		}

		/*
		===============
		AddMouseDelta
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void AddMouseDelta( Vector2 value ) {
			_mouseDelta = value;
			_axis2D[GetFlatIndex( InputDeviceSlot.Mouse, InputControlId.Delta )] = value;
		}

		/*
		===============
		IsAxis1DControl
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="control"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private static bool IsAxis1DControl( InputControlId control ) {
			return control == InputControlId.LeftTrigger || control == InputControlId.RightTrigger;
		}

		/*
		===============
		IsAxis2DControl
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="control"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private static bool IsAxis2DControl( InputControlId control ) {
			switch ( control ) {
				case InputControlId.LeftStick:
				case InputControlId.RightStick:
				case InputControlId.Delta:
				case InputControlId.Position:
				case InputControlId.Scroll:
					return true;
				default:
					return false;
			}
		}

		/*
		===============
		GetFlatIndex
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="slot"></param>
		/// <param name="control"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private static int GetFlatIndex( InputDeviceSlot slot, InputControlId control ) {
			return ((int)slot * CONTROL_COUNT) + (int)control;
		}

		/*
		===============
		SetPressedBit
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="slot"></param>
		/// <param name="control"></param>
		private void SetPressedBit( InputDeviceSlot slot, InputControlId control ) {
			int controlIndex = (int)control;
			int baseWord = ((int)slot * WORDS_PER_DEVICE) + (controlIndex >> 6);
			ulong mask = 1UL << (controlIndex & 63);
			_pressedBits[baseWord] |= mask;
		}

		/*
		===============
		ClearPressedBit
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="slot"></param>
		/// <param name="control"></param>
		private void ClearPressedBit( InputDeviceSlot slot, InputControlId control ) {
			int controlIndex = (int)control;
			int baseWord = ((int)slot * WORDS_PER_DEVICE) + (controlIndex >> 6);
			ulong mask = 1UL << (controlIndex & 63);
			_pressedBits[baseWord] &= ~mask;
		}
	};
};