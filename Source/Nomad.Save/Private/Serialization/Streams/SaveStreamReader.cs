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

namespace Nomad.Save.Private.Serialization.Streams {
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

	internal sealed class SaveStreamReader : ISaveFileStream {
		/// <summary>
		/// The length of the stream (the file's size).
		/// </summary>
		public int Length => _length;
		private readonly int _length = 0;

		/// <summary>
		/// The current offset in the buffer.
		/// </summary>
		public int Position => _position;
		private int _position = 0;

		/// <summary>
		/// The buffer the stream reads from.
		/// </summary>
		public byte[]? Buffer => _buffer;
		private readonly byte[] _buffer;

		/*
		===============
		SaveStreamReader
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <remarks>
		/// Exceptions such as IO or FileNotFound should be handled outside of this class.
		/// </remarks>
		/// <param name="filepath"></param>
		public SaveStreamReader( string filepath ) {
			using System.IO.FileStream stream = new System.IO.FileStream( filepath, System.IO.FileMode.Open, System.IO.FileAccess.Read );

			_length = (int)( stream.Length - stream.Position );
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
		/// Releases the read stream buffer's memory.
		/// </summary>
		public void Dispose() {
			if ( _buffer != null ) {
				ArrayPool<byte>.Shared.Return( _buffer );
			}
		}

		/*
		===============
		Seek
		===============
		*/
		/// <summary>
		/// Sets the stream offset to the desired <paramref name="position"/> based on the <paramref name="origin"/>
		/// </summary>
		/// <param name="position"></param>
		/// <param name="origin"></param>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public void Seek( int position, System.IO.SeekOrigin origin ) {
			ArgumentOutOfRangeException.ThrowIfLessThan( position, 0 );
			ArgumentNullException.ThrowIfNull( Buffer );

			switch ( origin ) {
				case System.IO.SeekOrigin.Begin:
					ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual( position, Buffer.Length );
					_position = position;
					break;
				case System.IO.SeekOrigin.Current:
					ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual( position + Position, Buffer.Length );
					_position += position;
					break;
				case System.IO.SeekOrigin.End:
					ArgumentOutOfRangeException.ThrowIfGreaterThan( position, 0 );
					_position = Buffer.Length - 1;
					break;
				default:
					throw new ArgumentOutOfRangeException( nameof( origin ) );
			}
		}

		/*
		===============
		Reset
		===============
		*/
		/// <summary>
		/// Resets all read progress in the stream.
		/// </summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void Reset() => _position = 0;

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
		ReadString
		===============
		*/
		/// <summary>
		/// Reads a string from the save file stream.
		/// </summary>
		/// <returns></returns>
		public string ReadString() {
			int byteCount = Read7BitEncodedInt();
			CheckRead( byteCount );
			string value = Encoding.UTF8.GetString( _buffer, Position, byteCount );
			_position += value.Length;
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
			_position += length;
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
		Read
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public T Read<T>() where T : unmanaged {
			int sizeOfData = Marshal.SizeOf<T>();

			CheckRead( sizeOfData );
			T value = Unsafe.ReadUnaligned<T>( ref _buffer[ Position ] );
			_position += sizeOfData;
			return value;
		}
	};
};
