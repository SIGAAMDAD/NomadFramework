/*
===========================================================================
The Nomad Framewrk
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
using Nomad.Core.CVars;
using Nomad.Core.Exceptions;
using Nomad.Core.OnlineServices;
using Nomad.OnlineServices.Steam.Private.Repositories;
using Nomad.OnlineServices.Steam.Private.ValueObjects;
using Steamworks;

namespace Nomad.OnlineServices.Steam.Private.Services.LobbyServices {
	/*
	===================================================================================
	
	SteamLobbyLocator
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>

	internal sealed class SteamLobbyLocator : IDisposable {
		private readonly SteamLobbyRepository _repository;
		private readonly SteamAppData _appData;

		private DateTime _lastFetchTime = DateTime.UtcNow;
		private readonly int _lobbyUpdateInterval = 0;
		private readonly SteamAsyncCallbackDispatcher<LobbyMatchList_t, ICollection<SteamLobbyData>> _lobbyMatchList;

		private ServerRange _lastRange;

		private bool _isDisposed = false;

		/*
		===============
		SteamLobbyLocator
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="repository"></param>
		/// <param name="appData"></param>
		/// <param name="cvarSystem"></param>
		/// <exception cref="CVarMissing"></exception>
		public SteamLobbyLocator( SteamLobbyRepository repository, SteamAppData appData, ICVarSystemService cvarSystem ) {
			_repository = repository;
			_lobbyMatchList = new SteamAsyncCallbackDispatcher<LobbyMatchList_t, ICollection<SteamLobbyData>>();

			_appData = appData;
			_lastRange = ServerRange.Count;

			var lobbyUpdateInterval = cvarSystem.GetCVar<int>( Constants.CVars.LOBBY_UDDATE_INTERVAL ) ?? throw new CVarMissing( Constants.CVars.LOBBY_UDDATE_INTERVAL );
			_lobbyUpdateInterval = lobbyUpdateInterval.Value;
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
			}
			GC.SuppressFinalize( this );
			_isDisposed = true;
		}

		/*
		===============
		FindLobbyWithParams
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="ct"></param>
		public async Task<ICollection<SteamLobbyData>> FindLobbiesWithParams( MatchMakingInfo info, CancellationToken ct = default ) {
			// TODO: implement map & gamemode filtering

			ct.ThrowIfCancellationRequested();

			// fetch the lobby list if we haven't updated for a while, or if we just don't have anything
			bool needRefresh = (DateTime.UtcNow - _lastFetchTime).TotalMilliseconds > _lobbyUpdateInterval
							|| _lastRange != info.Range
							|| _repository.Lobbies.Count == 0;
			if ( needRefresh ) {
				await RequestLobbyListAsync( info.Range, ct );
			}

			return _repository.Lobbies;
		}

		/*
		===============
		RequestLobbyList
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="range"></param>
		/// <param name="ct"></param>
		/// <returns></returns>
		public async Task<ICollection<SteamLobbyData>> RequestLobbyListAsync( ServerRange range, CancellationToken ct = default ) {
			return await _lobbyMatchList.Invoke(
				callback: result => {
					for ( int i = 0; i < result.m_nLobbiesMatching; i++ ) {
						CSteamID lobbyId = SteamMatchmaking.GetLobbyByIndex( i );
						_repository.AddLobby( new SteamLobbyKey( lobbyId, Guid.NewGuid() ) );
					}
					// remove lobbies that haven't been seen recently
					_repository.RemoveStaleLobbies();
					_lastRange = range;
					_lastFetchTime = DateTime.UtcNow;
					return _repository.Lobbies;
				},
				steamCallback: () => {
					ELobbyDistanceFilter distanceFilter = range switch {
						ServerRange.LAN => ELobbyDistanceFilter.k_ELobbyDistanceFilterClose,
						ServerRange.Region => ELobbyDistanceFilter.k_ELobbyDistanceFilterDefault,
						ServerRange.Continental => ELobbyDistanceFilter.k_ELobbyDistanceFilterFar,
						ServerRange.NoLimit => ELobbyDistanceFilter.k_ELobbyDistanceFilterWorldwide,
						_ => throw new ArgumentOutOfRangeException( nameof( range ) )
					};
					SteamMatchmaking.AddRequestLobbyListDistanceFilter( distanceFilter );
					SteamMatchmaking.RequestLobbyList();
				},
				ct
			);
		}
	};
};
