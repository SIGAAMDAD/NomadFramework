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

using System.Runtime.CompilerServices;
using Nomad.Core.FileSystem;

namespace Nomad.Save.Private.ValueObjects {
	/*
	===================================================================================
	
	GameVersion
	
	===================================================================================
	*/
	/// <summary>
	/// Represents the serialized game version in integer format.
	/// </summary>
	
	public readonly struct GameVersion {
		public readonly uint Major;
		public readonly uint Minor;
		public readonly ulong Patch;

		/*
		===============
		GameVersion
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="major"></param>
		/// <param name="minor"></param>
		/// <param name="patch"></param>
		public GameVersion( uint major, uint minor, ulong patch ) {
			Major = major;
			Minor = minor;
			Patch = patch;
		}

		/*
		===============
		ToInt
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public ulong ToInt() {
			return Major * 10000 + Minor * 100 + Patch;
		}

		/*
		===============
		Serialize
		===============
		*/
		/// <summary>
		/// Writes the GameVersion to disk.
		/// </summary>
		/// <param name="writer"></param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		internal void Serialize( IWriteStream writer ) {
			writer.WriteUInt32( Major );
			writer.WriteUInt32( Minor );
			writer.WriteUInt64( Patch );
		}

		/*
		===============
		Deserialize
		===============
		*/
		/// <summary>
		/// Reads the GameVersion from disk.
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		internal static GameVersion Deserialize( IReadStream reader ) {
			uint major = reader.ReadUInt32();
			uint minor = reader.ReadUInt32();
			ulong patch = reader.ReadUInt64();

			return new GameVersion(
				major: major,
				minor: minor,
				patch: patch
			);
		}
	};
};
