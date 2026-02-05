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
using Nomad.Core.Compatibility;
using Nomad.Core.FileSystem;
using Nomad.Core.Logger;

namespace Nomad.CVars.Private.Repositories {
	/*
	===================================================================================

	ConfigFileReader

	===================================================================================
	*/
	/// <summary>
	/// Loads configuration values from the provided .ini file
	/// </summary>

	internal readonly ref struct ConfigFileReader {
		private readonly IniLoader Loader;

		/*
		===============
		ConfigFileReader
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="fileSystem"></param>
		/// <param name="logger"></param>
		/// <param name="configFile"></param>
		public ConfigFileReader( IFileSystem fileSystem, ILoggerService logger, string configFile ) {
			ExceptionCompat.ThrowIfNullOrEmpty( configFile );

			logger.PrintLine( $"Loading configuration file {configFile}..." );

			Loader = new IniLoader( configFile, logger, fileSystem );
		}

		/*
		===============
		TryGetValue
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool TryGetValue( string name, out string value ) {
			ExceptionCompat.ThrowIfNullOrEmpty( name );

			return Loader.LoadConfigValue( name, out value );
		}
	};
};
