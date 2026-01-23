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
using System.Collections.Concurrent;
using Nomad.Core.Logger;
using Nomad.Save.Interfaces;
using Nomad.Save.Private.Entities;
using Nomad.Save.Private.Serialization.Streams;
using Nomad.Save.Private.ValueObjects;

namespace Nomad.Save.Private.Services {
	/*
	===================================================================================

	SaveWriterService

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	internal sealed class SaveWriterService( string filepath, ILoggerService logger ) : ISaveWriterService {
		public int SectionCount => _sections.Count;

		private readonly ConcurrentDictionary<string, SaveSectionWriter> _sections = new ConcurrentDictionary<string, SaveSectionWriter>();
		private readonly SaveStreamWriter _writer = new SaveStreamWriter( filepath, logger );

		/*
		===============
		Dispose
		===============
		*/
		/// <summary>
		///
		/// </summary>
		public void Dispose() {
			logger.PrintLine( $"Writing save data to {filepath}..." );

			var header = new SaveHeader( new GameVersion( 2, 0, 1 ), _sections.Count, Checksum.Empty );
			header.Serialize( _writer );

			foreach ( var section in _sections ) {
				section.Value.Dispose();
			}

			_writer.Dispose();
		}

		/*
		===============
		AddSection
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="sectionId"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public ISaveSectionWriter AddSection( string sectionId ) {
			if ( _sections.ContainsKey( sectionId ) ) {
				throw new Exception( $"Section {sectionId} added twice!" );
			}
			var writer = new SaveSectionWriter( sectionId, _writer );
			_sections[ sectionId ] = writer;
			return writer;
		}
	};
};
