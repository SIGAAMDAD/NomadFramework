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

namespace Nomad.Save.Data
{
    /// <summary>
    /// 
    /// </summary>
    public static class EventNames
    {
        /// <summary>
        /// 
        /// </summary>
        public const string NAMESPACE = nameof(Nomad.Save);

        /// <summary>
        /// 
        /// </summary>
        public const string SAVE_BEGIN_EVENT = NAMESPACE + ":SaveBegin";

        /// <summary>
        /// 
        /// </summary>
        public const string LOAD_BEGIN_EVENT = NAMESPACE + ":LoadBegin";
    }
}
