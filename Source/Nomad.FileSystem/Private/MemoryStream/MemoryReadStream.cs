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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nomad.Core.FileSystem;

namespace Nomad.FileSystem.Private.MemoryStream {
	/*
	===================================================================================

	MemoryReadStream

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	public class MemoryReadStream : MemoryStreamBase, IReadStream {
		public override bool CanRead => true;
		public override bool CanWrite => false;

		protected byte[] _buffer;

		/*
		===============
		MemoryReadStream
		===============
		*/
		/// <summary>
		///
		/// </summary>
		internal MemoryReadStream() {
		}

		/*
		===============
		MemoryReadStream
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="length"></param>
		public MemoryReadStream( int length ) {
		}

		/*
		===============
		MemoryReadStream
		===============
		*/
		/// <summary>
		/// Initializes a new instance of the MemoryReadStream class with the specified buffer and length.
		/// </summary>
		/// <param name="buffer">The byte array to read from.</param>
		/// <param name="length">The length of the data in the buffer.</param>
		public MemoryReadStream( byte[] buffer, int length ) {
			_buffer = buffer;
			_length = length;
			_position = 0;
		}

		/*
		===============
		Flush
		===============
		*/
		/// <summary>
		/// Flushes the stream. (Not implemented for read-only streams)
		/// </summary>
		/// <exception cref="NotImplementedException"></exception>
		public override void Flush() {
			throw new NotImplementedException();
		}

		/*
		===============
		FlushAsync
		===============
		*/
		/// <summary>
		/// Asynchronously flushes the stream. (Not implemented for read-only streams)
		/// </summary>
		/// <param name="ct">A token to cancel the operation.</param>
		/// <returns>A task that represents the asynchronous flush operation.</returns>
		/// <exception cref="NotImplementedException"></exception>
		public override ValueTask FlushAsync( CancellationToken ct = default( CancellationToken ) ) {
			throw new NotImplementedException();
		}

		/*
		===============
		Read
		===============
		*/
		/// <summary>
		/// Reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.
		/// </summary>
		/// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between offset and (offset + count - 1) replaced by the bytes read from the current source.</param>
		/// <param name="offset">The zero-based byte offset in buffer at which to begin storing the data read from the current stream.</param>
		/// <param name="count">The maximum number of bytes to be read from the current stream.</param>
		/// <returns>The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not currently available, or zero if the end of the stream has been reached.</returns>
		public int Read( byte[] buffer, int offset, int count ) {
			if ( offset + count >= buffer.Length ) {
				count = buffer.Length - offset;
			}
			unsafe {
				fixed ( byte* dst = buffer )
				fixed ( byte* src = _buffer ) {
					Buffer.MemoryCopy( src + _position, dst + offset, buffer.Length, count );
				}
			}
			_position += count;
			return count;
		}

		/*
		===============
		Read
		===============
		*/
		/// <summary>
		/// Reads a sequence of bytes from the current stream into a span and advances the position within the stream by the number of bytes read.
		/// </summary>
		/// <param name="buffer">A span of bytes. When this method returns, the span contains the bytes read from the current source.</param>
		/// <returns>The total number of bytes read into the span. This can be less than the length of the span if that many bytes are not currently available, or zero if the end of the stream has been reached.</returns>
		public int Read( Span<byte> buffer ) {
			int count = buffer.Length;
			if ( count > _length - _position ) {
				count = _length - _position;
			}
			unsafe {
				fixed ( byte* dst = buffer )
				fixed ( byte* src = _buffer ) {
					Buffer.MemoryCopy( src + _position, dst, buffer.Length, count );
				}
			}
			_position += count;
			return count;
		}

		/*
		===============
		ReadAsync
		===============
		*/
		/// <summary>
		/// Asynchronously reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.
		/// </summary>
		/// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between offset and (offset + count - 1) replaced by the bytes read from the current source.</param>
		/// <param name="offset">The zero-based byte offset in buffer at which to begin storing the data read from the current stream.</param>
		/// <param name="count">The maximum number of bytes to be read from the current stream.</param>
		/// <param name="ct">A token to cancel the operation.</param>
		/// <returns>A task that represents the asynchronous read operation, containing the total number of bytes read into the buffer.</returns>
		public async ValueTask<int> ReadAsync( byte[] buffer, int offset, int count, CancellationToken ct = default( CancellationToken ) ) {
			ct.ThrowIfCancellationRequested();
			return Read( buffer, offset, count );
		}

