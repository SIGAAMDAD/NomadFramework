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

#if !UNITY_EDITOR
using System;
using System.Runtime.CompilerServices;
using Godot;
using Nomad.Core.EngineUtils;
using Nomad.Core.Input;
using Nomad.Core.Input.ValueObjects;

namespace Nomad.EngineUtils.Private {
	/*
	===================================================================================
	
	GodotInputPump
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>

	internal sealed partial class GodotInputPump : Node, IInputAdapter {
		private readonly IInputSystem _system;

		/*
		===============
		GodotInputPump
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="system"></param>
		public GodotInputPump( IInputSystem system ) {
			_system = system;
		}

		/*
		===============
		_Input
		===============
		*/
		/// <summary>
		/// Intercepts all incoming input events.
		/// </summary>
		/// <param name="event"></param>
		public override void _Input( InputEvent @event ) {
			switch ( @event ) {
				case InputEventKey keyEvent:
					_system.PushKeyboardEvent(
						new KeyboardEvent(
							GodotKeyToNomadKey( keyEvent.GetKeycodeWithModifiers() ),
							DateTime.Now,
							keyEvent.Pressed
						)
					);
					break;
				case InputEventMouseButton mouseButton:
					_system.PushMouseButtonEvent(
						new MouseButtonEvent(
							GodotMouseButtonToNomadMouseButton( mouseButton.ButtonIndex ),
							mouseButton.Pressed
						)
					);
					break;
				case InputEventMouseMotion mouseMotion:
					_system.PushMouseMotionEvent(
						new MouseMotionEvent(
							( int )mouseMotion.Position.X,
							( int )mouseMotion.Position.Y
						)
					);
					break;
				case InputEventJoypadButton joypadButton:
					break;
				case InputEventJoypadMotion joypadMotion:
					break;
			}
		}

		/*
		===============
		GodotMouseButtonToNomadMouseButton
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="button"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		private Core.Input.MouseButton GodotMouseButtonToNomadMouseButton( Godot.MouseButton button ) => button switch {
			Godot.MouseButton.Left => Core.Input.MouseButton.Left,
			Godot.MouseButton.Right => Core.Input.MouseButton.Right,
			Godot.MouseButton.Middle => Core.Input.MouseButton.Middle,
			Godot.MouseButton.WheelDown => Core.Input.MouseButton.WheelDown,
			Godot.MouseButton.WheelUp => Core.Input.MouseButton.WheelUp,
			_ => throw new ArgumentOutOfRangeException( nameof( button ) )
		};

		/*
		===============
		GodotKeyToNomadKey
		===============
		*/
		/// <summary>
		/// Translates a godot keycode to a NomadFramework <see cref="KeyNum"/>. Hardcoded switch table because I do not believe we are changing how the alphabet/ASCII works anytime soon
		/// </summary>
		/// <param name="keyCode"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private KeyNum GodotKeyToNomadKey( Key keyCode ) => keyCode switch {
			Key.Q => KeyNum.Q,
			Key.W => KeyNum.W,
			Key.E => KeyNum.E,
			Key.R => KeyNum.R,
			Key.T => KeyNum.T,
			Key.Y => KeyNum.Y,
			Key.U => KeyNum.U,
			Key.I => KeyNum.I,
			Key.O => KeyNum.O,
			Key.P => KeyNum.P,
			Key.A => KeyNum.A,
			Key.S => KeyNum.S,
			Key.D => KeyNum.D,
			Key.F => KeyNum.F,
			Key.G => KeyNum.G,
			Key.H => KeyNum.H,
			Key.J => KeyNum.J,
			Key.K => KeyNum.K,
			Key.L => KeyNum.L,
			Key.Z => KeyNum.Z,
			Key.X => KeyNum.X,
			Key.C => KeyNum.C,
			Key.V => KeyNum.V,
			Key.B => KeyNum.B,
			Key.N => KeyNum.N,
			Key.M => KeyNum.M,
			Key.Backspace => KeyNum.BackSpace,
			Key.Space => KeyNum.Space,
			Key.Tab => KeyNum.Tab,
			Key.Shift => KeyNum.Shift,
			Key.Ctrl => KeyNum.Ctrl,
			Key.Left => KeyNum.LeftArrow,
			Key.Up => KeyNum.UpArrow,
			Key.Down => KeyNum.DownArrow,
			Key.Right => KeyNum.RightArrow,
			Key.Alt => KeyNum.Alt,
			_ => throw new ArgumentOutOfRangeException( nameof( keyCode ) )
		};
	};
};
#endif
