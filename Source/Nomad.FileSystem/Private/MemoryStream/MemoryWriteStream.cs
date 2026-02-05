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
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nomad.Core.Compatibility;
using Nomad.Core.FileSystem;
using Nomad.FileSystem.Private.MemoryStream;

namespace Nomad.FileSystem.Private {
	/*
	===================================================================================

	MemoryWriteStream

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	public class MemoryWriteStream : MemoryStreamBase, IWriteStream {
		private const int MAX_CAPACITY = 1 * 1024 * 1024 * 1024;
		private const int STACK_ALLOC_THRESHOLD = 256;
		public const int DEFAULT_CAPACITY = 8192;

		public override bool CanRead => false;
		public override bool CanWrite => true;

		protected byte[] _buffer;
		private bool _fixedSize;

		/*
		===============
		MemoryWriteStream
		===============
		*/
		/// <summary>
		/// Initializes a new instance of the MemoryWriteStream class with the specified initial length and fixed size option.
		/// </summary>
		/// <param name="length">The initial length of the buffer.</param>
		/// <param name="fixedSize">Whether the buffer size is fixed or can grow.</param>
		public MemoryWriteStream( int length, bool fixedSize = false ) {
			_buffer = ArrayPool<byte>.Shared.Rent( length );
			_fixedSize = fixedSize;

			_length = length;
			_position = 0;
		}

		/*
		===============
		Dispose
		===============
		*/
		/// <summary>
		/// Releases the resources used by the MemoryWriteStream.
		/// </summary>
		public override void Dispose() {
			if ( _buffer != null ) {
				ArrayPool<byte>.Shared.Return( _buffer );
				_buffer = null;
			}
			GC.SuppressFinalize( this );
		}

		/*
		===============
		DisposeAsync
		===============
		*/
		/// <summary>
		/// Asynchronously releases the resources used by the MemoryWriteStream.
		/// </summary>
		/// <returns>A task that represents the asynchronous dispose operation.</returns>
		public override async ValueTask DisposeAsync() {
			if ( _buffer != null ) {
				ArrayPool<byte>.Shared.Return( _buffer );
				_buffer = null;
			}
			GC.SuppressFinalize( this );
		}

		/*
		===============
		Flush
		===============
		*/
		/// <summary>
		/// Flushes the stream. This operation is a no-op for memory streams.
		/// </summary>
		public override void Flush() {
		}

		/*
		===============
		FlushAsync
		===============
		*/
		/// <summary>
		/// Asynchronously flushes the stream. This operation is a no-op for memory streams.
		/// </summary>
		/// <param name="cancellationToken">A token to cancel the operation.</param>
		/// <returns>A task that represents the asynchronous flush operation.</returns>
		public override async ValueTask FlushAsync( CancellationToken cancellationToken = default ) {
		}

		/*
		===============
		Write
		===============
		*/
		/// <summary>
		/// Writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.
		/// </summary>
		/// <param name="buffer">An array of bytes. This method copies count bytes from buffer to the current stream.</param>
		/// <param name="offset">The zero-based byte offset in buffer at which to begin copying bytes to the current stream.</param>
		/// <param name="count">The number of bytes to be written to the current stream.</param>
		public void Write( byte[] buffer, int offset, int count ) {
			ExceptionCompat.ThrowIfNull( _buffer );
			ExceptionCompat.ThrowIfNull( buffer );

			EnsureCapacity( count );
			Buffer.BlockCopy( buffer, offset, _buffer, _position, count );
			_position += count;
		}

		/*
		===============
		Write
		===============
		*/
		/// <summary>
		/// Writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.
		/// </summary>
		/// <param name="buffer">A read-only span of bytes. This method copies the contents of the span to the current stream.</param>
		public void Write( ReadOnlySpan<byte> buffer ) {
			ExceptionCompat.ThrowIfNull( _buffer );

			int count = buffer.Length;
			EnsureCapacity( count );
			buffer.CopyTo( _buffer.AsSpan( _position ) );
			_position += count;
		}

		/*
		===============
		WriteAsync
		===============
		*/
		/// <summary>
		/// Asynchronously writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.
		/// </summary>
		/// <param name="buffer">An array of bytes. This method copies count bytes from buffer to the current stream.</param>
		/// <param name="offset">The zero-based byte offset in buffer at which to begin copying bytes to the current stream.</param>
		/// <param name="count">The number of bytes to be written to the current stream.</param>
		/// <param name="cancellationToken">A token to cancel the operation.</param>
		/// <returns>A task that represents the asynchronous write operation.</returns>
		public ValueTask WriteAsync( byte[] buffer, int offset, int count, CancellationToken cancellationToken = default( CancellationToken ) ) {
			cancellationToken.ThrowIfCancellationRequested();
			Write( buffer, offset, count );
			return default;
		}