		/*
		===============
		ReadAsync
		===============
		*/
		/// <summary>
		/// Asynchronously reads a sequence of bytes from the current stream into a memory buffer and advances the position within the stream by the number of bytes read.
		/// </summary>
		/// <param name="buffer">A memory buffer. When this method returns, the buffer contains the bytes read from the current source.</param>
		/// <param name="ct">A token to cancel the operation.</param>
		/// <returns>A task that represents the asynchronous read operation, containing the total number of bytes read into the buffer.</returns>
		public async ValueTask<int> ReadAsync( Memory<byte> buffer, CancellationToken ct = default( CancellationToken ) ) {
			ct.ThrowIfCancellationRequested();
			return Read( buffer.Span );
		}

		/*
		===============
		ReadToEnd
		===============
		*/
		/// <summary>
		/// Reads all remaining bytes from the current position to the end of the stream.
		/// </summary>
		/// <returns>A byte array containing the remaining data in the stream.</returns>
		public byte[] ReadToEnd() {
			int remaining = _length - _position;
			byte[] result = new byte[remaining];
			Buffer.BlockCopy(_buffer, _position, result, 0, remaining);
			_position += remaining;
			return result;
		}

		/*
		===============
		ReadToEndAsync
		===============
		*/
		/// <summary>
		/// Asynchronously reads all remaining bytes from the current position to the end of the stream.
		/// </summary>
		/// <param name="cancellationToken">A token to cancel the operation.</param>
		/// <returns>A task that represents the asynchronous read operation, containing a byte array with the remaining data.</returns>
		public ValueTask<byte[]> ReadToEndAsync( CancellationToken cancellationToken = default( CancellationToken ) ) {
			cancellationToken.ThrowIfCancellationRequested();
			return new ValueTask<byte[]>(ReadToEnd());
		}

		/*
		===============
		ToArray
		===============
		*/
		/// <summary>
		/// Returns the underlying buffer as a byte array.
		/// </summary>
		/// <returns>The underlying byte array.</returns>
		public byte[] ToArray() {
			return _buffer;
		}

		/*
		===============
		ReadSByte
		===============
		*/
		/// <summary>
		/// Reads a signed byte from the stream.
		/// </summary>
		/// <returns>The signed byte value read from the stream.</returns>
		public sbyte ReadSByte() {
			return Read<sbyte>();
		}

		/*
		===============
		ReadShort
		===============
		*/
		/// <summary>
		/// Reads a 16-bit signed integer from the stream.
		/// </summary>
		/// <returns>The 16-bit signed integer value read from the stream.</returns>
		public short ReadShort() {
			return Read<short>();
		}

		/*
		===============
		ReadInt
		===============
		*/
		/// <summary>
		/// Reads a 32-bit signed integer from the stream.
		/// </summary>
		/// <returns>The 32-bit signed integer value read from the stream.</returns>
		public int ReadInt() {
			return Read<int>();
		}

		/*
		===============
		ReadLong
		===============
		*/
		/// <summary>
		/// Reads a 64-bit signed integer from the stream.
		/// </summary>
		/// <returns>The 64-bit signed integer value read from the stream.</returns>
		public long ReadLong() {
			return Read<long>();
		}

		/*
		===============
		ReadByte
		===============
		*/
		/// <summary>
		/// Reads an unsigned byte from the stream.
		/// </summary>
		/// <returns>The unsigned byte value read from the stream.</returns>
		public byte ReadByte() {
			return Read<byte>();
		}

		/*
		===============
		ReadUShort
		===============
		*/
		/// <summary>
		/// Reads a 16-bit unsigned integer from the stream.
		/// </summary>
		/// <returns>The 16-bit unsigned integer value read from the stream.</returns>
		public ushort ReadUShort() {
			return Read<ushort>();
		}

		/*
		===============
		ReadUInt
		===============
		*/
		/// <summary>
		/// Reads a 32-bit unsigned integer from the stream.
		/// </summary>
		/// <returns>The 32-bit unsigned integer value read from the stream.</returns>
		public uint ReadUInt() {
			return Read<uint>();
		}

		/*
		===============
		ReadULong
		===============
		*/
		/// <summary>
		/// Reads a 64-bit unsigned integer from the stream.
		/// </summary>
		/// <returns>The 64-bit unsigned integer value read from the stream.</returns>
		public ulong ReadULong() {
			return Read<ulong>();
		}

		/*
		===============
		ReadInt8
		===============
		*/
		/// <summary>
		/// Reads an 8-bit signed integer from the stream.
		/// </summary>
		/// <returns>The 8-bit signed integer value read from the stream.</returns>
		public sbyte ReadInt8() {
			return Read<sbyte>();
		}

