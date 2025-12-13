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

using NomadCore.Abstractions.Services;
using NomadCore.Infrastructure;
using NomadCore.Interfaces;
using NomadCore.Interfaces.Common;
using NomadCore.Interfaces.SaveSystem;
using NomadCore.Systems.SaveSystem.Events;
using NomadCore.Systems.SaveSystem.Infrastructure.Sections;
using NomadCore.Systems.SaveSystem.Infrastructure.Streams;
using NomadCore.Systems.SaveSystem.Interfaces;
using NomadCore.Utilities;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.CompilerServices;

namespace NomadCore.Systems.SaveSystem.Infrastructure {
	/*
	===================================================================================
	
	Slot
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	internal sealed class Slot : ISaveSlot {
		public readonly FilePath Filepath;
		
		public int Id => _index;
		private readonly int _index;

		public int Version => 0;

		public DateTime ModifiedAt => _lastWriteTime;
		private DateTime _lastWriteTime;

		public ISaveSection? this[ string name ] => GetSection( name );

		private readonly ConcurrentDictionary<string, ISaveSection> _sections = new ConcurrentDictionary<string, ISaveSection>();

		private ISaveFileStream _fileStream;

		/*
		===============
		Slot
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		public Slot( ILoggerService logger, int index ) {
			ArgumentOutOfRangeException.ThrowIfLessThan( index, 0, nameof( index ) );

			_index = index;
			Filepath = new FilePath( FilepathCache.GetSlotPath( index ), NomadCore.Enums.PathType.User );
		}

		/*
		===============
		Dispose
		===============
		*/
		public void Dispose() {
			_sections.Clear();
		}

		/*
		===============
		Equals
		===============
		*/
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public bool Equals( IEntity<int>? other ) {
			ArgumentNullException.ThrowIfNull( other );
			return _index == other.Id;
		}

		/*
		===============
		GetSection
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public ISaveSection? GetSection( string? name ) {
			ArgumentException.ThrowIfNullOrEmpty( name );
			return _sections.TryGetValue( name, out var section ) ? section : null;
		}

		/*
		===============
		Load
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		public void Load() {
			using Streams.SaveReaderStream reader = new Streams.SaveReaderStream( Filepath.OSPath );

			var header = SaveHeader.Load( reader );

			for ( int i = 0; i < header.SectionCount; i++ ) {
				var section = new SectionReader( reader );
				try {
					_sections.TryAdd( section.Name, section );
				} catch ( Exception e ) {
				}
			}
		}

		/*
		===============
		Save
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		public void Save() {
			using ( _fileStream = new SaveStreamWriter( Filepath.OSPath ) ) {
				_sections.Clear();

				var events = ServiceRegistry.Get<ISaveEvents>();
				events.SaveStarted.Publish( new SaveStartedEventData( this ) );
			}
		}

		/*
		===============
		CreateSection
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public ISectionWriter CreateSection( string name ) {
			ArgumentException.ThrowIfNullOrEmpty( name );

			return (ISectionWriter)_sections.GetOrAdd( name, ( name ) => new SectionWriter( _fileStream, name ) );
		}
	};
};