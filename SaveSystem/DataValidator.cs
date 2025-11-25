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

using System;
using System.Buffers;
using System.IO.Hashing;
using System.Runtime.CompilerServices;

namespace SaveSystem {
	/*
	===================================================================================
	
	DataChecksum
	
	===================================================================================
	*/
	/// <summary>
	/// Runs a 64-bit cyclic redundancy check (CRC64) on the provided save data buffer to
	/// make absolutely sure we don't have any save file corruption. If we're being handed
	/// a stream writer, then we just calculate the checksum.
	/// </summary>

	internal readonly ref struct DataChecksum {
		/// <summary>
		/// The calculated checksum.
		/// </summary>
		public readonly ulong Checksum = 0;

		/*
		===============
		DataChecksum
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="stream"></param>
		public DataChecksum( Streams.SaveReaderStream? stream ) {
			ArgumentNullException.ThrowIfNull( stream );

			int bytesToRead = stream.Length - stream.Position;
			byte[] buffer = ArrayPool<byte>.Shared.Rent( bytesToRead );
			stream.ReadExactly( buffer, 0, bytesToRead );

			Checksum = CalcCrc64( buffer.AsSpan( bytesToRead ) );
			ArrayPool<byte>.Shared.Return( buffer );
		}

		/*
		===============
		DataChecksum
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="stream"></param>
		public DataChecksum( Streams.SaveWriterStream? stream ) {
			ArgumentNullException.ThrowIfNull( stream );

			Checksum = CalcCrc64( stream.Buffer );
		}

		/*
		===============
		CalcCrc64
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="buffer"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private ulong CalcCrc64( in Span<byte> buffer ) {
			Crc64 check = new Crc64();
			check.Append( buffer );
			return BitConverter.ToUInt64( check.GetHashAndReset() );
		}
	};
};