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

using Nomad.Core.Events;
using Nomad.Core.Util;

namespace Nomad.Core.OnlineServices
{
    /// <summary>
    /// 
    /// </summary>
    [Event(
        name: nameof(AchievementProgressChangedEventArgs),
        nameSpace: "Nomad.Core.OnlineServices"
    )]
    public readonly partial struct AchievementProgressChangedEventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public string AchievementId => _achievementId!;
        private readonly InternString _achievementId;

        /// <summary>
        /// 
        /// </summary>
        public float Progress => _progress;
        private readonly float _progress;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="achievementId"></param>
        /// <param name="progress"></param>
        public AchievementProgressChangedEventArgs(InternString achievementId, float progress)
        {
            _achievementId = achievementId;
            _progress = progress;
        }
    }
}
