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

using NomadCore.Interfaces;
using System;
using System.Runtime.CompilerServices;

namespace NomadCore.Systems.EventSystem.Utilities {
	/*
	===================================================================================
	
	EventExtensions
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	public static class EventExtensions {
		/*
		===============
		Subscribe
		===============
		*/
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static IDisposable Subscribe<TArgs>( this IGameEvent<TArgs> gameEvent, object? subscriber, IGameEvent<TArgs>.GenericEventCallback callback ) where TArgs : IEventArgs {
			gameEvent.Subscribe( subscriber, callback );
			return new EventUnsubscriber( () => gameEvent.Unsubscribe( subscriber, callback ) );
		}

		/*
		===============
		SubscribeOnce
		===============
		*/
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static void SubscribeOnce<TArgs>( this IGameEvent<TArgs> gameEvent, object? subscriber, IGameEvent<TArgs>.GenericEventCallback callback ) where TArgs : IEventArgs {
			void Wrapper( in TArgs args ) {
				callback( args );
				gameEvent.Unsubscribe( subscriber, Wrapper );
			}
			gameEvent.Subscribe( subscriber, callback );
		}

		private sealed class EventUnsubscriber( Action unsubscribe ) : IDisposable {
			private readonly Action Unsubscribe = unsubscribe;

			public void Dispose() => Unsubscribe?.Invoke();
		};
	};
};