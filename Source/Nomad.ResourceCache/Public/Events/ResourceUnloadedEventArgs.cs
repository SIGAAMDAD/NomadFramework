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

namespace Nomad.ResourceCache.Events
{
    /// <summary>
    /// Event data regarding a resource that was just evicted from a data cache.
    /// </summary>
    public readonly record struct ResourceUnloadedEventArgs<TId>(
        TId Id,
        long FreedMemory,
        UnloadReason Reason
    );
}
