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

using System.Collections.Generic;

namespace SaveSystem {
	internal sealed class SaveRecoveryResult {
		public bool Success { get; set; }
		public SaveHeader Header { get; set; }
		public RecoveryMethod RecoveryMethod { get; set; }
		public readonly List<string> RecoveredSections = new List<string>();
		public readonly List<string> LostSections = new List<string>();
		public string Error { get; set; }

		public static SaveRecoveryResult AttemptRecovery( int slot ) {
			string filepath = FilepathCache.GetSlotPath( slot );
			if ( !SaveSlot.ValidateSaveFile( filepath, out var header ) ) {
				return TryRecoverCorruptedFile( filepath );
			}
			return new SaveRecoveryResult { Success = true, Header = header };
		}

		private static SaveRecoveryResult TryRecoverCorruptedFile( string filepath ) {
			var result = new SaveRecoveryResult();

			return result;
		}
	};

	internal enum RecoveryMethod : byte {
		None,
		PartialHeader,
		SectionScanning,
		BackupRestore
	};
};