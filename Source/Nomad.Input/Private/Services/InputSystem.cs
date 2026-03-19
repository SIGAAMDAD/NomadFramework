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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Nomad.Core.Collections;
using Nomad.Core.CVars;
using Nomad.Core.Events;
using Nomad.Core.Exceptions;
using Nomad.Core.FileSystem;
using Nomad.Core.Input;
using Nomad.Input.Private.Repositories;
using Nomad.Input.Private.ValueObjects;

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
		public InputMode Mode => _mode;
		private InputMode _mode;

		private readonly ConcurrentQueue<InputEventData> _eventPump;
		private readonly BindRepository _bindRepository;

		private readonly Task _inputPump;
		private readonly CancellationToken _inputCancellation;

		private int _inputDelayMs = 100;

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
		/// <param name="eventFactory"></param>
		public InputSystem( IFileSystem fileSystem, ICVarSystemService cvarSystem, IGameEventRegistryService eventFactory ) {
			_bindRepository = new BindRepository( fileSystem, cvarSystem );
			_eventPump = new ConcurrentQueue<InputEventData>();

			var inputDelayMs = cvarSystem.GetCVar<int>( Constants.CVars.INPUT_DELAY_MS ) ?? throw new CVarMissing( Constants.CVars.INPUT_DELAY_MS );
			_inputDelayMs = inputDelayMs.Value;

			_inputCancellation = new CancellationToken();
			_inputPump = Task.Run( InputPumpWorker );
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
				_bindRepository?.Dispose();
			}
			GC.SuppressFinalize( this );
			_isDisposed = true;
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
		public void PushGamepadAxisEvent( in GamepadAxisEvent gamepadAxisEvent ) {
			_eventPump.Enqueue( new InputEventData( in gamepadAxisEvent ) );
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
		public void PushGamepadButtonEvent( in GamepadButtonEvent gamepadButtonEvent ) {
			_eventPump.Enqueue( new InputEventData( in gamepadButtonEvent ) );
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
		public void PushKeyboardEvent( in KeyboardEvent keyEvent ) {
			_eventPump.Enqueue( new InputEventData( in keyEvent ) );
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
		public void PushMouseButtonEvent( in MouseButtonEvent mouseButtonEvent ) {
			_eventPump.Enqueue( new InputEventData( in mouseButtonEvent ) );
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
		public void PushMouseMotionEvent( in MouseMotionEvent mouseMotionEvent ) {
			_eventPump.Enqueue( new InputEventData( in mouseMotionEvent ) );
		}

		/*
		===============
		InputPumpWorker
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		private async Task InputPumpWorker() {
			while ( true ) {
				_inputCancellation.ThrowIfCancellationRequested();
				while ( _eventPump.TryDequeue( out var inputEvent ) ) {
					_inputCancellation.ThrowIfCancellationRequested();
				}
				await Task.Delay( _inputDelayMs );
			}
		}
	};
};
