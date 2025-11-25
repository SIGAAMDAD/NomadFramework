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
using NomadCore.Interfaces;
using System;
using System.Collections.Generic;
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

	public sealed class EventSubscriptionSet : IDisposable {
		private readonly struct WeakSubscription {
			public readonly WeakReference<object> Subscriber;
			public readonly IGameEvent.EventCallback Callback;
			public readonly bool IsAlive => Subscriber.TryGetTarget( out _ );

			public WeakSubscription( object? subscriber, IGameEvent.EventCallback? callback ) {
				ArgumentNullException.ThrowIfNull( subscriber );
				ArgumentNullException.ThrowIfNull( callback );

				Subscriber = new WeakReference<object>( subscriber );
				Callback = callback;
			}
		};

		/// <summary>
		/// The number of pumps before initiating a purge
		/// </summary>
		private const int CLEANUP_INTERVAL = 30;

		/// <summary>
		/// 
		/// </summary>
		public readonly IGameEvent? Event;

		private readonly IConsoleService Console;

		private readonly List<WeakSubscription> Subscriptions = new List<WeakSubscription>();
		private int CleanupCounter = 0;

		private readonly List<IGameEvent> Friends = new List<IGameEvent>();

		private readonly object Lock = new object();

		// perhaps... use only the readerwriter lock?
		private readonly ReaderWriterLockSlim PumpLock = new ReaderWriterLockSlim();

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
		public EventSubscriptionSet( IGameEvent? eventData, IConsoleService console ) {
			ArgumentNullException.ThrowIfNull( eventData );

			Event = eventData;
			Console = console;
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
			lock ( Lock ) {
				Subscriptions.Clear();
			}
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

			lock ( Lock ) {
				if ( ContainsCallback( subscriber, callback, out _ ) ) {
					Console?.PrintError( $"EventSubscriptionSet.CheckForDuplicateSubscription: duplicate subscription from '{subscriber.GetType().Name}'" );
					return;
				}
				Subscriptions.Add( new WeakSubscription( subscriber, callback ) );
				Console?.PrintLine( $"EventSubscriptionSet.AddSubsription: added subscription from '{subscriber.GetType().Name}'" );
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

			lock ( Lock ) {
				if ( !ContainsCallback( subscriber, callback, out int index ) ) {
					Console?.PrintWarning( $"EventSubscriptionSet.RemoveSubscription: subscription not found from '{subscriber.GetType().Name}'" );
					return;
				}
				Console?.PrintLine( $"EventSubscriptionSet.RemoveSubscription: removed subscription from '{subscriber.GetType().Name}'" );
				Subscriptions.RemoveAt( index );
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

			PumpLock.EnterWriteLock();
			try {
				int removed = 0;
				for ( int i = Subscriptions.Count - 1; i >= 0; i-- ) {
					if ( Subscriptions[ i ].Subscriber.TryGetTarget( out object? existingSubscriber ) && existingSubscriber == subscriber ) {
						Subscriptions.RemoveAt( i );
						removed++;
					}
				}
				Console?.PrintLine( $"EventSubscriptionSet.RemoveAllForSubscriber: removed {removed} subscriptions for '{subscriber.GetType().Name}'" );
			}
			finally {
				PumpLock.ExitWriteLock();
			}
		}

		/*
		===============
		Pump
		===============
		*/
		/// <summary>
		/// "Publishes" an event to the system.
		/// </summary>
		/// <param name="args"></param>
		/// <param name="singleThreaded"></param>
		/// <exception cref="ArgumentNullException">Thrown if <see cref="Event"/> or a callback in the <see cref="Subscriptions"/> is null.</exception>
		public void Pump( in IEventArgs args ) {
			PumpLock.EnterReadLock();
			try {
				if ( ++CleanupCounter >= CLEANUP_INTERVAL ) {
					PumpLock.EnterWriteLock();
					try {
						CleanupDeadSubscriptions();
						CleanupCounter = 0;
					}
					finally {
						PumpLock.ExitWriteLock();
					}
				}
				for ( int i = 0; i < Subscriptions.Count; i++ ) {
					NotifySubscriber( Subscriptions[ i ], in args );
				}
			}
			finally {
				PumpLock.ExitReadLock();
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
					subscription.Callback.Invoke( in Event, in args );
				} catch ( Exception e ) {
					Console?.PrintError( $"EventSubscriptionSet.Pump: exception thrown in callback - {e}" );
				}
			}
		}

		/*
		===============
		CleanupDeadSubscriptions
		===============
		*/
		private void CleanupDeadSubscriptions() {
			PumpLock.EnterWriteLock();
			int initialCount = Subscriptions.Count;

			for ( int i = Subscriptions.Count - 1; i >= 0; i-- ) {
				if ( !Subscriptions[ i ].IsAlive ) {
					Subscriptions.RemoveAt( i );
				}
			}

			int removed = initialCount - Subscriptions.Count;
			if ( removed > 0 ) {
				Console?.PrintLine( $"EventSubscriptionSet.CleanupDeadSubscriptions: removed {removed} dangling subscriptions" );
			}
			PumpLock.ExitWriteLock();
		}

		/*
		===============
		ContainsCallback
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="subscriber"></param>
		/// <param name="callback"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		private bool ContainsCallback( object subscriber, IGameEvent.EventCallback callback, out int index ) {
			for ( int i = 0; i < Subscriptions.Count; i++ ) {
				if ( !Subscriptions[ i ].Subscriber.TryGetTarget( out object? existingSubscriber ) ) {
					continue;
				} else if ( existingSubscriber == subscriber && Subscriptions[ i ].Callback == callback ) {
					index = i;
					return true;
				}
			}
			index = -1;
			return false;
		}

		/*
		===============
		HasFriend
		===============
		*/
		private bool HasFriend( in IGameEvent? otherEvent, out int index ) {
			for ( int i = 0; i < Friends.Count; i++ ) {
				if ( Friends[ i ].Equals( otherEvent ) ) {
					index = i;
					return true;
				}
			}
			index = -1;
			return false;
		}
	};
};