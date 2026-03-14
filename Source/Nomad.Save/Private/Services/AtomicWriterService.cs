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
using Nomad.Core.FileSystem.Streams;
using Nomad.Core.FileSystem;
using Nomad.Core.Engine.Services;

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
		private readonly IEngineService _engineService;
		private readonly BackupService _backupService;

		/*
		===============
		AtomicWriterService
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="engineService"></param>
		/// <param name="fileSystem"></param>
		public AtomicWriterService( IEngineService engineService, IFileSystem fileSystem ) {
			_engineService = engineService;
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

				Console.WriteLine( $"Replacing '{memoryWriter.FilePath}' {_fileSystem.FileExists( memoryWriter.FilePath )} with '{fileName}' {_fileSystem.FileExists( fileName )}" );
				_fileSystem.ReplaceFile( memoryWriter.FilePath, fileName, null! );
			} catch {
				throw;
			} finally {
				if ( _fileSystem.FileExists( memoryWriter.FilePath ) ) {
					_fileSystem.DeleteFile( memoryWriter.FilePath );
				}
				Console.WriteLine( $"Replaced '{memoryWriter.FilePath}' {_fileSystem.FileExists( memoryWriter.FilePath )} with '{fileName}' {_fileSystem.FileExists( fileName )}" );
			}
		}
	};
};
