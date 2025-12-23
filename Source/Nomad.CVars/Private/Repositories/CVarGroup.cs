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

using System.Collections.Generic;
using System;
using Nomad.Core.Util;
using Nomad.Core.Logger;

namespace Nomad.CVars.Private.Repositories {
	/*
	===================================================================================

	CVarGroup

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	public sealed class CVarGroup( string name, ILoggerService logger, ICVarSystemService cvarSystem ) {
		public readonly InternString Name = new( name );
		public readonly HashSet<InternString> Cvars = new HashSet<InternString>();

		private readonly ILoggerService _logger = logger;
		private readonly ICVarSystemService _cvarSystem = cvarSystem;

		/*
		===============
		Add
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="name"></param>
		public void Add( InternString name ) {
			ArgumentException.ThrowIfNullOrEmpty( name );

			if ( !_cvarSystem.CVarExists( name ) ) {
				_logger.PrintError( $"CVarGroup.Add: cvar '{name}' doesn't exist!" );
			} else if ( !Cvars.Add( name ) ) {
				_logger.PrintError( $"CVarGroup.Add: cvar '{name}' already added to group '{Name}'!" );
			}
		}
	};
};
