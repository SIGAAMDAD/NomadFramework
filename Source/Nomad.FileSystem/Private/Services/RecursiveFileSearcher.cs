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
using System.Linq;
using Nomad.Core.Compatibility;

namespace Nomad.FileSystem.Private.Services {
	/*
	===================================================================================

	PathService

	===================================================================================
	*/
	/// <summary>
	/// A file search system that can search recursively through multiple base directories
	/// </summary>

	internal sealed class RecursiveFileSearcher {
		/// <summary>
		/// Gets all search directories in their current priority order.
		/// </summary>
		public IReadOnlyList<string> SearchDirectories => _searchDirectories.AsReadOnly();
		private readonly List<string> _searchDirectories = new List<string>();
		
		/*
		===============
		AddSearchDirectory
		===============
		*/
		/// <summary>
		/// Add an additional search directory with optional priority
		/// </summary>
		/// <param name="directory">Directory to add</param>
		/// <param name="highPriority">If true, adds to beginning of search list</param>
		public void AddSearchDirectory( string directory, bool highPriority = false ) {
			if ( string.IsNullOrWhiteSpace( directory ) ) {
				throw new ArgumentException( "Directory cannot be null or empty", nameof( directory ) );
			}
			if ( !Directory.Exists( directory ) ) {
				throw new DirectoryNotFoundException( $"Directory not found: {directory}" );
			}

			var normalizedPath = NormalizeDirectoryPath( directory );

			if ( highPriority ) {
				_searchDirectories.Insert( 0, normalizedPath );
			} else {
				_searchDirectories.Add( normalizedPath );
			}
		}

		/*
		===============
		FindFile
		===============
		*/
		/// <summary>
		/// Find a file by its relative path
		/// </summary>
		/// <param name="relativePath">Relative path to search for (e.g., "Textures/wood.png")</param>
		/// <param name="searchOption">Search option (default: AllDirectories)</param>
		/// <returns>Full path to the found file, or null if not found</returns>
		public string? FindFile( string relativePath, SearchOption searchOption = SearchOption.AllDirectories ) {
			if ( string.IsNullOrWhiteSpace( relativePath ) ) {
				throw new ArgumentException( "Relative path cannot be null or empty", nameof( relativePath ) );
			}

			// Clean the relative path
			var cleanRelativePath = relativePath.Trim()
				.Replace( '/', Path.DirectorySeparatorChar )
				.Replace( '\\', Path.DirectorySeparatorChar );

			// First, try direct path combination (exact relative location)
			foreach ( var baseDir in _searchDirectories ) {
				var fullPath = Path.Combine( baseDir, cleanRelativePath );
				if ( File.Exists( fullPath ) ) {
					return fullPath;
				}
			}

			// If not found directly, search recursively by file name
			if ( searchOption == SearchOption.AllDirectories ) {
				var fileName = Path.GetFileName( cleanRelativePath );
				if ( string.IsNullOrEmpty( fileName ) ) {
					return null;
				}

				foreach ( var baseDir in _searchDirectories ) {
					var foundFile = FindFileRecursive( baseDir, fileName, cleanRelativePath );
					if ( foundFile != null ) {
						return foundFile;
					}
				}
			}

			return null;
		}

		/*
		===============
		RecursiveFileSearcher
		===============
		*/
		/// <summary>
		/// Find multiple files matching a pattern
		/// </summary>
		/// <param name="searchPattern">Search pattern (e.g., "*.png", "config*.json")</param>
		/// <param name="relativeDirectory">Optional relative directory to restrict search</param>
		/// <returns>Dictionary mapping search directory to found files</returns>
		public Dictionary<string, List<string>> FindFiles( string searchPattern, string? relativeDirectory = null ) {
			var results = new Dictionary<string, List<string>>();

			foreach ( var baseDir in _searchDirectories ) {
				var searchDir = string.IsNullOrEmpty( relativeDirectory )
					? baseDir
					: Path.Combine( baseDir, relativeDirectory );

				if ( !Directory.Exists( searchDir ) ) {
					continue;
				}

				var files = Directory.EnumerateFiles( searchDir, searchPattern, SearchOption.AllDirectories )
					.ToList();

				if ( files.Count > 0 ) {
					results[ baseDir ] = files;
				}
			}

			return results;
		}

