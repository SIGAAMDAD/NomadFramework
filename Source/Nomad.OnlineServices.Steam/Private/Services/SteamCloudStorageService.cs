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

		private record CloudFile(
			int Size,
			DateTime CloudAccessTime,
			DateTime LocalAccessTime
		);

		private readonly Dictionary<string, CloudFile> _cloudFiles = new Dictionary<string, CloudFile>();

		private readonly IFileSystem _fileSystem;

		private readonly ILoggerService _logger;
		private readonly ILoggerCategory _category;

		private readonly CallResult<RemoteStorageLocalFileChange_t> _fileChangeResult;
		private readonly CallResult<RemoteStorageFileWriteAsyncComplete_t> _fileWriteAsyncComplete;
		private readonly CallResult<RemoteStorageFileReadAsyncComplete_t> _fileReadAsyncComplete;

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
			_category = _logger.CreateCategory( nameof( SteamCloudStorageService ), LogLevel.Info, true );

			_fileSystem = fileSystem;

			_fileChangeResult = CallResult<RemoteStorageLocalFileChange_t>.Create( OnFileChange );

			RefreshCloudFiles();
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
			_category?.Dispose();
			_fileChangeResult?.Dispose();
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
		public async ValueTask<byte[]> ReadFile( string fileName ) {
			int fileSize = SteamRemoteStorage.GetFileSize( fileName );
			byte[] fileBuffer = ArrayPool<byte>.Shared.Rent( fileSize );
			SteamRemoteStorage.FileRead( fileName, fileBuffer, fileSize );

			return fileBuffer;
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
		public async ValueTask ResolveConflict( string fileName, byte[] localData, byte[] cloudData ) {
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

			int cloudFileCount = SteamRemoteStorage.GetFileCount();
			if ( _cloudFiles.Count != cloudFileCount ) {
				// we need to retrieve some files
				RefreshCloudFiles();
			}

			for ( int i = 0; i < fileCount; i++ ) {
				string name = SteamRemoteStorage.GetFileNameAndSize( i, out int fileSize );

				IReadStream? stream = _fileSystem.OpenRead( name );
				if ( stream == null ) {
					_logger.PrintWarning( in _category, $"SteamCloudStorageService.Synchronize: there is a cloud file that does not exist on the user's machine ('{name}'), starting download..." );
					RetrieveCloudFile( name );
					continue;
				}

				long timeStamp = SteamRemoteStorage.GetFileTimestamp( name );
				FileInfo info = new FileInfo( name );

				_logger.PrintLine( in _category, $"SteamCloudStorageService.Synchronize" );
				_cloudFiles.Add(
					new CloudFile(
						Name: name,
						Size: fileSize,
						CloudAccessTime: DateTimeOffset.FromUnixTimeMilliseconds( timeStamp ).UtcDateTime,
						LocalAccessTime: info.LastAccessTime
					)
				);
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

		/*
		===============
		RefreshCloudFiles
		===============
		*/
		/// <summary>
		/// Updates the current cloud-files list.
		/// </summary>
		private void RefreshCloudFiles() {
			int cloudFileCount = SteamRemoteStorage.GetFileCount();

			_cloudFiles.Clear();
			_cloudFiles.EnsureCapacity( cloudFileCount );

			for ( int i = 0; i < cloudFileCount; i++ ) {
				string name = SteamRemoteStorage.GetFileNameAndSize( i, out int fileSize );

				long timeStamp = SteamRemoteStorage.GetFileTimestamp( name );
				FileInfo info = new FileInfo( name );

				_logger.PrintLine( in _category, $"SteamCloudStorageService.Synchronize" );
				_cloudFiles.Add(
					name,
					new CloudFile(
						Size: fileSize,
						CloudAccessTime: DateTimeOffset.FromUnixTimeMilliseconds( timeStamp ).UtcDateTime,
						LocalAccessTime: info.LastAccessTime
					)
				);
			}
		}

		/*
		===============
		RetrieveCloudFile
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="fileName"></param>
		private void RetrieveCloudFile( string fileName ) {
			int fileSize = SteamRemoteStorage.GetFileSize( fileName );
			SteamRemoteStorage.FileReadAsync( fileName, 0, (uint)fileSize );
		}
	};
};
