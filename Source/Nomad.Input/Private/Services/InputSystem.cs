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
using System.Numerics;
using Nomad.Core.CVars;
using Nomad.Core.Events;
using Nomad.Core.FileSystem;
using Nomad.Core.Input;
using Nomad.Core.Logger;
using Nomad.Input.Private.Extensions;
using Nomad.Input.Private.Registries;
using Nomad.Input.Private.Repositories;
using Nomad.Input.Private.ValueObjects;
using Nomad.Input.ValueObjects;
using Nomad.Input.Interfaces;
using Nomad.Core.ServiceRegistry.Interfaces;

namespace Nomad.Input.Private.Services {
	/*
	===================================================================================
	
	InputSystem
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>

	internal sealed class InputSystem : IInputSystem {
		public InputScheme Mode => _mode;
		private InputScheme _mode;

		public uint ContextMask => _contextMask;
		private uint _contextMask = 0xFFFFFFFF;

		private readonly BindRepository _bindRepository;
		
		private readonly BindingMatcherService _matcherService;
		private readonly ActionResolverService _actionResolverService;

		private readonly InputStateService _stateService;
		private readonly InputDispatchService _dispatchService;
		
		private readonly BindingCompilerService _compilerService;
		private readonly CompiledBindingRepository _compiledBindings;

		private readonly IBindResolver _bindResolver;
		private readonly IInputRebindService _rebindService;

		private readonly ISubscriptionHandle _keyboardEvent;
		private readonly ISubscriptionHandle _mouseButtonEvent;
		private readonly ISubscriptionHandle _mouseMotionEvent;
		private readonly ISubscriptionHandle _gamepadAxisEvent;
		private readonly ISubscriptionHandle _gamepadButtonEvent;

		private readonly ILoggerService _logger;
		
		private bool _isDisposed = false;

		/*
		===============
		InputSystem
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="fileSystem"></param>
		/// <param name="cvarSystem"></param>
		/// <param name="logger"></param>
		/// <param name="eventFactory"></param>
		/// <param name="registry"></param>
		public InputSystem( IFileSystem fileSystem, ICVarSystemService cvarSystem, ILoggerService logger, IGameEventRegistryService eventFactory, IServiceRegistry registry ) {
			InputCVarRegistry.RegisterCVars( cvarSystem );

			_logger = logger;

			_bindRepository = new BindRepository( fileSystem, cvarSystem, logger );
			_dispatchService = new InputDispatchService( eventFactory );

			_compiledBindings = new CompiledBindingRepository();
			_compilerService = new BindingCompilerService( _compiledBindings );

			_stateService = new InputStateService();

			_compilerService.CompileIntoRepository( _bindRepository.GetAllBindings() );
			_matcherService = new BindingMatcherService( _compiledBindings, _stateService );
			_actionResolverService = new ActionResolverService( _compiledBindings, _stateService );

			_bindResolver = new BindResolver( _bindRepository, RecompileBindings );
			registry.AddSingleton( _bindResolver );

			_rebindService = new InputRebindService( _bindRepository, _compilerService, eventFactory );
			registry.AddSingleton( _rebindService );

			var keyboardEvent = eventFactory.GetEvent<KeyboardEventArgs>( Core.Constants.Events.Input.KEYBOARD_EVENT, Core.Constants.Events.Input.NAMESPACE );
			_keyboardEvent = keyboardEvent.Subscribe( OnKeyboardEventTriggered );

			var mouseButtonEvent = eventFactory.GetEvent<MouseButtonEventArgs>( Core.Constants.Events.Input.MOUSE_BUTTON_EVENT, Core.Constants.Events.Input.NAMESPACE );
			_mouseButtonEvent = mouseButtonEvent.Subscribe( OnMouseButtonEventTriggered );

			var mouseMotionEvent = eventFactory.GetEvent<MouseMotionEventArgs>( Core.Constants.Events.Input.MOUSE_MOTION_EVENT, Core.Constants.Events.Input.NAMESPACE );
			_mouseMotionEvent = mouseMotionEvent.Subscribe( OnMouseMotionEventTriggered );

			var gamepadAxisEvent = eventFactory.GetEvent<GamepadAxisEventArgs>( Core.Constants.Events.Input.GAMEPAD_AXIS_EVENT, Core.Constants.Events.Input.NAMESPACE );
			_gamepadAxisEvent = gamepadAxisEvent.Subscribe( OnGamepadAxisEventTriggered );

			var gamepadButtonEvent = eventFactory.GetEvent<GamepadButtonEventArgs>( Core.Constants.Events.Input.GAMEPAD_BUTTON_EVENT, Core.Constants.Events.Input.NAMESPACE );
			_gamepadButtonEvent = gamepadButtonEvent.Subscribe( OnGamepadButtonEventTriggered );
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
				_keyboardEvent?.Dispose();
				_mouseButtonEvent?.Dispose();
				_mouseMotionEvent?.Dispose();
				_gamepadAxisEvent?.Dispose();
				_gamepadButtonEvent?.Dispose();
				( _rebindService as IDisposable )?.Dispose();

				_bindRepository?.Dispose();
			}
			GC.SuppressFinalize( this );
			_isDisposed = true;
		}

		/*
		===============
		GetBindMapping
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="mapping"></param>
		/// <returns></returns>
		public IReadOnlyList<InputActionDefinition>? GetBindMapping( string mapping ) {
			return _bindRepository.TryGetBindMapping( mapping, out var actions ) ? actions : null;
		}

		/*
		===============
		PushGamepadAxisEvent
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="gamepadAxisEvent"></param>
		public void PushGamepadAxisEvent( in GamepadAxisEventArgs gamepadAxisEvent ) {
			InputDeviceSlot deviceSlot = GetGamepadDeviceSlot( gamepadAxisEvent.DeviceId );
			InputControlId controlId = gamepadAxisEvent.Stick.ToControlId();

			_stateService.SetAxis2D( deviceSlot, controlId, gamepadAxisEvent.Value );
			var matches = _matcherService.MatchGamepadAxis( deviceSlot, controlId, _contextMask, _mode ).Span;
			ResolveBindMatches( ref matches, gamepadAxisEvent.TimeStamp );
		}

		/*
		===============
		PushGamepadButtonEvent
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="gamepadButtonEvent"></param>
		public void PushGamepadButtonEvent( in GamepadButtonEventArgs gamepadButtonEvent ) {
			InputDeviceSlot deviceSlot = GetGamepadDeviceSlot( gamepadButtonEvent.DeviceId );

			_stateService.SetPressed( deviceSlot, gamepadButtonEvent.Button.ToControlId(), gamepadButtonEvent.Pressed );
			var matches = _matcherService.MatchGamepadButton( in gamepadButtonEvent, _contextMask, _mode ).Span;
			ResolveBindMatches( ref matches, gamepadButtonEvent.TimeStamp );
		}
		
		/*
		===============
		PushKeyboardEvent
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="keyEvent"></param>
		public void PushKeyboardEvent( in KeyboardEventArgs keyEvent ) {
			_stateService.SetPressed( InputDeviceSlot.Keyboard, keyEvent.KeyNum.ToControlId(), keyEvent.Pressed );
			var matches = _matcherService.MatchKeyboard( in keyEvent, _contextMask, _mode ).Span;
			ResolveBindMatches( ref matches, keyEvent.TimeStamp );
		}

		/*
		===============
		PushMouseButtonEvent
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="mouseButtonEvent"></param>
		public void PushMouseButtonEvent( in MouseButtonEventArgs mouseButtonEvent ) {
			_stateService.SetPressed( InputDeviceSlot.Mouse, mouseButtonEvent.Button.ToControlId(), mouseButtonEvent.Pressed );
			var matches = _matcherService.MatchMouseButton( in mouseButtonEvent, _contextMask, _mode ).Span;
			ResolveBindMatches( ref matches, mouseButtonEvent.TimeStamp );
		}

		/*
		===============
		PushMouseMotionEvent
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="mouseMotionEvent"></param>
		public void PushMouseMotionEvent( in MouseMotionEventArgs mouseMotionEvent ) {
			_stateService.AddMouseDelta( new Vector2( mouseMotionEvent.RelativeX, mouseMotionEvent.RelativeY ) );
			var matches = _matcherService.MatchMouseDelta( _contextMask, _mode ).Span;
			ResolveBindMatches( ref matches, mouseMotionEvent.TimeStamp );
		}

		/*
		===============
		Update
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="delta"></param>
		public void Update( float delta ) {
		}

		/*
		===============
		ResolveBindMatches
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="matches"></param>
		/// <param name="timeStamp"></param>
		private void ResolveBindMatches( ref ReadOnlySpan<BindingMatch> matches, long timeStamp ) {
			var actions = _actionResolverService.ResolveMatches( ref matches, timeStamp );
			actions = actions.AddRange( _actionResolverService.ResolveComposites( _contextMask, _mode, timeStamp ) );
			foreach ( var action in actions ) {
				_dispatchService.Dispatch( in action );
			}
		}

		/*
		===============
		RecompileBindings
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		private void RecompileBindings() {
			_compilerService.CompileIntoRepository( _bindRepository.GetAllBindings() );
		}

		/*
		===============
		GetGamepadDeviceSlot
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="deviceId"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		private static InputDeviceSlot GetGamepadDeviceSlot( int deviceId ) {
			return deviceId switch {
				0 => InputDeviceSlot.Gamepad0,
				1 => InputDeviceSlot.Gamepad1,
				2 => InputDeviceSlot.Gamepad2,
				3 => InputDeviceSlot.Gamepad3,
				_ => throw new ArgumentOutOfRangeException( nameof( deviceId ) )
			};
		}

		/*
		===============
		OnGamepadButtonEventTriggered
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		private void OnGamepadButtonEventTriggered( in GamepadButtonEventArgs args ) {
			PushGamepadButtonEvent( in args );
		}

		/*
		===============
		OnGamepadAxisEventTriggered
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		private void OnGamepadAxisEventTriggered( in GamepadAxisEventArgs args ) {
			PushGamepadAxisEvent( in args );
		}

		/*
		===============
		OnMouseMotionEventTriggered
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		private void OnMouseMotionEventTriggered( in MouseMotionEventArgs args ) {
			PushMouseMotionEvent( in args );
		}

		/*
		===============
		OnMouseButtonEventTriggered
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		private void OnMouseButtonEventTriggered( in MouseButtonEventArgs args ) {
			PushMouseButtonEvent( in args );
		}

		/*
		===============
		OnKeyboardEventTriggered
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		private void OnKeyboardEventTriggered( in KeyboardEventArgs args ) {
			PushKeyboardEvent( in args );
		}
	};
};
