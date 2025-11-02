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

using System;
using System.Collections.Concurrent;

namespace SaveSystem {
	/*
	===================================================================================
	
	SaveInfoLoad
	
	===================================================================================
	*/
	/// <summary>
	/// Loads a save file's data into RAM.
	/// </summary>

	public sealed class SaveInfoLoad {
		/// <summary>
		/// The amount of sections in the save file.
		/// </summary>
		public int SectionCount => SectionCache.Count;

		public readonly ConcurrentDictionary<string, SaveSectionReader?> SectionCache = new ConcurrentDictionary<string, SaveSectionReader?>();

		private readonly IConsoleService Console;

		/*
		===============
		SaveInfoLoad
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		public SaveInfoLoad( string? filePath, IConsoleService? console ) {
			ArgumentNullException.ThrowIfNull( console );
			ArgumentException.ThrowIfNullOrEmpty( filePath );

			Console = console;
			Load( filePath );
		}

		/*
		===============
		GetSection
		===============
		*/
		/// <summary>
		/// Performs a lookup for a section based on the name.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public SaveSectionReader? GetSection( string? name ) {
			ArgumentException.ThrowIfNullOrEmpty( name );

			if ( SectionCache.TryGetValue( name, out SaveSectionReader? value ) ) {
				return value;
			}
			Console.PrintError( $"SaveInfoLoad.GetSection: failed to find save section '{name}'!" );
			return null;
		}

		/*
		===============
		Load
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="filePath"></param>
		private void Load( string filePath ) {
			using System.IO.FileStream? stream = new System.IO.FileStream( filePath, System.IO.FileMode.Open );
			ArgumentNullException.ThrowIfNull( stream );

			using Streams.SaveReaderStream reader = new Streams.SaveReaderStream( stream );
			if ( !LoadHeader( reader, out int sectionCount ) ) {
				return;
			}

			LoadSections( sectionCount, reader );
		}

		/*
		===============
		LoadSections
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sectionCount"></param>
		/// <param name="stream"></param>
		private void LoadSections( int sectionCount, Streams.SaveReaderStream stream ) {
			for ( int i = 0; i < sectionCount; i++ ) {
				string name = stream.ReadString();
				ArgumentException.ThrowIfNullOrEmpty( name );

				Console.PrintLine( $"SaveInfoLoad.LoadSections: loading save section '{name}'..." );
				SectionCache.TryAdd( name, new SaveSectionReader( stream, Console ) );
				Console.PrintLine( "...Done" );
			}
		}

		/*
		===============
		LoadHeader
		===============
		*/
		/// <summary>
		/// Loads the header from the provided stream.
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="sectionCount"></param>
		/// <returns></returns>
		/// <exception cref="System.IO.InvalidDataException"></exception>
		private bool LoadHeader( Streams.SaveReaderStream reader, out int sectionCount ) {
			sectionCount = 0;

			if ( reader.ReadUInt64() != SaveSlot.MAGIC ) {
				Console.PrintError( "ArchiveSystem.LoadGame: save data has invalid magic in header!" );
				return false;
			}
			reader.ReadString();

			sectionCount = reader.ReadInt32();
			if ( sectionCount < 0 ) {
				Console.PrintError( "ArchiveSystem.LoadGame: save data section count isn't valid!" );
				return false;
			}

			ulong crc64 = reader.ReadUInt64();

			int position = reader.Position;
			DataChecksum checksum = new DataChecksum( reader );
			if ( checksum.Checksum != crc64 ) {
				throw new System.IO.InvalidDataException( "crc64 present in save file does not match the generated one!" );
			}
			reader.Seek( position, System.IO.SeekOrigin.Begin );

			return true;
		}
	};
};