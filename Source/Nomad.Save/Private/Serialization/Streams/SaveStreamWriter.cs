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
using Godot;
using Nomad.Core.Logger;

namespace Nomad.Save.Private.Serialization.Streams {
	/*
	===================================================================================

	SaveStreamWriter

	===================================================================================
	*/
	/// <summary>
	/// An abstracted interface to manage writing a save file to a filestream.
	/// </summary>

	internal sealed class SaveStreamWriter( string filepath, ILoggerService? logger ) : ISaveFileStream {
		// hard limit of 128 MiB
		private const int MAX_CAPACITY = 128 * 1024 * 1024;
		private const int STACK_ALLOC_THRESHOLD = 256;

		public int Position => _position;
		private int _position = 0;

		public int Length => _position;

		public byte[] Buffer => _buffer;
		private byte[] _buffer = new byte[ 8192 ];

		/*
		===============
		Dispose
		===============
		*/
		/// <summary>
		/// Dumps the buffer to the stream.
		/// </summary>
		public void Dispose() {
			if ( _buffer != null ) {
				using System.IO.FileStream writer = new System.IO.FileStream( filepath, System.IO.FileMode.Create, System.IO.FileAccess.Write );

				GD.Print( $"Saving data to {filepath}, stream size is {_position}" );

				writer.Write( _buffer, 0, _position );
				_buffer = null;

				writer?.Dispose();
			}
		}

		/*
		===============
		Write
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="value"></param>
		public void Write( string? value ) {
			ArgumentNullException.ThrowIfNull( _buffer );
			ArgumentNullException.ThrowIfNull( value );

			int maxByteCount = Encoding.UTF8.GetMaxByteCount( value.Length );

			if ( maxByteCount <= STACK_ALLOC_THRESHOLD ) {
				Span<byte> tempBuffer = stackalloc byte[ maxByteCount ];
				int actualByteCount = Encoding.UTF8.GetBytes( value, tempBuffer );
				Write( actualByteCount );
				EnsureCapacity( actualByteCount );
				tempBuffer[ ..actualByteCount ].CopyTo( _buffer.AsSpan( _position ) );
				_position += actualByteCount;
			} else {
				int byteCount = Encoding.UTF8.GetByteCount( value );
				Write( byteCount );
				EnsureCapacity( byteCount );
				_position += Encoding.UTF8.GetBytes( value, 0, value.Length, _buffer, _position );
			}
		}

		/*
		===============
		Write
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="value"></param>
		/// <param name="offset"></param>
		/// <param name="length"></param>
		public void Write( byte[]? value, int offset, int length ) {
			ArgumentNullException.ThrowIfNull( _buffer );
			ArgumentNullException.ThrowIfNull( value );
			ArgumentOutOfRangeException.ThrowIfLessThan( offset, 0 );
			ArgumentOutOfRangeException.ThrowIfLessThan( length, 0 );
			ArgumentOutOfRangeException.ThrowIfGreaterThan( offset + length, value.Length );

			EnsureCapacity( length );
			System.Buffer.BlockCopy( value, offset, _buffer, _position, length );
			_position += length;
		}

		/*
		===============
		Write
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="buffer"></param>
		public void Write( in ReadOnlySpan<byte> buffer ) {
			ArgumentNullException.ThrowIfNull( _buffer );

			EnsureCapacity( _buffer.Length );
			buffer.CopyTo( new Span<byte>( ref _buffer[ Position ] ) );
			_position += buffer.Length;
		}

		/*
		===============
		Write7BitEncodedInt
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="value"></param>
		public void Write7BitEncodedInt( int value ) {
			ArgumentNullException.ThrowIfNull( _buffer );

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
		///
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		public void Write<T>( T value ) where T : unmanaged {
			ArgumentNullException.ThrowIfNull( _buffer );

			int sizeOfData = Marshal.SizeOf<T>();
			EnsureCapacity( sizeOfData );
			Unsafe.WriteUnaligned( ref _buffer[ _position ], value );
			_position += sizeOfData;
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
				int newCapacity = _buffer.Length * 4;
				if ( newCapacity >= MAX_CAPACITY ) {
					throw new InvalidOperationException( $"Save file size has exceeded {MAX_CAPACITY} bytes... what the hell are you saving?" );
				} else if ( newCapacity >= GetFreeDiskSpace( filepath ) ) {
					throw new System.IO.IOException( $"Out of disk space in the file drive to write a buffer of {newCapacity} bytes" );
				}

				byte[] newBuffer = new byte[ newCapacity ];

//				byte[] newBuffer = ArrayPool<byte>.Shared.Rent( newCapacity );

				System.Buffer.BlockCopy( _buffer, 0, newBuffer, 0, Position );
//				ArrayPool<byte>.Shared.Return( _buffer );
				_buffer = newBuffer;
			}
		}

		/*
		===============
		GetFreeDiskSpace
		===============
		*/
		/// <summary>
		/// Gets the amount of available bytes left in the save file's current drive.
		/// </summary>
		/// <param name="path">The path of the save file.</param>
		/// <returns>The total available bytes left in the drive.</returns>
		private long GetFreeDiskSpace( string path ) {
			try {
				System.IO.DriveInfo driveInfo = new System.IO.DriveInfo( path );
				if ( driveInfo.IsReady ) {
					return driveInfo.AvailableFreeSpace;
				}
			} catch ( Exception ) {
				logger?.PrintError( $"SaveStreamWriter.GetFreeDiskSpace: error getting remaining disk space in drive {path}" );
			}
			return -1;
		}
	};
};
