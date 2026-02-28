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

using Nomad.Core.Util;
using Steamworks;

namespace Nomad.OnlineServices.Steam.Private.ValueObjects {
	/*
	===================================================================================
	
	SteamStatData
	
	===================================================================================
	*/
	/// <summary>
	/// Stores stat data for a steam value.
	/// </summary>

	internal struct SteamStatData {
		/// <summary>
		///
		/// </summary>
		public readonly string Name => _name;
		private readonly InternString _name;

		public float Value {
			readonly get => _value;
			set {
				_value = value;
				SteamUserStats.SetStat( _name, _value );
			}
		}
		private float _value;

		/*
		===============
		SteamStatData
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="name"></param>
		public SteamStatData( string name ) {
			_name = new InternString( name );
			SteamUserStats.GetStat( _name, out _value );
		}
	};
};
