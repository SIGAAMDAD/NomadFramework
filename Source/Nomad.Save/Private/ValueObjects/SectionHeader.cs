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
using Nomad.Save.Exceptions;

namespace Nomad.Save.Private.ValueObjects {
	/*
	===================================================================================

	SectionHeader

	===================================================================================
	*/
	/// <summary>
	/// Represents a save section's header containing the metadata.
	/// </summary>

	internal readonly ref struct SectionHeader {
		/// <summary>
		/// The section's name.
		/// </summary>
		public readonly string Name;

		/// <summary>
		/// The amount of written values in the section.
		/// </summary>
		public readonly int FieldCount;

		/// <summary>
		/// The total length of the section, used for corruption checking.
		/// </summary>
		public readonly int ByteLength;

		/// <summary>
		/// The section's CRC64.
		/// </summary>
		public readonly Checksum Checksum;

		/*
		===============
		SectionHeader
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="byteLength"></param>
		/// <param name="fieldCount"></param>
		/// <param name="checksum"></param>
		public SectionHeader( string name, int byteLength, int fieldCount, Checksum checksum ) {
			Name = name;
			ByteLength = byteLength;
			FieldCount = fieldCount;
			Checksum = checksum;
		}

		/*
		===============
		Save
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="stream"></param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void Save( in IWriteStream stream ) {
			stream.WriteString( Name );
			stream.WriteInt32( ByteLength );
			stream.WriteInt32( FieldCount );
			stream.WriteUInt64( Checksum.Value );
		}

		/*
		===============
		Load
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="index"></param>
		/// <param name="stream"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static SectionHeader Load( int index, in IMemoryReadStream stream ) {
			string name = stream.ReadString();
			if ( name.Length <= 0 || name.Length > Constants.SECTION_NAME_MAX_LENGTH ) {
				throw new SectionCorruptException( null, index, stream.Position, $"Section name corrupt or too long ({name.Length})" );
			}

			int byteLength = stream.ReadInt32();
			if ( byteLength < 0 ) {
				throw new SectionCorruptException( name, index, stream.Position, $"Byte length is invalid ({byteLength})" );
			}

			int fieldCount = stream.ReadInt32();
			if ( fieldCount < 0 ) {
				throw new SectionCorruptException( name, index, stream.Position, $"Field count is invalid ({fieldCount})" );
			}
			
			Checksum loadedChecksum = new Checksum( stream.ReadUInt64() );
			Checksum actualChecksum = Checksum.Compute( stream.Buffer.GetSlice( stream.Position, byteLength ) );
			if ( loadedChecksum != actualChecksum ) {
				throw new SectionCorruptException( name, index, stream.Position, $"Section checksum64 does not match the value found in the save file data ({loadedChecksum.Value} != {actualChecksum.Value})" );
			}

			return new SectionHeader( name, byteLength, fieldCount, actualChecksum );
		}
	};
};
