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

	LockFreeSubscriptionSet

	===================================================================================
	*/
	/// <summary>
	/// A completely lock free subscription set, this is meant for extremely low-latency high performance event pumping. This is explicitly meant to run single-threaded.
	/// </summary>
	/// <remarks>
	/// DO NOT MULTITHREAD THIS, YOU WILL GET RACE CONDITIONS!!!
	/// </remarks>

	internal sealed class LockFreeSubscriptionSet<TArgs> : ISubscriptionSet<TArgs>
		where TArgs : struct
	{
		private readonly ILoggerService _logger;
		private readonly IGameEvent<TArgs> _eventData;
		private readonly SubscriptionCache<TArgs> _genericSubscriptions;

		/// <summary>
		/// The number of pumps before initiating a purge.
		/// </summary>
		private readonly int _cleanupInterval;
		private int _cleanupCounter = 0;

		private readonly HashSet<WeakReference<IGameEvent>> _friends = new HashSet<WeakReference<IGameEvent>>();

		/*
		===============
		LockFreeSubscriptionSet
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="eventData"></param>
		/// <param name="logger"></param>
		/// <param name="cleanupInterval"></param>
		public LockFreeSubscriptionSet( IGameEvent<TArgs> eventData, ILoggerService logger, int cleanupInterval = 100 ) {
			_logger = logger;
			_genericSubscriptions = new SubscriptionCache<TArgs>( logger );
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
				_logger?.PrintWarning( $"LockFreeSubscriptionSet.BindEventFriend: event '{_eventData.DebugName}' already has friendship with event '{friend.DebugName}'" );
			}
#if EVENT_DEBUG
			_logger?.PrintLine( $"LockFreeSubscriptionSet.BindEventFriend: friendship created between events '{_eventData.DebugName}' and '{friend.DebugName}'" );
#endif
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
			ArgumentGuard.ThrowIfNull( subscriber );
			ArgumentGuard.ThrowIfNull( callback );

			_genericSubscriptions.Add( SubscriptionEntry<TArgs>.Create( subscriber, callback ) );
		}

		/*
		===============
		AddSubscriptionAsync
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="subscriber"></param>
		/// <param name="callback"></param>
		/// <exception cref="NotSupportedException"></exception>
		public void AddSubscriptionAsync( object subscriber, AsyncEventCallback<TArgs> callback ) {
			throw new NotSupportedException();
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
			ArgumentGuard.ThrowIfNull( subscriber );
			ArgumentGuard.ThrowIfNull( callback );

			if ( !ContainsCallback( subscriber, callback, out int index ) ) {
#if EVENT_DEBUG
				_logger.PrintWarning( $"" );
#endif
			}

			_genericSubscriptions.RemoveAt( index );
		}

		/*
		===============
		RemoveSubscription
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="subscriber"></param>
		/// <param name="callback"></param>
		/// <exception cref="NotSupportedException"></exception>
		public void RemoveSubscriptionAsync( object subscriber, AsyncEventCallback<TArgs> callback ) {
			throw new NotSupportedException();
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
			ArgumentGuard.ThrowIfNull( subscriber );

			_genericSubscriptions.RemoveAllOwnedBy( subscriber );
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
			for ( int i = 0; i < _genericSubscriptions.Count; i++ ) {
				var subscription = _genericSubscriptions[ i ];
				if ( subscription.TryGetCallback( out EventCallback<TArgs>? callback ) ) {
					callback?.Invoke( in args );
				}
			}
			_genericSubscriptions.CleanupIncremental( 24 );
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
		/// <exception cref="NotSupportedException"></exception>
		public async Task PumpAsync( TArgs args, CancellationToken ct ) {
			throw new NotSupportedException();
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
			index = -1;
			for ( int i = 0; i < _genericSubscriptions.Count; i++ ) {
				if ( _genericSubscriptions[ i ].Matches( subscriber, callback ) ) {
					index = i;
					return true;
				}
			}
			return false;
		}

		/*
		===============
		ContainsCallbackAsync
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="subscriber"></param>
		/// <param name="callback"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		/// <exception cref="NotSupportedException"></exception>
		public bool ContainsCallbackAsync( object subscriber, AsyncEventCallback<TArgs> callback, out int index ) {
			throw new NotSupportedException();
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
			_genericSubscriptions.CleanupFull();
		}
	};
};
