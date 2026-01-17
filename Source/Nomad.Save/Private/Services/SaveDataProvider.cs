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

using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using Nomad.Core.Events;
using Nomad.Core.Logger;
using Nomad.Core.Util;
using Nomad.Save.Data;
using Nomad.Save.Events;
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

	internal sealed class SaveDataProvider( IGameEventRegistryService eventFactory, ILoggerService logger ) : ISaveDataProvider {
		private readonly List<SaveFileMetadata> _saveFiles = new();

		private readonly IGameEvent<SaveBeginEventArgs> _saveBegin = eventFactory.GetEvent<SaveBeginEventArgs>( EventNames.NAMESPACE, EventNames.SAVE_BEGIN_EVENT );

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
		public async Task<IReadOnlyList<SaveFileMetadata>> ListSaveFiles() {
			LoadMetadata();
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
			using var writer = new SaveWriterService( fileId.FileName, logger );

			_saveBegin.Publish( new SaveBeginEventArgs( writer ) );
		}

		/*
		===============
		LoadMetadata
		===============
		*/
		/// <summary>
		///
		/// </summary>
		private void LoadMetadata() {
			var files = System.IO.Directory.GetFiles( FilePath.FromResourcePath( "user://SaveData" ).OSPath );
			_saveFiles.EnsureCapacity( files.Length );

			for ( int i = 0; i < files.Length; i++ ) {
				var fileName = files[ i ];
				System.IO.FileInfo info = new System.IO.FileInfo( fileName );

				_saveFiles.Add( new SaveFileMetadata( new SaveFileId( fileName.GetBaseName() ), info.Length, info.LastAccessTime ) );
			}
		}
	};
};
