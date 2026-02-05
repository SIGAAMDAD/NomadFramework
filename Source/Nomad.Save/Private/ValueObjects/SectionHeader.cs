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

using System.IO;
using System.Runtime.CompilerServices;
using Nomad.Core.FileSystem;
using Nomad.Save.Private.Exceptions;

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
		private const int SECTION_NAME_MAX_LENGTH = 128;

		/// <summary>
		/// The section's name.
		/// </summary>
		public readonly string Name;

		/// <summary>
		/// The amount of written primitives in the section.
		/// </summary>
		public readonly int FieldCount;

		/// <summary>
		/// The section's CRC64
		/// </summary>
		public readonly ulong Checksum;

		/*
		===============
		SectionHeader
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="fieldCount"></param>
		/// <param name="checksum"></param>
		public SectionHeader( string name, int fieldCount, ulong checksum ) {
			Name = name;
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
			stream.WriteInt32( FieldCount );
			stream.WriteUInt64( Checksum );
		}

		/*
		===============
		Load
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="stream"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static SectionHeader Load( in IReadStream stream ) {
			string name = stream.ReadString();
			if ( name.Length <= 0 || name.Length > SECTION_NAME_MAX_LENGTH ) {
				throw new IOException( $"Section name corrupt or too long ({name.Length})" );
			}

			int fieldCount = stream.ReadInt32();
			if ( fieldCount < 0 ) {
				throw new FailedSectionLoadException( name, new System.Exception( "field count is corrupt" ) );
			}

			return new SectionHeader( name, fieldCount, stream.ReadUInt64() );
		}
	};
};
