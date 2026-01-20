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
using System.IO;
using System.Threading.Tasks;
using Nomad.Core.Logger;
using Nomad.Core.OnlineServices;
using Steamworks;

namespace Nomad.OnlineServices.Steam.Private.Services {
	internal sealed class SteamCloudStorageService : ICloudStorageService {
		public bool SupportsCloudStorage => true;

		private readonly ILoggerService _logger;
		private readonly ILoggerCategory _category;

		private readonly CallResult<RemoteStorageLocalFileChange_t> _fileChangeResult;

		private bool IsEnabled => SteamRemoteStorage.IsCloudEnabledForApp() && SteamRemoteStorage.IsCloudEnabledForAccount();

		/*
		===============
		SteamCloudStorageService
		===============
		*/
		/// <summary>
		/// Creates a new SteamCloudStorageService instance.
		/// </summary>
		/// <param name="logger">The logger service to use for logging.</param>
		public SteamCloudStorageService( ILoggerService logger ) {
			_logger = logger;
			_category = _logger.CreateCategory( nameof( SteamCloudStorageService ), LogLevel.Info, true );

			_fileChangeResult = CallResult<RemoteStorageLocalFileChange_t>.Create( OnFileChange );
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
