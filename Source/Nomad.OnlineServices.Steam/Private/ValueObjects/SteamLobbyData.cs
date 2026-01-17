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

using Nomad.Core.OnlineServices;
using Steamworks;

namespace Nomad.OnlineServices.Steam {
	/*
	===================================================================================

	SteamLobbyData

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	internal sealed class SteamLobbyData {
		public string Name => _info.Name;
		public string GameMode => _info.GameMode;
		public string Map => _info.Map;
		public int MaxPlayers => _info.MaxPlayers;
		public LobbyVisibility Visibility => _info.Visibility;
		private LobbyInfo _info;

		public CSteamID Id => _id;
		private readonly CSteamID _id;

		/*
		===============
		SteamLobbyData
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="id"></param>
		/// <param name="info"></param>
		public SteamLobbyData( CSteamID id, LobbyInfo info ) {
			_id = id;
			_info = info;
		}

		/*
		===============
		Update
		===============
		*/
		/// <summary>
		///
		/// </summary>
		public void Update() {
			_info = _info with {
				Name = SteamMatchmaking.GetLobbyData( _id, nameof( LobbyInfo.Name ) ),
				Map = SteamMatchmaking.GetLobbyData( _id, nameof( LobbyInfo.Map ) ),
				GameMode = SteamMatchmaking.GetLobbyData( _id, nameof( LobbyInfo.GameMode ) )
			};
		}
	};
};
