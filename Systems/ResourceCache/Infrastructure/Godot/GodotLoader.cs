/*
===========================================================================
The Nomad AGPL Source Code
Copyright (C) 2025 Noah Van Til

The Nomad Source Code is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published
by the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

The Nomad Source Code is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with The Nomad Source Code.  If not, see <http://www.gnu.org/licenses/>.

If you have questions concerning this license or the applicable additional
terms, you may contact me via email at nyvantil@gmail.com.
===========================================================================
*/

using NomadCore.Systems.ResourceCache.Application.Interfaces;
using Godot;
using NomadCore.Domain.Models.ValueObjects;
using System;
using System.Threading.Tasks;
using NomadCore.Systems.ResourceCache.Domain.Models.ValueObjects;
using System.Threading;

namespace NomadCore.Systems.ResourceCache.Infrastructure.Godot {
	/*
	===================================================================================
	
	GodotLoader
	
	===================================================================================
	*/
	/// <summary>
	/// A godot-focused resource loader.
	/// </summary>
	
	public readonly struct GodotLoader : IResourceLoader<Resource, FilePath> {
		public IResourceLoader<Resource, FilePath>.LoadCallback Load => LoadResource;
		public IResourceLoader<Resource, FilePath>.LoadAsyncCallback LoadAsync => LoadResourceAsync;

		/*
		===============
		LoadResource
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		private Result<Resource> LoadResource( FilePath path ) {
			var resource = ResourceLoader.Load( path.GodotPath, String.Empty, ResourceLoader.CacheMode.ReplaceDeep );
			if ( resource == null ) {
				return Result<Resource>.Failure( LoadError.Create( $"Error loading godot resource '{path}'" ) );
			}
			return Result<Resource>.Success( resource );
		}

		/*
		===============
		LoadResourceAsync
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="path"></param>
		/// <param name="ct"></param>
		/// <returns></returns>
		private async Task<Result<Resource>> LoadResourceAsync( FilePath path, CancellationToken ct = default ) {
			global::Godot.Error requestError = ResourceLoader.LoadThreadedRequest( path, String.Empty, true, ResourceLoader.CacheMode.ReplaceDeep );
			if ( requestError != global::Godot.Error.Ok ) {
				return Result<Resource>.Failure( LoadError.Create( $"Error loading godot resource '{path}' - {requestError}" ) );
			}

			ResourceLoader.ThreadLoadStatus status = ResourceLoader.ThreadLoadStatus.Failed;
			SceneTree sceneTree = (SceneTree)Engine.GetMainLoop();

			do {
				if ( status == ResourceLoader.ThreadLoadStatus.InProgress ) {
					await sceneTree.ToSignal( sceneTree, SceneTree.SignalName.ProcessFrame );
				}
				status = ResourceLoader.LoadThreadedGetStatus( path );
			} while ( status == ResourceLoader.ThreadLoadStatus.InProgress );

			if ( status == ResourceLoader.ThreadLoadStatus.Loaded ) {
				return Result<Resource>.Success( ResourceLoader.LoadThreadedGet( path ) );
			}
			return Result<Resource>.Failure( LoadError.Create( $"godot resource '{path}' failed to load with thread status '{status}" ) );
		}
	};
};