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
using Nomad.Save.Exceptions;
using Nomad.Save.Interfaces;
using Nomad.Save.Private.Entities;
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

		private IWriteStream? _writer;

		private readonly IFileSystem _fileSystem;

		private readonly ILoggerService _logger;
		private readonly ILoggerCategory _category;

		/*
		===============
		SaveWriterService
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="fileSystem"></param>
		/// <param name="logger"></param>
		/// <param name="category"></param>
		public SaveWriterService( IFileSystem fileSystem, ILoggerService logger, ILoggerCategory category ) {
			_fileSystem = fileSystem;
			_logger = logger;
			_category = category;

			_category = _logger.CreateCategory( "Nomad.Save.WriterService", LogLevel.Info, true );
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
			_writer?.Dispose();
		}

		/*
		===============
		Save
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="filepath"></param>
		/// <param name="gameVersion"></param>
		void ISaveWriterService.BeginSave( string filepath, GameVersion gameVersion ) {
			_writer = _fileSystem.OpenWrite( filepath );
			if ( _writer == null ) {
				_logger.PrintError( $"Couldn't create save file {filepath}!" );
				return;
			}

			_logger.PrintLine( $"Writing save data to {filepath}..." );

			{
				var header = new SaveHeader( gameVersion, _sections.Count, Checksum.Empty );
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
		/// <param name="gameVersion"></param>
		void ISaveWriterService.EndSave(GameVersion gameVersion) {
			int sectionCount = _sections.Count;
			foreach ( var section in _sections ) {
				section.Value.Dispose();
			}
			
			_writer.Seek( 0, System.IO.SeekOrigin.Begin );
			var header = new SaveHeader( gameVersion, sectionCount, Checksum.Empty );
			header.Serialize( _writer );

			_writer.Seek( 0, System.IO.SeekOrigin.End );

			_writer?.Dispose();
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
			if ( _sections.ContainsKey( sectionId ) ) {
				throw new DuplicateSectionException( $"Section {sectionId} added twice!" );
			}
			var writer = new SaveSectionWriter( sectionId, _writer );
			_logger.PrintLine( $"AddSection: creating save section {sectionId}..." );
			_sections[ sectionId ] = writer;
			return writer;
		}
	};
};
