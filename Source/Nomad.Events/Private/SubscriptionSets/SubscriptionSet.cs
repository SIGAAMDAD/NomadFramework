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
		where TArgs : struct
	{
		/// <summary>
		/// The number of pumps before initiating a purge.
		/// </summary>
		private readonly int _cleanupInterval;
		private readonly ILoggerService _logger;
		private readonly IGameEvent<TArgs> _eventData;

		private readonly SubscriptionCache<TArgs> _genericSubscriptions;
		private readonly SubscriptionCache<TArgs> _asyncSubscriptions;
		private int _cleanupCounter = 0;

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
		/// <param name="cleanupInterval"></param>
		public SubscriptionSet( IGameEvent<TArgs> eventData, ILoggerService logger, int cleanupInterval = 30 ) {
			_genericSubscriptions = new SubscriptionCache<TArgs>( logger );
			_asyncSubscriptions = new SubscriptionCache<TArgs>( logger );
			_logger = logger;
			_cleanupInterval = cleanupInterval;
			_eventData = eventData;
		}

		/*
		===============
		Dispose
		===============
		*/
		/// <summary>
		/// Clears all the subscriptions within this set.
		/// </summary>
		public void Dispose() {
#if EVENT_DEBUG
			_logger.PrintLine( $"Releasing subscription set for event {_eventData.DebugName}..." );
#endif
			_genericSubscriptions.Dispose();
			_asyncSubscriptions.Dispose();
			_friends.Clear();
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
			var refEvent = new WeakReference<IGameEvent>( friend );
			if ( _friends.Contains( refEvent ) ) {
				_logger?.PrintWarning( $"SubscriptionSet.BindEventFriend: event '{_eventData.DebugName}' already has friendship with event '{friend.DebugName}'" );
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
		/// Adds a callback method to the <see cref="Subscriptions"/> list.
		/// </summary>
		/// <param name="subscriber"></param>
		/// <param name="callback">The method that is called whenever the event triggers.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="callback"/> is null.</exception>
		public void AddSubscription( object subscriber, EventCallback<TArgs> callback ) {
			ArgumentGuard.Null( subscriber );
			ArgumentGuard.Null( callback );

			if ( ContainsCallback( subscriber, callback, out _ ) ) {
				_logger.PrintWarning( $"SubscriptionSet.AddSubscription: subscription for owner '{subscriber.GetType().FullName}' and callback '{callback.Method.Name}' already exists!" );
				return;
			}

			_pumpLock.EnterWriteLock();
			try {
				_genericSubscriptions.Add( SubscriptionEntry<TArgs>.Create( subscriber, callback ) );
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
		/// Adds a callback method to the <see cref="Subscriptions"/> list.
		/// </summary>
		/// <param name="subscriber"></param>
		/// <param name="callback">The method that is called whenever the event triggers.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="callback"/> is null.</exception>
		public void AddSubscriptionAsync( object subscriber, AsyncEventCallback<TArgs> callback ) {
			ArgumentGuard.Null( subscriber );
			ArgumentGuard.Null( callback );

			if ( ContainsCallbackAsync( subscriber, callback, out _ ) ) {
				_logger.PrintWarning( $"SubscriptionSet.AddSubscriptionAsync: subscription for owner '{subscriber.GetType().FullName}' and callback '{callback.Method.Name}' already exists!" );
				return;
			}

			_pumpLock.EnterWriteLock();
			try {
				_asyncSubscriptions.Add( SubscriptionEntry<TArgs>.CreateAsync( subscriber, callback ) );
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
		/// <param name="subscriber"></param>
		/// <param name="callback">The callback to remove from the subscription list.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="callback"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the returned index from <see cref="ContainsCallback"/> is invalid.</exception>
		public void RemoveSubscription( object subscriber, EventCallback<TArgs> callback ) {
			ArgumentGuard.Null( subscriber );
			ArgumentGuard.Null( callback );

			if ( !ContainsCallback( subscriber, callback, out int index ) ) {
				_logger.PrintError( $"SubscriptionSet.RemoveSubscription: no such existing subscription for owner '{subscriber.GetType().FullName}' and callback '{callback.Method.Name}'" );
				return;
			}

			_pumpLock.EnterWriteLock();
			try {
				_genericSubscriptions.RemoveAt( index );
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
		/// <param name="subscriber"></param>
		/// <param name="callback">The callback to remove from the subscription list.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="callback"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the returned index from <see cref="ContainsCallback"/> is invalid.</exception>
		public void RemoveSubscriptionAsync( object subscriber, AsyncEventCallback<TArgs> callback ) {
			ArgumentGuard.Null( subscriber );
			ArgumentGuard.Null( callback );

			if ( !ContainsCallbackAsync( subscriber, callback, out int index ) ) {
				_logger.PrintError( $"SubscriptionSet.RemoveSubscriptionAsync: no such existing subscription for owner '{subscriber.GetType().FullName}' and callback '{callback.Method.Name}'" );
				return;
			}

			_pumpLock.EnterWriteLock();
			try {
				_asyncSubscriptions.RemoveAt( index );
			} finally {
				_pumpLock.ExitWriteLock();
			}
		}

		/*
		===============
		RemoveAllForSubscriber
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="subscriber"></param>
		public void RemoveAllForSubscriber( object subscriber ) {
			ArgumentGuard.Null( subscriber );

			_genericSubscriptions.RemoveAllOwnedBy( subscriber );
			_asyncSubscriptions.RemoveAllOwnedBy( subscriber );
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
#if EVENT_DEBUG
			_logger?.PrintLine( $"SubscriptionSet.Pump: publishing event {eventData.DebugName}" );
#endif
			_pumpLock.EnterUpgradeableReadLock();
			try {
				_genericSubscriptions.CleanupIncremental( 24 );
				for ( int i = 0; i < _genericSubscriptions.Count; i++ ) {
					var subscription = _genericSubscriptions[ i ];
					if ( !subscription.IsAlive ) {
						continue;
					}
					if ( subscription.TryGetCallback( out var callback ) ) {
						callback?.Invoke( in args );
					}
				}
			} finally {
				_pumpLock.ExitUpgradeableReadLock();
			}
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
#if EVENT_DEBUG
			_logger?.PrintLine( $"SubscriptionSet.PumpAsync: publishing event {eventData.DebugName} asynchronously..." );
#endif
			int subscriptionCount = _asyncSubscriptions.Count;

			// TODO: optimize
			List<Task> tasks = new List<Task>( subscriptionCount );

			for ( int i = 0; i < subscriptionCount; i++ ) {
				var subscription = _asyncSubscriptions[ i ];
				if ( subscription.IsAlive ) {
					continue;
				}
				if ( subscription.TryGetAsyncCallback( out var callback ) ) {
					tasks.Add( callback.Invoke( args, ct ) );
				}
			}

#if USE_COMPATIBILITY_EXTENSIONS
			Task.WaitAll( tasks.ToArray(), ct );
#else
			Task.WaitAll( tasks, ct );
#endif
		}

		/*
		===============
		ContainsCallback
		===============
		*/
		/// <summary>
		/// Public method that acquires the read lock and checks for callback existence.
		/// </summary>s
		/// <param name="subscriber"></param>
		/// <param name="callback"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		public bool ContainsCallback( object subscriber, EventCallback<TArgs> callback, out int index ) {
			_pumpLock.EnterReadLock();
			try {
				for ( int i = 0; i < _genericSubscriptions.Count; i++ ) {
					if ( _genericSubscriptions[ i ].Matches( subscriber, callback ) ) {
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
		/// </summary>s
		/// <param name="subscriber"></param>
		/// <param name="callback"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		public bool ContainsCallbackAsync( object subscriber, AsyncEventCallback<TArgs> callback, out int index ) {
			_pumpLock.EnterUpgradeableReadLock();
			try {
				for ( int i = 0; i < _asyncSubscriptions.Count; i++ ) {
					if ( _asyncSubscriptions[ i ].MatchesAsync( subscriber, callback ) ) {
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

		/*
		===============
		CleanupSubscriptions
		===============
		*/
		/// <summary>
		///
		/// </summary>
		public void CleanupSubscriptions() {
			_pumpLock.EnterWriteLock();
			try {
				_genericSubscriptions.CleanupFull();
				_asyncSubscriptions.CleanupFull();
			} finally {
				_pumpLock.ExitWriteLock();
			}
		}
	};
};
