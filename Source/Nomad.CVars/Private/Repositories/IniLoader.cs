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
using System.IO;
using Microsoft.Extensions.Configuration.Ini;
using Nomad.Core.Compatibility;
using Nomad.Core.FileSystem;
using Nomad.Core.Logger;

namespace Nomad.CVars.Private.Repositories {
	/*
	===================================================================================

	IniLoader

	===================================================================================
	*/
	/// <summary>
	/// Loads key-value pairs from .ini files, specifically configuration files
	/// </summary>

	internal readonly ref struct IniLoader {
		private readonly IDictionary<string, string?>? _iniData;

		/*
		===============
		IniLoader
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="configFile"></param>
		/// <param name="logger"></param>
		public IniLoader( string configFile, ILoggerService logger, IFileSystem fileSystem ) {
			ExceptionCompat.ThrowIfNullOrEmpty( configFile );
			ExceptionCompat.ThrowIfNull( logger );

			_iniData = null;

			try {
				using FileStream fileStream = new( $"{fileSystem.GetConfigPath()}/{configFile}", FileMode.Open, FileAccess.Read );

				_iniData = IniStreamConfigurationProvider.Read( fileStream );
				if ( _iniData == null ) {
					logger.PrintError( $"IniLoader: error parsing ini data from configuration file '{configFile}'" );
				}
			} catch ( Exception e ) {
				logger.PrintError( $"IniLoader: Error opening configuration file '{configFile}: {e.Message}" );
			}
		}

		/*
		===============
		LoadConfigValue
		===============
		*/
		/// <summary>
		/// Loads a configuration value in the form of an integer.
		/// </summary>
		/// <param name="name">The name of the key of the configuration value</param>
		/// <param name="value">The output value</param>
		public bool LoadConfigValue( string name, out int value ) {
			ExceptionCompat.ThrowIfNull( _iniData );
			ExceptionCompat.ThrowIfNullOrEmpty( name );

			if ( _iniData.TryGetValue( name, out string? data ) ) {
				ExceptionCompat.ThrowIfNullOrEmpty( data );
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
		public bool LoadConfigValue( string name, out float value ) {
			ExceptionCompat.ThrowIfNull( _iniData );
			ExceptionCompat.ThrowIfNullOrEmpty( name );

			if ( _iniData.TryGetValue( name, out string? data ) ) {
				ExceptionCompat.ThrowIfNullOrEmpty( data );
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
		public bool LoadConfigValue( string name, out bool value ) {
			ExceptionCompat.ThrowIfNull( _iniData );
			ExceptionCompat.ThrowIfNullOrEmpty( name );

			if ( _iniData.TryGetValue( name, out string? data ) ) {
				ExceptionCompat.ThrowIfNullOrEmpty( data );
				value = data.Trim().ToLower() switch {
					"1" or "true" or "yes" or "on" => true,
					"0" or "false" or "no" or "off" => false,
					_ => bool.TryParse( data, out bool result ) && result
				};
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
		public bool LoadConfigValue( string name, out string value ) {
			ExceptionCompat.ThrowIfNull( _iniData );
			ExceptionCompat.ThrowIfNullOrEmpty( name );

			if ( _iniData.TryGetValue( name, out string? data ) ) {
				ExceptionCompat.ThrowIfNullOrEmpty( data );
				value = data;
				return true;
			}
			value = String.Empty;
			return false;
		}
	};
};
