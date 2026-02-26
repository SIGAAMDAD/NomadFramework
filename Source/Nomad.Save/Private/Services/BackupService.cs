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
using System.IO;
using Nomad.Core.Exceptions;
using Nomad.Core.FileSystem;
using Nomad.Save.Private.Entities;
using Nomad.Save.Private.ValueObjects;

namespace Nomad.Save.Private.Services {
	/*
	===================================================================================
	
	BackupService
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	/// <remarks>
	/// Every time a new save file is written to, a backup is created of that save file. Which is just a save file with extra header information
	/// marking how old it is, what API version it was saved to, and what order it is in the backup chain. If the maximum number of backups is reached
	/// when creating a new backup, the oldest backup is deleted.
	/// </remarks>

	internal sealed class BackupService : IDisposable {
		private readonly List<BackupData> _backups = new List<BackupData>();
		private readonly SaveConfig _config;

		private readonly IFileSystem _fileSystem;

		private bool _isDisposed = false;

		/*
		===============
		BackupService
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="config"></param>
		/// <param name="fileSystem"></param>
		/// <exception cref="CVarMissing"></exception>
		public BackupService( SaveConfig config, IFileSystem fileSystem ) {
			_config = config;
			_fileSystem = fileSystem;
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
			// DISPOSAL GOES HERE
			GC.SuppressFinalize( this );
			_isDisposed = true;
		}

		/*
		===============
		CreateBackup
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public void CreateBackup( string fileName ) {
			string path = Path.Combine( _config.BackupPath, $"{fileName}.ngd.backup" );

			try {
				_fileSystem.CopyFile( fileName, path, true );
			} catch {
			}
		}
	};
};