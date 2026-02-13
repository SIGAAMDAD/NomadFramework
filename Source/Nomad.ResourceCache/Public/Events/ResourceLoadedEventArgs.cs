/*
===========================================================================
The Nomad Framework
Copyright (C) 2025 Noah Van Til

This Source Code Form is subject to the terms of the Mozilla Public
License, v2. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.

This software is provided "as is", without warranty of any kind,
express or implied, including but not limited to the warranties
of merchantability, fitness for a particular purpose and noninfringement.
===========================================================================
*/

using System;

namespace Nomad.ResourceCache.Events
{
    /// <summary>
    ///
    /// </summary>
    public readonly struct ResourceLoadedEventArgs<TId>
    {
        /// <summary>
        /// The resource's internal cache id.
        /// </summary>
        public readonly TId Id;

        /// <summary>
        /// 
        /// </summary>
        public readonly TimeSpan LoadTime;

        /// <summary>
        /// 
        /// </summary>
        public readonly long MemorySize;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="loadTime"></param>
        /// <param name="memorySize"></param>
        public ResourceLoadedEventArgs(TId id, TimeSpan loadTime, long memorySize)
        {
            Id = id;
            LoadTime = loadTime;
            MemorySize = memorySize;
        }
    }
}
