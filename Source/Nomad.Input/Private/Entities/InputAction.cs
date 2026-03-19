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
using System.Reflection.Metadata;
using Nomad.Core.Events;
using Nomad.Core.Util;
using Nomad.Input.Events;
using Nomad.Input.Interfaces;
using Nomad.Input.Private.ValueObjects;

namespace Nomad.Input.Private.Entities {
	internal sealed class InputAction : IInputAction {
		/// <summary>
		/// The action's name.
		/// </summary>
		public string Name => _name;
		private readonly InternString _name;

		/// <summary>
		/// 
		/// </summary>
		public float Value => _value;
		private float _value;

		public bool IsPressed => _value == 1.0f;

		public IGameEvent<BindPressedEventArgs> Pressed => _pressed;
		private readonly IGameEvent<BindPressedEventArgs> _pressed;

		public IGameEvent<BindReleasedEventArgs> Released => _released;
		private readonly IGameEvent<BindReleasedEventArgs> _released;

		public IGameEvent<BindHeldEventArgs> Held => _held;
		private readonly IGameEvent<BindHeldEventArgs> _held;

		public bool Enabled => _enabled;
		private bool _enabled = true;

		public IReadOnlyList<IInputBinding> Bindings => _bindings;
		private readonly InputBinding[] _bindings = new InputBinding[Constants.MAX_ACTION_BINDINGS];

		private bool _isDisposed = false;

		public InputAction( string name, bool enabled, IGameEventRegistryService eventFactory ) {
			_name = new InternString( name );
			_enabled = enabled;

			_held = eventFactory.GetEvent<BindHeldEventArgs>( $"{Name}:{Input.Constants.Events.ACTION_HELD}", Input.Constants.Events.NAMESPACE );
			_pressed = eventFactory.GetEvent<BindPressedEventArgs>( $"{Name}:{Input.Constants.Events.ACTION_PRESSED}", Input.Constants.Events.NAMESPACE );
			_released = eventFactory.GetEvent<BindReleasedEventArgs>( $"{Name}:{Input.Constants.Events.ACTION_RELEASED}", Input.Constants.Events.NAMESPACE );
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
				_held?.Dispose();
				_pressed?.Dispose();
				_released?.Dispose();
			}
			GC.SuppressFinalize( this );
			_isDisposed = true;
		}
	};
};