		/*
		===============
		ReadInt16
		===============
		*/
		/// <summary>
		/// Reads a 16-bit signed integer from the stream.
		/// </summary>
		/// <returns>The 16-bit signed integer value read from the stream.</returns>
		public short ReadInt16() {
			return Read<short>();
		}

		/*
		===============
		ReadInt32
		===============
		*/
		/// <summary>
		/// Reads a 32-bit signed integer from the stream.
		/// </summary>
		/// <returns>The 32-bit signed integer value read from the stream.</returns>
		public int ReadInt32() {
			return Read<int>();
		}

		/*
		===============
		ReadInt64
		===============
		*/
		/// <summary>
		/// Reads a 64-bit signed integer from the stream.
		/// </summary>
		/// <returns>The 64-bit signed integer value read from the stream.</returns>
		public long ReadInt64() {
			return Read<long>();
		}

		/*
		===============
		ReadUInt8
		===============
		*/
		/// <summary>
		/// Reads an 8-bit unsigned integer from the stream.
		/// </summary>
		/// <returns>The 8-bit unsigned integer value read from the stream.</returns>
		public byte ReadUInt8() {
			return Read<byte>();
		}

		/*
		===============
		ReadUInt16
		===============
		*/
		/// <summary>
		/// Reads a 16-bit unsigned integer from the stream.
		/// </summary>
		/// <returns>The 16-bit unsigned integer value read from the stream.</returns>
		public ushort ReadUInt16() {
			return Read<ushort>();
		}

		/*
		===============
		ReadUInt32
		===============
		*/
		/// <summary>
		/// Reads a 32-bit unsigned integer from the stream.
		/// </summary>
		/// <returns>The 32-bit unsigned integer value read from the stream.</returns>
		public uint ReadUInt32() {
			return Read<uint>();
		}

		/*
		===============
		ReadUInt64
		===============
		*/
		/// <summary>
		/// Reads a 64-bit unsigned integer from the stream.
		/// </summary>
		/// <returns>The 64-bit unsigned integer value read from the stream.</returns>
		public ulong ReadUInt64() {
			return Read<ulong>();
		}

		/*
		===============
		ReadFloat
		===============
		*/
		/// <summary>
		/// Reads a 32-bit floating-point number from the stream.
		/// </summary>
		/// <returns>The 32-bit floating-point value read from the stream.</returns>
		public float ReadFloat() {
			return Read<float>();
		}

		/*
		===============
		ReadSingle
		===============
		*/
		/// <summary>
		/// Reads a single-precision floating-point number from the stream.
		/// </summary>
		/// <returns>The single-precision floating-point value read from the stream.</returns>
		public float ReadSingle() {
			return Read<float>();
		}

		/*
		===============
		ReadDouble
		===============
		*/
		/// <summary>
		/// Reads a double-precision floating-point number from the stream.
		/// </summary>
		/// <returns>The double-precision floating-point value read from the stream.</returns>
		public double ReadDouble() {
			return Read<double>();
		}

		/*
		===============
		ReadFloat32
		===============
		*/
		/// <summary>
		/// Reads a 32-bit floating-point number from the stream.
		/// </summary>
		/// <returns>The 32-bit floating-point value read from the stream.</returns>
		public float ReadFloat32() {
			return Read<float>();
		}

		/*
		===============
		ReadFloat64
		===============
		*/
		/// <summary>
		/// Reads a 64-bit floating-point number from the stream.
		/// </summary>
		/// <returns>The 64-bit floating-point value read from the stream.</returns>
		public double ReadFloat64() {
			return Read<double>();
		}

		/*
		===============
		ReadString
		===============
		*/
		/// <summary>
		/// Reads a UTF-8 encoded string from the stream, prefixed with its length as a 7-bit encoded integer.
		/// </summary>
		/// <returns>The string read from the stream.</returns>
		public string ReadString() {
			int byteCount = Read7BitEncodedInt();
			string value = Encoding.UTF8.GetString( _buffer, _position, byteCount );
			_position += byteCount;
			return value;
		}

		/*
		===============
		Read7BitEncodedInt
		===============
		*/
		/// <summary>
		/// Reads an encoded/compressed integer from the save file stream.
		/// </summary>
		/// <returns></returns>
		/// <exception cref="FormatException"></exception>
		public int Read7BitEncodedInt() {
			int value = 0;
			int shift = 0;
			byte b;

			do {
				b = Read<byte>();
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
		Read
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private T Read<T>() where T : unmanaged {
			int sizeOfData = Marshal.SizeOf<T>();
			T value = Unsafe.ReadUnaligned<T>( ref _buffer[ Position ] );
			_position += sizeOfData;
			return value;
		}
	};
};
