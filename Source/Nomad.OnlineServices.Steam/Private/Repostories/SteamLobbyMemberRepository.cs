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
using Steamworks;

namespace Nomad.OnlineServices.Steam.Private.Repositories {
	/*
	===================================================================================
	
	SteamLobbyMemberRepository
	
	===================================================================================
	*/
	/// <summary>
	/// Handles management of lobby members in the player's currently joined lobby.
	/// </summary>

	internal sealed class SteamLobbyMemberRepository : IDisposable {
		private readonly HashSet<CSteamID> _members = new HashSet<CSteamID>();

		private readonly Callback<LobbyChatUpdate_t> _lobbyChatUpdate;

		private readonly IGameEvent<ulong> _userDisconnected;
		private readonly IGameEvent<ulong> _userKicked;
		private readonly IGameEvent<ulong> _userJoinedLobby;
		private readonly IGameEvent<ulong> _userLeftLobby;

		private bool _isDisposed = false;

		/*
		===============
		SteamLobbyMemberRepository
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		public SteamLobbyMemberRepository( IGameEventRegistryService eventFactory ) {

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
	};
};
