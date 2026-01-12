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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
using Nomad.ResourceCache.Private.Entities;
using Nomad.Core.Events;
using Nomad.Core.Logger;
using Nomad.Core;
using Nomad.Core.Util;
using Nomad.ResourceCache.Events;

namespace Nomad.ResourceCache
{
    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TResource"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public class BaseCache<TResource, TId> : IResourceCacheService<TResource, TId>
        where TResource : IDisposable
        where TId : IEquatable<TId>
    {
        public CacheStatistics Statistics => new CacheStatistics
        {
            CacheHits = _cacheHits,
            CacheMisses = _cacheMisses,
            TotalLoaded = _totalLoaded,
            MemoryUsage = _currentMemorySize,
            ActiveReferences = _cache.Values.Count(entry => entry.ReferenceCount > 0),
            AverageLoadTime = _totalLoaded > 0 ? TimeSpan.FromTicks(_totalLoadTime.Ticks / _totalLoaded) : TimeSpan.Zero
        };

        public CachePolicy Policy
        {
            get => _policy;
            set
            {
                _policy = value ?? throw new ArgumentNullException(nameof(value));
                EvictIfNeeded();
            }
        }
        private CachePolicy _policy = CachePolicy.Default;

        public long CurrentCacheSize => _currentMemorySize;
        public long MaxCacheSize
        {
            get => _policy.MaxMemorySize;
            set
            {
                _policy.MaxMemorySize = value;
                EvictIfNeeded();
            }
        }

        public int Count => _cache.Count;

        private readonly Dictionary<TId, CacheEntry<TResource, TId>> _cache = new();
        private readonly ReaderWriterLockSlim _cacheLock = new ReaderWriterLockSlim();

        private int _currentMemorySize = 0;
        private int _cacheHits = 0;
        private int _cacheMisses = 0;
        private int _totalLoaded = 0;
        private int _totalUnloaded = 0;
        private TimeSpan _totalLoadTime = TimeSpan.Zero;

        private long _isDisposed = 0;

        private readonly Timer _cleanupTimer;
        private readonly Task _cleanupThread;
        private readonly ILoggerService _logger;
        private readonly IResourceLoader<TResource, TId> _loader;

        public IGameEvent<ResourceLoadedEventArgs<TId>> ResourceLoaded => _resourceLoaded;
        private readonly IGameEvent<ResourceLoadedEventArgs<TId>> _resourceLoaded;

        public IGameEvent<ResourceUnloadedEventArgs<TId>> ResourceUnloaded => _resourceUnloaded;
        private readonly IGameEvent<ResourceUnloadedEventArgs<TId>> _resourceUnloaded;

        public IGameEvent<ResourceLoadFailedEventArgs<TId>> ResourceLoadFailed => _resourceLoadFailed;
        private readonly IGameEvent<ResourceLoadFailedEventArgs<TId>> _resourceLoadFailed;

        /// <summary>
        ///
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="eventFactory"></param>
        /// <param name="loader"></param>
        public BaseCache(ILoggerService logger, IGameEventRegistryService eventFactory, IResourceLoader<TResource, TId> loader)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(eventFactory);
            ArgumentNullException.ThrowIfNull(loader);

            _logger = logger;
            _loader = loader;

            _resourceLoaded = eventFactory.GetEvent<ResourceLoadedEventArgs<TId>>(Constants.Events.ResourceCache.NAMESPACE, Constants.Events.ResourceCache.RESOURCE_LOADED_EVENT);
            _resourceUnloaded = eventFactory.GetEvent<ResourceUnloadedEventArgs<TId>>(Constants.Events.ResourceCache.NAMESPACE, Constants.Events.ResourceCache.RESOURCE_UNLOADED_EVENT);
            _resourceLoadFailed = eventFactory.GetEvent<ResourceLoadFailedEventArgs<TId>>(Constants.Events.ResourceCache.NAMESPACE, Constants.Events.ResourceCache.RESOURCE_LOAD_FAILED_EVENT);

            //_cleanupTimer = new Timer( _ => ClearUnused(), null, TimeSpan.FromMinutes( 1 ), TimeSpan.FromMinutes( 10 ) );

            _cleanupThread = Task.Run(LaunchCleanupThread);
        }

