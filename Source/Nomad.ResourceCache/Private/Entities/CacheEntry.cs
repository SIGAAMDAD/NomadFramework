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
using System.Threading.Tasks;
using Nomad.Core.Abstractions;

namespace Nomad.ResourceCache.Private.Entities {
	/*
	===================================================================================

	CacheEntry

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	internal sealed class CacheEntry<TResource, TId>( IResourceCacheService<TResource, TId> owner, TId id, TResource cached, int memorySize, TimeSpan loadTime, ResourceLoadState loadState ) : ICacheEntry<TResource, TId>
		where TResource : notnull, IDisposable
		where TId : IEquatable<TId>
	{
		public TId Id => id;
		public DateTime CreatedAt => _createdAt;
		private readonly DateTime _createdAt = DateTime.UtcNow;

		public DateTime? ModifiedAt => _accessStats.LastAccessTime;
		public int Version => _accessStats.AccessCount;

		public EntryAccessStatistics AccessStats {
			get {
				lock ( _statsLock ) {
					return _accessStats;
				}
			}
		}
		private EntryAccessStatistics _accessStats;

		public int ReferenceCount = 1;
		public TimeSpan LoadTimer = loadTime;
		public ResourceLoadState LoadState { get; } = loadState;

		public readonly int MemorySize = memorySize;

		private readonly object _statsLock = new object();

		/*
		===============
		Get
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="resource"></param>
		public void Get( out TResource resource ) {
			UpdateAccessStats();
			resource = cached;
		}

		/*
		===============
		GetAsync
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		public async ValueTask<TResource> GetAsync() {
			UpdateAccessStats();
			return cached;
		}

		/*
		===============
		Dispose
		===============
		*/
		/// <summary>
		///
		/// </summary>
		public void Dispose() {
			cached?.Dispose();
			ReferenceCount = 0;
		}

		/*
		===============
		Equals
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Equals( IEntity<TId>? other ) {
			return other is not null && other.Id.Equals( Id );
		}

		/*
		===============
		UpdateAccessStats
		===============
		*/
		/// <summary>
		///
		/// </summary>
		public void UpdateAccessStats() {
			lock ( _statsLock ) {
				_accessStats = _accessStats with {
					LastAccessTime = DateTime.UtcNow,
					AccessCount = AccessStats.AccessCount + 1
				};
			}
		}
	};
};
