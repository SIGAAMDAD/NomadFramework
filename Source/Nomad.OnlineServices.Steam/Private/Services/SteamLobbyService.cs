/*
===========================================================================
The Nomad MPL Source Code
Copyright (C) 2025-2026 Noah Van Til

This Source Code Form is subject to the terms of the Mozilla Public
License, v2. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.

This software is provided "as is", without warranty of any kind,
express or implied, including but not limited to the warranties
of merchantability, fitness for a particular purpose and noninfringement.
===========================================================================
*/

using System.Threading.Tasks;
using Nomad.Core.OnlineServices;
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
		/*
		===============
		SteamLobbyService
		===============
		*/
		/// <summary>
		///
		/// </summary>
		public SteamLobbyService() {
		}

		public void Dispose() {
		}

		public async ValueTask<bool> CreateLobby( LobbyInfo lobbyInfo ) {
			throw new System.NotImplementedException();
		}

		public async ValueTask<bool> DeleteLobby() {
			throw new System.NotImplementedException();
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
