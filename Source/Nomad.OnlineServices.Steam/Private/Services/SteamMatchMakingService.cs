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
using System.Threading;
using System.Threading.Tasks;
using Nomad.Core.Events;
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
		private readonly CallResult<LobbyCreated_t> _lobbyCreated;
		private readonly CallResult<LobbyEnter_t> _lobbyEnter;

		private bool _isDisposed = false;

		public bool IsSearching {
			get {
				throw new NotImplementedException();
			}
		}

		public MatchMakingInfo? CurrentRequest {
			get {
				throw new NotImplementedException();
			}
		}

		public IGameEvent<EmptyEventArgs> SearchResultsUpdated {
			get {
				throw new NotImplementedException();
			}
		}

		public IGameEvent<Guid> MatchFound {
			get {
				throw new NotImplementedException();
			}
		}

		public IGameEvent<EmptyEventArgs> MatchMakingFailed {
			get {
				throw new NotImplementedException();
			}
		}

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
			_lobbyCreated = CallResult<LobbyCreated_t>.Create( OnLobbyCreated );
			_lobbyEnter = CallResult<LobbyEnter_t>.Create( OnLobbyEnter );
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
			if ( !_isDisposed ) {
				_lobbyMatchList?.Dispose();
				_lobbyCreated?.Dispose();
				_lobbyEnter?.Dispose();
			}
			GC.SuppressFinalize( this );
			_isDisposed = true;
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
			for ( int i = 0; i < param.m_nLobbiesMatching; i++ ) {
				CSteamID lobbyId = SteamMatchmaking.GetLobbyByIndex( i );
			}
		}

		/*
		===============
		OnLobbyEnter
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="param"></param>
		/// <param name="bIOFailure"></param>
		private void OnLobbyEnter( LobbyEnter_t param, bool bIOFailure ) {
		}

		/*
		===============
		OnLobbyCreated
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="param"></param>
		/// <param name="bIOFailure"></param>
		private void OnLobbyCreated( LobbyCreated_t param, bool bIOFailure ) {
		}

		public async Task<IReadOnlyList<LobbyInfo>> SearchLobbies( MatchMakingInfo info, CancellationToken ct = default ) {
			return null;
		}

		public async Task<LobbyInfo?> FindBestLobby( MatchMakingInfo info, CancellationToken ct = default ) {
			return null;
		}

		public async Task<bool> StartQuickPlay( MatchMakingInfo info, CancellationToken ct = default ) {
			return false;
		}

		public async Task Cancel( CancellationToken ct = default ) {
		}
	};
};
