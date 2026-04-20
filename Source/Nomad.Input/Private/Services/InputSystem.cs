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
using Nomad.Core.ServiceRegistry.Interfaces;
using Nomad.Input.Interfaces;
using Nomad.Input.Private.Extensions;
using Nomad.Input.Private.Registries;
using Nomad.Input.Private.Repositories;
using Nomad.Input.Private.ValueObjects;
using Nomad.Input.ValueObjects;

namespace Nomad.Input.Private.Services {
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
		private readonly IGameEventRegistryService _eventFactory;

		private bool _processEvents = true;
		private bool _isDisposed;

		public InputSystem( IFileSystem fileSystem, ICVarSystemService cvarSystem, ILoggerService logger, IGameEventRegistryService eventFactory, IServiceRegistry registry ) {
			InputCVarRegistry.RegisterCVars( cvarSystem );

			_eventFactory = eventFactory ?? throw new ArgumentNullException( nameof( eventFactory ) );

			_bindRepository = new BindRepository( fileSystem, cvarSystem, logger );
			_compiledBindings = new CompiledBindingRepository();
			_compilerService = new BindingCompilerService( _compiledBindings );

			_stateService = new InputStateService();
			_dispatchService = new InputDispatchService( eventFactory );
			_matcherService = new BindingMatcherService( _compiledBindings, _stateService );
			_actionResolverService = new ActionResolverService( _compiledBindings, _stateService );

			_compilerService.CompileIntoRepository( _bindRepository.GetAllBindings() );

			_bindResolver = new BindResolver( _bindRepository, RecompileBindings );
			_rebindService = new InputRebindService( _bindRepository, _compilerService, eventFactory );

			registry.AddSingleton( _bindResolver );
			registry.AddSingleton( _rebindService );
			registry.AddSingleton<IInputSnapshotService>( _stateService );

			eventFactory.GetEvent<bool>( Core.Constants.Events.EngineUtils.PAUSE_STATE_CHANGED, Core.Constants.Events.EngineUtils.NAMESPACE ).Subscribe( OnPauseStateChanged );
			eventFactory.GetEvent<KeyboardEventArgs>( Core.Constants.Events.Input.KEYBOARD_EVENT, Core.Constants.Events.Input.NAMESPACE ).Subscribe( OnKeyboardEventTriggered );
			eventFactory.GetEvent<MouseButtonEventArgs>( Core.Constants.Events.Input.MOUSE_BUTTON_EVENT, Core.Constants.Events.Input.NAMESPACE ).Subscribe( OnMouseButtonEventTriggered );
			eventFactory.GetEvent<MouseMotionEventArgs>( Core.Constants.Events.Input.MOUSE_MOTION_EVENT, Core.Constants.Events.Input.NAMESPACE ).Subscribe( OnMouseMotionEventTriggered );
			eventFactory.GetEvent<MousePositionChangedEventArgs>( Core.Constants.Events.Input.MOUSE_POSITION_CHANGED_EVENT, Core.Constants.Events.Input.NAMESPACE ).Subscribe( OnMousePositionChangedEventTriggered );
			eventFactory.GetEvent<GamepadAxisEventArgs>( Core.Constants.Events.Input.GAMEPAD_AXIS_EVENT, Core.Constants.Events.Input.NAMESPACE ).Subscribe( OnGamepadAxisEventTriggered );
			eventFactory.GetEvent<GamepadButtonEventArgs>( Core.Constants.Events.Input.GAMEPAD_BUTTON_EVENT, Core.Constants.Events.Input.NAMESPACE ).Subscribe( OnGamepadButtonEventTriggered );
		}

		public void Dispose() {
			if ( _isDisposed ) {
				return;
			}

			_eventFactory.GetEvent<KeyboardEventArgs>( Core.Constants.Events.Input.KEYBOARD_EVENT, Core.Constants.Events.Input.NAMESPACE ).Unsubscribe( OnKeyboardEventTriggered );
			_eventFactory.GetEvent<MouseButtonEventArgs>( Core.Constants.Events.Input.MOUSE_BUTTON_EVENT, Core.Constants.Events.Input.NAMESPACE ).Unsubscribe( OnMouseButtonEventTriggered );
			_eventFactory.GetEvent<MousePositionChangedEventArgs>( Core.Constants.Events.Input.MOUSE_POSITION_CHANGED_EVENT, Core.Constants.Events.Input.NAMESPACE ).Unsubscribe( OnMousePositionChangedEventTriggered );
			_eventFactory.GetEvent<GamepadAxisEventArgs>( Core.Constants.Events.Input.GAMEPAD_AXIS_EVENT, Core.Constants.Events.Input.NAMESPACE ).Unsubscribe( OnGamepadAxisEventTriggered );
			_eventFactory.GetEvent<GamepadButtonEventArgs>( Core.Constants.Events.Input.GAMEPAD_BUTTON_EVENT, Core.Constants.Events.Input.NAMESPACE ).Unsubscribe( OnGamepadButtonEventTriggered );

			( _rebindService as IDisposable )?.Dispose();
			_bindRepository.Dispose();
			_stateService.Dispose();

			_isDisposed = true;
			GC.SuppressFinalize( this );
		}

		public IReadOnlyList<InputActionDefinition>? GetBindMapping( string mapping ) {
			return _bindRepository.TryGetBindMapping( mapping, out var actions ) ? actions : null;
		}

