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
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Nomad.Core.FileSystem;
using Nomad.Core.Logger;
using Nomad.Core.Memory.Buffers;
using Nomad.Core.OnlineServices;
using Steamworks;

namespace Nomad.OnlineServices.Steam.Private.Services {
	/*
	===================================================================================
	
	SteamCloudStorageService
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>

	internal sealed class SteamCloudStorageService : ICloudStorageService {
		private static bool IsEnabled => SteamRemoteStorage.IsCloudEnabledForApp() && SteamRemoteStorage.IsCloudEnabledForAccount();

		public bool SupportsCloudStorage => true;

		private struct CloudFile {
			public int Size { get; set; }
			public DateTime CloudAccessTime { get; set; }
			public DateTime LocalAccessTime { get; set; }
		};

		private readonly Dictionary<string, CloudFile> _cloudFiles = new Dictionary<string, CloudFile>();

		private readonly IFileSystem _fileSystem;

		private readonly ILoggerService _logger;
		private readonly ILoggerCategory _category;

		private readonly CallResult<RemoteStorageLocalFileChange_t> _fileChangeResult;
		private readonly CallResult<RemoteStorageFileWriteAsyncComplete_t> _fileWriteAsyncComplete;
		private readonly CallResult<RemoteStorageFileReadAsyncComplete_t> _fileReadAsyncComplete;

		private bool _isDisposed = false;

		/*
		===============
		SteamCloudStorageService
		===============
		*/
		/// <summary>
		/// Creates a new SteamCloudStorageService instance.
		/// </summary>
		/// <param name="logger">The logger service to use for logging.</param>
		/// <param name="fileSystem"></param>
		public SteamCloudStorageService( ILoggerService logger, IFileSystem fileSystem ) {
			_logger = logger;
			_category = logger.CreateCategory( nameof( SteamCloudStorageService ), LogLevel.Info, true );

			_fileSystem = fileSystem;

			_fileChangeResult = CallResult<RemoteStorageLocalFileChange_t>.Create( OnFileChange );
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
				_fileChangeResult?.Dispose();
				_category?.Dispose();
			}
			GC.SuppressFinalize( this );
			_isDisposed = true;
		}

		/*
		===============
		OnFileChange
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="param"></param>
		/// <param name="bIOFailure"></param>
		private void OnFileChange( RemoteStorageLocalFileChange_t param, bool bIOFailure ) {
		}

		/*
		===============
		InitializeCloudFileCache
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		private void InitializeCloudFileCache() {
			int fileCount = SteamRemoteStorage.GetFileCount();

			for ( int i = 0; i < fileCount; i++ ) {
				string fileName = SteamRemoteStorage.GetFileNameAndSize( i, out int fileSize );

				DateTime cloudAccessTimestamp = DateTime.FromFileTimeUtc( SteamRemoteStorage.GetFileTimestamp( fileName ) );
				FileInfo info = new FileInfo( fileName );

				_cloudFiles.Add( fileName, new CloudFile { CloudAccessTime = cloudAccessTimestamp, LocalAccessTime = info.LastAccessTimeUtc, Size = fileSize } );
			}
		}

		/*
		===============
		FileExists
		===============
		*/
		/// <summary>
		/// Checks if a file exists in cloud storage.
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public async ValueTask<bool> FileExists( string fileName ) {
			return SteamRemoteStorage.FileExists( fileName );
		}

		/*
		===============
		ReadFile
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public async ValueTask<IBufferHandle?> ReadFile( string fileName ) {
			int fileSize = SteamRemoteStorage.GetFileSize( fileName );

			byte[] fileBuffer = new byte[fileSize];
			int readBytes = SteamRemoteStorage.FileRead( fileName, fileBuffer, fileSize );
			IBufferHandle buffer = new SharedBufferHandle( fileBuffer, fileSize );

			return buffer;
		}

		/*
		===============
		ResolveConflict
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="localData"></param>
		/// <param name="cloudData"></param>
		/// <returns></returns>
		public async ValueTask ResolveConflict( string fileName, IBufferHandle localData, IBufferHandle cloudData ) {
		}

		/*
		===============
		Synchronize
		===============
		*/
		/// <summary>
		/// Synchronizes local files with cloud storage.
		/// </summary>
		/// <returns></returns>
		public async ValueTask Synchronize() {
			if ( !IsEnabled ) {
				_logger.PrintWarning( in _category, "Cloud storage is not enabled for this application." );
				return;
			}
		}

		/*
		===============
		WriteFile
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public async ValueTask WriteFile( string fileName ) {
		}
	};
};
