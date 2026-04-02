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
using Nomad.Input.Interfaces;
using Nomad.Input.Private.Repositories;
using Nomad.Input.ValueObjects;

namespace Nomad.Input.Private.Services {
	/*
	===================================================================================
	
	BindResolver
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	internal sealed class BindResolver : IBindResolver {
		private readonly BindRepository _repository;

		/*
		===============
		BindResolver
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="repository"></param>
		public BindResolver( BindRepository repository ) {
			_repository = repository ?? throw new ArgumentNullException( nameof( repository ) );
		}

		/*
		===============
		GetBindMapping
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="mappingName"></param>
		/// <returns></returns>
		public IReadOnlyList<InputActionDefinition>? GetBindMapping( string mappingName ) {
			return _repository.TryGetBindMapping( mappingName, out var bindings ) ? bindings : null;
		}

		/*
		===============
		SetActionBindings
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="mappingName"></param>
		/// <param name="actionId"></param>
		/// <param name="bindings"></param>
		public void SetActionBindings( string mappingName, string actionId, ImmutableArray<InputBindingDefinition> bindings ) {
			if ( !_repository.TryGetBindMapping( mappingName, out var mapping ) ) {
				return;
			}

			InputActionDefinition? actionDefinition = null;
			for ( int i = 0; i < mapping.Length; i++ ) {
				if ( mapping[ i ].Name.Equals( actionId, StringComparison.InvariantCulture ) ) {
					actionDefinition = mapping[ i ];
					break;
				}
			}
			if ( actionDefinition == null ) {
				return;
			}
			actionDefinition.Bindings = bindings;
		}
	};
};