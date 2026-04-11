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
using Nomad.Core.Input;
using Nomad.Input.Interfaces;
using System.Runtime.InteropServices;
using System;

namespace Nomad.Input.Private.Services {
	/*
	===================================================================================
	
	InputStateService
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>

	internal unsafe sealed class InputStateService : IInputSnapshotService, IDisposable {
		private const int DEVICE_SLOT_COUNT = (int)InputDeviceSlot.Count;
		private const int CONTROL_COUNT = (int)InputControlId.Count;

		private const int WORDS_PER_DEVICE = 4;

		private readonly ulong* _pressedBits;
		private readonly float* _axis1D;
		private readonly Vector2* _axis2D;

		private readonly void* _pFrameStateMemoryBuffer;

		public Vector2 MouseDelta => _mouseDelta;
		private Vector2 _mouseDelta;

		public Vector2 MousePosition => _mousePosition;
		private Vector2 _mousePosition;

		private bool _isDisposed = false;

		public InputStateService() {
			long totalBytes = 0;
			totalBytes += PadBytes( sizeof( ulong ) * DEVICE_SLOT_COUNT * WORDS_PER_DEVICE, Core.Constants.WORDSIZE );
			totalBytes += PadBytes( sizeof( float ) * DEVICE_SLOT_COUNT * CONTROL_COUNT, Core.Constants.WORDSIZE );
			totalBytes += PadBytes( (sizeof( float ) * 2) * DEVICE_SLOT_COUNT * CONTROL_COUNT, Core.Constants.WORDSIZE );

#if NET6_0_OR_GREATER
			_pFrameStateMemoryBuffer = NativeMemory.AlignedAlloc( (nuint)totalBytes, Core.Constants.WORDSIZE );
#else
			_pFrameStateMemoryBuffer = (void*)Marshal.AllocHGlobal( (int)totalBytes );
#endif
			_pressedBits = (ulong*)_pFrameStateMemoryBuffer;
			_axis1D = (float*)((byte*)_pressedBits + PadBytes( sizeof( ulong ) * DEVICE_SLOT_COUNT * WORDS_PER_DEVICE, Core.Constants.WORDSIZE ));
			_axis2D = (Vector2*)((byte*)_axis1D + PadBytes( sizeof( float ) * DEVICE_SLOT_COUNT * CONTROL_COUNT, Core.Constants.WORDSIZE ));
			new Span<byte>( (byte*)_pFrameStateMemoryBuffer, (int)totalBytes ).Clear();
		}

		/*
		===============
		Dispose
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		public void Dispose() {
			if ( !_isDisposed ) {
				if ( _pFrameStateMemoryBuffer != null ) {
#if NET6_0_OR_GREATER
					NativeMemory.AlignedFree( _pFrameStateMemoryBuffer );
#else
					Marshal.FreeHGlobal( (nint)_pFrameStateMemoryBuffer );
#endif
				}
			}
			GC.SuppressFinalize( this );
			_isDisposed = true;
		}

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
			value = GetAxis1DUnchecked( slot, control );
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
			value = GetAxis2DUnchecked( slot, control );
			return true;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public float GetAxis1DUnchecked( InputDeviceSlot slot, InputControlId control ) {
			return _axis1D[GetFlatIndex( slot, control )];
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public Vector2 GetAxis2DUnchecked( InputDeviceSlot slot, InputControlId control ) {
			return _axis2D[GetFlatIndex( slot, control )];
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
		private static bool IsAxis2DControl( InputControlId control ) => control switch {
			InputControlId.LeftStick or InputControlId.RightStick or InputControlId.Delta or InputControlId.Position or InputControlId.Scroll => true,
			_ => false,
		};

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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="size"></param>
		/// <param name="alignment"></param>
		/// <returns></returns>
		private static long PadBytes( long size, long alignment ) {
			return (size + alignment - 1) & ~(alignment - 1);
		}
	};
};
