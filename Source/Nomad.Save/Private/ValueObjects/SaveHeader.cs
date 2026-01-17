/*
===========================================================================
The Nomad Framework
Copyright (C) 2025-2026 Noah Van Til

This Source Code Form is subject to the terms of the Mozilla Public
License, v2. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.

This software is provided "as is", without warranty of any kind,
express or implied, including but not limited to the warranties
of merchantability, fitness for a particular purpose and noninfringement.
===========================================================================
*/

using System;
using Nomad.Save.Private.Serialization.Streams;

namespace Nomad.Save.Private.ValueObjects {
	public readonly record struct SaveHeader(
		GameVersion Version,
		int SectionCount,
		Checksum Checksum
	) : IEquatable<SaveHeader> {
		/*
		===============
		Serialize
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="writer"></param>
		internal void Serialize( SaveStreamWriter writer ) {
			Version.Serialize( writer );
			writer.Write( SectionCount );
			writer.Write( Checksum.Value );
		}

		/*
		===============
		Serialize
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		internal static SaveHeader Deserialize( SaveStreamReader reader ) {;
			GameVersion version = GameVersion.Deserialize( reader );
			int sectionCount = reader.Read<int>();
			uint checksum = reader.Read<uint>();

			return new SaveHeader {
				Version = version,
				SectionCount = sectionCount,
				Checksum = new Checksum { Value = checksum }
			};
		}
	};
};
