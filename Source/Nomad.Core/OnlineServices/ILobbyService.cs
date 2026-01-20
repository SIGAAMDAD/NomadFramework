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

namespace Nomad.Core.OnlineServices
{
    /// <summary>
    /// Interface for lobby services.
    /// </summary>
    public interface ILobbyService : IDisposable
    {
        /// <summary>
        /// Creates a new lobby with the provided parameters
        /// </summary>
        /// <param name="lobbyInfo">The information to create the lobby with.</param>
        /// <returns></returns>
        ValueTask<bool> CreateLobby(LobbyInfo lobbyInfo);

        /// <summary>
        ///
        /// </summary>
        /// <param name="lobbyName">The name of the lobby to join.</param>
        /// <returns>True if the lobby was joined successfully, false otherwise.</returns>
        ValueTask<bool> JoinLobby(string lobbyName);

        /// <summary>
        /// Leaves the current lobby.
        /// </summary>
        /// <returns>True if the lobby was left successfully, false otherwise.</returns>
        ValueTask<bool> LeaveLobby();

        /// <summary>
        /// Promotes a member to lobby leader.
        /// </summary>
        /// <param name="player">The player to promote.</param>
        /// <returns>True if the player was promoted successfully, false otherwise.</returns>
        ValueTask<bool> PromoteMember(PlayerId player);
    }
}
