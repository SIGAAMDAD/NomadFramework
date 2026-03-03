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
        /// <summary>
        /// 
        /// </summary>
        public static partial class CVars
        {
            /// <summary>
            /// 
            /// </summary>
            internal const string NAMESPACE = "Nomad.OnlineServices.Steam";

            /// <summary>
            /// 
            /// </summary>
            internal const string LOBBY_UDDATE_INTERVAL = NAMESPACE + ".LobbyUpdateInterval";

            /// <summary>
            /// 
            /// </summary>
			internal const string LOBBY_METADATA_FETCH_INTERVAL = NAMESPACE + ".LobbyMetadataFetchInterval";

            /// <summary>
            /// 
            /// </summary>
			internal const string LOBBY_MAX_CLIENTS = NAMESPACE + ".LobbyMaxClients";

            /// <summary>
            /// 
            /// </summary>
			internal const string LOBBY_PURGE_INTERVAL = NAMESPACE + ".LobbyPurgeInterval";
        }

        /// <summary>
        /// 
        /// </summary>
        public static partial class Events
        {
            /// <summary>
            /// 
            /// </summary>
            public const string NAMESPACE = "Nomad.OnlineServices";

            /// <summary>
            /// 
            /// </summary>
            public const string ACHIEVEMENT_UNLOCKED = NAMESPACE + ":AchievementUnlocked";

            /// <summary>
            /// 
            /// </summary>
            public const string ACHIEVEMENT_PROGRESS_CHANGED = NAMESPACE + ":AchievementProgressChanged";

            /// <summary>
            /// 
            /// </summary>
            public const string USER_JOINED_LOBBY = NAMESPACE + ":UserJoinedLobby";

            /// <summary>
            /// 
            /// </summary>
			public const string USER_LEFT_LOBBY = NAMESPACE + ":UserLeftLobby";

            /// <summary>
            /// 
            /// </summary>
			public const string USER_DISCONNECTED = NAMESPACE + ":UserDisconnected";

            /// <summary>
            /// 
            /// </summary>
			public const string USER_KICKED = NAMESPACE + ":UserKicked";

            /// <summary>
            /// 
            /// </summary>
			public const string USER_BANNED = NAMESPACE + ":UserBanned";

            /// <summary>
            /// 
            /// </summary>
			public const string LOBBY_INSTANCE_CREATED = NAMESPACE + ":LobbyInstanceCreated";

            /// <summary>
            /// 
            /// </summary>
            public const string CLIENT_CONNECTED = NAMESPACE + ":ClientConnected";

            /// <summary>
            /// 
            /// </summary>
            public const string CLIENT_DISCONNECTED = NAMESPACE + ":ClientDisconnected";
        }
    }
}