		/*
		===============
		FindAllFiles
		===============
		*/
		/// <summary>
		/// Find all occurrences of a file (returns first match from each directory)
		/// </summary>
		/// <param name="relativePath">Relative path to search for</param>
		/// <returns>List of all found files, ordered by search priority</returns>
		public List<string> FindAllFiles( string relativePath ) {
			var results = new List<string>();
			var cleanRelativePath = NormalizePath( relativePath );
			var fileName = Path.GetFileName( cleanRelativePath );

			if ( string.IsNullOrEmpty( fileName ) ) {
				return results;
			}

			foreach ( var baseDir in _searchDirectories ) {
				// Try exact match first
				var exactPath = Path.Combine( baseDir, cleanRelativePath );
				if ( File.Exists( exactPath ) ) {
					results.Add( exactPath );
					continue;
				}

				// Then recursive search
				var foundFile = FindFileRecursive( baseDir, fileName, cleanRelativePath );
				if ( foundFile != null ) {
					results.Add( foundFile );
				}
			}

			return results;
		}

		/*
		===============
		FindFileWithExtensions
		===============
		*/
		/// <summary>
		/// Find file with support for different extensions
		/// </summary>
		/// <param name="relativePathWithoutExtension">Relative path without extension</param>
		/// <param name="extensions">Array of extensions to try (e.g., [".json", ".yaml", ".yml"])</param>
		/// <returns>Full path to found file, or null</returns>
		public string? FindFileWithExtensions( string relativePathWithoutExtension, params string[] extensions ) {
			foreach ( var ext in extensions ) {
				var filePath = FindFile( relativePathWithoutExtension + ext );
				if ( filePath != null ) {
					return filePath;
				}
			}

			return null;
		}

		/*
		===============
		FindFileRecursive
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="baseDirectory"></param>
		/// <param name="targetFileName"></param>
		/// <param name="originalRelativePath"></param>
		/// <returns></returns>
		private string? FindFileRecursive( string baseDirectory, string targetFileName, string originalRelativePath ) {
			try {
				// If the original path has directories, try to maintain some structure
				var relativeDir = Path.GetDirectoryName( originalRelativePath );
				if ( !string.IsNullOrEmpty( relativeDir ) ) {
					// Search in the expected directory structure first
					var expectedDir = Path.Combine( baseDirectory, relativeDir );
					if ( Directory.Exists( expectedDir ) ) {
						var file = Directory.EnumerateFiles( expectedDir, targetFileName, SearchOption.TopDirectoryOnly ).FirstOrDefault();
						if ( file != null ) {
							return file;
						}
					}
				}

				// Full recursive search as fallback
				return Directory.EnumerateFiles( baseDirectory, targetFileName, SearchOption.AllDirectories ).FirstOrDefault();
			} catch ( UnauthorizedAccessException ) {
				// Skip directories we can't access
				return null;
			} catch ( PathTooLongException ) {
				// Skip paths that are too long
				return null;
			}
		}

		/*
		===============
		NormalizeDirectoryPath
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		private string NormalizeDirectoryPath( string path ) {
			var normalized = Path.GetFullPath( path );
			if ( !normalized.EndsWith( Path.DirectorySeparatorChar ) ) {
				normalized += Path.DirectorySeparatorChar;
			}
			return normalized;
		}

		/*
		===============
		NormalizePath
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		private string NormalizePath( string path ) {
			return path.Trim()
				.Replace( '/', Path.DirectorySeparatorChar )
				.Replace( '\\', Path.DirectorySeparatorChar );
		}
	};
};
