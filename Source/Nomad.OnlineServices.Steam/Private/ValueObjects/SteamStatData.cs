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

using System.Runtime.InteropServices;
using Steamworks;

namespace Nomad.OnlineServices.Steam.Private.ValueObjects {
	/// <summary>
	/// Stores stat data for a steam value.
	/// </summary>
	/// <param name="name"></param>
	/// <param name="value"></param>
	internal struct SteamStatData {
		[StructLayout( LayoutKind.Explicit, Pack = 1, Size = 4 )]
		private struct ValueData {
			[FieldOffset( 0 )] public float FloatValue;
			[FieldOffset( 0 )] public float IntValue;
		};

		/// <summary>
		///
		/// </summary>
		public readonly string Name => _name;
		private readonly string _name;

		/// <summary>
		///
		/// </summary>
		public float Value {
			readonly get => _value;
			set {
				_value = value;
				SteamUserStats.SetStat( _name, value );
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
			_name = name;
			SteamUserStats.GetStat( _name, out _value );
		}
	};
};
