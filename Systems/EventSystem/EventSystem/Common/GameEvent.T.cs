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

using NomadCore.Enums.EventSystem;
using NomadCore.Interfaces.EventSystem;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace NomadCore.Systems.EventSystem.Common {
	/*
	===================================================================================
	
	GameEvent<TArgs>
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="TArgs">The type of <see cref="TypedEventArgs"/> that'll be used with this event type.</typeparam>
	/// <remarks>
	/// Creates a new GameEvent object with the debugging alias of <paramref name="name"/>.
	/// </remarks>
	/// <param name="name">The name of the event, should be unique.</param>
	/// <exception cref="ArgumentException">Thrown if name is null or empty.</exception>

	public class GameEvent<TArgs>( string? name ) : IGameEvent<TArgs> where TArgs : IEventArgs {
		/// <summary>
		/// The name of the event.
		/// </summary>
		public string? Name => _nonGenericEvent.Name;
		public EventThreadingPolicy ThreadingPolicy => EventThreadingPolicy.MainThread;

		private readonly GameEvent _nonGenericEvent = new GameEvent( name );
		private readonly ConcurrentDictionary<object, List<IGameEvent<TArgs>.GenericEventCallback>> _typedSubscriptions = new();
		private readonly ConcurrentDictionary<object, List<Func<TArgs, CancellationToken, Task>>> _asyncSubscriptions = new();

		/*
		===============
		Subscribe
		===============
		*/

		/// <summary>
		/// 
		/// </summary>
		/// <param name="subscriber"></param>
		/// <param name="callback"></param>
		/// <exception cref="InvalidCastException"></exception>
		public virtual void Subscribe( object? subscriber, IGameEvent<TArgs>.GenericEventCallback? callback ) {
			ArgumentNullException.ThrowIfNull( subscriber );
			ArgumentNullException.ThrowIfNull( callback );

			var subscriptions = _typedSubscriptions.GetOrAdd( subscriber, _ => new List<IGameEvent<TArgs>.GenericEventCallback>() );
			lock ( subscriptions ) {
				subscriptions.Add( callback );
			}

			_nonGenericEvent.Subscribe( subscriber, ( in eventData, in args ) => {
				if ( args is TArgs typedArgs ) {
					callback.Invoke( typedArgs );
				} else {
					throw new InvalidCastException( nameof( args ) );
				}
			} );
		}

		/*
		===============
		Subscribe
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="subscriber"></param>
		/// <param name="asyncCallback"></param>
		public void Subscribe( object? subscriber, Func<TArgs, CancellationToken, Task>? asyncCallback ) {
			ArgumentNullException.ThrowIfNull( subscriber );
			ArgumentNullException.ThrowIfNull( asyncCallback );

			var subscriptions = _asyncSubscriptions.GetOrAdd( subscriber, _ => new List<Func<TArgs, CancellationToken, Task>>() );
			lock ( subscriptions ) {
				subscriptions.Add( asyncCallback );
			}
		}

		/*
		===============
		Unsubscribe
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="subscriber"></param>
		/// <param name="asyncCallback"></param>
		public void Unsubscribe( object? subscriber, Func<TArgs, CancellationToken, Task>? asyncCallback ) {
			ArgumentNullException.ThrowIfNull( subscriber );
			ArgumentNullException.ThrowIfNull( asyncCallback );
			
			if ( _asyncSubscriptions.TryGetValue( subscriber, out var subscribers ) ) {
				_asyncSubscriptions.TryRemove( new KeyValuePair<object, List<Func<TArgs, CancellationToken, Task>>>( subscriber, subscribers ) );
			} else {
				_nonGenericEvent.Logger.PrintError( $"EventSubscriptionSet.Unsubscribe: attempted removal on non-existent subscription for callback '{asyncCallback.Method.Name}'s" );
			}
		}

		/*
		===============
		Unsubscribe
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="subscriber"></param>
		/// <param name="callback"></param>
		public virtual void Unsubscribe( object? subscriber, IGameEvent<TArgs>.GenericEventCallback? callback ) {
			ArgumentNullException.ThrowIfNull( subscriber );
			ArgumentNullException.ThrowIfNull( callback );

			if ( _typedSubscriptions.TryGetValue( subscriber, out var subscriptions ) ) {
				lock ( subscriptions ) {
					subscriptions.Remove( callback );
					if ( subscriptions.Count == 0 ) {
						_typedSubscriptions.TryRemove( subscriber, out _ );
					}
				}
			}
			_nonGenericEvent.EventBus.Unsubscribe( subscriber, this, callback );
		}

		/*
		===============
		Publish
		===============
		*/
		public virtual void Publish( TArgs eventArgs ) {
			_nonGenericEvent.EventBus.Publish( this, eventArgs );
		}

		/*
		===============
		PublishAsync
		===============
		*/
		public Task PublishAsync( TArgs eventArgs, CancellationToken cancellationToken = default ) {
			var tasks = new List<Task>( _asyncSubscriptions.Count );
			foreach ( var subscriberPair in _asyncSubscriptions ) {
				foreach ( var asyncCallback in subscriberPair.Value ) {
					tasks.Add( asyncCallback( eventArgs, cancellationToken ) );
				}
			}
			return Task.WhenAll( tasks );
		}

		/*
		===============
		Subscribe
		===============
		*/
		void IGameEvent.Subscribe( object? subscriber, IGameEvent.EventCallback? callback ) {
			_nonGenericEvent.EventBus.Subscribe( subscriber, this, callback );
		}

		/*
		===============
		Unsubscribe
		===============
		*/
		void IGameEvent.Unsubscribe( object? subscriber, IGameEvent.EventCallback? callback ) {
			_nonGenericEvent.EventBus.Unsubscribe( subscriber, this, callback );
		}

		/*
		===============
		Publish
		===============
		*/
		void IGameEvent.Publish( IEventArgs args ) {
			_nonGenericEvent.EventBus.Publish( this, args );
		}

		/*
		===============
		Dispose
		===============
		*/
		public virtual void Dispose() {
			foreach ( var subscriptionList in _typedSubscriptions.Values ) {
				subscriptionList.Clear();
			}
			_typedSubscriptions.Clear();
			_nonGenericEvent.Dispose();

			GC.SuppressFinalize( this );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public virtual EventThreadingPolicy GetDefaultThreadingPolicy() => EventThreadingPolicy.MainThread;
	};
};