		/*
		===============
		WriteAsync
		===============
		*/
		/// <summary>
		/// Asynchronously writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.
		/// </summary>
		/// <param name="buffer">A read-only memory buffer. This method copies the contents of the buffer to the current stream.</param>
		/// <param name="cancellationToken">A token to cancel the operation.</param>
		/// <returns>A task that represents the asynchronous write operation.</returns>
		public ValueTask WriteAsync( ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default( CancellationToken ) ) {
			cancellationToken.ThrowIfCancellationRequested();
			Write( buffer.Span );
			return default;
		}

		/*
		===============
		WriteByte
		===============
		*/
		/// <summary>
		/// Writes a byte to the current stream and advances the current position within this stream by one byte.
		/// </summary>
		/// <param name="value">The byte to write to the stream.</param>
		public void WriteByte( byte value ) {
			Write( value );
		}

		/*
		===============
		WriteFromStream
		===============
		*/
		/// <summary>
		/// Reads all bytes from the specified read stream and writes them to the current stream.
		/// </summary>
		/// <param name="stream">The read stream to copy from.</param>
		public void WriteFromStream( IReadStream stream ) {
			ExceptionCompat.ThrowIfNull( stream );

			byte[] buffer = ArrayPool<byte>.Shared.Rent( 4096 );
			try {
				int bytesRead;
				while ( ( bytesRead = stream.Read( buffer, 0, buffer.Length ) ) > 0 ) {
					Write( buffer, 0, bytesRead );
				}
			} finally {
				ArrayPool<byte>.Shared.Return( buffer );
			}
		}

		/*
		===============
		WriteFromStreamAsync
		===============
		*/
		/// <summary>
		/// Asynchronously reads all bytes from the specified read stream and writes them to the current stream.
		/// </summary>
		/// <param name="stream">The read stream to copy from.</param>
		/// <param name="cancellationToken">A token to cancel the operation.</param>
		/// <returns>A task that represents the asynchronous copy operation.</returns>
		public async ValueTask WriteFromStreamAsync( IReadStream stream, CancellationToken cancellationToken = default( CancellationToken ) ) {
			ExceptionCompat.ThrowIfNull( stream );

			byte[] buffer = ArrayPool<byte>.Shared.Rent( 4096 );
			try {
				int bytesRead;
				while ( ( bytesRead = await stream.ReadAsync( buffer, 0, buffer.Length, cancellationToken ) ) > 0 ) {
					Write( buffer, 0, bytesRead );
				}
			} finally {
				ArrayPool<byte>.Shared.Return( buffer );
			}
		}

		/*
		===============
		Write
		===============
		*/
		/// <summary>
		/// Writes a UTF-8 encoded string to the stream, prefixed with its length as a 7-bit encoded integer.
		/// </summary>
		/// <param name="value">The string to write.</param>
		public void Write( string? value ) {
			ExceptionCompat.ThrowIfNull( _buffer );
			ExceptionCompat.ThrowIfNull( value );

			int maxByteCount = Encoding.UTF8.GetMaxByteCount( value.Length );

			if ( maxByteCount <= STACK_ALLOC_THRESHOLD ) {
				Span<byte> tempBuffer = stackalloc byte[ maxByteCount ];
				int actualByteCount = Encoding.UTF8.GetBytes( value, tempBuffer );
				Write7BitEncodedInt( actualByteCount );
				EnsureCapacity( actualByteCount );
				tempBuffer[ ..actualByteCount ].CopyTo( _buffer.AsSpan( _position ) );
				_position += actualByteCount;
			} else {
				int byteCount = Encoding.UTF8.GetByteCount( value );
				Write7BitEncodedInt( byteCount );
				EnsureCapacity( byteCount );
				_position += Encoding.UTF8.GetBytes( value, 0, value.Length, _buffer, _position );
			}
		}

		/*
		===============
		Write7BitEncodedInt
		===============
		*/
		/// <summary>
		/// Writes an integer to the stream using 7-bit encoded format.
		/// </summary>
		/// <param name="value">The integer value to write.</param>
		public void Write7BitEncodedInt( int value ) {
			ExceptionCompat.ThrowIfNull( _buffer );

			uint uValue = (uint)value;
			while ( uValue >= 0x80 ) {
				Write( (byte)( uValue | 0x80 ) );
				uValue >>= 7;
			}
			Write( (byte)uValue );
		}