        /*
		===============
		Dispose
		===============
		*/
        /// <summary>
        ///
        /// </summary>
        public virtual void Dispose()
        {
            _cleanupTimer?.Dispose();

            Interlocked.Increment(ref _isDisposed);
            _cleanupThread.Wait();

            UnloadAll();

            _resourceLoaded?.Dispose();
            _resourceUnloaded?.Dispose();
            _resourceLoadFailed?.Dispose();

            _cacheLock?.Dispose();

            GC.SuppressFinalize(this);
        }

        /*
		===============
		ClearUnushed
		===============
		*/
        /// <summary>
        ///
        /// </summary>
        public virtual void ClearUnused()
        {
#if DEBUG
            _logger.PrintDebug($"BaseCache{typeof(TResource)}.ClearUnused: removing unused entries...");
#endif
            var toRemove = new List<TId>();
            foreach (KeyValuePair<TId, CacheEntry<TResource, TId>> kvp in _cache)
            {
                if (kvp.Value.ReferenceCount == 0)
                {
                    toRemove.Add(kvp.Key);
                }
            }

            int removedCount = 0;
            _cacheLock.EnterWriteLock();
            try
            {
                for (int i = 0; i < toRemove.Count; i++)
                {
                    TId id = toRemove[i];
                    if (_cache.Remove(id, out CacheEntry<TResource, TId>? entry) && entry.ReferenceCount <= 0)
                    {
                        removedCount++;
                        _currentMemorySize -= entry.MemorySize;
                        ResourceUnloaded.Publish(new ResourceUnloadedEventArgs<TId>(id, entry.MemorySize, UnloadReason.ReferenceCountZero));
                        entry.Dispose();
                    }
                    else
                    {
                        _logger.PrintError($"BaseCache{typeof(TResource)}.ClearUnused: Dictionary.Remove failed!");
                    }
                }
            }
            finally
            {
                _cacheLock.ExitWriteLock();
            }

            Interlocked.Add(ref _totalUnloaded, removedCount);
        }

