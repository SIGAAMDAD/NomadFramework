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

namespace Nomad.ResourceCache
{
    /// <summary>
    /// 
    /// </summary>
    public readonly struct CacheStatistics
    {
        public readonly int CacheHits;
        public readonly int CacheMisses;
        public readonly int TotalLoaded;
        public readonly int TotalEvictions;
        public readonly int MemoryUsage;
        public readonly int ActiveReferences;
        public readonly TimeSpan AverageLoadTime;

        public CacheStatistics(int cacheHits, int cacheMisses, int totalLoaded, int totalEvictions, int memoryUsage, int activeReferences, TimeSpan averageLoadTime)
        {
            CacheHits = cacheHits;
            CacheMisses = cacheMisses;
            TotalLoaded = totalLoaded;
            TotalEvictions = totalEvictions;
            MemoryUsage = memoryUsage;
            ActiveReferences = activeReferences;
            AverageLoadTime = averageLoadTime;
        }
    }
}
