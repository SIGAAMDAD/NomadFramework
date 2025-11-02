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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace CVars {
	/*
	===================================================================================
	
	CVarSystem
	
	===================================================================================
	*/
	/// <summary>
	/// Global manager for CVars
	/// </summary>

	public sealed class CVarSystem : ICVarSystem {
		private readonly ConcurrentDictionary<string, ICVar> Cvars = new ConcurrentDictionary<string, ICVar>( StringComparer.OrdinalIgnoreCase );
		private readonly HashSet<CVarGroup> Groups = new HashSet<CVarGroup>();
		private readonly IConsoleService Console;

		private static CVarSystem? Instance;

		/*
		===============
		CVarSystem
		===============
		*/
		public CVarSystem( IConsoleService? console ) {
			ArgumentNullException.ThrowIfNull( console );

			Console = console;
			Instance = this;
		}

		/*
		===============
		Register
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="cvar"></param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static void Register<T>( CVar<T> cvar ) {
			ArgumentNullException.ThrowIfNull( Instance );

			if ( !Instance.Cvars.ContainsKey( cvar.Name ) ) {
				Instance.Cvars.TryAdd( cvar.Name, cvar );
			}
		}

		/*
		===============
		Register
		===============
		*/
		/// <summary>
		/// Registers a CVar with the global CVarSystem and adds it to the CVar cache
		/// </summary>
		/// <typeparam name="T">The internal type of the cvar's value</typeparam>
		/// <param name="name">The cvar's name</param>
		/// <param name="defaultValue">The cvar's default value</param>
		/// <param name="description">The cvar's description (optional)</param>
		/// <param name="flags">The cvar's flags (optional)</param>
		/// <param name="validator">The cvar's value validator (optional)</param>
		/// <returns></returns>
		public ICVar? Register<T>( string? name, T? defaultValue, string? description = "", CVarFlags flags = CVarFlags.None, Func<T, bool>? validator = null ) {
			ArgumentException.ThrowIfNullOrEmpty( name );
			ArgumentNullException.ThrowIfNull( defaultValue );

			if ( Cvars.TryGetValue( name, out ICVar? var ) ) {
				ArgumentNullException.ThrowIfNull( var );
				CVar<T>? value;
				try {
					value = var as CVar<T>;
				} catch ( InvalidCastException ) {
					throw new InvalidOperationException( $"CVar {name} found in CVarSystem cache isn't a valid CVar object!" );
				}
				return value;
			}

			CVar<T> cvar = new CVar<T>( name, defaultValue, description, flags, validator );
			Cvars.TryAdd( name, cvar );

			Console.PrintLine( $"CVarSystem.Register: registered CVar '{name}' with default value {defaultValue} and flags {flags}." );

			return cvar;
		}

		/*
		===============
		RemoveCVar
		===============
		*/
		/// <summary>
		/// Removes the given CVar from the cache
		/// </summary>
		/// <param name="cvar">The cvar to remove</param>
		public static void RemoveCVar( in ICVar? cvar ) {
			ArgumentNullException.ThrowIfNull( Instance );
			ArgumentNullException.ThrowIfNull( cvar );

			try {
				Instance.Cvars.TryRemove( new KeyValuePair<string, ICVar>( cvar.Name, cvar ) );
			} catch ( Exception e ) {
				Instance.Console.PrintError( $"CVarSystem.RemoveCVar: error removing cached CVar {cvar.Name} - {e.Message}" );
			}
		}

		/*
		===============
		GetCVar
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static CVar<T>? GetCVar<T>( string? name ) {
			ArgumentNullException.ThrowIfNull( Instance );
			ArgumentException.ThrowIfNullOrEmpty( name );

			return Instance.Cvars.TryGetValue( name, out ICVar? cvar ) ? cvar as CVar<T> : null;
		}

		/*
		===============
		AddGroup
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="group"></param>
		/// <exception cref="InvalidOperationException"></exception>
		public static void AddGroup( in CVarGroup? group ) {
			ArgumentNullException.ThrowIfNull( Instance );
			ArgumentNullException.ThrowIfNull( group );

			if ( Instance.Groups.Contains( group ) ) {
				throw new InvalidOperationException( $"CVarGroup {group.Name} added twice" );
			}
			Instance.Groups.Add( group );

			Instance.Console.PrintLine( $"CVarSystem.AddGroup: Added CVar group {group.Name}." );
		}

		/*
		===============
		Find
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static ICVar? Find( string? name ) {
			ArgumentNullException.ThrowIfNull( Instance );
			ArgumentException.ThrowIfNullOrEmpty( name );
			ArgumentNullException.ThrowIfNull( Instance.Cvars );

			return Instance.Cvars.TryGetValue( name, out ICVar? cvar ) ? cvar : null;
		}

		/*
		===============
		TryFind
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="name"></param>
		/// <param name="cvar"></param>
		/// <returns></returns>
		public static bool TryFind<T>( string? name, out CVar<T>? cvar ) {
			ArgumentNullException.ThrowIfNull( Instance );
			ArgumentException.ThrowIfNullOrEmpty( name );

			if ( Instance.Cvars.TryGetValue( name, out ICVar? var ) && var is CVar<T> typedVar ) {
				cvar = typedVar;
				return true;
			}

			cvar = null;
			return false;
		}

		/*
		===============
		ResetAll
		===============
		*/
		/// <summary>
		/// Resets all CVars within this group to their default values
		/// </summary>
		public static void ResetAll() {
			ArgumentNullException.ThrowIfNull( Instance );
			foreach ( var cvar in Instance.Cvars.Values ) {
				cvar.Reset();
			}
		}

		/*
		===============
		Save
		===============
		*/
		/// <summary>
		/// Writes all CVars and their corresponding groups to the provided configuration file in .ini format
		/// </summary>
		/// <param name="configFile">The file to write .ini values tos</param>
		public static void Save( string? configFile ) {
			ArgumentNullException.ThrowIfNull( Instance );
			ArgumentException.ThrowIfNullOrEmpty( configFile );
			ArgumentNullException.ThrowIfNull( Instance.Cvars );

			// ensure we block all access
			lock ( Instance.Cvars ) {
				ConfigFileWriter writer = new ConfigFileWriter( configFile, Instance.Console, Instance.Groups );
			}
		}

		/*
		===============
		Load
		===============
		*/
		/// <summary>
		/// Loads cvar values from the provided configuration file
		/// </summary>
		/// <param name="configFile">The file to load .ini values from</param>
		public static void Load( string? configFile ) {
			ArgumentNullException.ThrowIfNull( Instance );
			ArgumentException.ThrowIfNullOrEmpty( configFile );

			// make sure we actually have something to load
			if ( Instance.Cvars == null || Instance.Cvars.IsEmpty ) {
				Instance.Console.PrintLine( "CVarSystem.Load: no cvars yet." );
				return;
			}

			// ensure we block all access
			lock ( Instance.Cvars ) {
				ConfigFileReader reader = new ConfigFileReader( configFile, Instance.Console );

				foreach ( var group in Instance.Groups ) {
					group.ForEachInGroup(
						( cvar ) => {
							if ( reader.TryGetValue( cvar.Name, out string? value ) ) {
								ArgumentException.ThrowIfNullOrEmpty( value );

								cvar.SetFromString( value );
							}
						}
					);
				}
			}
		}
	};
};