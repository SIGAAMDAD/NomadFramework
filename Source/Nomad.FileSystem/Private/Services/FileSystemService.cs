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
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Nomad.Core.Compatibility;
using Nomad.Core.EngineUtils;
using Nomad.Core.FileSystem;
using Nomad.Core.Logger;
using Nomad.FileSystem.Private.FileStream;

namespace Nomad.FileSystem.Private.Services {
	/*
	===================================================================================
	
	FileSystem
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>

	public sealed class FileSystemService : IFileSystem {
		private readonly IEngineService _engineService;
		private readonly RecursiveFileSearcher _searchHelper;

		private readonly ILoggerService _logger;
		private readonly ILoggerCategory _category;

		/*
		===============
		FileSystemService
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="engineService"></param>
		public FileSystemService( IEngineService engineService, ILoggerService logger ) {
			ExceptionCompat.ThrowIfNull( engineService );
			ExceptionCompat.ThrowIfNull( logger );
			
			_engineService = engineService;

			_logger = logger;
			_category = logger.CreateCategory( "Nomad.FileSystem", LogLevel.Info, true );

			_searchHelper = new RecursiveFileSearcher();
			_searchHelper.AddSearchDirectory( engineService.GetStoragePath( StorageScope.StreamingAssets ) );
			_searchHelper.AddSearchDirectory( engineService.GetStoragePath( StorageScope.UserData ) );
			_searchHelper.AddSearchDirectory( engineService.GetStoragePath( StorageScope.Install ) );
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
		CopyFile
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sourcePath"></param>
		/// <param name="destinationPath"></param>
		/// <param name="overwrite"></param>
		/// <exception cref="NotImplementedException"></exception>
		public void CopyFile( string sourcePath, string destinationPath, bool overwrite ) {
			System.IO.File.Copy( sourcePath, destinationPath, overwrite );
		}

		/*
		===============
		CreateDirectory
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="path"></param>
		public void CreateDirectory( string path ) {
			System.IO.Directory.CreateDirectory( path );
		}

		/*
		===============
		DirectoryExists
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public bool DirectoryExists( string path ) {
			return !string.IsNullOrEmpty( path ) && System.IO.Directory.Exists( path );
		}

		/*
		===============
		DeleteDirectory
		===============
		*/
		/// <summary>
		/// Deletes a directory at the specified path.
		/// </summary>
		/// <param name="path"></param>
		/// <param name="recursive"></param>
		public void DeleteDirectory( string path, bool recursive ) {
			System.IO.Directory.Delete( path, recursive );
		}

		/*
		===============
		DeleteFile
		===============
		*/
		/// <summary>
		/// Deletes a file at the specified path.
		/// </summary>
		/// <param name="path">The path of the file to delete.</param>
		public void DeleteFile( string path ) {
			System.IO.File.Delete( path );
		}
		
