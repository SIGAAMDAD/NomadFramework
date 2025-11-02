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
using SaveSystem.Streams;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace SaveSystem {
	/*
	===================================================================================
	
	SaveSlot
	
	===================================================================================
	*/
	/// <summary>
	/// Stores the data of the game state for a player's progress.
	/// </summary>

	public sealed class SaveSlot {
		public const ulong MAGIC = 0xFFEAD4546B727449;
		public static readonly string SAVE_DIRECTORY = "user://SaveData";

		public readonly int Slot;
		public readonly string Filepath;

		public readonly ConcurrentDictionary<string, Dictionary<string, object>> Sections = new ConcurrentDictionary<string, Dictionary<string, object>>();

		private readonly IConsoleService Console;

		/*
		===============
		SaveSlot
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="slot"></param>
		public SaveSlot( int slot, IConsoleService? console ) {
			ArgumentNullException.ThrowIfNull( console );

			Console = console;
			Slot = slot;

			try {
				System.IO.Directory.CreateDirectory( ProjectSettings.GlobalizePath( SAVE_DIRECTORY ) );
			} catch ( Exception ) {
			}

			Filepath = ProjectSettings.GlobalizePath( $"{SAVE_DIRECTORY}/GameData_{Slot}.ngd" );
		}

		/*
		===============
		Delete
		===============
		*/
		/// <summary>
		/// Deletes the save slot's file (the associated data).
		/// </summary>
		public void Delete() {
			System.IO.File.Delete( Filepath );
		}

		/*
		===============
		Create
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		public void Create( string? name ) {
			ArgumentException.ThrowIfNullOrEmpty( name );

			using System.IO.FileStream stream = new System.IO.FileStream( Filepath, System.IO.FileMode.Create );
			using SaveWriterStream writer = new SaveWriterStream( stream );

			SaveHeader( writer );
		}

		/*
		===============
		LoadHeader
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="reader"></param>
		public void LoadHeader( SaveReaderStream? reader = null ) {
			if ( reader == null ) {
				using System.IO.FileStream stream = new System.IO.FileStream( Filepath, System.IO.FileMode.Open );
				reader = new SaveReaderStream( stream );
			}

			if ( reader.ReadUInt64() != MAGIC ) {
				Console.PrintError( "SaveSlot.LoadHeader: save data has invalid identifier in header!" );
				return;
			}

			int sectionCount = reader.ReadInt32();
			ulong checksum = reader.ReadUInt64();
		}

		/*
		===============
		SaveHeader
		===============
		*/
		public void SaveHeader( SaveWriterStream writer, int sectionCount = 0, DataChecksum? checksum = null ) {
			writer.Write( MAGIC );
			writer.Write( 0 );
			if ( checksum.HasValue ) {
				writer.Write( checksum.Value.Checksum );
			} else {
				// write a placeholder
				writer.Write( 0 );
			}
		}

		/*
		===============
		AddSection
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="fields"></param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void AddSection( string? name, Dictionary<string, object>? fields ) {
			ArgumentException.ThrowIfNullOrEmpty( name );
			ArgumentNullException.ThrowIfNull( fields );

			Sections.TryAdd( name, fields );
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
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public Dictionary<string, object>? GetSection( string? name ) {
			ArgumentException.ThrowIfNullOrEmpty( name );

			return Sections.TryGetValue( name, out Dictionary<string, object>? value ) ? value : null;
		}
	};
};