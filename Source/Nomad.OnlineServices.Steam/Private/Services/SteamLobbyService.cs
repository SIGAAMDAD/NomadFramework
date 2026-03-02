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
using System.Threading.Tasks;
using Nomad.Core.OnlineServices;
using Nomad.OnlineServices.Steam.Private.Services.LobbyServices;
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

	public sealed class SteamLobbyService : ILobbyService {
		private readonly LobbyLocator _locator;

		private readonly Callback<LobbyInvite_t> _lobbyInvite;
		private readonly Callback<LobbyChatUpdate_t> _lobbyChatUpdate;
		private readonly Callback<LobbyChatMsg_t> _lobbyChatMsg;

		private bool _isDisposed = false;

		/*
		===============
		SteamLobbyService
		===============
		*/
		/// <summary>
		///
		/// </summary>
		public SteamLobbyService() {
			_lobbyInvite = Callback<LobbyInvite_t>.Create( OnLobbyInvite );
			_lobbyChatUpdate = Callback<LobbyChatUpdate_t>.Create( OnLobbyChatUpdate );
			_lobbyChatMsg = Callback<LobbyChatMsg_t>.Create( OnLobbyChatMsg );
		}

		private void OnLobbyChatUpdate( LobbyChatUpdate_t pCallback ) {
			switch ( (EChatMemberStateChange)pCallback.m_rgfChatMemberStateChange ) {
				case EChatMemberStateChange.k_EChatMemberStateChangeBanned:
				case EChatMemberStateChange.k_EChatMemberStateChangeDisconnected:
				case EChatMemberStateChange.k_EChatMemberStateChangeEntered:
					break;
				case EChatMemberStateChange.k_EChatMemberStateChangeKicked:
					break;
				case EChatMemberStateChange.k_EChatMemberStateChangeLeft:
					break;
			}
		}

		private void OnLobbyChatMsg( LobbyChatMsg_t pCallback ) {
		}

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
				_locator?.Dispose();

				_lobbyInvite?.Dispose();
				_lobbyChatMsg?.Dispose();
				_lobbyChatUpdate?.Dispose();
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
		/// <returns></returns>
		public async ValueTask<bool> CreateLobby( LobbyInfo lobbyInfo ) {
			return false;
		}

		/*
		===============
		DeleteLobby
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public async ValueTask<bool> DeleteLobby() {
			throw new NotImplementedException();
		}

		public async ValueTask<bool> JoinLobby( string lobbyName ) {
			throw new System.NotImplementedException();
		}

		public async ValueTask<bool> LeaveLobby() {
			throw new System.NotImplementedException();
		}

		public async ValueTask<bool> PromoteMember( PlayerId player ) {
			throw new System.NotImplementedException();
		}
	}
}
