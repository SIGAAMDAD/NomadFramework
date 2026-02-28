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
using Nomad.Core.FileSystem.Streams;
using Nomad.Save.ValueObjects;

namespace Nomad.Save.Private.ValueObjects {
	/// <summary>
	/// 
	/// </summary>
	internal readonly struct BackupData {
		/// <summary>
		/// The API version we used when creating the backup.
		/// </summary>
		public readonly SaveFileVersion Version { get; }

		/// <summary>
		/// The Id of the save file we're backing up.
		/// </summary>
		public readonly ulong Guid { get; }

		/// <summary>
		/// When the backup was created.
		/// </summary>
		public readonly DateTime CreatedAt { get; }

		/// <summary>
		/// The backup's index in the chain.
		/// </summary>
		public readonly int Number { get; }

		/*
		===============
		BackupData
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="version"></param>
		/// <param name="guid"></param>
		/// <param name="createdAt"></param>
		/// <param name="number"></param>
		public BackupData( SaveFileVersion version, ulong guid, DateTime createdAt, int number ) {
			Version = version;
			Guid = guid;
			CreatedAt = createdAt;
			Number = number;
		}

		/*
		===============
		Serialize
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="stream"></param>
		public void Serialize( IWriteStream stream ) {
			Version.Serialize( stream );
			stream.WriteUInt64( Guid );
			stream.WriteInt64( CreatedAt.ToFileTimeUtc() );
			stream.WriteInt32( Number );
		}

		/*
		===============
		Deserialize
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="stream"></param>
		/// <returns></returns>
		public static BackupData Deserialize( IReadStream stream ) {
			return new BackupData(
				SaveFileVersion.Deserialize( stream ),
				stream.ReadUInt64(),
				DateTime.FromFileTimeUtc( stream.ReadInt64() ),
				stream.ReadInt32()
			);
		}
	};
};
