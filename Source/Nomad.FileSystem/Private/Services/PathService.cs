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
using System.Text;
using Nomad.Core.EngineUtils;

namespace Nomad.FileSystem.Private.Services {
	/*
	===================================================================================

	PathService

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	internal sealed class PathService : IDisposable {
		private readonly List<string> _paths;
		private readonly IEngineService _engineService;

		/*
		===============
		PathService
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="engineService"></param>
		public PathService( IEngineService engineService ) {
			_engineService = engineService;
			_paths = new List<string>() {
				engineService.GetStoragePath( StorageScope.Install ),
				engineService.GetStoragePath( StorageScope.StreamingAssets ),
				engineService.GetStoragePath( StorageScope.UserData )
			};
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
		FindFile
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="localPath"></param>
		/// <returns></returns>
		public string? FindFile( string localPath ) {
			var pathBuilder = new StringBuilder( 4096 );

			for ( int i = 0; i < _paths.Count; i++ ) {
				pathBuilder.Clear();
				pathBuilder.AppendFormat( $"{_paths[ i ]}{localPath}" );

				string path = pathBuilder.ToString();
				if ( System.IO.File.Exists( path ) ) {
					return path;
				}
			}

			return null;
		}

		/*
		===============
		FindDirectories
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="directory"></param>
		/// <param name="paths"></param>
		private void FindDirectories( string directory, List<string> paths ) {
			var directories = System.IO.Directory.GetDirectories( directory );
			for ( int i = 0; i < directories.Length; i++ ) {
				FindDirectories( directories[ i ], paths );
				paths.Add( directories[ i ] );
			}
		}
	};
};