		/*
		===============
		FileExists
		===============
		*/
		/// <summary>
		/// Checks if a file exists at the specified path.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public bool FileExists( string path ) {
			return System.IO.File.Exists( path );
		}

		/*
		===============
		GetConfigPath
		===============
		*/
		/// <summary>
		/// Gets the configuration path.
		/// </summary>
		/// <returns></returns>
		public string GetConfigPath() {
			return _engineService.GetStoragePath( "Config", StorageScope.UserData );
		}

		/*
		===============
		GetResourcePath
		===============
		*/
		/// <summary>
		/// Gets the resource path.
		/// </summary>
		/// <returns>The resource path.</returns>
		public string GetResourcePath() {
			return _engineService.GetStoragePath( StorageScope.StreamingAssets );
		}

		/*
		===============
		GetSavePath
		===============
		*/
		/// <summary>
		/// Gets the save directory.
		/// </summary>
		/// <returns>The save directory path.</returns>
		public string GetSavePath() {
			return _engineService.GetStoragePath( "SaveData", StorageScope.UserData );
		}

		/*
		===============
		GetUserDataPath
		===============
		*/
		/// <summary>
		/// Gets the user data directory.
		/// </summary>
		/// <returns>The user data directory path.</returns>
		public string GetUserDataPath() {
			return _engineService.GetStoragePath( StorageScope.UserData );
		}

		/*
		===============
		GetDirectories
		===============
		*/
		/// <summary>
		/// Gets the directories at the specified path.
		/// </summary>
		/// <param name="path">The path to get directories from.</param>
		/// <returns>A list of directory paths.</returns>
		public IReadOnlyList<string> GetDirectories( string path ) {
			return System.IO.Directory.GetDirectories( path );
		}

		/*
		===============
		GetFiles
		===============
		*/
		/// <summary>
		/// Gets the files at the specified path.
		/// </summary>
		/// <param name="path">The path to get files from.</param>
		/// <param name="searchPattern">The search pattern to use.</param>
		/// <param name="recursive">Whether to search recursively.</param>
		/// <returns>A list of file paths.</returns>
		public IReadOnlyList<string> GetFiles( string path, string searchPattern, bool recursive ) {
			return System.IO.Directory.GetFiles( path, searchPattern, recursive ? System.IO.SearchOption.AllDirectories : System.IO.SearchOption.TopDirectoryOnly );
		}

		/*
		===============
		GetFileSize
		===============
		*/
		/// <summary>
		/// Gets the size of the file at the specified path.
		/// </summary>
		/// <param name="path">The path of the file to get the size of.</param>
		/// <returns>The size of the file in bytes.</returns>
		public long GetFileSize( string path ) {
			var info = new System.IO.FileInfo( path );
			return info.Length;
		}

		/*
		===============
		GetLastWriteTime
		===============
		*/
		/// <summary>
		/// Gets the last write time of the file at the specified path.
		/// </summary>
		/// <param name="path">The path of the file to get the last write time of.</param>
		/// <returns>The last write time of the file.</returns>
		public DateTime GetLastWriteTime( string path ) {
			var info = new System.IO.FileInfo( path );
			return info.LastWriteTime;
		}

		/*
		===============
		MoveFile
		===============
		*/
		/// <summary>
		/// Moves a file from the source path to the destination path.
		/// </summary>
		/// <param name="sourcePath">The path of the file to move.</param>
		/// <param name="destinationPath">The path to move the file to.</param>
		public void MoveFile( string sourcePath, string destinationPath ) {
			System.IO.File.Move( sourcePath, destinationPath );
		}

		/*
		===============
		OpenRead
		===============
		*/
		/// <summary>
		/// Opens a read stream for the specified file path.
		/// </summary>
		/// <param name="path">The path of the file to open.</param>
		/// <returns>The read stream for the specified file path.</returns>
		public IReadStream? OpenRead( string path ) {
			var fullPath = _searchHelper.FindFile( path, System.IO.SearchOption.AllDirectories );
			if ( fullPath == null ) {
				return null;
			}

			return new FileReadStream( fullPath );
		}

		/*
		===============
		OpenReadAsync
		===============
		*/
		/// <summary>
		/// Opens a read stream for the specified file path asynchronously.
		/// </summary>
		/// <param name="path">The path of the file to open.</param>
		/// <param name="ct">The cancellation token.</param>
		/// <returns>The read stream for the specified file path.</returns>
		public async ValueTask<IReadStream?> OpenReadAsync( string path, CancellationToken ct = default ) {
			ct.ThrowIfCancellationRequested();
			
			var fullPath = _searchHelper.FindFile( path, System.IO.SearchOption.AllDirectories );
			if ( fullPath == null ) {
				return null;
			}

			return new FileReadStream( fullPath );
		}

		/*
		===============
		OpenWrite
		===============
		*/
		/// <summary>
		/// Opens a write stream for the specified file path.
		/// </summary>
		/// <param name="path">The path of the file to open.</param>
		/// <param name="append">Whether to append to the file or overwrite it.</param>
		/// <returns>The write stream for the specified file path.</returns>
		public IWriteStream? OpenWrite( string path, bool append = false ) {
			return new FileWriteStream( path, append );
		}

		/*
		===============
		OpenWriteAsync
		===============
		*/
		/// <summary>
		/// Opens a write stream for the specified file path asynchronously.
		/// </summary>
		/// <param name="path">The path of the file to open.</param>
		/// <param name="append">Whether to append to the file or overwrite it.</param>
		/// <param name="ct">The cancellation token.</param>
		/// <returns>The write stream for the specified file path.</returns>
		public async ValueTask<IWriteStream?> OpenWriteAsync( string path, bool append = false, CancellationToken ct = default ) {
			ct.ThrowIfCancellationRequested();

			return new FileWriteStream( path, append );
		}

		/*
		===============
		CreateStream
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="accessMode"></param>
		/// <param name="outputFile"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		/// <exception cref="NotImplementedException"></exception>
		public IDataStream CreateStream( System.IO.FileAccess accessMode, string outputFile = "", int length = 0 ) {
			throw new NotImplementedException();
		}
	};
};