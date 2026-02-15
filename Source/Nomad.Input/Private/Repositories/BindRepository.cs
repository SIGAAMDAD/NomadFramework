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
using System.Collections.Immutable;
using System.Linq;
using Nomad.Input.Private.ValueObjects;
using Nomad.Input.ValueObjects;

namespace Nomad.Input.Private.Repositories {
	/*
	===================================================================================
	
	BindRepository
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	internal sealed class BindRepository : IDisposable {
		private readonly Dictionary<BindKey, Binding> _bindings;
		private readonly Dictionary<InputEventData, BindKey> _bindTriggers;

		private readonly ImmutableDictionary<BindKey, Binding> _defaultMap;

		/*
		===============
		BindRepository
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		public BindRepository( ImmutableDictionary<BindKey, Binding> defaults ) {
			_bindings = new Dictionary<BindKey, Binding>();
			_bindTriggers = new Dictionary<InputEventData, BindKey>();

			_defaultMap = defaults;
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
		}

		/*
		===============
		TryGetBind
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <param name="binding"></param>
		/// <returns></returns>
		public bool TryGetBind( in InputEventData key, out Binding? binding ) {
			if ( _bindTriggers.TryGetValue( key, out var bindKey ) ) {
				binding = _bindings[ bindKey ];
				return true;
			}
			binding = null;
			return false;
		}
		
		/*
		===============
		SetBindingTrigger
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="bindId"></param>
		/// <param name="trigger"></param>
		public void SetBindingTrigger( in BindKey bindId, in InputEventData trigger ) {
			_bindTriggers[ trigger ] = bindId;
		}

		/*
		===============
		Reset
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		public void Reset() {
			_bindings.Clear();
			_bindings.EnsureCapacity( _defaultMap.Count );

			foreach ( var binding in _defaultMap ) {
				_bindings[ binding.Key ] = binding.Value;
			}
		}
	};
};