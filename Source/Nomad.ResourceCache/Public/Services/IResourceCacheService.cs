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
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Nomad.Core.Events;
using Nomad.Core.Abstractions;
using Nomad.ResourceCache.Events;

namespace Nomad.ResourceCache
{
    /// <summary>
    /// A specialized repository for game/engine assets.
    /// </summary>
    public interface IResourceCacheService<TResource, TId> :
        IReadRepository<ICacheEntry<TResource, TId>, TId>,
        IAsyncReadRepository<ICacheEntry<TResource, TId>, TId>,
        IDisposable
        where TResource : notnull, IDisposable
        where TId : IEquatable<TId>
    {
        long CurrentCacheSize { get; }

        [Event("Nomad.ResourceCache")]
        [EventPayload("Id", "TId", Order = 1)]
        [EventPayload("LoadTime", typeof(TimeSpan), Order = 2)]
        [EventPayload("MemorySize", typeof(long), Order = 3)]
        IGameEvent<ResourceLoadedEventArgs<TId>> ResourceLoaded { get; }

        [Event("Nomad.ResourceCache")]
        [EventPayload("Id", "TId", Order = 1)]
        [EventPayload("FreedMemory", typeof(long), Order = 2)]
        [EventPayload("Reason", typeof(UnloadReason), Order = 3)]
        IGameEvent<ResourceUnloadedEventArgs<TId>> ResourceUnloaded { get; }

        [Event("Nomad.ResourceCache")]
        [EventPayload("Id", "TId", Order = 1)]
        [EventPayload("Error", typeof(string), Order = 2)]
        [EventPayload("Exception", typeof(Exception), Order = 3)]
        IGameEvent<ResourceLoadFailedEventArgs<TId>> ResourceLoadFailed { get; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ICacheEntry<TResource, TId> GetCached(TId id);

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <param name="progress"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        ValueTask<ICacheEntry<TResource, TId>> GetCachedAsync(TId id, IProgress<ResourceLoadProgressEventArgs<TId>>? progress = null, CancellationToken ct = default);

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <param name="resource"></param>
        /// <returns></returns>
        bool TryGetCached(TId id, out ICacheEntry<TResource, TId>? resource);

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        void Preload(TId id);

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        ValueTask PreloadAsync(TId id, CancellationToken ct = default);

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        void Unload(TId id);

        /// <summary>
        ///
        /// </summary>
        void UnloadAll();

        /// <summary>
        ///
        /// </summary>
        void ClearUnused();

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        void AddReference(TId id);

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        int GetReferenceCount(TId id);

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        void ReleaseReference(TId id);

        /// <summary>
        ///
        /// </summary>
        /// <param name="ids"></param>
        void Preload(IEnumerable<TId> ids);

        /// <summary>
        ///
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task PreloadAsync(IEnumerable<TId> ids, CancellationToken ct = default);
    }
}
