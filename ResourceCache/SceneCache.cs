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

using Godot;
using System;
using System.Collections.Concurrent;

namespace ResourceCache {
	/*
	===================================================================================
	
	SceneCache
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	public sealed class SceneCache : IResourceCache<PackedScene> {
		private readonly ConcurrentDictionary<string, PackedScene> Cache = new ConcurrentDictionary<string, PackedScene>();
		private static readonly SceneCache Instance;

		/*
		===============
		SceneCache
		===============
		*/
		static SceneCache() {
			Instance = new SceneCache();
		}

		/*
		===============
		GetCached
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public PackedScene GetCached( string? path ) {
			ArgumentException.ThrowIfNullOrEmpty( path );

			PackedScene scene = Cache.GetOrAdd( path, ResourceLoader.Load<PackedScene>( path ) );
			return scene;
		}

		/*
		===============
		Clear
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		public void Clear() {
			Cache.Clear();
		}

		/*
		===============
		GetScene
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static PackedScene GetScene( string? path ) {
			ArgumentNullException.ThrowIfNull( Instance );
			return Instance.GetCached( path );
		}
	};
};