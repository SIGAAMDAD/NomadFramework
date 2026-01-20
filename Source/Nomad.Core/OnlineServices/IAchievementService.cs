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

using System.Threading.Tasks;

namespace Nomad.Core.OnlineServices
{
    /// <summary>
    /// Interface for achievement services.
    /// </summary>
    public interface IAchievementService
    {
        /// <summary>
        /// Indicates whether the service supports achievements.
        /// </summary>
        bool SupportsAchievements { get; }

        /// <summary>
        /// Unlocks the specified achievement.
        /// </summary>
        /// <param name="achievementId">The ID of the achievement to unlock.</param>
        /// <returns></returns>
        ValueTask UnlockAchievement(string achievementId);

        /// <summary>
        /// Locks the specified achievement.
        /// </summary>
        /// <param name="achievementId">The ID of the achievement to lock.</param>
        /// <returns></returns>
        ValueTask LockAchievement(string achievementId);

        /// <summary>
        /// Sets the progress for the specified achievement.
        /// </summary>
        /// <param name="achievementId">The ID of the achievement to set progress for.</param>
        /// <param name="current">The current progress value.</param>
        /// <returns></returns>
        ValueTask SetAchievementProgress(string achievementId, float current);
    }
}
