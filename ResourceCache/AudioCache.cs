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
	
	AudioCache
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	public sealed class AudioCache : IResourceCache<AudioStream> {
		private readonly ConcurrentDictionary<string, AudioStream> Cache = new ConcurrentDictionary<string, AudioStream>();
		private readonly static AudioCache Instance;

		/*
		===============
		AudioCache
		===============
		*/
		static AudioCache() {
			Instance = new AudioCache();
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
		public AudioStream GetCached( string? path ) {
			ArgumentException.ThrowIfNullOrEmpty( path );

			AudioStream stream = Cache.GetOrAdd( path, ResourceLoader.Load<AudioStream>( path ) );
			return stream;
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
		GetStream
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static AudioStream GetStream( string? path ) {
			ArgumentNullException.ThrowIfNull( Instance );
			return Instance.GetCached( path );
		}
	};
};