		/*
		===============
		Write
		===============
		*/
		/// <summary>
		/// Writes a value of an unmanaged type to the stream.
		/// </summary>
		/// <typeparam name="T">The type of the value to write.</typeparam>
		/// <param name="value">The value to write.</param>
		public void Write<T>( T value ) where T : unmanaged {
			ExceptionCompat.ThrowIfNull( _buffer );

			int sizeOfData = Marshal.SizeOf<T>();
			EnsureCapacity( sizeOfData );
			Unsafe.WriteUnaligned( ref _buffer[ _position ], value );
			_position += sizeOfData;
		}

		/*
		===============
		WriteSByte
		===============
		*/
		/// <summary>
		/// Writes a signed byte to the stream.
		/// </summary>
		/// <param name="value">The signed byte value to write.</param>
		public void WriteSByte( sbyte value ) {
			Write( value );
		}

		/*
		===============
		WriteShort
		===============
		*/
		/// <summary>
		/// Writes a 16-bit signed integer to the stream.
		/// </summary>
		/// <param name="value">The 16-bit signed integer value to write.</param>
		public void WriteShort( short value ) {
			Write( value );
		}

		/*
		===============
		WriteInt
		===============
		*/
		/// <summary>
		/// Writes a 32-bit signed integer to the stream.
		/// </summary>
		/// <param name="value">The 32-bit signed integer value to write.</param>
		public void WriteInt( int value ) {
			Write( value );
		}

		/*
		===============
		WriteLong
		===============
		*/
		/// <summary>
		/// Writes a 64-bit signed integer to the stream.
		/// </summary>
		/// <param name="value">The 64-bit signed integer value to write.</param>
		public void WriteLong( long value ) {
			Write( value );
		}

		/*
		===============
		WriteUShort
		===============
		*/
		/// <summary>
		/// Writes a 16-bit unsigned integer to the stream.
		/// </summary>
		/// <param name="value">The 16-bit unsigned integer value to write.</param>
		public void WriteUShort( ushort value ) {
			Write( value );
		}

		/*
		===============
		WriteUInt
		===============
		*/
		/// <summary>
		/// Writes a 32-bit unsigned integer to the stream.
		/// </summary>
		/// <param name="value">The 32-bit unsigned integer value to write.</param>
		public void WriteUInt( uint value ) {
			Write( value );
		}

		/*
		===============
		WriteULong
		===============
		*/
		/// <summary>
		/// Writes a 64-bit unsigned integer to the stream.
		/// </summary>
		/// <param name="value">The 64-bit unsigned integer value to write.</param>
		public void WriteULong( ulong value ) {
			Write( value );
		}

		/*
		===============
		WriteInt8
		===============
		*/
		/// <summary>
		/// Writes an 8-bit signed integer to the stream.
		/// </summary>
		/// <param name="value">The 8-bit signed integer value to write.</param>
		public void WriteInt8( sbyte value ) {
			Write( value );
		}

		/*
		===============
		WriteInt16
		===============
		*/
		/// <summary>
		/// Writes a 16-bit signed integer to the stream.
		/// </summary>
		/// <param name="value">The 16-bit signed integer value to write.</param>
		public void WriteInt16( short value ) {
			Write( value );
		}

		/*
		===============
		WriteInt32
		===============
		*/
		/// <summary>
		/// Writes a 32-bit signed integer to the stream.
		/// </summary>
		/// <param name="value">The 32-bit signed integer value to write.</param>
		public void WriteInt32( int value ) {
			Write( value );
		}

		/*
		===============
		WriteInt64
		===============
		*/
		/// <summary>
		/// Writes a 64-bit signed integer to the stream.
		/// </summary>
		/// <param name="value">The 64-bit signed integer value to write.</param>
		public void WriteInt64( long value ) {
			Write( value );
		}

		/*
		===============
		WriteUInt8
		===============
		*/
		/// <summary>
		/// Writes an 8-bit unsigned integer to the stream.
		/// </summary>
		/// <param name="value">The 8-bit unsigned integer value to write.</param>
		public void WriteUInt8( byte value ) {
			Write( value );
		}

		/*
		===============
		WriteUInt16
		===============
		*/
		/// <summary>
		/// Writes a 16-bit unsigned integer to the stream.
		/// </summary>
		/// <param name="value">The 16-bit unsigned integer value to write.</param>
		public void WriteUInt16( ushort value ) {
			Write( value );
		}

