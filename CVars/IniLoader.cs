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
using Microsoft.Extensions.Configuration.Ini;
using System;
using System.Collections.Generic;

namespace CVars {
	/*
	===================================================================================
	
	IniLoader
		
	===================================================================================
	*/
	/// <summary>
	/// Loads key-value pairs from .ini files, specifically configuration files
	/// </summary>

	public sealed class IniLoader : IDisposable {
		private readonly IDictionary<string, string?>? IniData;

		/*
		===============
		IniLoader
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="configFile"></param>
		public IniLoader( string? configFile, IConsoleService console ) {
			ArgumentException.ThrowIfNullOrEmpty( configFile );

			System.IO.FileStream? fileStream;
			try {
				fileStream = new System.IO.FileStream( configFile, System.IO.FileMode.Open );
			} catch ( Exception e ) {
				console.PrintWarning( $"Error opening configuration file {configFile}: {e.Message}" );
				return;
			}

			IniData = IniStreamConfigurationProvider.Read( fileStream );
			ArgumentNullException.ThrowIfNull( IniData );
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
		}

		/*
		===============
		LoadConfigValue
		===============
		*/
		/// <summary>
		/// Loads a configuration value in the form of an integer
		/// </summary>
		/// <param name="name">The name of the key of the configuration value</param>
		/// <param name="value">The output value</param>
		public bool LoadConfigValue( string? name, out int value ) {
			ArgumentNullException.ThrowIfNull( IniData );
			ArgumentException.ThrowIfNullOrEmpty( name );

			if ( IniData.TryGetValue( name, out string? data ) ) {
				ArgumentException.ThrowIfNullOrEmpty( data );

				value = Convert.ToInt32( data );
				return true;
			}
			value = 0;
			return false;
		}

		/*
		===============
		LoadConfigValue
		===============
		*/
		/// <summary>
		/// Loads a configuration value in the form of a float
		/// </summary>
		/// <param name="name">The name of the key of the configuration value</param>
		/// <param name="value">The output value</param>
		public bool LoadConfigValue( string? name, out float value ) {
			ArgumentNullException.ThrowIfNull( IniData );
			ArgumentException.ThrowIfNullOrEmpty( name );

			if ( IniData.TryGetValue( name, out string? data ) ) {
				ArgumentException.ThrowIfNullOrEmpty( data );

				value = Convert.ToSingle( data );
				return true;
			}
			value = 0.0f;
			return false;
		}

		/*
		===============
		LoadConfigValue
		===============
		*/
		/// <summary>
		/// Loads a configuration value in the form of a boolean
		/// </summary>
		/// <param name="name">The name of the key of the configuration value</param>
		/// <param name="value">The output value</param>
		public bool LoadConfigValue( string? name, out bool value ) {
			ArgumentNullException.ThrowIfNull( IniData );
			ArgumentException.ThrowIfNullOrEmpty( name );

			if ( IniData.TryGetValue( name, out string? data ) ) {
				ArgumentException.ThrowIfNullOrEmpty( data );

				value = Convert.ToBoolean( data.ToInt() );
				return true;
			}
			value = false;
			return false;
		}

		/*
		===============
		LoadConfigValue
		===============
		*/
		/// <summary>
		/// Loads a configuration value in the form of a string
		/// </summary>
		/// <param name="name">The name of the key of the configuration value</param>
		/// <param name="value">The output value</param>
		public bool LoadConfigValue( string? name, out string value ) {
			ArgumentNullException.ThrowIfNull( IniData );
			ArgumentException.ThrowIfNullOrEmpty( name );

			if ( IniData.TryGetValue( name, out string? data ) ) {
				ArgumentException.ThrowIfNullOrEmpty( data );

				value = data;
				return true;
			}
			value = "";
			return false;
		}
	};
};