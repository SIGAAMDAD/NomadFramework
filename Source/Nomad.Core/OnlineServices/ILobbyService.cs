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

namespace Nomad.Core.OnlineServices
{
    /// <summary>
    /// Interface for lobby services.
    /// </summary>
    public interface ILobbyService : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        bool IsInLobby { get; }

        /// <summary>
        /// 
        /// </summary>
        bool IsLobbyLeader { get; }

        /// <summary>
        /// 
        /// </summary>
        LobbyInfo? Current { get; }

        /// <summary>
        /// Creates a new lobby with the provided parameters
        /// </summary>
        /// <param name="lobbyInfo">The information to create the lobby with.</param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<Guid> CreateLobby(LobbyInfo lobbyInfo, CancellationToken ct = default);

        /// <summary>
        ///
        /// </summary>
        /// <param name="lobbyId">The lobby's unique 64-bit id.</param>
        /// <param name="ct"></param>
        /// <returns>True if the lobby was joined successfully, false otherwise.</returns>
        Task<bool> JoinLobby(Guid lobbyId, CancellationToken ct = default);

        /// <summary>
        /// Leaves the current lobby.
        /// </summary>
        /// <param name="ct"></param>
        /// <returns><c>True</c> if the lobby was left successfully, <c>false</c> otherwise.</returns>
        Task<bool> LeaveLobby(CancellationToken ct = default);

        /// <summary>
        /// Promotes a member to lobby leader.
        /// </summary>
        /// <param name="player">The player to promote.</param>
        /// <param name="ct"></param>
        /// <returns><c>True</c> if the player was promoted successfully, <c>false</c> otherwise.</returns>
        Task<bool> PromoteMember(Guid player, CancellationToken ct = default);
    }
}
