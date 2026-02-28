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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Nomad.Core.Compatibility.Guards;
using Nomad.Core.Events;
using Nomad.Core.Logger;
#if DEBUG
using System.Diagnostics;
#endif

namespace Nomad.Events.Private.SubscriptionSets {
	/*
	===================================================================================

	SubscriptionSet

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	internal sealed class SubscriptionSet<TArgs> : ISubscriptionSet<TArgs>
		where TArgs : struct {
#if DEBUG
		public int SubscriberCount => _subscriberCount;
		private int _subscriberCount = 0;

		public long PublishCount => _publishCount;
		private long _publishCount = 0;

		public long MaxSyncDurationTicks => _maxSyncDurationTicks;
		private long _maxSyncDurationTicks = 0;

		public long TotalSyncDurationTicks => _totalSyncDurationTicks;
		private long _totalSyncDurationTicks = 0;
#endif
		private readonly ILoggerService _logger;
		private readonly IGameEvent<TArgs> _eventData;

		private readonly SubscriptionCache<TArgs, EventCallback<TArgs>> _genericSubscriptions;
		private readonly SubscriptionCache<TArgs, AsyncEventCallback<TArgs>> _asyncSubscriptions;

		private bool _isDisposed = false;

		private readonly HashSet<WeakReference<IGameEvent>> _friends = new HashSet<WeakReference<IGameEvent>>();
		private readonly ReaderWriterLockSlim _pumpLock = new ReaderWriterLockSlim();

		/*
		===============
		SubscriptionSet
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="eventData"></param>
		/// <param name="logger"></param>
		public SubscriptionSet( IGameEvent<TArgs> eventData, ILoggerService logger ) {
			_genericSubscriptions = new SubscriptionCache<TArgs, EventCallback<TArgs>>( logger );
			_asyncSubscriptions = new SubscriptionCache<TArgs, AsyncEventCallback<TArgs>>( logger );
			_logger = logger;
			_eventData = eventData;
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
			if ( !_isDisposed ) {
				_logger?.PrintLine( $"Releasing subscription set for event {_eventData.DebugName}..." );
				
				_genericSubscriptions?.Dispose();
				_asyncSubscriptions?.Dispose();
				_friends.Clear();
				_pumpLock?.Dispose();
			}
			GC.SuppressFinalize( this );
			_isDisposed = true;
		}

		/*
		===============
		BindEventFriend
		===============
		*/
		/// <summary>
		/// Binds the provided event to the owned event in a "friendship".
		/// </summary>
		/// <param name="friend"></param>
		public void BindEventFriend( IGameEvent friend ) {
			StateGuard.ThrowIfDisposed( _isDisposed, this );

			var refEvent = new WeakReference<IGameEvent>( friend );
			if ( _friends.Contains( refEvent ) ) {
				_logger?.PrintWarning( $"SubscriptionSet.BindEventFriend: event '{_eventData.DebugName}' already has friendship with event '{friend.DebugName}'" );
				return;
			}

			_logger?.PrintLine( $"SubscriptionSet.BindEventFriend: friendship created between events '{_eventData.DebugName}' and '{friend.DebugName}'" );
			_friends.Add( refEvent );
		}

		/*
		===============
		AddSubscription
		===============
		*/
		/// <summary>
		/// Adds a callback method to the subscriptions list.
		/// </summary>
		/// <param name="callback">The method that is called whenever the event triggers.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="callback"/> is null.</exception>
		public bool AddSubscription( EventCallback<TArgs> callback ) {
			StateGuard.ThrowIfDisposed( _isDisposed, this );
			ArgumentGuard.ThrowIfNull( callback );

			_pumpLock.EnterWriteLock();
			try {
				_genericSubscriptions.Add( callback );
#if DEBUG
				Interlocked.Increment( ref _subscriberCount );
#endif
				return true;
			} finally {
				_pumpLock.ExitWriteLock();
			}
		}

		/*
		===============
		AddSubscriptionAsync
		===============
		*/
		/// <summary>
		/// Adds a callback method to the subscriptions list.
		/// </summary>
		/// <param name="callback">The method that is called whenever the event triggers.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="callback"/> is null.</exception>
		public bool AddSubscriptionAsync( AsyncEventCallback<TArgs> callback ) {
			StateGuard.ThrowIfDisposed( _isDisposed, this );
			ArgumentGuard.ThrowIfNull( callback );

			if ( ContainsCallbackAsync( callback, out _ ) ) {
				_logger?.PrintWarning( $"SubscriptionSet.AddSubscriptionAsync: subscription for callback '{callback.Method.Name}' already exists!" );
				return false;
			}

			_pumpLock.EnterWriteLock();
			try {
				_asyncSubscriptions.Add( callback );
#if DEBUG
				Interlocked.Increment( ref _subscriberCount );
#endif
				return true;
			} finally {
				_pumpLock.ExitWriteLock();
			}
		}

