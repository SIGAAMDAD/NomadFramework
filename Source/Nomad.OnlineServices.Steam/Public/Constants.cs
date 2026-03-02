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

namespace Nomad.OnlineServices.Steam
{
    /// <summary>
    /// Steam Integration global constants.
    /// </summary>
    public static partial class Constants
    {
        public static partial class CVars
        {
        }

        public static partial class Events
        {
            /// <summary>
            /// Steam Integration event namespace.
            /// </summary>
            public const string NAMESPACE = nameof(Nomad.OnlineServices.Steam);

            public const string ACHIEVEMENT_UNLOCKED = NAMESPACE + ":AchievementUnlocked";
            public const string ACHIEVEMENT_PROGRESS_CHANGED = NAMESPACE + ":AchievementProgressChanged";
        }
    }
}