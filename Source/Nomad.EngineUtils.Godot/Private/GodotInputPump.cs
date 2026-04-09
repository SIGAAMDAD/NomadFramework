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
using System.Collections.Generic;
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

	internal sealed partial class GodotInputPump : Node, IInputAdapter {
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

		private readonly Dictionary<int, System.Numerics.Vector2> _leftStickState = new();
		private readonly Dictionary<int, System.Numerics.Vector2> _rightStickState = new();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="eventFactory"></param>
		public GodotInputPump( IGameEventRegistryService eventFactory ) {
			Name = nameof( GodotInputPump );
			ProcessMode = ProcessModeEnum.Always;
			
			_keyboardEvent = eventFactory.GetEvent<KeyboardEventArgs>( Core.Constants.Events.Input.KEYBOARD_EVENT, Core.Constants.Events.Input.NAMESPACE );
			_mouseButtonEvent = eventFactory.GetEvent<MouseButtonEventArgs>( Core.Constants.Events.Input.MOUSE_BUTTON_EVENT, Core.Constants.Events.Input.NAMESPACE );
			_mouseMotionEvent = eventFactory.GetEvent<MouseMotionEventArgs>( Core.Constants.Events.Input.MOUSE_MOTION_EVENT, Core.Constants.Events.Input.NAMESPACE );
			_gamepadAxisEvent = eventFactory.GetEvent<GamepadAxisEventArgs>( Core.Constants.Events.Input.GAMEPAD_AXIS_EVENT, Core.Constants.Events.Input.NAMESPACE );
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
			var now = DateTime.Now.ToFileTimeUtc();
			switch ( @event ) {
				case InputEventKey keyEvent:
					_keyboardEvent.Publish(
						new KeyboardEventArgs(
							GodotKeyToNomadKey( keyEvent.PhysicalKeycode ),
							now,
							keyEvent.Pressed
						)
					);
					break;
				case InputEventMouseButton mouseButton:
					_mouseButtonEvent.Publish(
						new MouseButtonEventArgs(
							GodotMouseButtonToNomadMouseButton( mouseButton.ButtonIndex ),
							now,
							mouseButton.Pressed
						)
					);
					break;
				case InputEventMouseMotion mouseMotion:
					_mouseMotionEvent.Publish(
						new MouseMotionEventArgs(
							now,
							(int)mouseMotion.Relative.X,
							(int)mouseMotion.Relative.Y
						)
					);
					break;
				case InputEventJoypadButton joypadButton:
					_gamepadButtonEvent.Publish(
						new GamepadButtonEventArgs(
							GodotJoypadButtonToNomadGamepadButton( joypadButton.ButtonIndex ),
							joypadButton.Device,
							now,
							joypadButton.Pressed
						)
					);
					break;
				case InputEventJoypadMotion joypadMotion:
					_gamepadAxisEvent.Publish(
						new GamepadAxisEventArgs(
							GodotStickToNomadGamepadStick( joypadMotion.Axis ),
							now,
							joypadMotion.Device,
							UpdateStickValue( joypadMotion )
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
		UpdateStickValue
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="motion"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private System.Numerics.Vector2 UpdateStickValue( InputEventJoypadMotion motion ) {
			Dictionary<int, System.Numerics.Vector2> cache = motion.Axis switch {
				JoyAxis.LeftX or JoyAxis.LeftY => _leftStickState,
				JoyAxis.RightX or JoyAxis.RightY => _rightStickState,
				_ => throw new ArgumentOutOfRangeException( nameof( motion ) ),
			};

			cache.TryGetValue( motion.Device, out var value );

			switch ( motion.Axis ) {
				case JoyAxis.LeftX:
				case JoyAxis.RightX:
					value.X = motion.AxisValue;
					break;
				case JoyAxis.LeftY:
				case JoyAxis.RightY:
					value.Y = motion.AxisValue;
					break;
				default:
					throw new ArgumentOutOfRangeException( nameof( motion ) );
			}

			cache[ motion.Device ] = value;
			return value;
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
			Key.Quoteleft => KeyNum.Grave,
			Key.Key0 => KeyNum.Num0,
			Key.Key1 => KeyNum.Num1,
			Key.Key2 => KeyNum.Num2,
			Key.Key3 => KeyNum.Num3,
			Key.Key4 => KeyNum.Num4,
			Key.Key5 => KeyNum.Num5,
			Key.Key6 => KeyNum.Num6,
			Key.Key7 => KeyNum.Num7,
			Key.Key8 => KeyNum.Num8,
			Key.Key9 => KeyNum.Num9,
			Key.Escape => KeyNum.Escape,
			_ => throw new ArgumentOutOfRangeException( nameof( keyCode ) )
		};
	};
};