		/*
		===============
		RemoveSubscription
		===============
		*/
		/// <summary>
		/// Removes the provided <paramref name="callback"/> from the event's subscription list.
		/// </summary>
		/// <param name="callback">The callback to remove from the subscription list.</param>
		public bool RemoveSubscription( EventCallback<TArgs> callback ) {
			StateGuard.ThrowIfDisposed( _isDisposed, this );
			ArgumentGuard.ThrowIfNull( callback );

			if ( !ContainsCallback( callback, out int index ) ) {
				_logger?.PrintError( $"SubscriptionSet.RemoveSubscription: no such existing subscription for callback '{callback.Method.Name}'" );
				return false;
			}

			_pumpLock.EnterWriteLock();
			try {
				_genericSubscriptions.RemoveAt( index );
#if DEBUG
				Interlocked.Decrement( ref _subscriberCount );
#endif
				return true;
			} finally {
				_pumpLock.ExitWriteLock();
			}
		}

		/*
		===============
		RemoveSubscription
		===============
		*/
		/// <summary>
		/// Removes the provided <paramref name="callback"/> from the event's subscription list.
		/// </summary>
		/// <param name="callback">The callback to remove from the subscription list.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="callback"/> is null.</exception>
		public bool RemoveSubscriptionAsync( AsyncEventCallback<TArgs> callback ) {
			StateGuard.ThrowIfDisposed( _isDisposed, this );
			ArgumentGuard.ThrowIfNull( callback );

			if ( !ContainsCallbackAsync( callback, out int index ) ) {
				_logger?.PrintError( $"SubscriptionSet.RemoveSubscriptionAsync: no such existing subscription for callback '{callback.Method.Name}'" );
				return false;
			}

			_pumpLock.EnterWriteLock();
			try {
				_asyncSubscriptions.RemoveAt( index );
#if DEBUG
				Interlocked.Increment( ref _subscriberCount );
#endif
				return true;
			} finally {
				_pumpLock.ExitWriteLock();
			}
		}

		/*
		===============
		PumpWithLock
		===============
		*/
		/// <summary>
		/// "Publishes" an event to all subscribers with thread-safety.
		/// </summary>
		/// <param name="args"></param>
		public void Pump( in TArgs args ) {
			StateGuard.ThrowIfDisposed( _isDisposed, this );
			
#if EVENT_DEBUG
			_logger?.PrintLine( $"SubscriptionSet.Pump: publishing event {eventData.DebugName}" );
#endif
#if DEBUG
			var sw = Stopwatch.StartNew();
#endif
			_pumpLock.EnterUpgradeableReadLock();
			try {
				for ( int i = 0; i < _genericSubscriptions.Count; i++ ) {
					try {
						_genericSubscriptions[ i ].Invoke( in args );
					} catch {
#if DEBUG
#endif
					}
				}
			} finally {
				_pumpLock.ExitUpgradeableReadLock();
			}
#if DEBUG
			sw.Stop();
			Interlocked.Increment( ref _publishCount );
			Interlocked.Add( ref _totalSyncDurationTicks, sw.ElapsedTicks );

			long ticks = sw.ElapsedTicks;
			long original;
			do {
				original = _maxSyncDurationTicks;
				if ( ticks <= original ) {
					break;
				}
			} while ( Interlocked.CompareExchange( ref _maxSyncDurationTicks, ticks, original ) != original );
#endif
		}

		/*
		===============
		PumpAsync
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="args"></param>
		/// <param name="ct"></param>
		/// <returns></returns>
		public async Task PumpAsync( TArgs args, CancellationToken ct ) {
			StateGuard.ThrowIfDisposed( _isDisposed, this );

#if EVENT_DEBUG
			_logger?.PrintLine( $"SubscriptionSet.PumpAsync: publishing event {eventData.DebugName} asynchronously..." );
#endif
			int subscriptionCount = _asyncSubscriptions.Count;

			ct.ThrowIfCancellationRequested();

			// TODO: optimize
			Task[] tasks = new Task[ subscriptionCount ];
			for ( int i = 0; i < subscriptionCount; i++ ) {
				ct.ThrowIfCancellationRequested();
				tasks[i] = _asyncSubscriptions[ i ].Invoke( args, ct );
			}

			ct.ThrowIfCancellationRequested();

			await Task.WhenAll( tasks ).ConfigureAwait( false );
		}

		/*
		===============
		ContainsCallback
		===============
		*/
		/// <summary>
		/// Public method that acquires the read lock and checks for callback existence.
		/// </summary>
		/// <param name="callback"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		public bool ContainsCallback( EventCallback<TArgs> callback, out int index ) {
			StateGuard.ThrowIfDisposed( _isDisposed, this );
			
			_pumpLock.EnterReadLock();
			try {
				for ( int i = 0; i < _genericSubscriptions.Count; i++ ) {
					if ( _genericSubscriptions[ i ] == callback ) {
						index = i;
						return true;
					}
				}
				index = -1;
				return false;
			} finally {
				_pumpLock.ExitReadLock();
			}
		}

		/*
		===============
		ContainsCallbackAsync
		===============
		*/
		/// <summary>
		/// Public method that acquires the read lock and checks for callback existence.
		/// </summary>
		/// <param name="callback"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		public bool ContainsCallbackAsync( AsyncEventCallback<TArgs> callback, out int index ) {
			StateGuard.ThrowIfDisposed( _isDisposed, this );

			_pumpLock.EnterUpgradeableReadLock();
			try {
				for ( int i = 0; i < _asyncSubscriptions.Count; i++ ) {
					if ( _asyncSubscriptions[ i ] == callback ) {
						index = i;
						return true;
					}
				}
				index = -1;
				return false;
			} finally {
				_pumpLock.ExitUpgradeableReadLock();
			}
		}
	};
};
