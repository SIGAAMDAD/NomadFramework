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
using Nomad.Core.Compatibility.Guards;
using Nomad.Core.FileSystem;
using Nomad.Core.Logger;
using Nomad.Save.Exceptions;
using Nomad.Save.Interfaces;
using Nomad.Save.Private.Entities;
using Nomad.Save.Private.Repositories;
using Nomad.Save.Private.ValueObjects;
using Nomad.Save.ValueObjects;

namespace Nomad.Save.Private.Services {
	/*
	===================================================================================

	SaveWriterService

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	internal sealed class SaveWriterService : ISaveWriterService {
		public int SectionCount => _sections.Count;

		private readonly ConcurrentDictionary<string, SaveSectionWriter> _sections = new ConcurrentDictionary<string, SaveSectionWriter>();

		private IMemoryFileWriteStream? _writer;

		private readonly SaveConfig _config;

		private readonly SlotRepository _slotRepository;
		private readonly IFileSystem _fileSystem;

		private readonly ILoggerService _logger;
		private readonly ILoggerCategory _category;

		private readonly AtomicWriterService _atomicWriter;

		/*
		===============
		SaveWriterService
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="config"></param>
		/// <param name="slotRepository"></param>
		/// <param name="fileSystem"></param>
		/// <param name="logger"></param>
		public SaveWriterService( in SaveConfig config, SlotRepository slotRepository, IFileSystem fileSystem, ILoggerService logger ) {
			_slotRepository = slotRepository;
			_fileSystem = fileSystem;

			_config = config;

			_logger = logger;
			_category = _logger.CreateCategory( Constants.Logger.WRITER_SERVICE_CATEGORY_NAME, LogLevel.Info, true );

			_atomicWriter = new AtomicWriterService( fileSystem );
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
		Save
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="gameVersion"></param>
		void ISaveWriterService.BeginSave( string name, GameVersion gameVersion ) {
			var filepath = _slotRepository.AddSaveFile( name, false );

			_writer = _fileSystem.OpenWrite( AtomicWriterService.GetAtomicPathName(), new WriteConfig( StreamType.MemoryFile ) ) as IMemoryFileWriteStream;
			if ( _writer == null ) {
				_logger.PrintError( in _category, $"Couldn't create save file {filepath}!" );
				return;
			}

			_logger.PrintLine( in _category, $"Writing save data to {filepath}..." );

			{
				var header = new SaveHeader( name, gameVersion, _sections.Count, Checksum.Empty );
				header.Serialize( _writer );
			}
		}

		/*
		===============
		EndSave
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="gameVersion"></param>
		void ISaveWriterService.EndSave( string name, GameVersion gameVersion ) {
			ArgumentGuard.ThrowIfNull( _writer );

			int sectionCount = _sections.Count;
			foreach ( var section in _sections ) {
				section.Value.Dispose();
			}

			var checksum = Checksum.Compute( _writer.Buffer.Buffer );

			_writer.Seek( 0, System.IO.SeekOrigin.Begin );
			var header = new SaveHeader( name, gameVersion, sectionCount, checksum );
			header.Serialize( _writer );

			_writer.Seek( 0, System.IO.SeekOrigin.End );

			if ( _config.DebugLogging ) {
				_logger.PrintLine( in _category, $"Finalized save data file:" );
				_logger.PrintLine( in _category, $"\tMagic: {Constants.HEADER_MAGIC}" );
				_logger.PrintLine( in _category, $"\tSectionCount: {header.SectionCount}" );
				_logger.PrintLine( in _category, $"\tSaveName: {header.Name}" );
				_logger.PrintLine( in _category, $"\tChecksum64: {header.Checksum.Value}" );
				_logger.PrintLine( in _category, $"\tGameVersion: {header.Version.ToInt()}" );
			}

			// clear the memory buffer
			var filepath = _slotRepository.AddSaveFile( name, false );
			_atomicWriter.FinalizeSaveData( filepath, _writer );

			_sections.Clear();
		}

		/*
		===============
		AddSection
		===============
		*/
		/// <summary>
		/// Creates a new section with the id of <paramref name="sectionId"/> and returns it.
		/// </summary>
		/// <param name="sectionId">The name of the section to add.</param>
		/// <returns>The new write-dedicated section.</returns>
		/// <exception cref="DuplicateSectionException">Thrown if <paramref name="sectionId"/> already exists in the section cache.</exception>
		public ISaveSectionWriter AddSection( string sectionId ) {
			ArgumentGuard.ThrowIfNull( _writer );

			if ( _sections.ContainsKey( sectionId ) ) {
				throw new DuplicateSectionException( $"Section {sectionId} added twice!" );
			}
			var writer = new SaveSectionWriter( in _config, _logger, _category, sectionId, _writer );
			_sections[ sectionId ] = writer;
			
			if ( _config.LogSerializationTree ) {
				_logger.PrintLine( in _category, $"\t[Section] (NAME) {sectionId}" );
			}

			return writer;
		}
	};
};
