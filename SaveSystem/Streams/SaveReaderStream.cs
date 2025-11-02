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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace SaveSystem.Streams {
	/*
	===================================================================================
	
	SaveReaderStream
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	/// <remarks>
	/// Reads strings in <see cref="Encoding.UTF8"/> format.
	/// </remarks>

	public sealed class SaveReaderStream : SaveStream {
		/// <summary>
		/// The length of the stream (the file's size).
		/// </summary>
		public readonly int Length = 0;

		/// <summary>
		/// 
		/// </summary>
		public override byte[]? Buffer {
			get => _buffer;
		}
		private readonly byte[] _buffer;

		/*
		===============
		SaveReaderStream
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="stream"></param>
		public SaveReaderStream( System.IO.FileStream? stream ) {
			ArgumentNullException.ThrowIfNull( stream );

			Length = (int)( stream.Length - stream.Position );
			_buffer = ArrayPool<byte>.Shared.Rent( Length );
			stream.ReadExactly( _buffer, 0, Length );

			// kill the stream now that we've read it all
			stream.Dispose();
		}

		/*
		===============
		Dispose
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		public override void Dispose() {
			base.Dispose();
			if ( Buffer != null ) {
				ArrayPool<byte>.Shared.Return( Buffer );
			}
		}

		/*
		===============
		ReadInt8
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public sbyte ReadInt8() {
			return ReadPrimitive<sbyte>();
		}

		/*
		===============
		ReadInt16
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public short ReadInt16() {
			return ReadPrimitive<short>();
		}

		/*
		===============
		ReadInt32
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public int ReadInt32() {
			return ReadPrimitive<int>();
		}

		/*
		===============
		ReadInt64
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public long ReadInt64() {
			return ReadPrimitive<long>();
		}

		/*
		===============
		ReadUInt8
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public byte ReadUInt8() {
			return ReadPrimitive<byte>();
		}

		/*
		===============
		ReadUInt16
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public ushort ReadUInt16() {
			return ReadPrimitive<ushort>();
		}

		/*
		===============
		ReadUInt32
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public uint ReadUInt32() {
			return ReadPrimitive<uint>();
		}

		/*
		===============
		ReadUInt64
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public ulong ReadUInt64() {
			return ReadPrimitive<ulong>();
		}

		/*
		===============
		ReadFloat
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public float ReadFloat() {
			return ReadPrimitive<float>();
		}

		/*
		===============
		ReadDouble
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public double ReadDouble() {
			return ReadPrimitive<double>();
		}

		/*
		===============
		ReadBoolean
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public bool ReadBoolean() {
			return ReadPrimitive<bool>();
		}

		/*
		===============
		Read7BitEncodedInt
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		/// <exception cref="FormatException"></exception>
		public int Read7BitEncodedInt() {
			int value = 0;
			int shift = 0;
			byte b;

			do {
				b = ReadUInt8();
				value |= ( b & 0x7F ) << shift;
				shift += 7;
				if ( shift > 35 ) {
					throw new FormatException( "Invalid 7-bit encoded integer formatting in save file." );
				}
			} while ( ( b & 0x80 ) != 0 );

			return value;
		}

		/*
		===============
		ReadString
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public string ReadString() {
			int byteCount = Read7BitEncodedInt();
			CheckRead( byteCount );
			string value = Encoding.UTF8.GetString( new ReadOnlySpan<byte>( ref _buffer[ Position ] ).Slice( Position, byteCount ) );
			Position += value.Length;
			return value;
		}

		/*
		===============
		ReadBuffer
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="offset"></param>
		/// <param name="length"></param>
		public void ReadExactly( byte[] buffer, int offset, int length ) {
			ArgumentNullException.ThrowIfNull( buffer );
			ArgumentOutOfRangeException.ThrowIfLessThan( offset, 0 );
			ArgumentOutOfRangeException.ThrowIfLessThan( length, 0 );
			ArgumentOutOfRangeException.ThrowIfGreaterThan( offset + length, buffer.Length );

			CheckRead( length - offset );
			System.Buffer.BlockCopy( _buffer, Position, buffer, offset, length );
			Position += length;
		}

		/*
		===============
		CheckRead
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="size"></param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private void CheckRead( int size ) {
			if ( Position + size >= _buffer.Length ) {
				throw new System.IO.EndOfStreamException( "End of save reader stream hit!" );
			}
		}

		/*
		===============
		ReadPrimitive
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private T ReadPrimitive<T>() where T : unmanaged {
			int sizeOfData = Marshal.SizeOf<T>();

			CheckRead( sizeOfData );
			T value = Unsafe.ReadUnaligned<T>( ref _buffer[ Position ] );
			Position += sizeOfData;
			return value;
		}
	};
};