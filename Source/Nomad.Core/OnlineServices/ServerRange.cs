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

namespace Nomad.Core.OnlineServices
{
    /// <summary>
    /// 
    /// </summary>
    public enum ServerRange : byte
    {
        /// <summary>
        /// Only search for servers created on the same network as the user is currently connected to.
        /// </summary>
        LAN,

        /// <summary>
        /// Only search for servers created by users marked as friends.
        /// </summary>
        Friends,

        /// <summary>
        /// The default server range, search for any and all servers available.
        /// </summary>
        Internet,

        /// <summary>
        /// 
        /// </summary>
        Count
    }
}
