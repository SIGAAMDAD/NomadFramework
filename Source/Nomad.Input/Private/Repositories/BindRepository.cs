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
using System.Collections.Immutable;
using System.IO;
using Nomad.Core.Compatibility.Guards;
using Nomad.Core.CVars;
using Nomad.Core.FileSystem;
using Nomad.Core.Logger;
using Nomad.CVars;
using Nomad.Input.Private.Services;
using Nomad.Input.Private.ValueObjects;

namespace Nomad.Input.Private.Repositories {
	/*
	===================================================================================
	
	BindRepository
	
	===================================================================================
	*/
	/// <summary>
	/// Loads binding databases from disk and exposes the merged action definitions used by the input pipeline.
	/// </summary>
	internal sealed class BindRepository : IDisposable {
		private const string BINDINGS_DIRECTORY = "Assets/Config/Bindings/";

		private readonly IFileSystem _fileSystem;
		private readonly BindLoader _loader;
		private readonly string _defaultsPath;

		private ImmutableArray<InputActionDefinition> _defaultBindings = ImmutableArray<InputActionDefinition>.Empty;
		private ImmutableDictionary<string, ImmutableArray<InputActionDefinition>> _bindMappings = ImmutableDictionary<string, ImmutableArray<InputActionDefinition>>.Empty;
		private ImmutableArray<InputActionDefinition> _allBindings = ImmutableArray<InputActionDefinition>.Empty;

		private bool _isDisposed = false;

		/*
		===============
		BindRepository
		===============
		*/
		/// <summary>
		/// Initializes the repository and loads the default bindings plus every named binding mapping on disk.
		/// </summary>
		/// <param name="fileSystem">The file system used to discover and read binding definition files.</param>
		/// <param name="cvarSystem">The cvar system that supplies the configured defaults file path.</param>
		/// <param name="logger"></param>
		/// <exception cref="ArgumentNullException">
		/// Thrown when <paramref name="fileSystem"/> or <paramref name="cvarSystem"/> is <see langword="null"/>.
		/// </exception>
		/// <exception cref="FileNotFoundException">Thrown when the configured defaults binding file cannot be found.</exception>
		public BindRepository( IFileSystem fileSystem, ICVarSystemService cvarSystem, ILoggerService logger ) {
			ArgumentGuard.ThrowIfNull( cvarSystem );

			_fileSystem = fileSystem ?? throw new ArgumentNullException( nameof( fileSystem ) );
			_loader = new BindLoader( _fileSystem, logger );

			var defaultsPath = cvarSystem.GetCVarOrThrow<string>( Constants.CVars.DEFAULTS_PATH );
			_defaultsPath = defaultsPath.Value;

			Reload();
		}

		/*
		===============
		GetDefaultBindings
		===============
		*/
		/// <summary>
		/// Gets the bindings loaded from the configured defaults database.
		/// </summary>
		/// <returns>The default action definitions.</returns>
		/// <exception cref="ObjectDisposedException">Thrown when the repository has been disposed.</exception>
		public ImmutableArray<InputActionDefinition> GetDefaultBindings() {
			ThrowIfDisposed();
			return _defaultBindings;
		}

		/*
		===============
		GetAllBindings
		===============
		*/
		/// <summary>
		/// Gets a merged view of the defaults database and every discovered binding mapping.
		/// </summary>
		/// <returns>All loaded action definitions merged by action name.</returns>
		/// <exception cref="ObjectDisposedException">Thrown when the repository has been disposed.</exception>
		public ImmutableArray<InputActionDefinition> GetAllBindings() {
			ThrowIfDisposed();
			return _allBindings;
		}

		/*
		===============
		GetBindMappings
		===============
		*/
		/// <summary>
		/// Gets every discovered named binding mapping with the defaults already merged into each mapping.
		/// </summary>
		/// <returns>A dictionary keyed by mapping name.</returns>
		/// <exception cref="ObjectDisposedException">Thrown when the repository has been disposed.</exception>
		public ImmutableDictionary<string, ImmutableArray<InputActionDefinition>> GetBindMappings() {
			ThrowIfDisposed();
			return _bindMappings;
		}

		/*
		===============
		TryGetBindMapping
		===============
		*/
		/// <summary>
		/// Attempts to retrieve a named binding mapping with the defaults already merged into it.
		/// </summary>
		/// <param name="mappingName">The mapping name, typically derived from the binding file name.</param>
		/// <param name="bindings">The merged bindings for the requested mapping when it exists.</param>
		/// <returns><see langword="true"/> when the mapping exists; otherwise, <see langword="false"/>.</returns>
		/// <exception cref="ObjectDisposedException">Thrown when the repository has been disposed.</exception>
		public bool TryGetBindMapping( string mappingName, out ImmutableArray<InputActionDefinition> bindings ) {
			ThrowIfDisposed();

			if ( string.IsNullOrWhiteSpace( mappingName ) ) {
				bindings = ImmutableArray<InputActionDefinition>.Empty;
				return false;
			}

			if ( !_bindMappings.TryGetValue( mappingName, out bindings ) ) {
				bindings = ImmutableArray<InputActionDefinition>.Empty;
				return false;
			}
			return true;
		}

