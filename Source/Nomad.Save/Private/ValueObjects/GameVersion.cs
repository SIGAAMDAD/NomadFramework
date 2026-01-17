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

using Nomad.Save.Private.Serialization.Streams;

namespace Nomad.Save.Private.ValueObjects {
	public readonly record struct GameVersion(
		int Major,
		int Minor,
		int Patch
	) {
		public int ToInt() {
			return Major * 10000 + Minor * 100 + Patch;
		}

		internal void Serialize( SaveStreamWriter writer ) {
			writer.Write( Major );
			writer.Write( Minor );
			writer.Write( Patch );
		}

		internal static GameVersion Deserialize( SaveStreamReader reader ) {
			int major = reader.Read<int>();
			int minor = reader.Read<int>();
			int patch = reader.Read<int>();

			return new GameVersion {
				Major = major,
				Minor = minor,
				Patch = patch
			};
		}
	};
};
