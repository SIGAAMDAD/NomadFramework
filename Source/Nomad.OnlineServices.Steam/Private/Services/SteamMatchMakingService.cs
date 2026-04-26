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
using Nomad.Core.Compatibility.Guards;
using Nomad.Core.CVars;
using Nomad.Core.Events;
using Nomad.Core.OnlineServices;
using Nomad.CVars;
using Nomad.OnlineServices.Steam.Private.Repositories;
using Nomad.OnlineServices.Steam.Private.Services;
using Nomad.OnlineServices.Steam.Private.ValueObjects;
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

		private readonly SteamAsyncCallbackDispatcher<LobbyMatchList_t, ICollection<SteamLobbyData>> _lobbyMatchList;
		private CancellationTokenSource? _cancellationToken = null;

		private readonly SteamLobbyRepository _repository;

		private DateTime _lastFetchTime = DateTime.UtcNow;
		private readonly int _lobbyUpdateInterval = 0;

		private ServerRange _lastRange;

		public bool IsSearching => _activeRequest != null;

		public MatchMakingInfo? CurrentRequest => _activeRequest;
		private MatchMakingInfo? _activeRequest = null;

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

		private bool _isDisposed = false;

		/*
		===============
		SteamMatchMakingService
		=============== 
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="repository"></param>
		/// <param name="cvarSystem"></param>
		/// <exception cref="ArgumentNullException"></exception>
		public SteamMatchMakingService( SteamLobbyRepository repository, ICVarSystemService cvarSystem ) {
			ArgumentGuard.ThrowIfNull( cvarSystem );

			_lobbyMatchList = new SteamAsyncCallbackDispatcher<LobbyMatchList_t, ICollection<SteamLobbyData>>();

			_lastRange = ServerRange.Count;
			_repository = repository ?? throw new ArgumentNullException( nameof( repository ) );

			var lobbyUpdateInterval = cvarSystem.GetCVarOrThrow<int>( Constants.CVars.LOBBY_UDDATE_INTERVAL );
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
		SearchLobbies
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="ct"></param>
		/// <returns></returns>
		public async Task<IReadOnlyList<LobbyInfo>> SearchLobbies( MatchMakingInfo info, CancellationToken ct = default ) {
			_cancellationToken = CancellationTokenSource.CreateLinkedTokenSource( ct );
			ct.ThrowIfCancellationRequested();

			_activeRequest = info;

			// fetch the lobby list if we haven't updated for a while, or if we just don't have anything
			bool needRefresh = (DateTime.UtcNow - _lastFetchTime).TotalMilliseconds > _lobbyUpdateInterval
							|| _lastRange != info.Range
							|| _repository.Lobbies.Count == 0;
			if ( needRefresh ) {
				await RequestLobbyListAsync( info.Range, ct );
			}

			var steamLobbies = _repository.Lobbies;
			var lobbies = new List<LobbyInfo>( steamLobbies.Count );
			foreach ( var lobby in steamLobbies ) {
				lobbies.Add( lobby.Info );
			}
			_activeRequest = null;

			return lobbies;
		}

		/*
		===============
		FindBestLobby
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="ct"></param>
		/// <returns></returns>
		public async Task<LobbyInfo?> FindBestLobby( MatchMakingInfo info, CancellationToken ct = default ) {
			ct.ThrowIfCancellationRequested();

			var lobbies = await SearchLobbies( info, ct );
			
			Span<int> scores = stackalloc int[ lobbies.Count ];
			scores.Clear();

			for ( int i = 0; i < lobbies.Count; i++ ) {
				var lobby = lobbies[ i ];

				foreach ( var gameMode in info.GameModes ) {
					if ( lobby.GameMode.Equals( gameMode, StringComparison.InvariantCulture ) ) {
						scores[ i ] += 5;
						break;
					}
				}
				foreach ( var map in info.Maps ) {
					if ( lobby.Map.Equals( map, StringComparison.InvariantCulture ) ) {
						scores[ i ] += 5;
						break;
					}
 				}
			}

			return null;
		}

		public async Task<bool> StartQuickPlay( MatchMakingInfo info, CancellationToken ct = default ) {

			return false;
		}

		public async Task Cancel( CancellationToken ct = default ) {
			_cancellationToken.Cancel();
			_activeRequest = null;
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
		private async Task<ICollection<SteamLobbyData>> RequestLobbyListAsync( ServerRange range, CancellationToken ct = default ) {
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