		/*
		===============
		Reload
		===============
		*/
		/// <summary>
		/// Reloads the defaults database and all discovered binding mapping files from disk.
		/// </summary>
		/// <exception cref="ObjectDisposedException">Thrown when the repository has been disposed.</exception>
		/// <exception cref="FileNotFoundException">Thrown when the configured defaults binding file cannot be found.</exception>
		/// <exception cref="InvalidOperationException">Thrown when two mapping files resolve to the same mapping name.</exception>
		public void Reload() {
			ThrowIfDisposed();

			if ( !_loader.LoadBindDatabase( _defaultsPath, out var defaultBindings ) ) {
				throw new FileNotFoundException( $"Default bindings file '{_defaultsPath}' could not be found.", _defaultsPath );
			}

			_defaultBindings = defaultBindings;
			LoadAllBindMappings();
		}

		/*
		===============
		LoadAllBindMappings
		===============
		*/
		/// <summary>
		/// Loads every binding mapping file from the configured mappings directory and rebuilds the merged caches.
		/// </summary>
		private void LoadAllBindMappings() {
			var mappingBuilder = ImmutableDictionary.CreateBuilder<string, ImmutableArray<InputActionDefinition>>( StringComparer.OrdinalIgnoreCase );
			var allSources = new List<ImmutableArray<InputActionDefinition>> { _defaultBindings };

			if ( _fileSystem.DirectoryExists( BINDINGS_DIRECTORY ) ) {
				var files = _fileSystem.GetFiles( BINDINGS_DIRECTORY, "*.json", true );

				for ( int i = 0; i < files.Count; i++ ) {
					string filePath = files[i];
					if ( string.Equals( filePath, _defaultsPath, StringComparison.OrdinalIgnoreCase ) ) {
						continue;
					}

					if ( !_loader.LoadBindDatabase( filePath, out var mappingBindings ) ) {
						continue;
					}

					string mappingName = GetBindMappingName( filePath );
					if ( mappingBuilder.ContainsKey( mappingName ) ) {
						throw new InvalidOperationException( $"Duplicate binding mapping name '{mappingName}' discovered while loading '{filePath}'." );
					}

					mappingBuilder[mappingName] = MergeActions( _defaultBindings, mappingBindings );
					allSources.Add( mappingBindings );
				}
			}

			_bindMappings = mappingBuilder.ToImmutable();
			_allBindings = MergeActions( allSources );
		}

		/*
		===============
		GetBindMappingName
		===============
		*/
		/// <summary>
		/// Derives the repository mapping key from a binding file path.
		/// </summary>
		/// <param name="filePath">The binding file path.</param>
		/// <returns>The file name without its extension.</returns>
		private static string GetBindMappingName( string filePath ) {
			return Path.GetFileNameWithoutExtension( filePath );
		}

		/*
		===============
		MergeActions
		===============
		*/
		/// <summary>
		/// Merges action definition sets by action name while preserving the order in which the sources were supplied.
		/// </summary>
		/// <param name="sources">The action definition sets to merge.</param>
		/// <returns>A single merged action definition array.</returns>
		private static ImmutableArray<InputActionDefinition> MergeActions( params ImmutableArray<InputActionDefinition>[] sources ) {
			return MergeActions( (IEnumerable<ImmutableArray<InputActionDefinition>>)sources );
		}

		/*
		===============
		MergeActions
		===============
		*/
		/// <summary>
		/// Merges action definition sets by action name while concatenating the bindings for matching actions.
		/// </summary>
		/// <param name="sources">The action definition sets to merge.</param>
		/// <returns>A single merged action definition array.</returns>
		/// <exception cref="InvalidOperationException">Thrown when two actions share a name but disagree on value type.</exception>
		private static ImmutableArray<InputActionDefinition> MergeActions( IEnumerable<ImmutableArray<InputActionDefinition>> sources ) {
			var actionIndices = new Dictionary<string, int>( StringComparer.Ordinal );
			var builder = ImmutableArray.CreateBuilder<InputActionDefinition>();

			foreach ( var source in sources ) {
				for ( int i = 0; i < source.Length; i++ ) {
					var action = source[i];
					if ( !actionIndices.TryGetValue( action.Name, out int actionIndex ) ) {
						actionIndices.Add( action.Name, builder.Count );
						builder.Add( action );
						continue;
					}

					var existing = builder[actionIndex];
					if ( existing.ValueType != action.ValueType ) {
						throw new InvalidOperationException( $"Binding action '{action.Name}' has conflicting value types across binding databases." );
					}

					builder[actionIndex] = new InputActionDefinition(
						existing.Name,
						existing.ValueType,
						existing.Bindings.AddRange( action.Bindings )
					);
				}
			}

			return builder.ToImmutable();
		}

		/*
		===============
		ThrowIfDisposed
		===============
		*/
		/// <summary>
		/// Throws when the repository has already been disposed.
		/// </summary>
		/// <exception cref="ObjectDisposedException">Thrown when the repository has already been disposed.</exception>
		private void ThrowIfDisposed() {
			StateGuard.ThrowIfDisposed( _isDisposed, this );
		}

		/*
		===============
		Dispose
		===============
		*/
		/// <summary>
		/// Marks the repository as disposed so it can no longer be queried or reloaded.
		/// </summary>
		public void Dispose() {
			if ( !_isDisposed ) {
				_isDisposed = true;
			}
			GC.SuppressFinalize( this );
		}
	};
};
