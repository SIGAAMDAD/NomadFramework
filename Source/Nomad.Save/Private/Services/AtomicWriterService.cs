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
using System.IO;
using System.Runtime.CompilerServices;
using Nomad.Core.FileSystem;

namespace Nomad.Save.Private.Services {
	/*
	===================================================================================
	
	AtomicWriterService
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>

	internal sealed class AtomicWriterService {
		private readonly IFileSystem _fileSystem;
		private readonly BackupService _backupService;

		/*
		===============
		AtomicWriterService
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="fileSystem"></param>
		public AtomicWriterService( IFileSystem fileSystem ) {
			_fileSystem = fileSystem;
		}

		/*
		===============
		GetAtomicPathName
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static string GetAtomicPathName()
			=> Path.Combine( Path.GetTempPath(), Guid.NewGuid().ToString() + ".tmp" );

		/*
		===============
		FinalizeSaveData
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="memoryWriter"></param>
		public void FinalizeSaveData( string fileName, IMemoryFileWriteStream memoryWriter ) {
			try {
				memoryWriter.Dispose();
				File.Move( memoryWriter.FilePath, fileName, true );
			} catch {
				_fileSystem.DeleteFile( memoryWriter.FilePath );
				throw;
			}
		}
	};
};