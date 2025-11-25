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
using NomadCore.Systems.EventSystem.Common;
using System;
using System.Collections.Concurrent;

namespace NomadCore.Systems.EventSystem.Services {
	/*
	===================================================================================
	
	GameEventRegistry
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	public static class GameEventRegistry {
		private static readonly ConcurrentDictionary<string, GameEvent> EventCache = new ConcurrentDictionary<string, GameEvent>();
		private static readonly Func<string, GameEvent> CreateEvent = ( name ) => new GameEvent( name );

		/*
		===============
		RegisterEvent
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static IGameEvent RegisterEvent( string? name ) {
			ArgumentException.ThrowIfNullOrEmpty( name );

			var eventData = EventCache.GetOrAdd( name, CreateEvent( name ) );

			return eventData;
		}

		/*
		===============
		GetEvent
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static IGameEvent? GetEvent( string? name ) {
			ArgumentException.ThrowIfNullOrEmpty( name );
			return EventCache.TryGetValue( name, out GameEvent? eventData ) ? eventData : null;
		}
	};
};