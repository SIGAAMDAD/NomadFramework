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

	SubscriptionSetBase

	===================================================================================
	*/
	/// <summary>
	/// Shared base class for subscription set implementations.
	/// Handles common lifecycle, counters, event metadata, and callback lookup helpers.
	/// </summary>
	internal abstract class SubscriptionSetBase<TArgs> : ISubscriptionSet<TArgs>
		where TArgs : struct
	{
		public int SubscriberCount => _subscriberCount;
		private int _subscriberCount = 0;

		public long PublishCount => _publishCount;
		private long _publishCount = 0;

		protected ILoggerService Logger { get; }
		protected IGameEvent<TArgs> EventData { get; }

		protected bool IsDisposed { get; private set; } = false;

		/*
		===============
		SubscriptionSetBase
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="eventData"></param>
		/// <param name="logger"></param>
		protected SubscriptionSetBase( IGameEvent<TArgs> eventData, ILoggerService logger ) {
			ArgumentGuard.ThrowIfNull( eventData );
			EventData = eventData;
			Logger = logger;
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
			if ( IsDisposed ) {
				return;
			}

			Logger?.PrintLine( $"Releasing subscription set for event {EventData.DebugName}..." );
			OnDispose();

			IsDisposed = true;
			GC.SuppressFinalize( this );
		}

		/*
		===============
		OnDispose
		===============
		*/
		/// <summary>
		/// Allows derived classes to dispose owned resources.
		/// </summary>
		protected abstract void OnDispose();

		/*
		===============
		ThrowIfDisposed
		===============
		*/
		/// <summary>
		///
		/// </summary>
		protected void ThrowIfDisposed() {
			StateGuard.ThrowIfDisposed( IsDisposed, this );
		}

		/*
		===============
		IncrementSubscriberCount
		===============
		*/
		/// <summary>
		///
		/// </summary>
		protected void IncrementSubscriberCount() {
			Interlocked.Increment( ref _subscriberCount );
		}

		/*
		===============
		DecrementSubscriberCount
		===============
		*/
		/// <summary>
		///
		/// </summary>
		protected void DecrementSubscriberCount() {
			Interlocked.Decrement( ref _subscriberCount );
		}

		/*
		===============
		IncrementPublishCount
		===============
		*/
		/// <summary>
		///
		/// </summary>
		protected void IncrementPublishCount() {
			Interlocked.Increment( ref _publishCount );
		}

		/*
		===============
		TryFindCallback
		===============
		*/
		/// <summary>
		/// Shared callback lookup helper used by concrete implementations.
		/// </summary>
		protected static bool TryFindCallback<TCallback>(
			SubscriptionCache<TArgs, TCallback> subscriptions,
			TCallback callback,
			out int index
		) {
			for ( int i = 0; i < subscriptions.Count; i++ ) {
				if ( EqualityComparer<TCallback>.Default.Equals( subscriptions[i], callback ) ) {
					index = i;
					return true;
				}
			}

			index = -1;
			return false;
		}

		public abstract bool AddSubscription( EventCallback<TArgs> callback );
		public abstract bool AddSubscriptionAsync( AsyncEventCallback<TArgs> callback );

		public abstract bool RemoveSubscription( EventCallback<TArgs> callback );
		public abstract bool RemoveSubscriptionAsync( AsyncEventCallback<TArgs> callback );

		public abstract void Pump( in TArgs args );
		public abstract Task PumpAsync( TArgs args, CancellationToken ct );

		public abstract bool ContainsCallback( EventCallback<TArgs> callback, out int index );
		public abstract bool ContainsCallbackAsync( AsyncEventCallback<TArgs> callback, out int index );
	};
};