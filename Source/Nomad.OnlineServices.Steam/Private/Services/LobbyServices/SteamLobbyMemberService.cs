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
using Nomad.Core.Events;
using Nomad.Core.OnlineServices;
using Steamworks;

namespace Nomad.OnlineServices.Steam.Private.Services.LobbyServices {
	/*
	===================================================================================
	
	SteamLobbyMemberService
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>

	internal sealed class SteamLobbyMemberService : IDisposable {
		public HashSet<ulong> Members => _members;
		private readonly HashSet<ulong> _members;

		private readonly Callback<LobbyChatUpdate_t> _lobbyChatUpdate;

		private readonly IGameEvent<ulong> _userDisconnected;
		private readonly IGameEvent<ulong> _userKicked;
		private readonly IGameEvent<ulong> _userJoinedLobby;
		private readonly IGameEvent<ulong> _userLeftLobby;

		private bool _isDisposed = false;

		/*
		===============
		SteamLobbyMemberService
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="eventFactory"></param>
		public SteamLobbyMemberService( IGameEventRegistryService eventFactory ) {
			_lobbyChatUpdate = Callback<LobbyChatUpdate_t>.Create( OnLobbyChatUpdate );

			_userDisconnected = eventFactory.GetEvent<ulong>( Constants.Events.USER_DISCONNECTED, Constants.Events.NAMESPACE );
			_userKicked = eventFactory.GetEvent<ulong>( Constants.Events.USER_KICKED, Constants.Events.NAMESPACE );
			_userJoinedLobby = eventFactory.GetEvent<ulong>( Constants.Events.USER_JOINED_LOBBY, Constants.Events.NAMESPACE );
			_userLeftLobby = eventFactory.GetEvent<ulong>( Constants.Events.USER_LEFT_LOBBY, Constants.Events.NAMESPACE );
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
				_userDisconnected?.Dispose();
				_userKicked?.Dispose();
				_userJoinedLobby?.Dispose();
				_userLeftLobby?.Dispose();

				_lobbyChatUpdate?.Dispose();
			}
			GC.SuppressFinalize( this );
			_isDisposed = true;
		}

		/*
		===============
		PlayerIsInLobby
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="playerId"></param>
		/// <returns></returns>
		public bool PlayerIsInLobby( ulong playerId ) {
			return _members.Contains( playerId );
		}

		/*
		===============
		RemoveMember
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="reason"></param>
		/// <param name="playerId"></param>
		private void RemoveMember( PlayerRemovalReason reason, ulong playerId ) {
			_members.Remove( playerId );

			switch ( reason ) {
				case PlayerRemovalReason.Left:
					_userLeftLobby.Publish( playerId );
					break;
				case PlayerRemovalReason.Disconnected:
					_userDisconnected.Publish( playerId );
					break;
				case PlayerRemovalReason.Kicked:
					_userKicked.Publish( playerId );
					break;
			}
		}

		/*
		===============
		OnLobbyChatUpdate
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="pCallback"></param>
		private void OnLobbyChatUpdate( LobbyChatUpdate_t pCallback ) {
			switch ( (EChatMemberStateChange)pCallback.m_rgfChatMemberStateChange ) {
				case EChatMemberStateChange.k_EChatMemberStateChangeEntered:
					_members.Add( pCallback.m_ulSteamIDUserChanged );
					_userJoinedLobby.Publish( pCallback.m_ulSteamIDMakingChange );
					break;
				case EChatMemberStateChange.k_EChatMemberStateChangeDisconnected:
					RemoveMember( PlayerRemovalReason.Disconnected, pCallback.m_ulSteamIDUserChanged );
					break;
				case EChatMemberStateChange.k_EChatMemberStateChangeBanned: // special condition here?
				case EChatMemberStateChange.k_EChatMemberStateChangeKicked:
					RemoveMember( PlayerRemovalReason.Kicked, pCallback.m_ulSteamIDUserChanged );
					break;
				case EChatMemberStateChange.k_EChatMemberStateChangeLeft:
					RemoveMember( PlayerRemovalReason.Left, pCallback.m_ulSteamIDUserChanged );
					break;
			}
		}
	};
};