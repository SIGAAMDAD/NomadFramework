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

namespace Nomad.ResourceCache.Events
{
    /// <summary>
    /// Event data regarding a resource that was just evicted from a data cache.
    /// </summary>
    public readonly struct ResourceUnloadedEventArgs<TId>
    {
        /// <summary>
        /// The resource's internal cache id.
        /// </summary>
        public readonly TId Id;

        /// <summary>
        /// 
        /// </summary>
        public readonly long FreedMemory;

        /// <summary>
        /// Cause of death.
        /// </summary>
        public readonly UnloadReason Reason;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="freedMemory"></param>
        /// <param name="reason"></param>
        public ResourceUnloadedEventArgs(TId id, long freedMemory, UnloadReason reason)
        {
            Id = id;
            FreedMemory = freedMemory;
            Reason = reason;
        }
    }
}