		public void PushKeyboardEvent( in KeyboardEventArgs keyEvent ) {
			_stateService.SetPressed( InputDeviceSlot.Keyboard, keyEvent.KeyNum.ToControlId(), keyEvent.Pressed );

			CompiledBindingGraph graph = _compiledBindings.Current;
			BindingMatchSet matches = _matcherService.MatchKeyboard( graph, in keyEvent, _contextMask, _mode );

			DispatchResolved( graph, _actionResolverService.ResolveMatchesNonAlloc( graph, matches, keyEvent.TimeStamp ) );
			DispatchResolved( graph, _actionResolverService.ResolveKeyboardCompositesNonAlloc( graph, _contextMask, _mode, keyEvent.TimeStamp ) );
		}

		public void PushMouseButtonEvent( in MouseButtonEventArgs mouseButtonEvent ) {
			_stateService.SetPressed( InputDeviceSlot.Mouse, mouseButtonEvent.Button.ToControlId(), mouseButtonEvent.Pressed );

			CompiledBindingGraph graph = _compiledBindings.Current;
			BindingMatchSet matches = _matcherService.MatchMouseButton( graph, in mouseButtonEvent, _contextMask, _mode );

			DispatchResolved( graph, _actionResolverService.ResolveMatchesNonAlloc( graph, matches, mouseButtonEvent.TimeStamp ) );
		}

		public void PushMouseMotionEvent( in MouseMotionEventArgs mouseMotionEvent ) {
			_stateService.AddMouseDelta( new Vector2( mouseMotionEvent.RelativeX, mouseMotionEvent.RelativeY ) );

			CompiledBindingGraph graph = _compiledBindings.Current;
			BindingMatchSet matches = _matcherService.MatchMouseDelta( graph, _contextMask, _mode );

			DispatchResolved( graph, _actionResolverService.ResolveMatchesNonAlloc( graph, matches, mouseMotionEvent.TimeStamp ) );
		}

		public void PushMousePositionChangedEvent( in MousePositionChangedEventArgs mousePositionChangedEvent ) {
			_stateService.SetMousePosition( new Vector2( mousePositionChangedEvent.PositionX, mousePositionChangedEvent.PositionY ) );

			CompiledBindingGraph graph = _compiledBindings.Current;
			BindingMatchSet matches = _matcherService.MatchMouseDelta( graph, _contextMask, _mode );

			DispatchResolved( graph, _actionResolverService.ResolveMatchesNonAlloc( graph, matches, mousePositionChangedEvent.TimeStamp ) );
		}

		public void PushGamepadButtonEvent( in GamepadButtonEventArgs gamepadButtonEvent ) {
			InputDeviceSlot deviceSlot = GetGamepadDeviceSlot( gamepadButtonEvent.DeviceId );
			_stateService.SetPressed( deviceSlot, gamepadButtonEvent.Button.ToControlId(), gamepadButtonEvent.Pressed );

			CompiledBindingGraph graph = _compiledBindings.Current;
			BindingMatchSet matches = _matcherService.MatchGamepadButton( graph, in gamepadButtonEvent, _contextMask, _mode );

			DispatchResolved( graph, _actionResolverService.ResolveMatchesNonAlloc( graph, matches, gamepadButtonEvent.TimeStamp ) );
		}

		public void PushGamepadAxisEvent( in GamepadAxisEventArgs gamepadAxisEvent ) {
			InputDeviceSlot deviceSlot = GetGamepadDeviceSlot( gamepadAxisEvent.DeviceId );
			InputControlId controlId = gamepadAxisEvent.Stick.ToControlId();

			_stateService.SetAxis2D( deviceSlot, controlId, gamepadAxisEvent.Value );

			CompiledBindingGraph graph = _compiledBindings.Current;
			BindingMatchSet matches = _matcherService.MatchGamepadAxis( graph, deviceSlot, controlId, _contextMask, _mode );

			DispatchResolved( graph, _actionResolverService.ResolveMatchesNonAlloc( graph, matches, gamepadAxisEvent.TimeStamp ) );
		}

		public void Update( float delta ) {
		}

		private void DispatchResolved( CompiledBindingGraph graph, ReadOnlySpan<ResolvedAction> actions ) {
			for ( int i = 0; i < actions.Length; i++ ) {
				_dispatchService.Dispatch( graph, in actions[i] );
			}
		}

		private void RecompileBindings() {
			_compilerService.CompileIntoRepository( _bindRepository.GetAllBindings() );
		}

		private static InputDeviceSlot GetGamepadDeviceSlot( int deviceId ) {
			return deviceId switch {
				0 => InputDeviceSlot.Gamepad0,
				1 => InputDeviceSlot.Gamepad1,
				2 => InputDeviceSlot.Gamepad2,
				3 => InputDeviceSlot.Gamepad3,
				_ => throw new ArgumentOutOfRangeException( nameof( deviceId ) )
			};
		}

		private void OnKeyboardEventTriggered( in KeyboardEventArgs args ) {
			if ( _processEvents ) {
				PushKeyboardEvent( in args );
			}
		}

		private void OnMouseButtonEventTriggered( in MouseButtonEventArgs args ) {
			if ( _processEvents ) {
				PushMouseButtonEvent( in args );
			}
		}

		private void OnMouseMotionEventTriggered( in MouseMotionEventArgs args ) {
			if ( _processEvents ) {
				PushMouseMotionEvent( in args );
			}
		}

		private void OnMousePositionChangedEventTriggered( in MousePositionChangedEventArgs args ) {
			if ( _processEvents ) {
				PushMousePositionChangedEvent( in args );
			}
		}

		private void OnGamepadAxisEventTriggered( in GamepadAxisEventArgs args ) {
			if ( _processEvents ) {
				PushGamepadAxisEvent( in args );
			}
		}

		private void OnGamepadButtonEventTriggered( in GamepadButtonEventArgs args ) {
			if ( _processEvents ) {
				PushGamepadButtonEvent( in args );
			}
		}

		private void OnPauseStateChanged( in bool args ) {
			_processEvents = !args;
		}
	}
}