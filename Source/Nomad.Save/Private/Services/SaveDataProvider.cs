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
using System.Threading.Tasks;
using Nomad.Core.Events;
using Nomad.Core.FileSystem;
using Nomad.Core.Logger;
using Nomad.Save.Data;
using Nomad.Save.Events;
using Nomad.Save.Private.Exceptions;
using Nomad.Save.Services;
using Nomad.Save.ValueObjects;

namespace Nomad.Save.Private.Services {
	/*
	===================================================================================

	SaveDataProvider

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	public sealed class SaveDataProvider : ISaveDataProvider {
		private readonly List<SaveFileMetadata> _saveFiles;

		private readonly SaveWriterService _writerService;
		private readonly SaveReaderService _readerService;

		private readonly IFileSystem _vfs;

		private readonly ILoggerService _logger;
		private readonly ILoggerCategory _category;

		private readonly IGameEvent<SaveBeginEventArgs> _saveBegin;

		/*
		===============
		SaveDataProvider
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="eventFactory"></param>
		/// <param name="fileSystem"></param>
		/// <param name="logger"></param>
		public SaveDataProvider( IGameEventRegistryService eventFactory, IFileSystem fileSystem, ILoggerService logger ) {
			_saveBegin = eventFactory.GetEvent<SaveBeginEventArgs>( EventNames.NAMESPACE, EventNames.SAVE_BEGIN_EVENT );

			_vfs = fileSystem;
			_logger = logger;
			_category = logger.CreateCategory( "Nomad.Save", LogLevel.Info, true );

			_saveFiles = new List<SaveFileMetadata>();
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
		ListSaveFiles
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		public IReadOnlyList<SaveFileMetadata> ListSaveFiles( string saveDirectory ) {
			LoadMetadata( saveDirectory );
			return _saveFiles;
		}

		/*
		===============
		Load
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="fileId"></param>
		/// <returns></returns>
		public async Task Load( SaveFileId fileId ) {
			try {
				_readerService.Load( fileId.FileName );
			} catch ( FieldCorruptException fieldCorrupt ) {
				_logger.PrintError( $"Field corruption - {fieldCorrupt}: {fieldCorrupt.Error}" );
			} catch ( Exception e ) {
				_logger.PrintError( $"Exception caught - {e}" );
			}
		}

		/*
		===============
		Save
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="fileId"></param>
		/// <returns></returns>
		public async Task Save( SaveFileId fileId ) {
			try {
				_writerService.Save( fileId.FileName );

				_saveBegin.Publish( new SaveBeginEventArgs( _writerService ) );
			} catch ( Exception e ) {
				_logger.PrintError( $"Exception caught - {e}" );
			}
		}

		/*
		===============
		LoadMetadata
		===============
		*/
		/// <summary>
		///
		/// </summary>
		private void LoadMetadata( string saveDirectory ) {
			var files = System.IO.Directory.GetFiles( saveDirectory );
	
#if !NETSTANDARD2_1
			_saveFiles.EnsureCapacity( files.Length );
#endif

			for ( int i = 0; i < files.Length; i++ ) {
				var fileName = files[ i ];
				System.IO.FileInfo info = new System.IO.FileInfo( fileName );

				_saveFiles.Add( new SaveFileMetadata( new SaveFileId( fileName.Substring( fileName.LastIndexOf( System.IO.Path.PathSeparator ) ) ), info.Length, info.LastAccessTime ) );
			}
		}
	};
};
