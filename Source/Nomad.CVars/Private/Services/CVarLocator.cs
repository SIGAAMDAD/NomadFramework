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

using System.Collections.Concurrent;
using System.Collections.Generic;
using Nomad.Core.Util;
using Nomad.Core.CVars;
using Nomad.CVars.Interfaces;

namespace Nomad.CVars.Private.Services {
	/*
	===================================================================================

	CVarLocator

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	internal sealed class CVarLocator : ICVarLocator {
		private readonly ConcurrentDictionary<InternString, ICVar> _cvars = new();

		/*
		===============
		CVarLocator
		===============
		*/
		/// <summary>
		///
		/// </summary>
		public CVarLocator() {
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
			_cvars.Clear();
		}

		/*
		===============
		CVarExists
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="name"></param>
		/// <returns></returns>
		public bool CVarExists<T>( string name ) {
			return _cvars.TryGetValue( new InternString( name ), out ICVar cvar ) && cvar.Type.GetSystemType() == typeof( T );
		}

		/*
		===============
		CVarExists
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public bool CVarExists( string name ) {
			return _cvars.ContainsKey( new InternString( name ) );
		}

		/*
		===============
		GetCVar
		===============
		*/
		public ICVar<T>? GetCVar<T>( string name ) {
			if ( _cvars.TryGetValue( new InternString( name ), out ICVar cvar ) ) {
				if ( cvar is ICVar<T> typedVar ) {
					return typedVar;
				} else {
				}
			}
			return null;
		}

		/*
		===============
		GetCVar
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public ICVar? GetCVar( string name ) {
			return _cvars.TryGetValue( new InternString( name ), out ICVar cvar ) ? cvar : null;
		}

		/*
		===============
		GetCVars
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		public IReadOnlyList<ICVar> GetCVars() {
			return (IReadOnlyList<ICVar>)_cvars.Values;
		}
	};
};
