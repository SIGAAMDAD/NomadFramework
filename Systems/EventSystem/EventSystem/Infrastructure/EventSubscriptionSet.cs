/*
===========================================================================
The Nomad AGPL Source Code
Copyright (C) 2025 Noah Van Til

The Nomad Source Code is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published
by the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

The Nomad Source Code is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with The Nomad Source Code.  If not, see <http://www.gnu.org/licenses/>.

If you have questions concerning this license or the applicable additional
terms, you may contact me via email at nyvantil@gmail.com.
===========================================================================
*/

using NomadCore.Abstractions.Services;
using NomadCore.Infrastructure;
using NomadCore.Interfaces.EventSystem;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace NomadCore.Systems.EventSystem.Infrastructure {
	/*
	===================================================================================
	
	EventSubscriptionSet
	
	===================================================================================
	*/
	/// <summary>
	/// Manages event subscriptions with automatic cleanup using weak references.
	/// </summary>

	internal sealed class EventSubscriptionSet : IDisposable {
		private readonly struct WeakSubscription {
			public readonly WeakReference<object> Subscriber;
			public readonly WeakReference<IGameEvent.EventCallback> Callback;
			public readonly bool IsAlive => Subscriber.TryGetTarget( out _ ) && Callback.TryGetTarget( out _ );

			public WeakSubscription( object? subscriber, IGameEvent.EventCallback? callback ) {
				ArgumentNullException.ThrowIfNull( subscriber );
				ArgumentNullException.ThrowIfNull( callback );

				Subscriber = new WeakReference<object>( subscriber );
				Callback = new WeakReference<IGameEvent.EventCallback>( callback );
			}
		};

		/// <summary>
		/// The number of pumps before initiating a purge.
		/// </summary>
		private const int CLEANUP_INTERVAL = 30;

		/// <summary>
		/// The event that we're handling.
		/// </summary>
		public readonly IGameEvent Event;

		private readonly ILoggerService? _logger = ServiceRegistry.Get<ILoggerService>();

		private readonly Dictionary<int, List<int>> _subscriptionIndexMap = new Dictionary<int, List<int>>( 24 );
		private readonly List<WeakSubscription> _subscriptions = new List<WeakSubscription>( 24 );
		private int _cleanupCounter = 0;

		private readonly HashSet<WeakReference<IGameEvent>> _friends = new HashSet<WeakReference<IGameEvent>>();

		private readonly ReaderWriterLockSlim _pumpLock = new ReaderWriterLockSlim();

		/*
		===============
		EventSubscriptionSet
		===============
		*/
		/// <summary>
		/// Constructs a new EventSubscriptionSet
		/// </summary>
		/// <param name="eventData">The event to use.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="eventData"/> is null.</exception>
		public EventSubscriptionSet( IGameEvent eventData ) {
			ArgumentNullException.ThrowIfNull( eventData );
			Event = eventData;
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
			_subscriptions.Clear();
			_subscriptionIndexMap.Clear();

			for ( int i = 0; i < _friends.Count; i++ ) {
				
			}
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
				_logger?.PrintWarning( $"EventSubscriptionSet.BindEventFriend: event '{Event.Name}' already has friendship with event '{friend.Name}'" );
			}

			_logger?.PrintLine( $"EventSubscriptionSet.BindEventFriend: friendship created between events '{Event.Name}' and '{friend.Name}'" );
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
		public void AddSubscription( object? subscriber, IGameEvent.EventCallback? callback ) {
			ArgumentNullException.ThrowIfNull( subscriber );
			ArgumentNullException.ThrowIfNull( callback );

			_pumpLock.EnterWriteLock();
			try {
				if ( ContainsCallback( subscriber, callback, out _ ) ) {
					_logger?.PrintError( $"EventSubscriptionSet.CheckForDuplicateSubscription: duplicate subscription from '{subscriber.GetType().Name}'" );
					return;
				}

				int index = _subscriptions.Count;
				_subscriptions.Add( new WeakSubscription( subscriber, callback ) );
				_logger?.PrintLine( $"EventSubscriptionSet.AddSubsription: added subscription from '{subscriber.GetType().Name}'" );

				int key = RuntimeHelpers.GetHashCode( subscriber );
				if ( _subscriptionIndexMap.TryGetValue( key, out List<int>? indices ) ) {
					indices.Add( index );
				} else {
					_subscriptionIndexMap[ key ] = new List<int>() { index };
				}
			}
			finally {
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
		public void RemoveSubscription( object? subscriber, IGameEvent.EventCallback? callback ) {
			ArgumentNullException.ThrowIfNull( subscriber );
			ArgumentNullException.ThrowIfNull( callback );

			_pumpLock.EnterWriteLock();
			try {
				if ( !ContainsCallbackInternal( subscriber, callback, out int index ) ) {
					_logger?.PrintWarning( $"EventSubscriptionSet.RemoveSubscription: subscription not found from '{subscriber.GetType().Name}'" );
					return;
				}
				_logger?.PrintLine( $"EventSubscriptionSet.RemoveSubscription: removed subscription from '{subscriber.GetType().Name}'" );
				RemoveAtIndexInternal( index, subscriber );
			}
			finally {
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
		public void RemoveAllForSubscriber( object? subscriber ) {
			ArgumentNullException.ThrowIfNull( subscriber );

			_pumpLock.EnterWriteLock();
			try {
				int removed = 0;
				for ( int i = _subscriptions.Count - 1; i >= 0; i-- ) {
					if ( _subscriptions[ i ].Subscriber.TryGetTarget( out object? existingSubscriber ) && existingSubscriber == subscriber ) {
						RemoveAtIndexInternal( i, subscriber );
						removed++;
					}
				}
				_logger?.PrintLine( $"EventSubscriptionSet.RemoveAllForSubscriber: removed {removed} subscriptions for '{subscriber.GetType().Name}'" );
			}
			finally {
				_pumpLock.ExitWriteLock();
			}
		}

		/*
		===============
		PumpWithLock
		===============
		*/
		/// <summary>
		/// "Publishes" an event to all subscribers with thread-safety
		/// </summary>
		/// <param name="args"></param>
		public void Pump( in IEventArgs args ) {
			_pumpLock.EnterUpgradeableReadLock();
			try {
				bool shouldCleanup = false;
				if ( ++_cleanupCounter >= CLEANUP_INTERVAL ) {
					shouldCleanup = true;
					_cleanupCounter = 0;
				}
				for ( int i = 0; i < _subscriptions.Count; i++ ) {
					NotifySubscriber( _subscriptions[ i ], in args );
				}

				if ( shouldCleanup ) {
					_pumpLock.EnterWriteLock();
					try {
						CleanupDeadSubscriptionsInternal();
					}
					finally {
						_pumpLock.ExitWriteLock();
					}
				}
			}
			finally {
				_pumpLock.ExitUpgradeableReadLock();
			}
		}

		/*
		===============
		NotifySubscriber
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="subscription"></param>
		/// <param name="args"></param>
		private void NotifySubscriber( in WeakSubscription subscription, in IEventArgs args ) {
			if ( subscription.IsAlive ) {
				try {
					if ( subscription.Callback.TryGetTarget( out var callback ) ) {
						callback.Invoke( in Event, in args );
					}
				} catch ( Exception e ) {
					_logger?.PrintError( $"EventSubscriptionSet.Pump: exception thrown in callback - {e}" );
				}
			}
		}

		/*
		===============
		CleanupDeadSubscriptions
		===============
		*/
		private void CleanupDeadSubscriptionsInternal() {
			int initialCount = _subscriptions.Count;

			for ( int i = _subscriptions.Count - 1; i >= 0; i-- ) {
				if ( !_subscriptions[ i ].IsAlive ) {
					_subscriptions.RemoveAt( i );
				}
			}

			int removed = initialCount - _subscriptions.Count;
			if ( removed > 0 ) {
				_logger?.PrintLine( $"EventSubscriptionSet.CleanupDeadSubscriptions: removed {removed} dangling subscriptions" );
				RebuildIndexMap();
			}
		}

		/*
		===============
		ContainsCallback
		===============
		*/
		/// <summary>
		/// Public method that acquires the read lock and checks for callback existence.
		/// </summary>
		/// <param name="subscriber"></param>
		/// <param name="callback"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		public bool ContainsCallback( object subscriber, IGameEvent.EventCallback callback, out int index ) {
			_pumpLock.EnterReadLock();
			try {
				return ContainsCallbackInternal( subscriber, callback, out index );
			}
			finally {
				_pumpLock.ExitReadLock();
			}
		}

		/*
		===============
		ContainsCallbackInternal
		===============
		*/
		/// <summary>
		/// Internal method that assumes the write lock is already held.
		/// </summary>
		/// <param name="subscriber"></param>
		/// <param name="callback"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		private bool ContainsCallbackInternal( object subscriber, IGameEvent.EventCallback callback, out int index ) {
			index = -1;
			int key = RuntimeHelpers.GetHashCode( subscriber );
			if ( _subscriptionIndexMap.TryGetValue( key, out var candidates ) ) {
				for ( int i = 0; i < candidates.Count; i++ ) {
					int idx = candidates[ i ];
					if ( idx >= 0 && idx < _subscriptions.Count ) {
						var subscription = _subscriptions[ idx ];
						if ( subscription.Subscriber.TryGetTarget( out var existing ) && existing == subscriber
							&& subscription.Callback.TryGetTarget( out var caller ) && caller == callback ) {
							index = idx;
							return true;
						}
					}
				}
			}
			return false;
		}

		/*
		===============
		RemoveAtIndexInternal
		===============
		*/
		/// <summary>
		/// Removes a subscription at the given index and updates the index map using swap-and-pop.
		/// Must be called with the write lock held.
		/// </summary>
		private void RemoveAtIndexInternal( int index, object subscriber ) {
			if ( index < 0 || index >= _subscriptions.Count ) {
				return;
			}

			int lastIndex = _subscriptions.Count - 1;
			if ( index != lastIndex ) {
				// Swap with the last element
				WeakSubscription swapped = _subscriptions[ lastIndex ];
				_subscriptions[ index ] = swapped;

				// Update index map for the swapped element
				if ( swapped.Subscriber.TryGetTarget( out object? swappedSubscriber ) ) {
					int swappedKey = RuntimeHelpers.GetHashCode( swappedSubscriber );
					if ( _subscriptionIndexMap.TryGetValue( swappedKey, out var swappedIndices ) ) {
						int swappedIdx = swappedIndices.IndexOf( lastIndex );
						if ( swappedIdx >= 0 ) {
							swappedIndices[ swappedIdx ] = index;
						}
					}
				}
			}

			// Remove the last element
			_subscriptions.RemoveAt( lastIndex );

			// Update index map for the removed element
			int key = RuntimeHelpers.GetHashCode( subscriber );
			if ( _subscriptionIndexMap.TryGetValue( key, out var indices ) ) {
				indices.Remove( lastIndex );
				if ( indices.Count == 0 ) {
					_subscriptionIndexMap.Remove( key );
				}
			}
		}

		/*
		===============
		RebuildIndexMap
		===============
		*/
		/// <summary>
		/// Rebuilds the subscription index map from scratch, must be called with the write lock held.
		/// </summary>
		private void RebuildIndexMap() {
			_subscriptionIndexMap.Clear();
			for ( int i = 0; i < _subscriptions.Count; i++ ) {
				if ( _subscriptions[ i ].Subscriber.TryGetTarget( out object? subscriber ) ) {
					int key = RuntimeHelpers.GetHashCode( subscriber );
					if ( _subscriptionIndexMap.TryGetValue( key, out var indices ) ) {
						indices.Add( i );
					} else {
						_subscriptionIndexMap[ key ] = new List<int>() { i };
					}
				}
			}
		}
	};
};