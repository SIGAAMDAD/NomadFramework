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
using System.Collections.Concurrent;
using Nomad.Core.Events;
using Nomad.OnlineServices.Steam.Private.ValueObjects;
using Steamworks;

namespace Nomad.OnlineServices.Steam.Private.Services.LobbyServices {
	internal sealed class SteamConnectionService : IDisposable {
		private readonly Callback<SteamNetConnectionStatusChangedCallback_t> _netConnectionStatusChanged;

		private readonly ConcurrentDictionary<CSteamID, SteamNetConnection> _connectionCache = new ConcurrentDictionary<CSteamID, SteamNetConnection>();

		private bool _isDisposed = false;

		private readonly ISubscriptionHandle _userJoined;
		private readonly ISubscriptionHandle _userLeft;

		public IGameEvent<ulong> ClientConnected => _clientConnected;
		private readonly IGameEvent<ulong> _clientConnected;

		public IGameEvent<ulong> ClientDisconnected => _clientDisconnected;
		private readonly IGameEvent<ulong> _clientDisconnected;

		/*
		===============
		SteamConnectionService
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="eventFactory"></param>
		public SteamConnectionService( IGameEventRegistryService eventFactory ) {
			_netConnectionStatusChanged = Callback<SteamNetConnectionStatusChangedCallback_t>.Create( OnNetConnectionStatusChanged );

			_clientConnected = eventFactory.GetEvent<ulong>( Constants.Events.CLIENT_CONNECTED, Constants.Events.NAMESPACE );
			_clientDisconnected = eventFactory.GetEvent<ulong>( Constants.Events.CLIENT_DISCONNECTED, Constants.Events.NAMESPACE );

			var userJoined = eventFactory.GetEvent<ulong>( Constants.Events.USER_JOINED_LOBBY, Constants.Events.NAMESPACE );
			_userJoined = userJoined.Subscribe( OnUserJoined );

			var userLeft = eventFactory.GetEvent<ulong>( Constants.Events.USER_LEFT_LOBBY, Constants.Events.NAMESPACE );
			_userLeft = userLeft.Subscribe( OnUserLeft );
		}

		private void OnUserJoined( in ulong userId ) {
		}

		private void OnUserLeft( in ulong userId ) {
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
				_netConnectionStatusChanged?.Dispose();

				_clientConnected?.Dispose();
				_clientDisconnected?.Dispose();

				_userJoined?.Dispose();
				_userLeft?.Dispose();
			}
			GC.SuppressFinalize( this );
			_isDisposed = true;
		}

		/*
		===============
		OnIncomingConnection
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="senderId"></param>
		private void OnIncomingConnection( CSteamID senderId ) {
			if ( _connectionCache.TryGetValue( senderId, out var connection ) && connection.Status != SteamConnectionStatus.Opened ) {

			} else {

			}
		}

		/*
		===============
		OnNetConnectionStatusChanged
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="pCallback"></param>
		private void OnNetConnectionStatusChanged( SteamNetConnectionStatusChangedCallback_t pCallback ) {
			CSteamID senderId = pCallback.m_info.m_identityRemote.GetSteamID();

			var connection = _connectionCache.GetOrAdd( senderId, f => new SteamNetConnection( pCallback.m_hConn ) );
			switch ( pCallback.m_info.m_eState ) {
				case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connected:
					connection.SetStatus( SteamConnectionStatus.Connected );
					break;
				case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connecting:
					connection.SetStatus( SteamConnectionStatus.Opened );
					OnIncomingConnection( senderId );
					break;
				case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Dead:
				case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_ProblemDetectedLocally:
				case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_ClosedByPeer:
					connection.SetStatus( SteamConnectionStatus.Closed );
					_clientDisconnected.Publish( (ulong)senderId );
					break;
			}
		}
	};
};
