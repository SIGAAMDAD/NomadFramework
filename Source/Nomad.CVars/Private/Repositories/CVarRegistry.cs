/*
===========================================================================
The Nomad Framework
Copyright (C) 2025 Noah Van Til

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

namespace Nomad.CVars.Private.Repositories {
	/*
	===================================================================================

	CVarRegistry

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	internal sealed class CVarRegistry {
		private readonly HashSet<ICVar> _cvarCollection;

		public CVarRegistry() {
			_cvarCollection = new HashSet<ICVar>();
		}

		public void AddCVar<T>( ICVar<T> cvar ) {
			_cvarCollection.Add( cvar );
		}

		/*
		===============
		Save
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="configFile"></param>
		public void Save( string configFile ) {
			ArgumentException.ThrowIfNullOrEmpty( configFile );
		}

		/*
		===============
		Load
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="configFile"></param>
		public void Load( string configFile ) {
			ArgumentException.ThrowIfNullOrEmpty( configFile );
		}
	};
};
