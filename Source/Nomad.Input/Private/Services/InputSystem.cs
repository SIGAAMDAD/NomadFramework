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

using System.Collections.Generic;
using Nomad.Core.Events;
using Nomad.Core.Input;
using Nomad.Input.Private.Repositories;
using Nomad.Input.Private.ValueObjects;

namespace Nomad.Input.Private.Services {
	internal sealed class InputSystem : IInputSystem {
		public InputMode Mode => _mode;
		private InputMode _mode;

		private readonly Stack<InputEventData> _eventPump;

		private readonly BindRepository _bindRepository;
	
		public InputSystem( IGameEventRegistryService eventFactory ) {
			_bindRepository = new BindRepository();
		}

		public void Dispose() {
		}

		public void PushGamepadAxisEvent() {
		}

		public void PushKeyboardEvent( in KeyboardEvent keyEvent ) {
		}

		public void PushMouseButtonEvent( in MouseButtonEvent mouseButtonEvent ) {
		}

		public void PushMouseMotionEvent( in MouseMotionEvent mouseMotionEvent ) {
		}
	};
};