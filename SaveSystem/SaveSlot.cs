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
using System.Text.Json;

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
		internal const ulong MAGIC = 0xFFEAD4546B727449;
		internal static readonly string SAVE_DIRECTORY = "user://SaveData";

		internal readonly int Slot;
		internal readonly string Filepath;

		internal ConcurrentDictionary<string, List<SaveField>> Sections = new ConcurrentDictionary<string, List<SaveField>>();

		private int BackupCount = 0;

		/*
		===============
		SaveSlot
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="slot"></param>
		internal SaveSlot( int slot ) {
			Slot = slot;

			try {
				System.IO.Directory.CreateDirectory( ProjectSettings.GlobalizePath( SAVE_DIRECTORY ) );
			} catch ( Exception ) {
			}

			BackupCount = GetNumBackups();
			Filepath = FilepathCache.GetSlotPath( Slot );
		}

		/*
		===============
		Delete
		===============
		*/
		/// <summary>
		/// Deletes the save slot's file (the associated data).
		/// </summary>
		internal void Delete() {
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
		internal void Create( string? name ) {
			ArgumentException.ThrowIfNullOrEmpty( name );

			using System.IO.FileStream stream = new System.IO.FileStream( Filepath, System.IO.FileMode.Create );
			using SaveWriterStream writer = new SaveWriterStream( stream );

			SaveHeader( writer, new DataChecksum(), false );
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
		internal void LoadHeader( SaveReaderStream? reader = null ) {
			if ( reader == null ) {
				using System.IO.FileStream stream = new System.IO.FileStream( Filepath, System.IO.FileMode.Open );
				reader = new SaveReaderStream( stream );
			}

			if ( reader.ReadUInt64() != MAGIC ) {
				ConsoleSystem.Console.PrintError( "SaveSlot.LoadHeader: save data has invalid identifier in header!" );
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
		/// <summary>
		/// Writes the save file header
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="checksum"></param>
		/// <param name="hasChecksum"></param>
		/// <param name="sectionCount"></param>
		internal static void SaveHeader( SaveWriterStream writer, DataChecksum checksum, bool hasChecksum, int sectionCount = 0 ) {
			writer.Write( MAGIC );
			writer.Write( 0 );
			if ( hasChecksum ) {
				writer.Write( checksum.Checksum );
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
		public void AddSection( string? name, List<SaveField>? fields ) {
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
		public List<SaveField>? GetSection( string? name ) {
			ArgumentException.ThrowIfNullOrEmpty( name );

			return Sections.TryGetValue( name, out List<SaveField>? value ) ? value : null;
		}

		/*
		===============
		ValidateSaveFile
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="filepath"></param>
		/// <param name="header"></param>
		/// <returns></returns>
		internal static bool ValidateSaveFile( string? filepath, out SaveHeader header ) {
			ArgumentException.ThrowIfNullOrEmpty( filepath );

			header = new SaveHeader();
			try {
				using System.IO.FileStream stream = new System.IO.FileStream( filepath, System.IO.FileMode.Open, System.IO.FileAccess.Read );
				using System.IO.BinaryReader reader = new System.IO.BinaryReader( stream );

				header.Magic = reader.ReadUInt64();
				if ( header.Magic != MAGIC ) {
					return false;
				}

				header.VersionMajor = reader.ReadUInt32();
				header.VersionMinor = reader.ReadUInt32();
				header.VersionPatch = reader.ReadUInt32();

				header.SectionCount = reader.ReadUInt32();

				header.Timestamp = reader.ReadInt64();
				header.Checksum = reader.ReadUInt64();

				if ( header.VersionMajor > SaveManager.VersionMajor ) {
					return false;
				}
				if ( header.SectionCount == 0 || header.SectionCount > 100000 ) {
					return false;
				}
				if ( header.Timestamp < 0 || header.Timestamp > DateTime.Now.AddYears( 1 ).Ticks ) {
					return false;
				}
				return true;
			} catch {
				return false;
			}
		}

		/*
		===============
		CreateBackup
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		internal void CreateBackup() {
			if ( BackupCount >= SaveManager.MaxBackups ) {
				return;
			}
			for ( int i = SaveManager.MaxBackups - 1; i > 0; i-- ) {
				string oldBackup = FilepathCache.GetBackupPath( Slot, i );
				string newBackup = FilepathCache.GetBackupPath( Slot, i + 1 );

				if ( System.IO.File.Exists( oldBackup ) ) {
					if ( System.IO.File.Exists( newBackup ) ) {
						System.IO.File.Delete( newBackup );
					}
					System.IO.File.Move( oldBackup, newBackup );
				}
			}

			string lastestBackup = FilepathCache.GetBackupPath( Slot, 0 );
			System.IO.File.Copy( Filepath, lastestBackup, true );
		}

		/*
		===============
		ToJson
		===============
		*/
		public bool ToJson( string? output ) {
			ArgumentException.ThrowIfNullOrEmpty( output );

			try {
				using System.IO.FileStream stream = new System.IO.FileStream( output, System.IO.FileMode.CreateNew, System.IO.FileAccess.Write );
				using System.IO.StreamWriter writer = new System.IO.StreamWriter( stream );
				writer.Write( JsonSerializer.Serialize( Sections ) );
			} catch {
				return false;
			}
			return true;
		}

		/*
		===============
		FromJson
		===============
		*/
		public bool FromJson( string? input ) {
			ArgumentException.ThrowIfNullOrEmpty( input );

			try {
				var dict = JsonSerializer.Deserialize<ConcurrentDictionary<string, List<SaveField>>>(
					new System.IO.FileStream( input, System.IO.FileMode.Open, System.IO.FileAccess.Read )
				);
				ArgumentNullException.ThrowIfNull( dict );
				Sections = dict;
			} catch {
				return false;
			}
			return true;
		}

		/*
		===============
		RestoreBackup
		===============
		*/
		internal bool RestoreBackup( int backupIndex ) {
			string backupPath = FilepathCache.GetBackupPath( Slot, backupIndex );
			string primaryPath = Filepath;

			if ( System.IO.File.Exists( backupPath ) && ValidateSaveFile( backupPath, out _ ) ) {
				System.IO.File.Copy( backupPath, primaryPath, true );
				return true;
			}
			return false;
		}
		
		/*
		===============
		GetNumBackups
		===============
		*/
		/// <summary>
		/// Counts the number of backups associated with the current save file.
		/// </summary>
		private int GetNumBackups() {
			int backupCount = 0;

			string[] files = System.IO.Directory.GetFiles( ProjectSettings.GlobalizePath( $"{SAVE_DIRECTORY}" ) );
			for ( int b = 0; b < SaveManager.MaxBackups; b++ ) {
				string path = $"GameData_{Slot}.ngd.backup{b}";
				for ( int i = 0; i < files.Length; i++ ) {
					if ( string.Equals( files[ i ], path ) ) {
						backupCount++;
					}
				}
			}
			return backupCount;
		}
	};
};