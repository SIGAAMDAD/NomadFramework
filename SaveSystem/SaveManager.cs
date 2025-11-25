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
using EventSystem;
using System;
using SaveSystem.Streams;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SaveSystem {
	/*
	===================================================================================
	
	SaveManager
	
	===================================================================================
	*/
	/// <summary>
	/// Manages save slots, loading and writing game state to and from disk.
	/// </summary>

	public sealed class SaveManager {
		[StructLayout( LayoutKind.Sequential, Pack = 1 )]
		public readonly struct SaveSectionNodesEventData : IEventArgs {
			public readonly SaveSlot Slot;

			public SaveSectionNodesEventData( SaveSlot? slot ) {
				ArgumentNullException.ThrowIfNull( slot );

				Slot = slot;
			}
		};
		[StructLayout( LayoutKind.Sequential, Pack = 1 )]
		public readonly struct LoadSectionNodesEventData : IEventArgs {
			public readonly SaveSlot Slot;

			public LoadSectionNodesEventData( SaveSlot? slot ) {
				ArgumentNullException.ThrowIfNull( slot );

				Slot = slot;
			}
		};

		public readonly List<SaveSlot> Slots = new List<SaveSlot>();

		private static SaveManager Instance;

		/// <summary>
		/// 
		/// </summary>
		public readonly GameEvent SaveGameBegin = new GameEvent( nameof( SaveGameBegin ) );

		/// <summary>
		/// 
		/// </summary>
		public readonly GameEvent SaveGameEnd = new GameEvent( nameof( SaveGameEnd ) );

		/// <summary>
		/// 
		/// </summary>
		public readonly GameEvent LoadGameBegin = new GameEvent( nameof( LoadGameBegin ) );

		/// <summary>
		/// 
		/// </summary>
		public readonly GameEvent LoadGameEnd = new GameEvent( nameof( LoadGameEnd ) );

		/// <summary>
		/// Event that fires when we're writing objects to the save file.
		/// </summary>
		public readonly GameEvent SaveSectionNodes = new GameEvent( nameof( SaveSectionNodes ) );

		/// <summary>
		/// Event that fires when we're loading objects from the save file.
		/// </summary>
		public readonly GameEvent LoadSectionNodes = new GameEvent( nameof( LoadSectionNodes ) );

		/*
		===============
		SaveManager
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		public SaveManager() {
			Instance = this;

			LoadSaveSlots();

			Config.LastSaveSlot.Register();
			Config.MaxBackups.Register();
			Config.VersionMajor.Register();
			Config.VersionMajor.Register();
			Config.VersionPatch.Register();
		}

		/*
		===============
		SlotExists
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="slot"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static bool SlotExists( int slot ) {
			return System.IO.File.Exists( ProjectSettings.GlobalizePath( $"{SaveSlot.SAVE_DIRECTORY}/GameData_{slot}.ngd" ) );
		}

		/*
		===============
		Save
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		public static void Save() {
			Instance.SaveGameBegin.Publish( EmptyEventArgs.Args );

			try {
				SaveSlot current = Instance.Slots[ Config.LastSaveSlot ];

				using SaveWriterStream stream = new SaveWriterStream( new System.IO.FileStream( current.Filepath, System.IO.FileMode.CreateNew ) );
				SaveSlot.SaveHeader( stream, new DataChecksum(), false );

				Instance.SaveSectionNodes.Publish( new SaveSectionNodesEventData( current ) );
				foreach ( var section in current.Sections ) {
					SaveSectionWriter writer = new SaveSectionWriter( section.Key, section.Value, stream );
				}

				SaveSlot.SaveHeader( stream, new DataChecksum( stream ), true );
				current.CreateBackup();
			} catch ( Exception ) {
			}

			Instance.SaveGameEnd.Publish( EmptyEventArgs.Args );
		}

		/*
		===============
		Load
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="slot"></param>
		public static void Load( int slot ) {
			ArgumentNullException.ThrowIfNull( Instance );
			ArgumentOutOfRangeException.ThrowIfLessThan( slot, 0, nameof( slot ) );
			ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual( slot, Instance.Slots.Count, nameof( slot ) );

			Instance.LoadGameBegin.Publish( EmptyEventArgs.Args );

			try {
				Config.LastSaveSlot.Value = slot;
				SaveSlot current = Instance.Slots[ slot ];

				if ( SaveSlot.ValidateSaveFile( current.Filepath, out var header ) ) {
				}
			} catch ( Exception e ) {
				ConsoleSystem.Console.PrintError( $"SaveManager.Load: exception thrown while loading save data - {e}" );
			}

			Instance.LoadGameEnd.Publish( EmptyEventArgs.Args );
		}

		/*
		===============
		DeleteSlot
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="slot">The slot to clear/delete.</param>
		public static void DeleteSlot( int slot ) {
			ArgumentNullException.ThrowIfNull( Instance );
			if ( slot < 0 || slot >= Instance.Slots.Count ) {
				throw new ArgumentOutOfRangeException( nameof( slot ) );
			}

			if ( Instance.Slots[ slot ] == null ) {
				ConsoleSystem.Console.PrintError( $"SaveManager.DeleteSlot: no existing slot for index {slot}!" );
				return;
			}
			Instance.Slots[ slot ].Delete();
			Instance.Slots.RemoveAt( slot );
		}

		/*
		===============
		LoadSaveSlots
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		private void LoadSaveSlots() {
			string[] slots = System.IO.Directory.GetFiles( ProjectSettings.GlobalizePath( SaveSlot.SAVE_DIRECTORY ) );
			for ( int i = 0; i < slots.Length; i++ ) {
				Slots.Add( new SaveSlot( i ) );
			}
		}
	};
};