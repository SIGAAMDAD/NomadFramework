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

using Nomad.CVars.Private.Repositories;

namespace Nomad.CVars.Private.Services {
	/*
	===================================================================================

	CVarFactory

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	public sealed class CVarFactory : ICVarFactory {
		private readonly CVarRegistry _repository;

		/*
		===============
		CVarFactory
		===============
		*/
		public CVarFactory() {
			_repository = new CVarRegistry();
		}

		/*
		===============
		Register
		===============
		*/
		/// <summary>
		/// Creates a new cvar with the provided creation information.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="createInfo"></param>
		/// <returns></returns>
		public ICVar<T> Register<T>( in CVarCreateInfo<T> createInfo ) {
			var cvar = new CVar<T>( in createInfo );
			_repository.AddCVar( cvar );
			return cvar;
		}
	};
};
