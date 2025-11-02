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

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace CVars {
	/*
	===================================================================================
	
	CVarGroup
	
	===================================================================================
	*/
	/// <summary>
	/// Handles a cvar set/system
	/// </summary>

	public sealed class CVarGroup {
		public readonly string? Name;

		public ICVar? this[ string name ] {
			get => Cvars.TryGetValue( name, out ICVar? cvar ) ? cvar : null;
		}

		public readonly Dictionary<string, ICVar> Cvars = null;

		/*
		===============
		CVarGroup
		===============
		*/
		public CVarGroup( string? name, int initialCapacity = 64 ) {
			ArgumentException.ThrowIfNullOrEmpty( name );
			ArgumentOutOfRangeException.ThrowIfLessThan( initialCapacity, 0, nameof( initialCapacity ) );

			Cvars = new Dictionary<string, ICVar>( initialCapacity );
			Name = name;
		}

		/*
		===============
		Add
		===============
		*/
		/// <summary>
		/// Adds the provided <paramref name="cvar"/> to the group
		/// </summary>
		/// <param name="cvar">The cvar to add to the group</param>
		public void Add( in ICVar? cvar ) {
			ArgumentNullException.ThrowIfNull( cvar );
			ArgumentNullException.ThrowIfNull( Cvars );

			Cvars.Add( cvar.Name, cvar );
		}

		/*
		===============
		IsInGroup
		===============
		*/
		/// <summary>
		/// Checks if the provided <paramref name="cvar"/> is within the group
		/// </summary>
		/// <param name="cvar">The cvar to search for</param>
		/// <returns>Returns true of the cvar is within the group</returns>
		public bool IsInGroup( in ICVar? cvar ) {
			ArgumentNullException.ThrowIfNull( cvar );
			ArgumentNullException.ThrowIfNull( Cvars );

			return Cvars.ContainsKey( cvar.Name );
		}

		/*
		===============
		GetCvar
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="name"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public CVar<T>? GetCvar<T>( string? name ) {
			ArgumentException.ThrowIfNullOrEmpty( name );

			return Cvars.TryGetValue( name, out ICVar? cvar ) ? cvar as CVar<T> : null;
		}

		/*
		===============
		ForEachInGroup
		===============
		*/
		public void ForEachInGroup( Action<ICVar>? callback ) {
			ArgumentNullException.ThrowIfNull( callback );
			ArgumentNullException.ThrowIfNull( Cvars );

			foreach ( var cvar in Cvars ) {
				callback.Invoke( cvar.Value );
			}
		}
	};
};