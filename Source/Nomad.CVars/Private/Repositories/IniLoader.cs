/*
===========================================================================
The Nomad Framework
Copyright (C) 2025 Noah Van Til

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
using Nomad.Core.Logger;
using Nomad.Core.Util;

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
		/// <param name="logger"></param>
		public IniLoader( string configFile, ILoggerService logger ) {
			ArgumentException.ThrowIfNullOrEmpty( configFile );
			ArgumentNullException.ThrowIfNull( logger );

			try {
				using FileStream fileStream = new( FilePath.FromResourcePath( configFile ).OSPath, FileMode.Open, FileAccess.Read );

				IniData = IniStreamConfigurationProvider.Read( fileStream );
				if ( IniData == null ) {
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
		public bool LoadConfigValue( string name, out float value ) {
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
		public bool LoadConfigValue( string name, out bool value ) {
			ArgumentNullException.ThrowIfNull( IniData );
			ArgumentException.ThrowIfNullOrEmpty( name );

			if ( IniData.TryGetValue( name, out string? data ) ) {
				ArgumentException.ThrowIfNullOrEmpty( data );
				value = data.Trim() switch {
					"1" or "true" or "True" or "TRUE" or "yes" or "on" => true,
					"0" or "false" or "False" or "FALSE" or "no" or "off" => false,
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
			ArgumentNullException.ThrowIfNull( IniData );
			ArgumentException.ThrowIfNullOrEmpty( name );

			if ( IniData.TryGetValue( name, out string? data ) ) {
				ArgumentException.ThrowIfNullOrEmpty( data );
				value = data;
				return true;
			}
			value = String.Empty;
			return false;
		}
	};
};
