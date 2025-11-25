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

using NomadCore.Enums;
using NomadCore.Systems.ConsoleSystem.CVars;

namespace NomadCore.Systems.SaveSystem {
	/*
	===================================================================================
	
	Config
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	public static class Config {
		/// <summary>
		/// 
		/// </summary>
		public static readonly CVar<int> LastSaveSlot = new CVar<int>(
			name: "gameplay.LastSaveSlot",
			defaultValue: 0,
			description: "The current save slot index.",
			flags: CVarFlags.Archive | CVarFlags.Init
		);

		/// <summary>
		/// 
		/// </summary>
		public static readonly CVar<int> MaxBackups = new CVar<int>(
			name: "gameplay.MaxBackups",
			defaultValue: 2,
			description: "The maximum backups that the game can create for each save file.",
			flags: CVarFlags.Archive
		);

		/// <summary>
		/// 
		/// </summary>
		public static readonly CVar<uint> VersionMajor = new CVar<uint>(
			name: "gameplay.VersionMajor",
			defaultValue: 2,
			description: "The game's major version.",
			flags: CVarFlags.ReadOnly
		);

		/// <summary>
		/// 
		/// </summary>
		public static readonly CVar<uint> VersionMinor = new CVar<uint>(
			name: "gameplay.VersionMinor",
			defaultValue: 1,
			description: "The game's minor version.",
			flags: CVarFlags.ReadOnly
		);

		/// <summary>
		/// 
		/// </summary>
		public static readonly CVar<ulong> VersionPatch = new CVar<ulong>(
			name: "gameplay.VersionPatch",
			defaultValue: 0,
			description: "The game's current patch id.",
			flags: CVarFlags.ReadOnly
		);

		public static readonly CVar<string> SaveLocation = new CVar<string>(
			name: "gameplay.SaveLocation",
			defaultValue: "user://SaveData",
			description: "The location in the filesystem where all save data is stored.",
			flags: CVarFlags.Archive
		);
	};
};