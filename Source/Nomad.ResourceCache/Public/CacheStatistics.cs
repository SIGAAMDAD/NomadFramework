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

namespace Nomad.ResourceCache
{
    public readonly struct CacheStatistics
    {
        public int CacheHits { get; init; }
        public int CacheMisses { get; init; }
        public int TotalLoaded { get; init; }
        public int TotalEvictions { get; init; }
        public int MemoryUsage { get; init; }
        public int ActiveReferences { get; init; }
        public TimeSpan AverageLoadTime { get; init; }
    }
}
