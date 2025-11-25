/*
===========================================================================
The Nomad AGPL Source Code
Copyright (C) 2025 Noah Van Til

The Nomad Source Code is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published
by the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

The Nomad Source Code is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with The Nomad Source Code.  If not, see <http://www.gnu.org/licenses/>.

If you have questions concerning this license or the applicable additional
terms, you may contact me via email at nyvantil@gmail.com.
===========================================================================
*/

namespace NomadCore.Systems.LobbySystem.Server {
	/// <summary>
	/// A server command's indexer.
	/// </summary>
	public enum CommandType : uint {
		/// <summary>
		/// Sent only by the host when we're starting the game, triggers a map load.
		/// </summary>
		StartGame,

		/// <summary>
		/// Sent only by the host when we're ending a game, triggers the scoreboard to show
		/// and blocks all client movement.
		/// </summary>
		EndGame,

		StartCountdown,

		ConnectedToLobby,

		PlayerReady,

		/// <summary>
		/// Sent by a client to begin a vote to start the current game without the host's permission.
		/// </summary>
		VoteStart,

		/// <summary>
		/// Sent by a client to end a vote. The timer runs on the client hosting the polls.
		/// </summary>
		VoteEnd,

		/// <summary>
		/// Sent by a client to being a vote to kick a different client.
		/// </summary>
		StartVoteKick,

		/// <summary>
		/// Sent by a client who votes "yes" to kicking another client targeted by the votekick system.
		/// </summary>
		VoteKickResponse_Yes,

		/// <summary>
		/// Sent by a client who votes "no" to kicking another client targeted by the votekick system.
		/// </summary>
		VoteKickResponse_No,

		/// <summary>
		/// Sent by the host of the lobby to a targeted client who is being votekicked.
		/// </summary>
		KickPlayer,

		/// <summary>
		/// Sent by the host to all of the clients in the case of the lobby's ownership changing.
		/// </summary>
		OwnershipChanged,

		Count
	};
};