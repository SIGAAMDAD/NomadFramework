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
using Nomad.Core.FileSystem;
using Nomad.Core.FileSystem.Streams;
using Nomad.Core.FileSystem.Configs;
using Nomad.Core.Logger;
using Nomad.Save.Interfaces;
using Nomad.Save.Private.Entities;
using Nomad.Save.Private.Repositories;
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

		private readonly SlotRepository _slotRepository;
		private readonly SaveConfig _config;

		private readonly IFileSystem _fileSystem;

		private readonly ILoggerService _logger;
		private readonly ILoggerCategory _category;

		private bool _isDisposed = false;

		/*
		===============
		SaveReaderService
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="config"></param>
		/// <param name="slotRepository"></param>
		/// <param name="fileSystem"></param>
		/// <param name="logger"></param>
		public SaveReaderService( SaveConfig config, SlotRepository slotRepository, IFileSystem fileSystem, ILoggerService logger ) {
			_slotRepository = slotRepository;
			_fileSystem = fileSystem;
			_sections = new ConcurrentDictionary<string, SaveSectionReader>();

			_config = config;

			_logger = logger;
			_category = _logger.CreateCategory( Constants.Logger.READER_SERVICE_CATEGORY_NAME, LogLevel.Info, true );
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
			if ( !_isDisposed ) {
				_category?.Dispose();
			}
			GC.SuppressFinalize( this );
			_isDisposed = true;
		}

		/*
		===============
		Load
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		void ISaveReaderService.Load( string name ) {
			var filepath = _slotRepository.AddSaveFile( name, false );

			_logger.PrintLine( in _category, $"Loading save data from {filepath}..." );

			using var reader = _fileSystem.OpenRead( new MemoryFileReadConfig { FilePath = filepath, MaxCapacity = 8 * 1024 * 1024 } ) as IMemoryReadStream;
			if ( reader == null ) {
				_logger.PrintError( in _category, $"SaveReaderService.Load: couldn't open save file '{filepath}'!" );
				return;
			}
			var header = SaveHeader.Deserialize( reader, out bool magicMatches );

			if ( _config.DebugLogging ) {
				_logger.PrintLine( in _category, "Got save file metadata:" );
				_logger.PrintLine( in _category, $"\tMagic: {Constants.HEADER_MAGIC}" );
				_logger.PrintLine( in _category, $"\tSectionCount: {header.SectionCount}" );
				_logger.PrintLine( in _category, $"\tSaveName: {header.Name}" );
				_logger.PrintLine( in _category, $"\tChecksum64: {header.Checksum.Value}" );
				_logger.PrintLine( in _category, $"\tGameVersion: {header.Version.ToInt()}" );
			}

			for ( int i = 0; i < header.SectionCount; i++ ) {
				var section = new SaveSectionReader( _config, i, reader, _logger );
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
