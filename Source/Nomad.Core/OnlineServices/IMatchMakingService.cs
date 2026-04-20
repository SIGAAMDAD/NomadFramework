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
using System.Threading;
using System.Collections.Generic;
using Nomad.Core.Events;

namespace Nomad.Core.OnlineServices
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMatchMakingService : IDisposable
    {
        bool IsSearching { get; }
        MatchMakingInfo? CurrentRequest { get; }

        IGameEvent<EmptyEventArgs> SearchResultsUpdated { get; }

        IGameEvent<Guid> MatchFound { get; }

        IGameEvent<EmptyEventArgs> MatchMakingFailed { get; }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        Task<IReadOnlyList<LobbyInfo>> SearchLobbies(MatchMakingInfo info, CancellationToken ct = default);

        Task<LobbyInfo?> FindBestLobby(MatchMakingInfo info, CancellationToken ct = default);

        Task<bool> StartQuickPlay(MatchMakingInfo info, CancellationToken ct = default);

        Task Cancel(CancellationToken ct = default);
    }
}
