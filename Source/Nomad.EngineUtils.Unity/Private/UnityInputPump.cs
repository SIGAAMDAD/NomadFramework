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

#pragma warning disable CA1812
using System;
using System.Collections.Generic;
using Nomad.Core.Engine.Services;
using Nomad.Core.Events;
using Nomad.Core.Input;
using Nomad.Core.Input.ValueObjects;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Nomad.EngineUtils.Private {
	/*
	===================================================================================
	
	UnityInputPump
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>

	internal sealed class UnityInputPump : MonoBehaviour, IInputAdapter {
		private readonly struct KeyTranslation {
			public Key UnityKey => _unityKey;
			private readonly Key _unityKey;

			public KeyNum NomadKey => _nomadKey;
			private readonly KeyNum _nomadKey;

			public KeyTranslation( Key unityKey, KeyNum nomadKey ) {
				_unityKey = unityKey;
				_nomadKey = nomadKey;
			}
		};

		private static readonly KeyTranslation[] _keyboardMap = {
			new KeyTranslation( Key.Q, KeyNum.Q ),
			new KeyTranslation( Key.W, KeyNum.W ),
			new KeyTranslation( Key.E, KeyNum.E ),
			new KeyTranslation( Key.R, KeyNum.R ),
			new KeyTranslation( Key.T, KeyNum.T ),
			new KeyTranslation( Key.Y, KeyNum.Y ),
			new KeyTranslation( Key.U, KeyNum.U ),
			new KeyTranslation( Key.I, KeyNum.I ),
			new KeyTranslation( Key.O, KeyNum.O ),
			new KeyTranslation( Key.P, KeyNum.P ),
			new KeyTranslation( Key.A, KeyNum.A ),
			new KeyTranslation( Key.S, KeyNum.S ),
			new KeyTranslation( Key.D, KeyNum.D ),
			new KeyTranslation( Key.F, KeyNum.F ),
			new KeyTranslation( Key.G, KeyNum.G ),
			new KeyTranslation( Key.H, KeyNum.H ),
			new KeyTranslation( Key.J, KeyNum.J ),
			new KeyTranslation( Key.K, KeyNum.K ),
			new KeyTranslation( Key.L, KeyNum.L ),
			new KeyTranslation( Key.Z, KeyNum.Z ),
			new KeyTranslation( Key.X, KeyNum.X ),
			new KeyTranslation( Key.C, KeyNum.C ),
			new KeyTranslation( Key.V, KeyNum.V ),
			new KeyTranslation( Key.B, KeyNum.B ),
			new KeyTranslation( Key.N, KeyNum.N ),
			new KeyTranslation( Key.M, KeyNum.M ),
			new KeyTranslation( Key.Backspace, KeyNum.BackSpace ),
			new KeyTranslation( Key.Space, KeyNum.Space ),
			new KeyTranslation( Key.Tab, KeyNum.Tab ),
            new KeyTranslation( Key.LeftShift, KeyNum.Shift ),
			new KeyTranslation( Key.RightShift, KeyNum.Shift ),
			new KeyTranslation( Key.LeftCtrl, KeyNum.Ctrl ),
			new KeyTranslation( Key.RightCtrl, KeyNum.Ctrl ),
			new KeyTranslation( Key.LeftAlt, KeyNum.Alt ),
			new KeyTranslation( Key.RightAlt, KeyNum.Alt ),
			new KeyTranslation( Key.LeftArrow, KeyNum.LeftArrow ),
			new KeyTranslation( Key.UpArrow, KeyNum.UpArrow ),
			new KeyTranslation( Key.DownArrow, KeyNum.DownArrow ),
			new KeyTranslation( Key.RightArrow, KeyNum.RightArrow )
		};

		private const float AXIS_EPSILON = 0.0001f;

		private IInputSystem _system;
		private Vector2 _lastMousePosition;

		private readonly Dictionary<int, Vector2> _lastLeftStick = new();
		private readonly Dictionary<int, Vector2> _lastRightStick = new();

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
		/// <param name="system"></param>
		/// <exception cref="ArgumentNullException"></exception>
		public void Initialize( IInputSystem system ) {
			_system = system ?? throw new ArgumentNullException( nameof( system ) );
		}

		/// <summary>
		/// 
		/// </summary>
		private void Awake() {
			_lastMousePosition = Mouse.current?.position.ReadValue() ?? Vector2.zero;
		}

		/// <summary>
		/// 
		/// </summary>
		private void Update() {
			if ( _system == null ) {
				return;
			}

			long now = DateTime.UtcNow.ToFileTimeUtc();

			PumpKeyboard( now );
			PumpMouse( now );
			PumpGamepads( now );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="now"></param>
		private void PumpKeyboard( long now ) {
			var keyboard = Keyboard.current;
			if ( keyboard == null ) {
				return;
			}

			foreach ( var translation in _keyboardMap ) {
				var control = keyboard[translation.UnityKey];
				if ( control == null ) {
					continue;
				}

				if ( control.wasPressedThisFrame ) {
					_system.PushKeyboardEvent(
						new KeyboardEventArgs( translation.NomadKey, now, true )
					);
				}

				if ( control.wasReleasedThisFrame ) {
					_system.PushKeyboardEvent(
						new KeyboardEventArgs( translation.NomadKey, now, false )
					);
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="now"></param>
		private void PumpMouse( long now ) {
			var mouse = Mouse.current;
			if ( mouse == null ) {
				return;
			}

			PumpMouseButton( now, mouse.leftButton, MouseButton.Left );
			PumpMouseButton( now, mouse.rightButton, MouseButton.Right );
			PumpMouseButton( now, mouse.middleButton, MouseButton.Middle );

			Vector2 position = mouse.position.ReadValue();
			Vector2 delta = mouse.delta.ReadValue();

			if ( delta != Vector2.zero || position != _lastMousePosition ) {
				_system.PushMouseMotionEvent(
					new MouseMotionEventArgs(
						now,
						Mathf.RoundToInt( position.x ),
						Mathf.RoundToInt( position.y )
					)
				);

				_lastMousePosition = position;
			}

			// Unity scroll is a delta, not a pressed/released wheel button,
			// so synthesize a tap-like press/release pair.
			Vector2 scroll = mouse.scroll.ReadValue();
			if ( scroll.y > 0f ) {
				EmitScrollWheel( now, MouseButton.WheelUp );
			} else if ( scroll.y < 0f ) {
				EmitScrollWheel( now, MouseButton.WheelDown );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="now"></param>
		/// <param name="control"></param>
		/// <param name="button"></param>
		private void PumpMouseButton( long now, ButtonControl control, MouseButton button ) {
			if ( control.wasPressedThisFrame ) {
				_system.PushMouseButtonEvent(
					new MouseButtonEventArgs( button, now, true )
				);
			}

			if ( control.wasReleasedThisFrame ) {
				_system.PushMouseButtonEvent(
					new MouseButtonEventArgs( button, now, false )
				);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="now"></param>
		/// <param name="button"></param>
		private void EmitScrollWheel( long now, MouseButton button ) {
			_system.PushMouseButtonEvent( new MouseButtonEventArgs( button, now, true ) );
			_system.PushMouseButtonEvent( new MouseButtonEventArgs( button, now, false ) );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="now"></param>
		private void PumpGamepads( long now ) {
			foreach ( var gamepad in Gamepad.all ) {
				int deviceId = gamepad.deviceId;

				PumpGamepadButton( now, gamepad.buttonSouth, deviceId, GamepadButton.A );
				PumpGamepadButton( now, gamepad.buttonEast, deviceId, GamepadButton.B );
				PumpGamepadButton( now, gamepad.buttonWest, deviceId, GamepadButton.X );
				PumpGamepadButton( now, gamepad.buttonNorth, deviceId, GamepadButton.Y );

				PumpGamepadButton( now, gamepad.dpad.up, deviceId, GamepadButton.DPadUp );
				PumpGamepadButton( now, gamepad.dpad.down, deviceId, GamepadButton.DPadDown );
				PumpGamepadButton( now, gamepad.dpad.left, deviceId, GamepadButton.DPadLeft );
				PumpGamepadButton( now, gamepad.dpad.right, deviceId, GamepadButton.DPadRight );

				PumpGamepadButton( now, gamepad.leftShoulder, deviceId, GamepadButton.LeftShoulder );
				PumpGamepadButton( now, gamepad.rightShoulder, deviceId, GamepadButton.RightShoulder );

				PumpGamepadButton( now, gamepad.leftStickButton, deviceId, GamepadButton.LeftStick );
				PumpGamepadButton( now, gamepad.rightStickButton, deviceId, GamepadButton.RightStick );

				PumpGamepadButton( now, gamepad.selectButton, deviceId, GamepadButton.Back );
				PumpGamepadButton( now, gamepad.startButton, deviceId, GamepadButton.Start );

				PumpStick(
					now,
					deviceId,
					GamepadStick.Left,
					gamepad.leftStick.ReadValue(),
					_lastLeftStick
				);

				PumpStick(
					now,
					deviceId,
					GamepadStick.Right,
					gamepad.rightStick.ReadValue(),
					_lastRightStick
				);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="now"></param>
		/// <param name="control"></param>
		/// <param name="deviceId"></param>
		/// <param name="button"></param>
		private void PumpGamepadButton( long now, ButtonControl control, int deviceId, GamepadButton button ) {
			if ( control == null ) {
				return;
			}

			if ( control.wasPressedThisFrame ) {
				_system.PushGamepadButtonEvent(
					new GamepadButtonEventArgs( button, deviceId, now, true )
				);
			}

			if ( control.wasReleasedThisFrame ) {
				_system.PushGamepadButtonEvent(
					new GamepadButtonEventArgs( button, deviceId, now, false )
				);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="now"></param>
		/// <param name="deviceId"></param>
		/// <param name="stick"></param>
		/// <param name="currentValue"></param>
		/// <param name="cache"></param>
		private void PumpStick( long now, int deviceId, GamepadStick stick, Vector2 currentValue, Dictionary<int, Vector2> cache ) {
			if ( !cache.TryGetValue( deviceId, out var previousValue ) ||
				(currentValue - previousValue).sqrMagnitude > (AXIS_EPSILON * AXIS_EPSILON) ) {
				cache[deviceId] = currentValue;

				_system.PushGamepadAxisEvent(
					new GamepadAxisEventArgs(
						stick,
						now,
						deviceId,
						new System.Numerics.Vector2( currentValue.x, currentValue.y )
					)
				);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		private void OnDisable() {
			_lastLeftStick.Clear();
			_lastRightStick.Clear();
		}
	};
};
#pragma warning restore CA1812
