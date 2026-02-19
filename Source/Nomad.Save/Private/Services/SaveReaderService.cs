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

using System.Collections.Concurrent;
using Nomad.Core.FileSystem;
using Nomad.Core.Logger;
using Nomad.Save.Interfaces;
using Nomad.Save.Private.Entities;
using Nomad.Save.Private.ValueObjects;

namespace Nomad.Save.Private.Services {
	/*
	===================================================================================

	SaveReaderService

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	internal sealed class SaveReaderService : ISaveReaderService {
		public int SectionCount => _sections.Count;
		private readonly ConcurrentDictionary<string, SaveSectionReader> _sections;

		private readonly IFileSystem _fileSystem;

		private readonly ILoggerService _logger;
		private readonly ILoggerCategory _category;

		/*
		===============
		SaveReaderService
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="fileSystem"></param>
		/// <param name="logger"></param>
		public SaveReaderService( IFileSystem fileSystem, ILoggerService logger ) {
			_fileSystem = fileSystem;
			_logger = logger;
			_sections = new ConcurrentDictionary<string, SaveSectionReader>();

			_category = _logger.CreateCategory( "Nomad.Save.ReaderService", LogLevel.Info, true );
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
			_sections.Clear();
		}

		/*
		===============
		Load
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="filepath"></param>
		void ISaveReaderService.Load( string filepath ) {
			_logger.PrintLine( in _category, $"Loading save data..." );

			using var reader = _fileSystem.OpenRead( filepath, new ReadConfig( StreamType.MemoryFile ) );
			if ( reader == null ) {
				_logger.PrintError( in _category, $"SaveReaderService.Load: couldn't open save file '{filepath}'!" );
				return;
			}
			var header = SaveHeader.Deserialize( reader, out bool magicMatches );

			_logger.PrintLine( in _category, $"...Section Count: {header.SectionCount}" );
			_logger.PrintLine( in _category, $"...Version: {header.Version}" );

			for ( int i = 0; i < header.SectionCount; i++ ) {
				var section = new SaveSectionReader( in reader, _logger );
				_sections[ section.Name ] = section;
			}

			_logger.PrintLine( in _category, "...Finished loading save data" );
		}

		/*
		===============
		FindSection
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sectionName"></param>
		/// <returns></returns>
		public ISaveSectionReader? FindSection( string sectionName ) {
			if ( !_sections.TryGetValue( sectionName, out var section ) ) {
				_logger.PrintError( in _category, $"SaveReaderService.FindSection: section '{sectionName}' not found!" );
				return null;
			}
			return section;
		}
	};
};