        /*
		===============
		GetCached
		===============
		*/
        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public virtual ICacheEntry<TResource, TId> GetCached(TId id)
        {
            _cacheLock.EnterUpgradeableReadLock();
            try
            {
                if (_cache.TryGetValue(id, out CacheEntry<TResource, TId>? entry))
                {
                    Interlocked.Increment(ref _cacheHits);
                    entry.UpdateAccessStats();
                    return entry;
                }
            }
            finally
            {
                _cacheLock.ExitUpgradeableReadLock();
            }

            Interlocked.Increment(ref _cacheMisses);
            ICacheEntry<TResource, TId>? cacheEntry = LoadAndCacheResource(id);
            if (cacheEntry == null)
            {
                _logger.PrintError($"BaseCache{typeof(TResource)}.GetCached: failed to load resource '{id}'");
            }
            return cacheEntry;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual bool TryGetById(TId id, out ICacheEntry<TResource, TId>? entity)
        {
            return TryGetCached(id, out entity);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual ICacheEntry<TResource, TId>? GetById(TId id)
        {
            return GetCached(id);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public virtual async ValueTask<ICacheEntry<TResource, TId>?> GetByIdAsync(TId id, CancellationToken ct = default)
        {
            return await GetCachedAsync(id, null, ct);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public virtual IEnumerable<ICacheEntry<TResource, TId>> GetByIds(ReadOnlySpan<TId> ids)
        {
            var cached = new ICacheEntry<TResource, TId>[ids.Length];
            for (int i = 0; i < ids.Length; i++)
            {
                cached[i] = GetCached(ids[i]);
            }
            return cached;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <param name="progress"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public virtual async ValueTask<ICacheEntry<TResource, TId>> GetCachedAsync(TId id, IProgress<ResourceLoadProgressEventArgs<TId>>? progress = null, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();

            _cacheLock.EnterUpgradeableReadLock();
            try
            {
                if (_cache.TryGetValue(id, out CacheEntry<TResource, TId>? entry) && entry.LoadState == ResourceLoadState.Complete)
                {
                    Interlocked.Increment(ref _cacheHits);
                    entry.UpdateAccessStats();
                    return entry;
                }
            }
            finally
            {
                _cacheLock.ExitUpgradeableReadLock();
            }

            ICacheEntry<TResource, TId>? cacheEntry = await LoadAndCacheResourceAsyncValue(id, progress, ct);
            if (cacheEntry == null)
            {
                _logger.PrintError($"BaseCache{typeof(TResource)}.GetCachedAsync: failed to load resource '{id}'!");
            }
            return cacheEntry;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        public void AddReference(TId id)
        {
            _cacheLock.EnterWriteLock();
            try
            {
                if (_cache.TryGetValue(id, out CacheEntry<TResource, TId>? entry))
                {
                    entry.ReferenceCount++;
                    _logger.PrintLine($"BaseCache{typeof(TResource)}.AddReference: adding reference to cache entry '{entry.Id}'");
                    entry.UpdateAccessStats();
                }
                else
                {
                    throw new KeyNotFoundException(id.ToString());
                }
            }
            finally
            {
                _cacheLock.ExitWriteLock();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetReferenceCount(TId id)
        {
            _cacheLock.EnterReadLock();
            try
            {
                return _cache.TryGetValue(id, out CacheEntry<TResource, TId>? entry) ? entry.ReferenceCount : 0;
            }
            finally
            {
                _cacheLock.ExitReadLock();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        public virtual void ReleaseReference(TId id)
        {
            bool shouldUnload = false;

            _cacheLock.EnterUpgradeableReadLock();
            try
            {
                if (_cache.TryGetValue(id, out CacheEntry<TResource, TId>? entry))
                {
                    int newCount = Interlocked.Decrement(ref entry.ReferenceCount);
                    if (newCount == 0 && (DateTime.UtcNow - entry.AccessStats.LastAccessTime) > _policy.UnloadUnusedAfter)
                    {
                        shouldUnload = true;
                    }
                }
                else
                {
                    throw new KeyNotFoundException(id.ToString());
                }
            }
            finally
            {
                _cacheLock.ExitUpgradeableReadLock();
            }
            if (shouldUnload)
            {
                Unload(id);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        public virtual void Preload(TId id)
        {
            LoadAndCacheResource(id);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="ids"></param>
        public virtual void Preload(IEnumerable<TId> ids)
        {
            foreach (TId id in ids)
            {
                if (_cache.ContainsKey(id))
                {
                    continue;
                }
                LoadAndCacheResource(id);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public virtual async ValueTask PreloadAsync(TId id, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();

            await LoadAndCacheResourceAsyncValue(id, null, ct);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public virtual async Task PreloadAsync(IEnumerable<TId> ids, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();

            var tasks = new List<Task>();
            foreach (TId id in ids)
            {
                if (_cache.ContainsKey(id))
                {
                    continue;
                }
                tasks.Add(LoadAndCacheResourceAsync(id, null, ct));
            }
            await Task.WhenAll(tasks);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <param name="resource"></param>
        /// <returns></returns>
        public virtual bool TryGetCached(TId id, out ICacheEntry<TResource, TId>? resource)
        {
            resource = null;

            _cacheLock.EnterWriteLock();
            try
            {
                if (_cache.TryGetValue(id, out CacheEntry<TResource, TId>? entry))
                {
                    _cacheHits++;
                    entry.UpdateAccessStats();
                    resource = entry;
                    return true;
                }
            }
            finally
            {
                _cacheLock.ExitWriteLock();
            }

            Interlocked.Increment(ref _cacheMisses);
            return false;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        public virtual void Unload(TId id)
        {
            _cacheLock.EnterWriteLock();
            try
            {
                if (_cache.Remove(id, out CacheEntry<TResource, TId>? entry))
                {
                    _currentMemorySize -= entry.MemorySize;
                    Interlocked.Increment(ref _totalLoaded);
                    ResourceUnloaded.Publish(new ResourceUnloadedEventArgs<TId>(id, entry.MemorySize, UnloadReason.Manual));
                }
                else
                {
                    _logger.PrintError($"BaseCache{typeof(TResource)}.Unload: Dictionary.Remove failed!");
                }
            }
            finally
            {
                _cacheLock.ExitWriteLock();
            }
        }

        /// <summary>
        ///
        /// </summary>
        public virtual void UnloadAll()
        {
            _cacheLock.EnterWriteLock();
            try
            {
                foreach (KeyValuePair<TId, CacheEntry<TResource, TId>> kvp in _cache)
                {
                    ResourceUnloaded.Publish(new ResourceUnloadedEventArgs<TId>(kvp.Key, kvp.Value.MemorySize, UnloadReason.Manual));
                    kvp.Value.Dispose();
                }
            }
            finally
            {
                _cacheLock.ExitWriteLock();
            }
        }

        /// <summary>
        ///
        /// </summary>
        public virtual void EvictIfNeeded()
        {
            if (_policy.EvictionPolicy == EvictionPolicy.Never || (_currentMemorySize <= _policy.MaxMemorySize && _cache.Count <= _policy.MaxResourceCount))
            {
                return;
            }

            _cacheLock.EnterWriteLock();
            try
            {
                var candidates = _cache.Where(kvp => kvp.Value.ReferenceCount == 0).ToList();
                if (candidates.Count == 0)
                {
                    return;
                }

                IEnumerable<KeyValuePair<TId, CacheEntry<TResource, TId>>> sortedCandidates = _policy.EvictionPolicy switch
                {
                    EvictionPolicy.LRU => candidates.OrderBy(kvp => kvp.Value.AccessStats.LastAccessTime),
                    EvictionPolicy.LFU => candidates.OrderBy(kvp => kvp.Value.AccessStats.AccessCount),
                    EvictionPolicy.SizeBased => candidates.OrderByDescending(kvp => kvp.Value.MemorySize),
                    _ => candidates.OrderByDescending(kvp => kvp.Value.AccessStats.LastAccessTime)
                };

                int removedCount = 0;
                foreach (KeyValuePair<TId, CacheEntry<TResource, TId>> candidate in sortedCandidates)
                {
                    if (_currentMemorySize <= _policy.MaxMemorySize && _cache.Count <= _policy.MaxResourceCount)
                    {
                        break;
                    }
                    if (_cache.Remove(candidate.Key, out CacheEntry<TResource, TId>? entry))
                    {
                        removedCount++;
                        _currentMemorySize -= entry.MemorySize;
                        ResourceUnloaded.Publish(new ResourceUnloadedEventArgs<TId>(
                            candidate.Key, entry.MemorySize, UnloadReason.CacheFull
                        ));
                        entry.Dispose();
                    }
                }
                Interlocked.Add(ref _totalUnloaded, removedCount);
            }
            finally
            {
                _cacheLock.ExitWriteLock();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private ICacheEntry<TResource, TId>? LoadAndCacheResource(TId id)
        {
            var loadTimer = Stopwatch.StartNew();
            try
            {
                Result<TResource> result = _loader.Load.Invoke(id);
                if (result.IsFailure)
                {
                    _logger.PrintError($"BaseCache.LoadAndCacheResource: failed to load resource '{id}'");
                    return null;
                }

                int memorySize = CalculateMemorySize(result.Value);
                return CacheResource(id, result.Value, memorySize, loadTimer.Elapsed);
            }
            catch (Exception e)
            {
                _logger.PrintError($"BaseCache{typeof(TResource)}.LoadAndCacheResource: exception thrown while loading resource '{id}' - {e}");
                loadTimer.Stop();
                throw;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <param name="progress"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<ICacheEntry<TResource, TId>?> LoadAndCacheResourceAsync(TId id, IProgress<ResourceLoadProgressEventArgs<TId>>? progress = null, CancellationToken ct = default)
        {
            progress?.Report(new ResourceLoadProgressEventArgs<TId>(id, 0.0f, ResourceLoadState.Queued));

            var loadTimer = Stopwatch.StartNew();
            try
            {
                Result<TResource> result = await _loader.LoadAsync(id, ct);
                ct.ThrowIfCancellationRequested();

                if (result.IsFailure)
                {
                    _logger.PrintError($"Base_cache.LoadAndCacheResourceAsync: failed to load resource '{id}'");
                    return null;
                }
                progress?.Report(new ResourceLoadProgressEventArgs<TId>(id, 0.0f, ResourceLoadState.Processing));

                int memorySize = CalculateMemorySize(result.Value);
                return CacheResource(id, result.Value, memorySize, loadTimer.Elapsed);
            }
            catch (Exception e)
            {
                loadTimer.Stop();

                _cache.Remove(id, out _);
                ResourceLoadFailed.Publish(new ResourceLoadFailedEventArgs<TId>(id, e.Message, e));
                throw;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <param name="progress"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async ValueTask<ICacheEntry<TResource, TId>?> LoadAndCacheResourceAsyncValue(TId id, IProgress<ResourceLoadProgressEventArgs<TId>>? progress = null, CancellationToken ct = default)
        {
            progress?.Report(new ResourceLoadProgressEventArgs<TId>(id, 0.0f, ResourceLoadState.Queued));

            var loadTimer = Stopwatch.StartNew();
            try
            {
                Result<TResource> result = await _loader.LoadAsync(id, ct);
                ct.ThrowIfCancellationRequested();

                if (result.IsFailure)
                {
                    _logger.PrintError($"Base_cache.LoadAndCacheResourceAsync: failed to load resource '{id}'");
                    return null;
                }
                progress?.Report(new ResourceLoadProgressEventArgs<TId>(id, 0.0f, ResourceLoadState.Processing));

                int memorySize = CalculateMemorySize(result.Value);
                return CacheResource(id, result.Value, memorySize, loadTimer.Elapsed);
            }
            catch (Exception e)
            {
                loadTimer.Stop();

                _cache.Remove(id, out _);
                ResourceLoadFailed.Publish(new ResourceLoadFailedEventArgs<TId>(id, e.Message, e));
                throw;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <param name="resource"></param>
        /// <param name="memorySize"></param>
        /// <param name="loadTime"></param>
        private ICacheEntry<TResource, TId> CacheResource(TId id, TResource resource, int memorySize, TimeSpan loadTime)
        {
            _cacheLock.EnterWriteLock();

            var entry = new CacheEntry<TResource, TId>(
                this, id, resource, memorySize, loadTime, ResourceLoadState.Complete
            );
            try
            {
                _cache[id] = entry;
                _currentMemorySize += memorySize;
                _totalLoaded++;
                _totalLoadTime += loadTime;

                ResourceLoaded.Publish(new ResourceLoadedEventArgs<TId>(id, loadTime, memorySize));
            }
            finally
            {
                _cacheLock.ExitWriteLock();
            }

            // flush the cache if needed
            EvictIfNeeded();

            return entry;
        }

        /// <summary>
        /// Calculates the memory size of the provided resource.
        /// </summary>
        /// <param name="resource"></param>
        /// <returns></returns>
        protected virtual int CalculateMemorySize(TResource resource)
        {
            return 0;
        }

        /// <summary>
        /// The "cleanup crew" thread that runs in the backround to evict any unused resources.
        /// </summary>
        /// <returns></returns>
        private async Task LaunchCleanupThread()
        {
            while (Interlocked.Read(ref _isDisposed) == 0)
            {
                await Task.Delay(_policy.UnloadUnusedAfter);
                ClearUnused();
            }
        }
    };
};
