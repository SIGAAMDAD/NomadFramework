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
using CVars;
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

		public static readonly CVar<int> LastSaveSlot = new CVar<int>(
			name: "gameplay.LastSaveSlot",
			defaultValue: 0,
			description: "The current save slot index, DO NOT MODIFY.",
			flags: CVarFlags.Archive
		);

		private static SaveManager Instance;

		/// <summary>
		/// 
		/// </summary>
		public readonly IGameEvent SaveGameBegin;

		/// <summary>
		/// 
		/// </summary>
		public readonly IGameEvent SaveGameEnd;

		/// <summary>
		/// 
		/// </summary>
		public readonly IGameEvent LoadGameBegin;

		/// <summary>
		/// 
		/// </summary>
		public readonly IGameEvent LoadGameEnd;

		/// <summary>
		/// Event that fires when we're writing objects to the save file.
		/// </summary>
		public readonly IGameEvent SaveSectionNodes;

		/// <summary>
		/// Event that fires when we're loading objects from the save file.
		/// </summary>
		public readonly IGameEvent LoadSectionNodes;

		/*
		===============
		SaveManager
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="console"></param>
		/// <param name="eventBus"></param>
		public SaveManager( IConsoleService? console, IGameEventBusService? eventBus, ICVarSystem? cvarSystem ) {
			ArgumentNullException.ThrowIfNull( console );
			ArgumentNullException.ThrowIfNull( eventBus );
			ArgumentNullException.ThrowIfNull( cvarSystem );

			SaveGameBegin = eventBus.CreateEvent( nameof( SaveGameBegin ) );
			SaveGameEnd = eventBus.CreateEvent( nameof( SaveGameEnd ) );
			LoadGameBegin = eventBus.CreateEvent( nameof( LoadGameBegin ) );
			LoadGameEnd = eventBus.CreateEvent( nameof( LoadGameEnd ) );
			SaveSectionNodes = eventBus.CreateEvent( nameof( SaveSectionNodes ) );
			LoadSectionNodes = eventBus.CreateEvent( nameof( LoadSectionNodes ) );

			Instance = this;

			LoadSaveSlots( console );

			LastSaveSlot.Register();
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
				SaveSlot current = Instance.Slots[ LastSaveSlot.Value ];

				using SaveWriterStream stream = new SaveWriterStream( new System.IO.FileStream( current.Filepath, System.IO.FileMode.CreateNew ) );
				current.SaveHeader( stream );

				Instance.SaveSectionNodes.Publish( new SaveSectionNodesEventData( current ) );
				foreach ( var section in current.Sections ) {
					using SaveSectionWriter writer = new SaveSectionWriter( section.Key, section.Value, stream );
				}

				current.SaveHeader( stream, 0, new DataChecksum( stream ) );
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
				LastSaveSlot.Set( slot );
				SaveSlot current = Instance.Slots[ slot ];
			} catch ( Exception ) {
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
		private void LoadSaveSlots( IConsoleService console ) {
			string[] slots = System.IO.Directory.GetFiles( ProjectSettings.GlobalizePath( SaveSlot.SAVE_DIRECTORY ) );
			for ( int i = 0; i < slots.Length; i++ ) {
				Slots.Add( new SaveSlot( i, console ) );
			}
		}
	};
};