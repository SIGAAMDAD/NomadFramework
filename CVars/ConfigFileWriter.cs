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

namespace CVars {
	/*
	===================================================================================
	
	ConfigFileWriter
	
	===================================================================================
	*/
	/// <summary>
	/// Writes CVars and CVarGroups to a configuration file, specifically in .ini format
	/// </summary>

	public readonly ref struct ConfigFileWriter {
		private readonly System.IO.StreamWriter? Writer = null;

		/*
		===============
		ConfigFileWriter
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="configFile"></param>
		/// <param name="groups"></param>
		public ConfigFileWriter( string? configFile, IConsoleService console, in HashSet<CVarGroup>? groups ) {
			ArgumentException.ThrowIfNullOrEmpty( configFile );
			ArgumentNullException.ThrowIfNull( groups );

			// make sure we aren't being handed garbage
			ArgumentOutOfRangeException.ThrowIfLessThanOrEqual( groups.Count, 0, "CVar Groups Count" );

			try {
				using System.IO.FileStream? fileStream = new System.IO.FileStream( configFile, System.IO.FileMode.CreateNew );
				Writer = new System.IO.StreamWriter( fileStream );

				foreach ( var group in groups ) {
					WriteGroup( group );
				}
			} catch ( Exception e ) {
				console.PrintError( $"...Error writing configuration file {configFile}: {e.Message}" );
			}
		}

		/*
		===============
		WriteVariable
		===============
		*/
		/// <summary>
		/// Writes a cvar's value to the configuration file
		/// </summary>
		/// <param name="cvar">The cvar to serialize</param>
		public readonly void WriteVariable( in ICVar? cvar ) {
			ArgumentNullException.ThrowIfNull( cvar );
			ArgumentNullException.ThrowIfNull( Writer );

			Writer.WriteLine( $"{cvar.Name}={cvar.GetStringValue()}" );
		}

		/*
		===============
		WriteGroup
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="group"></param>
		private void WriteGroup( in CVarGroup group ) {
		}
	};
};