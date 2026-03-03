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
using Nomad.Core.CVars;
using Nomad.Core.Events;
using Nomad.Core.Logger;
using Nomad.Core.OnlineServices;
using Nomad.OnlineServices.Steam.Private.Entities;
using Nomad.OnlineServices.Steam.Private.Repositories;
using Nomad.OnlineServices.Steam.Private.Services.LobbyServices;
using Nomad.OnlineServices.Steam.Private.ValueObjects;
using Steamworks;

namespace Nomad.OnlineServices.Steam.Private.Services {
	/*
	===================================================================================

	SteamLobbyService

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	internal sealed class SteamLobbyService : ILobbyService {
		private readonly SteamLobbyLocator _locator;
		private readonly SteamLobbyRepository _repository;
		private readonly SteamLobbyFactory _factory;

		private readonly object _operationsLock = new object();
		private SteamLobbyInstance? _current = null;

		private readonly Callback<LobbyInvite_t> _lobbyInvite;
		private readonly Callback<LobbyChatMsg_t> _lobbyChatMsg;

		private readonly ICVarSystemService _cvarSystem;
		private readonly IGameEventRegistryService _eventFactory;

		private bool _isDisposed = false;

		/*
		===============
		SteamLobbyService
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="userData"></param>
		/// <param name="appData"></param>
		/// <param name="logger"></param>
		/// <param name="cvarSystem"></param>
		/// <param name="eventFactory"></param>
		public SteamLobbyService( SteamUserData userData, SteamAppData appData, ILoggerService logger, ICVarSystemService cvarSystem, IGameEventRegistryService eventFactory ) {
			_cvarSystem = cvarSystem;
			_eventFactory = eventFactory;

			_lobbyInvite = Callback<LobbyInvite_t>.Create( OnLobbyInvite );
			_lobbyChatMsg = Callback<LobbyChatMsg_t>.Create( OnLobbyChatMsg );

			_repository = new SteamLobbyRepository( cvarSystem );
			_locator = new SteamLobbyLocator( _repository, appData, cvarSystem );
			_factory = new SteamLobbyFactory( userData, logger, cvarSystem );
		}

		/*
		===============
		OnLobbyChatMsg
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="pCallback"></param>
		private void OnLobbyChatMsg( LobbyChatMsg_t pCallback ) {
		}

		/*
		===============
		OnLobbyInvite
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="pCallback"></param>
		private void OnLobbyInvite( LobbyInvite_t pCallback ) {
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
				_current?.Dispose();

				_factory?.Dispose();
				_locator?.Dispose();
				_repository?.Dispose();

				_lobbyInvite?.Dispose();
				_lobbyChatMsg?.Dispose();
			}
			GC.SuppressFinalize( this );
			_isDisposed = true;
		}

		/*
		===============
		CreateLobby
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="lobbyInfo"></param>
		/// <param name="ct"></param>
		/// <returns></returns>
		public async ValueTask<Guid> CreateLobby( LobbyInfo lobbyInfo, CancellationToken ct = default ) {
			var lobby = await _factory.CreateLobby( lobbyInfo, ct );
			if ( lobby == null ) {
				return Guid.Empty;
			}

			lock ( _operationsLock ) {
				_repository.AddLobby( lobby );
				_current = new SteamLobbyInstance( lobby, _cvarSystem, _eventFactory );
			}
			return lobby.Guid;
		}

		public async ValueTask<bool> JoinLobby( Guid lobbyId, CancellationToken ct = default ) {
			return false;
		}

		public async ValueTask<bool> LeaveLobby( CancellationToken ct = default ) {
			throw new System.NotImplementedException();
		}

		public async ValueTask<bool> PromoteMember( Guid player, CancellationToken ct = default ) {
			throw new System.NotImplementedException();
		}
	}
}
