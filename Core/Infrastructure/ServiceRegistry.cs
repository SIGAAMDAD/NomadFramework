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

using Godot;
using NomadCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace NomadCore.Infrastructure {
	/*
	===================================================================================
	
	ServiceRegistry
	
	===================================================================================
	*/
	/// <summary>
	/// A global registration cache for services.
	/// </summary>
	
	public static class ServiceRegistry {
		private static readonly Dictionary<Type, IGameService> Services = new Dictionary<Type, IGameService>();

		/*
		===============
		Register
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="service"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static T Register<T>( T service ) where T : IGameService {
			Services[ typeof( T ) ] = service;
			return service;
		}

		/*
		===============
		Resolve
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static T Resolve<T>() where T : IGameService {
			return (T)Services[ typeof( T ) ];
		}

		/*
		===============
		Resolve
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static T? Get<T>() where T : IGameService {
			return Services.TryGetValue( typeof( T ), out IGameService? value ) ? (T)value : default;
		}

		/*
		===============
		HasService
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static bool HasService<T>() where T : IGameService {
			return Services.ContainsKey( typeof( T ) );
		}

		/*
		===============
		InitializeAll
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		public static void InitializeAll() {
			foreach ( var service in Services ) {
				service.Value.Initialize();
			}
		}
	};
};