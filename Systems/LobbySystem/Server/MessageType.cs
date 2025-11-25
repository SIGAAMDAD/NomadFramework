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
	/// The type of message we are sending through a steam socket connection.s
	/// </summary>
	public enum MessageType : byte {
		/// <summary>
		/// A P2P handshake, sent to establish a connection between two clients.
		/// </summary>
		Handshake,

		/// <summary>
		/// A command sent to all clients with absolute authority to execute a specific action on all client machines.
		/// </summary>
		ServerCommand,

		/// <summary>
		/// Data sent to the host of the lobby to be processed from a client.
		/// </summary>
		ClientData,

		/// <summary>
		/// Data sent from the host of the lobby to a specific client to synchronize state.
		/// </summary>
		ServerSync,

		/// <summary>
		/// Data sent from the host of the lobby to a specific client to synchronize a non-player object's state.
		/// </summary>
		GameStateSync,

		ChatMessage_TeamOnly,
		ChatMessage_PlayerOnly,
		ChatMessage_FriendsOnly,

		/// <summary>
		/// A highly optimized and compressed voice packet sent directly across clients.
		/// </summary>
		VoiceChat,

		EncryptionKey,

		Count
	};
};