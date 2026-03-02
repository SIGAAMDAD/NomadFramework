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
using Nomad.Core.Events;
using Nomad.Core.OnlineServices;
using Nomad.OnlineServices.Steam.Private.ValueObjects;
using Steamworks;

namespace Nomad.OnlineServices.Steam.Private.Services.LobbyServices {
	/*
	===================================================================================
	
	LobbyLocator
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>

	public sealed class LobbyLocator : IDisposable {
		private readonly Callback<LobbyMatchList_t> _lobbyMatchList;

		private readonly List<LobbyInfo> _lobbies = new List<LobbyInfo>();

		private DateTime _lastFetchTime;
		private volatile bool _listRetrieved = true;

		private bool _isDisposed = false;

		/*
		===============
		LobbyLocator
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		public LobbyLocator() {
			_lobbyMatchList = Callback<LobbyMatchList_t>.Create( OnLobbyMatchList );
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
				if ( !_listRetrieved ) {
					// kill the getter thread if we have any.
					_listRetrieved = true;
				}
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
		public async ValueTask FindLobbyWithParams( MatchMakingInfo info, CancellationToken ct = default ) {
			ELobbyDistanceFilter distanceFilter = info.Range switch {
				ServerRange.LAN => ELobbyDistanceFilter.k_ELobbyDistanceFilterClose,
				ServerRange.Region => ELobbyDistanceFilter.k_ELobbyDistanceFilterDefault,
				ServerRange.Continental => ELobbyDistanceFilter.k_ELobbyDistanceFilterFar,
				ServerRange.NoLimit => ELobbyDistanceFilter.k_ELobbyDistanceFilterWorldwide,
				_ => throw new ArgumentOutOfRangeException( nameof( info ) )
			};
			SteamMatchmaking.AddRequestLobbyListDistanceFilter( distanceFilter );

			if ( info.Map != null ) {
				SteamMatchmaking.AddRequestLobbyListStringFilter( nameof( LobbyInfo.Map ), info.Map, ELobbyComparison.k_ELobbyComparisonEqual );
			}
			if ( info.GameMode != null ) {
				SteamMatchmaking.AddRequestLobbyListStringFilter( nameof( LobbyInfo.GameMode ), info.GameMode, ELobbyComparison.k_ELobbyComparisonEqual );
			}
			if ( info.Metadata != null ) {
				int index = 0;
				foreach ( var metadata in info.Metadata ) {
					SteamMatchmaking.AddRequestLobbyListStringFilter( $"MetadataKey{index}", $"MetadataValue{index}", ELobbyComparison.k_ELobbyComparisonEqual );
					index++;
				}
			}

			await RequestLobbyList( ct );
		}

		/*
		===============
		RequestLobbyList
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="ct"></param>
		/// <returns></returns>
		public async ValueTask<List<LobbyInfo>?> RequestLobbyList( CancellationToken ct = default ) {
			if ( !_listRetrieved ) {
				return null;
			}

			_listRetrieved = false;
			_lastFetchTime = DateTime.UtcNow;
			SteamAPICall_t hResult = SteamMatchmaking.RequestLobbyList();

			while ( !_listRetrieved ) {
				ct.ThrowIfCancellationRequested();
				await Task.Delay( 100, ct );
			}

			return _lobbies;
		}

		/*
		===============
		OnLobbyMatchList
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="pCallback"></param>
		private void OnLobbyMatchList( LobbyMatchList_t pCallback ) {
			if ( pCallback.m_nLobbiesMatching == 0 ) {
				return;
			}

			for ( int i = 0; i < pCallback.m_nLobbiesMatching; i++ ) {
				CSteamID lobbyId = SteamMatchmaking.GetLobbyByIndex( i );

				_lobbies.Add( SteamLobbyData.GetInfo( lobbyId ) );
			}

			_listRetrieved = true;
		}
	};
};
