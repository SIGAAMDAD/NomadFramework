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
using System.Threading.Tasks;
using Nomad.Core.OnlineServices;
using Steamworks;

namespace Nomad.OnlineServices.Steam.Private {
	/*
	===================================================================================

	SteamMatchMakingService

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	internal sealed class SteamMatchMakingService : IMatchMakingService {
		private readonly List<LobbyInfo> _lobbies = new();

		private readonly CallResult<LobbyMatchList_t> _lobbyMatchList;

		/*
		===============
		SteamMatchMakingService
		===============
		*/
		/// <summary>
		///
		/// </summary>
		public SteamMatchMakingService( SteamService service ) {
			_lobbyMatchList = CallResult<LobbyMatchList_t>.Create( OnLobbyListFound );
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
		FindLobby
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		public ValueTask<LobbyInfo> FindLobby() {
			HServerListRequest hRequest = SteamMatchmakingServers.RequestInternetServerList(  );
			SteamMatchmaking.RequestLobbyList()

			LobbyInfo info = new LobbyInfo();

			return new ValueTask<LobbyInfo>( info );
		}

		/*
		===============
		OnLobbyListFound
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="param"></param>
		/// <param name="bIOFailure"></param>
		private void OnLobbyListFound( LobbyMatchList_t param, bool bIOFailure ) {
			_lobbies.EnsureCapacity( (int)param.m_nLobbiesMatching );
			for ( int i = 0; i < param.m_nLobbiesMatching; i++ ) {
				CSteamID lobbyId = SteamMatchmaking.GetLobbyByIndex( i );
			}
		}
	};
};
