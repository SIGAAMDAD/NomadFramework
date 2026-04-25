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
	
	internal sealed class SubscriptionSet<TArgs> : SubscriptionSetBase<TArgs>
		where TArgs : struct
	{
		private readonly SubscriptionCache<TArgs, EventCallback<TArgs>> _genericSubscriptions;
		private readonly SubscriptionCache<TArgs, AsyncEventCallback<TArgs>> _asyncSubscriptions;
		private readonly List<Task> _taskCache = new List<Task>();

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
		/// <param name="exceptionPolicy"></param>
		public SubscriptionSet( IGameEvent<TArgs> eventData, ILoggerService logger, EventExceptionPolicy exceptionPolicy )
			: base( eventData, logger, exceptionPolicy )
		{
			_genericSubscriptions = new SubscriptionCache<TArgs, EventCallback<TArgs>>();
			_asyncSubscriptions = new SubscriptionCache<TArgs, AsyncEventCallback<TArgs>>();
		}

		/*
		===============
		OnDispose
		===============
		*/
		/// <summary>
		///
		/// </summary>
		protected override void OnDispose() {
			_genericSubscriptions?.Dispose();
			_asyncSubscriptions?.Dispose();
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
		public override bool AddSubscription( EventCallback<TArgs> callback ) {
			ThrowIfDisposed();
			ArgumentGuard.ThrowIfNull( callback );

			lock ( _genericSubscriptions ) {
				_genericSubscriptions.Add( callback );
				IncrementSubscriberCount();
				return true;
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
		public override bool AddSubscriptionAsync( AsyncEventCallback<TArgs> callback ) {
			ThrowIfDisposed();
			ArgumentGuard.ThrowIfNull( callback );

			if ( ContainsCallbackAsync( callback, out _ ) ) {
				Logger?.PrintWarning( $"SubscriptionSet.AddSubscriptionAsync: subscription for callback '{callback.Method.Name}' already exists!" );
				return false;
			}

			lock ( _asyncSubscriptions ) {
				_asyncSubscriptions.Add( callback );
				_taskCache.Add( null! );
				IncrementSubscriberCount();
				return true;
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
		public override bool RemoveSubscription( EventCallback<TArgs> callback ) {
			ThrowIfDisposed();
			ArgumentGuard.ThrowIfNull( callback );

			if ( !ContainsCallback( callback, out int index ) ) {
				Logger?.PrintError( $"SubscriptionSet.RemoveSubscription: no such existing subscription for callback '{callback.Method.Name}'" );
				return false;
			}

			lock ( _genericSubscriptions ) {
				_genericSubscriptions.RemoveAt( index );
				DecrementSubscriberCount();
				return true;
			}
		}

		/*
		===============
		RemoveSubscriptionAsync
		===============
		*/
		/// <summary>
		/// Removes the provided <paramref name="callback"/> from the event's subscription list.
		/// </summary>
		/// <param name="callback">The callback to remove from the subscription list.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="callback"/> is null.</exception>
		public override bool RemoveSubscriptionAsync( AsyncEventCallback<TArgs> callback ) {
			ThrowIfDisposed();
			ArgumentGuard.ThrowIfNull( callback );

			if ( !ContainsCallbackAsync( callback, out int index ) ) {
				Logger?.PrintError( $"SubscriptionSet.RemoveSubscriptionAsync: no such existing subscription for callback '{callback.Method.Name}'" );
				return false;
			}

			lock ( _asyncSubscriptions ) {
				_asyncSubscriptions.RemoveAt( index );
				_taskCache.RemoveAt( index );
				DecrementSubscriberCount();
				return true;
			}
		}

		/*
		===============
		Pump
		===============
		*/
		/// <summary>
		/// "Publishes" an event to all subscribers with thread-safety.
		/// </summary>
		/// <param name="args"></param>
		public override void Pump( in TArgs args ) {
			ThrowIfDisposed();

#if EVENT_DEBUG
			Logger?.PrintLine( $"SubscriptionSet.Pump: publishing event {EventData.DebugName}" );
#endif
			
			List<EventHandlerException>? failures = null;

			lock ( _genericSubscriptions ) {
				for ( int i = 0; i < _genericSubscriptions.Count; i++ ) {
					var callback = _genericSubscriptions[i];
					InvokeCallback( callback, i, in args );
				}
			}

			IncrementPublishCount();

			if ( failures is  { Count: > 0 } && ExceptionPolicy == EventExceptionPolicy.AggregateAfterDispatch ) {
				throw new EventPublishException(
					EventData.DebugName,
					typeof( TArgs ),
					failures
				);
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
		public override async Task PumpAsync( TArgs args, CancellationToken ct ) {
			ThrowIfDisposed();

#if EVENT_DEBUG
			Logger?.PrintLine( $"SubscriptionSet.PumpAsync: publishing event {EventData.DebugName} asynchronously..." );
#endif
			int subscriptionCount = _asyncSubscriptions.Count;

			ct.ThrowIfCancellationRequested();

			for ( int i = 0; i < subscriptionCount; i++ ) {
				ct.ThrowIfCancellationRequested();
				_taskCache[i] = _asyncSubscriptions[i].Invoke( args, ct );
			}

			try {
				await Task.WhenAll( _taskCache ).ConfigureAwait( false );
			} catch {
				List<EventHandlerException> failures = new();

				for ( int i = 0; i < subscriptionCount; i++ ) {
					Task task = _taskCache[i];

					if ( task.IsFaulted && task.Exception != null ) {
						foreach ( Exception ex in task.Exception.InnerExceptions ) {
							failures.Add(
								new EventHandlerException(
									EventData.DebugName,
									typeof( TArgs ),
									_asyncSubscriptions[i].Method.Name,
									i,
									ex
								)
							);
						}
					}
				}

				foreach ( var failure in failures ) {
					Logger?.PrintError( failure.ToString() );
				}

				if ( ExceptionPolicy != EventExceptionPolicy.ReportAndContinue ) {
					throw new EventPublishException(
						EventData.DebugName,
						typeof( TArgs ),
						failures
					);
				}
			} finally {
				for ( int i = 0; i < subscriptionCount; i++ ) {
					_taskCache[i] = null;
				}
			}
			IncrementPublishCount();
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
		public override bool ContainsCallback( EventCallback<TArgs> callback, out int index ) {
			ThrowIfDisposed();

			lock ( _genericSubscriptions ) {
				return TryFindCallback( _genericSubscriptions, callback, out index );
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
		public override bool ContainsCallbackAsync( AsyncEventCallback<TArgs> callback, out int index ) {
			ThrowIfDisposed();

			lock ( _asyncSubscriptions ) {
				return TryFindCallback( _asyncSubscriptions, callback, out index );
			}
		}
	};
};