		/*
		===============
		WriteUInt32
		===============
		*/
		/// <summary>
		/// Writes a 32-bit unsigned integer to the stream.
		/// </summary>
		/// <param name="value">The 32-bit unsigned integer value to write.</param>
		public void WriteUInt32( uint value ) {
			Write( value );
		}

		/*
		===============
		WriteUInt64
		===============
		*/
		/// <summary>
		/// Writes a 64-bit unsigned integer to the stream.
		/// </summary>
		/// <param name="value">The 64-bit unsigned integer value to write.</param>
		public void WriteUInt64( ulong value ) {
			Write( value );
		}

		/*
		===============
		WriteFloat
		===============
		*/
		/// <summary>
		/// Writes a 32-bit floating-point number to the stream.
		/// </summary>
		/// <param name="value">The 32-bit floating-point value to write.</param>
		public void WriteFloat( float value ) {
			Write( value );
		}

		/*
		===============
		WriteSingle
		===============
		*/
		/// <summary>
		/// Writes a single-precision floating-point number to the stream.
		/// </summary>
		/// <param name="value">The single-precision floating-point value to write.</param>
		public void WriteSingle( float value ) {
			Write( value );
		}

		/*
		===============
		WriteDouble
		===============
		*/
		/// <summary>
		/// Writes a double-precision floating-point number to the stream.
		/// </summary>
		/// <param name="value">The double-precision floating-point value to write.</param>
		public void WriteDouble( double value ) {
			Write( value );
		}

		/*
		===============
		WriteFloat32
		===============
		*/
		/// <summary>
		/// Writes a 32-bit floating-point number to the stream.
		/// </summary>
		/// <param name="value">The 32-bit floating-point value to write.</param>
		public void WriteFloat32( float value ) {
			Write( value );
		}

		/*
		===============
		WriteFloat64
		===============
		*/
		/// <summary>
		/// Writes a 64-bit floating-point number to the stream.
		/// </summary>
		/// <param name="value">The 64-bit floating-point value to write.</param>
		public void WriteFloat64( double value ) {
			Write( value );
		}

		/*
		===============
		WriteBoolean
		===============
		*/
		/// <summary>
		/// Writes an 8-bit boolean value to the stream.
		/// </summary>
		/// <param name="value">The 8-bit boolean value to write</param>
		public void WriteBoolean( bool value ) {
			Write( value );
		}

		/*
		===============
		WriteString
		===============
		*/
		/// <summary>
		/// Writes a UTF-8 encoded string to the stream, prefixed with its length as a 7-bit encoded integer.
		/// </summary>
		/// <param name="value">The string to write.</param>
		public void WriteString( string value ) {
			ExceptionCompat.ThrowIfNull( _buffer );
			ExceptionCompat.ThrowIfNull( value );

			int maxByteCount = Encoding.UTF8.GetMaxByteCount( value.Length );

			if ( maxByteCount <= STACK_ALLOC_THRESHOLD ) {
				Span<byte> tempBuffer = stackalloc byte[ maxByteCount ];
				int actualByteCount = Encoding.UTF8.GetBytes( value, tempBuffer );
				Write7BitEncodedInt( actualByteCount );
				EnsureCapacity( actualByteCount );
				tempBuffer[ ..actualByteCount ].CopyTo( _buffer.AsSpan( _position ) );
				_position += actualByteCount;
			} else {
				int byteCount = Encoding.UTF8.GetByteCount( value );
				Write7BitEncodedInt( byteCount );
				EnsureCapacity( byteCount );
				_position += Encoding.UTF8.GetBytes( value, 0, value.Length, _buffer, _position );
			}
		}

		/*
		===============
		EnsureCapacity
		===============
		*/
		/// <summary>
		/// Ensures we have enough space in the buffer to support the provided required <paramref name="required"/>.
		/// </summary>
		/// <remarks>
		/// We're already checking the <see cref="_buffer"/> is null from all the calling functions, so a null check isn't necessary here.
		/// </remarks>
		/// <param name="required">The bytes needed to write the data.</param>
		private void EnsureCapacity( int required ) {
			if ( _position + required > _buffer.Length ) {
				int newCapacity = _buffer.Length * 2;
				if ( newCapacity >= MAX_CAPACITY ) {
					throw new InvalidOperationException( $"Memory stream size has exceeded {MAX_CAPACITY} bytes... what the hell are you doing?" );
				}

				byte[] newBuffer = ArrayPool<byte>.Shared.Rent( newCapacity );

				Buffer.BlockCopy( _buffer, 0, newBuffer, 0, Position );
				ArrayPool<byte>.Shared.Return( _buffer );
				_buffer = newBuffer;
			}
		}
	};
};
