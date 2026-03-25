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
using Godot;
using Nomad.Core.Engine.Services;
using Nomad.Core.Events;
using Nomad.Core.Input;
using Nomad.Core.Input.ValueObjects;

namespace Nomad.EngineUtils.Godot.Private {
	/*
	===================================================================================

	GodotInputPump

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	public sealed partial class GodotInputPump : Node, IInputAdapter {
		/// <summary>
		/// 
		/// </summary>
		public IGameEvent<KeyboardEventArgs> KeyboardEvent => _keyboardEvent;
		private readonly IGameEvent<KeyboardEventArgs> _keyboardEvent;

		/// <summary>
		/// 
		/// </summary>
		public IGameEvent<MouseButtonEventArgs> MouseButtonEvent => _mouseButtonEvent;
		private readonly IGameEvent<MouseButtonEventArgs> _mouseButtonEvent;

		/// <summary>
		/// 
		/// </summary>
		public IGameEvent<MouseMotionEventArgs> MouseMotionEvent => _mouseMotionEvent;
		private readonly IGameEvent<MouseMotionEventArgs> _mouseMotionEvent;

		/// <summary>
		/// 
		/// </summary>
		public IGameEvent<GamepadAxisEventArgs> GamepadAxisEvent => _gamepadAxisEvent;
		private readonly IGameEvent<GamepadAxisEventArgs> _gamepadAxisEvent;

		/// <summary>
		/// 
		/// </summary>
		public IGameEvent<GamepadButtonEventArgs> GamepadButtonEvent => _gamepadButtonEvent;
		private readonly IGameEvent<GamepadButtonEventArgs> _gamepadButtonEvent;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="eventFactory"></param>
		public GodotInputPump( IGameEventRegistryService eventFactory ) {
			_keyboardEvent = eventFactory.GetEvent<KeyboardEventArgs>( Core.Constants.Events.Input.KEYBOARD_EVENT, Constants.Events.NAMESPACE );
			_mouseButtonEvent = eventFactory.GetEvent<MouseButtonEventArgs>( Core.Constants.Events.Input.MOUSE_BUTTON_EVENT, Constants.Events.NAMESPACE );
			_mouseMotionEvent = eventFactory.GetEvent<MouseMotionEventArgs>( Core.Constants.Events.Input.MOUSE_MOTION_EVENT, Constants.Events.NAMESPACE );
			_gamepadAxisEvent = eventFactory.GetEvent<GamepadAxisEventArgs>( Core.Constants.Events.Input.GAMEPAD_AXIS_EVENT, Constants.Events.NAMESPACE );
			_gamepadButtonEvent = eventFactory.GetEvent<GamepadButtonEventArgs>( Core.Constants.Events.Input.GAMEPAD_BUTTON_EVENT, Core.Constants.Events.Input.NAMESPACE );
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
			var now = DateTime.UtcNow.ToFileTimeUtc();
			switch ( @event ) {
				case InputEventKey keyEvent:
					_keyboardEvent.Publish(
						new KeyboardEvent(
							GodotKeyToNomadKey( keyEvent.GetPhysicalKeycodeWithModifiers() ),
							now,
							keyEvent.Pressed
						)
					);
					break;
				case InputEventMouseButton mouseButton:
					_mouseButtonEvent.Publish(
						new MouseButtonEvent(
							GodotMouseButtonToNomadMouseButton( mouseButton.ButtonIndex ),
							now,
							mouseButton.Pressed
						)
					);
					break;
				case InputEventMouseMotion mouseMotion:
					_mouseMotionEvent.Publish(
						new MouseMotionEvent(
							now,
							(int)mouseMotion.Position.X,
							(int)mouseMotion.Position.Y
						)
					);
					break;
				case InputEventJoypadButton joypadButton:
					_gamepadButtonEvent.Publish(
						new GamepadButtonEvent(
							GodotJoypadButtonToNomadGamepadButton( joypadButton.ButtonIndex ),
							joypadButton.Device,
							now,
							joypadButton.Pressed
						)
					);
					break;
				case InputEventJoypadMotion joypadMotion:
					_gamepadAxisEvent.Publish(
						new GamepadAxisEvent(
							GodotStickToNomadGamepadStick( joypadMotion.Axis ),
							now,
							joypadMotion.Device,
							GodotAxisValueToVector2( joypadMotion )
						)
					);
					break;
				case InputEventScreenDrag screenDrag:
					break;
				case InputEventScreenTouch screenTouch:
					break;
			}
		}

		/*
		===============
		GodotAxisValueToVector2
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="motion"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private static System.Numerics.Vector2 GodotAxisValueToVector2( InputEventJoypadMotion motion ) {
			return motion.Axis switch {
				JoyAxis.LeftX or JoyAxis.RightX => new System.Numerics.Vector2( motion.AxisValue, 0.0f ),
				JoyAxis.LeftY or JoyAxis.RightY => new System.Numerics.Vector2( 0.0f, motion.AxisValue ),
				_ => throw new ArgumentOutOfRangeException( nameof( motion ) ),
			};
		}

		/*
		===============
		GodotStickToNomadGamepadStick
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="axis"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private static GamepadStick GodotStickToNomadGamepadStick( JoyAxis axis ) {
			return axis switch {
				JoyAxis.LeftX or JoyAxis.LeftY => GamepadStick.Left,
				JoyAxis.RightX or JoyAxis.RightY => GamepadStick.Right,
				_ => throw new ArgumentOutOfRangeException( nameof( axis ) ),
			};
		}

		/*
		===============
		GodotJoypadButtonToNomadGamepadButton
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="button"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private static GamepadButton GodotJoypadButtonToNomadGamepadButton( JoyButton button ) => button switch {
			JoyButton.A => GamepadButton.A,
			JoyButton.B => GamepadButton.B,
			JoyButton.X => GamepadButton.X,
			JoyButton.Y => GamepadButton.Y,
			JoyButton.DpadDown => GamepadButton.DPadDown,
			JoyButton.DpadUp => GamepadButton.DPadUp,
			JoyButton.DpadLeft => GamepadButton.DPadLeft,
			JoyButton.DpadRight => GamepadButton.DPadRight,
			JoyButton.LeftShoulder => GamepadButton.LeftShoulder,
			JoyButton.RightShoulder => GamepadButton.RightShoulder,
			JoyButton.LeftStick => GamepadButton.LeftStick,
			JoyButton.RightStick => GamepadButton.RightStick,
			JoyButton.Back => GamepadButton.Back,
			JoyButton.Guide => GamepadButton.Guide,
			JoyButton.Start => GamepadButton.Start,
			_ => throw new ArgumentOutOfRangeException( nameof( button ) )
		};

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
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private static Core.Input.MouseButton GodotMouseButtonToNomadMouseButton( global::Godot.MouseButton button ) => button switch {
			global::Godot.MouseButton.Left => Core.Input.MouseButton.Left,
			global::Godot.MouseButton.Right => Core.Input.MouseButton.Right,
			global::Godot.MouseButton.Middle => Core.Input.MouseButton.Middle,
			global::Godot.MouseButton.WheelDown => Core.Input.MouseButton.WheelDown,
			global::Godot.MouseButton.WheelUp => Core.Input.MouseButton.WheelUp,
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
		private static KeyNum GodotKeyToNomadKey( Key keyCode ) => keyCode switch {
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
