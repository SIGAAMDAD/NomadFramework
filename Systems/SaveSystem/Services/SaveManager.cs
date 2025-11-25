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
using NomadCore.Systems.SaveSystem.Infrastructure;
using NomadCore.Interfaces.SaveSystem;
using NomadCore.Utilities;
using NomadCore.Enums;
using System;
using System.Collections.Generic;
using NomadCore.Systems.SaveSystem.Interfaces;
using NomadCore.Systems.SaveSystem.Events;

namespace NomadCore.Systems.SaveSystem.Services {
	/*
	===================================================================================
	
	SaveManager
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>

	public sealed class SaveManager : ISaveService, ISaveEvents {
		public int SlotCount => Slots.Count;
		public int ActiveSlot => SlotIndex.Value;

		private readonly List<Slot> Slots = new List<Slot>();
		private readonly ILoggerService? Logger = ServiceRegistry.Get<ILoggerService>();
		private readonly ICVar<int> SlotIndex;

		public LoadFailed LoadFailed => _loadFailed;
		private readonly LoadFailed _loadFailed = new LoadFailed();

		public LoadStarted LoadStarted => _loadStarted;
		private readonly LoadStarted _loadStarted = new LoadStarted();
		
		public SaveCompleted SaveCompleted => _saveCompleted;
		private readonly SaveCompleted _saveCompleted = new SaveCompleted();

		public SaveFailed SaveFailed => _saveFailed;
		private readonly SaveFailed _saveFailed = new SaveFailed();

		public SaveStarted SaveStarted => _saveStarted;
		private readonly SaveStarted _saveStarted = new SaveStarted();

		public SlotCreated SlotCreated => _slotCreated;
		private readonly SlotCreated _slotCreated = new SlotCreated();

		public SlotDeleted SlotDeleted => _slotDeleted;
		private readonly SlotDeleted _slotDeleted = new SlotDeleted();

		/*
		===============
		SaveManager
		===============
		*/
		public SaveManager() {
			SlotIndex = ServiceRegistry.Get<ICVarSystemService>().Register(
				new CVarCreateInfo<int>(
					name: "game.SlotIndex",
					defaultValue: 0,
					description: "The current save slot index.",
					flags: CVarFlags.Archive | CVarFlags.Hidden,
					validator: ( slot ) => slot > 0 && slot < Slots.Count
				)
			);
			ServiceRegistry.Register<ISaveEvents>( this );
		}

		/*
		===============
		Initialize
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		public void Initialize() {
			Logger?.PrintLine( "SaveManager.Initialize: creating save file cache..." );
			var files = System.IO.Directory.GetFiles( FilepathCache.SavePath.OSPath, "ngd" );
			Logger?.PrintLine( $"...Found {files.Length} save files" );

			for ( int i = 0; i < files.Length; i++ ) {
				Slots.Add( new Slot( i ) );
			}
		}

		/*
		===============
		Shutdown
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		public void Shutdown() {
			_loadFailed.Dispose();
			_loadStarted.Dispose();
			_saveCompleted.Dispose();
			_saveFailed.Dispose();
			_saveStarted.Dispose();
			_slotCreated.Dispose();
			_slotDeleted.Dispose();
		}

		/*
		===============
		GetSlot
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="slot"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public ISaveSlot GetSlot( int slot ) {
			if ( slot < 0 || slot > Slots.Count ) {
				throw new ArgumentOutOfRangeException( nameof( slot ) );
			}

			ISaveSlot current = Slots[ slot ];
			return current;
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
		public bool SlotExists( int slot ) {
			return GetSlot( slot ) != null;
		}

		/*
		===============
		DeleteSlot
		===============
		*/
		/// <summary>
		/// Deletes the provided slot
		/// </summary>
		/// <param name="slot"></param>
		public void DeleteSlot( ISaveSlot slot ) {
			ArgumentNullException.ThrowIfNull( slot );

			Logger?.PrintLine( $"SaveManager.Delete: removing save slot {slot.Index}..." );
			Slots.Remove( (Slot)slot );
			_slotDeleted.PublishAsync( new SlotDeletedEventData( slot.Index ) );
		}

		/*
		===============
		DeleteSlot
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="slot"></param>
		public void DeleteSlot( int slot ) {
			Logger?.PrintLine( $"SaveManager.Delete: removing save slot {slot}..." );
			Slots.RemoveAt( slot );
			_slotDeleted.PublishAsync( new SlotDeletedEventData( slot ) );
		}

		/*
		===============
		CreateSlot
		===============
		*/
		/// <summary>
		/// Creates a new save slot.
		/// </summary>
		public void CreateSlot() {
			Slots.Add( new Slot( SlotIndex.Value++ ) );
			_slotCreated.PublishAsync( new SlotCreatedEventData( SlotIndex.Value ) );
		}
	};
};