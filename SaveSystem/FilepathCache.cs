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
using System.Collections.Generic;
using System.Text;

namespace SaveSystem {
	/*
	===================================================================================
	
	FilepathCache

	===================================================================================
	*/
	/// <summary>
	/// Caches all filepaths to reduce string allocation overhead
	/// </summary>
	
	internal static class FilepathCache {
		private static readonly string SavePath;
		private static readonly Dictionary<int, string> SlotPaths = new Dictionary<int, string>();
		private static readonly Dictionary<KeyValuePair<int, int>, string> BackupPaths = new Dictionary<KeyValuePair<int, int>, string>();

		static FilepathCache() {
			SavePath = ProjectSettings.GlobalizePath( $"{SaveSlot.SAVE_DIRECTORY}/" );
		}

		/*
		===============
		GetSlotPath
		===============
		*/
		public static string GetSlotPath( int slot ) {
			if ( !SlotPaths.TryGetValue( slot, out string? value ) ) {
				StringBuilder sb = new StringBuilder( 4096 );
				sb.Append( SavePath );
				sb.Append( "GameData_" );
				sb.Append( slot );
				sb.Append( ".ngd" );
				value = sb.ToString();
				SlotPaths.Add( slot, value );
			}
			return value;
		}

		/*
		===============
		GetBackupPath
		===============
		*/
		public static string GetBackupPath( int slot, int backupIndex ) {
			KeyValuePair<int, int> kvp = new KeyValuePair<int, int>( slot, backupIndex );
			if ( !BackupPaths.TryGetValue( kvp, out string? value ) ) {
				StringBuilder sb = new StringBuilder( 4096 );
				sb.Append( SavePath );
				sb.Append( "GameData_" );
				sb.Append( slot );
				sb.Append( ".ngd.backup" );
				sb.Append( backupIndex );
				value = sb.ToString();
				BackupPaths.Add( kvp, value );
			}
			return value;
		}
	};
};