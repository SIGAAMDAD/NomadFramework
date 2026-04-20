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
using System.Threading;
using System.Threading.Tasks;
using Nomad.Core.Events;
using Nomad.Core.OnlineServices;

namespace Nomad.OnlineServices.Steam.Private.Services {
	internal sealed class SteamNetworkSessionService : INetworkSessionService {
		public bool IsSessionActive {
			get {
				throw new NotImplementedException();
			}
		}

		public bool IsHost {
			get {
				throw new NotImplementedException();
			}
		}

		public bool IsClient {
			get {
				throw new NotImplementedException();
			}
		}

		public NetworkSessionInfo? CurrentSession {
			get {
				throw new NotImplementedException();
			}
		}

		public IGameEvent<EmptyEventArgs> SessionChanged {
			get {
				throw new NotImplementedException();
			}
		}

		public IGameEvent<PeerId> PeerConnected {
			get {
				throw new NotImplementedException();
			}
		}

		public IGameEvent<PeerId> PeerDisconnected {
			get {
				throw new NotImplementedException();
			}
		}
		
		private readonly ILobbyService _lobbyService;

		public SteamNetworkSessionService( ILobbyService lobbyService ) {
			_lobbyService = lobbyService ?? throw new ArgumentNullException( nameof( lobbyService ) );
		}

		public async Task BroadcastAsync<TMessage>( TMessage message, CancellationToken ct = default ) {
		}

		public async Task<bool> JoinAsClientAsync( Guid info, CancellationToken ct = default ) {
			return false;
		}

		public async Task SendToHostAsync<TMessage>( TMessage message, CancellationToken ct = default ) {
		}

		public async Task SendToPeerAsync<TMessage>( PeerId peerId, TMessage message, CancellationToken ct = default ) {
		}

		public async Task<bool> StartHostAsync( LobbyInfo info, CancellationToken ct = default ) {
			return false;
		}

		public async Task StopAsync( CancellationToken ct = default ) {
		}
	};
};