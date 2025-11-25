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
	
	SaveWriterStream
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	/// <remarks>
	/// Writes strings in <see cref="Encoding.UTF8"/> format.
	/// </remarks>

	internal sealed class SaveWriterStream : SaveStream {
		private readonly System.IO.FileStream Stream;

		/*
		===============
		SaveWriterStream
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="stream">The file stream to dump the buffer into once we're done writing.</param>
		/// <param name="initialCapacity">The initial size of the write buffer.</param>
		public SaveWriterStream( System.IO.FileStream? stream, int initialCapacity = 8192 ) {
			ArgumentNullException.ThrowIfNull( stream );

			Stream = stream;
			Buffer = ArrayPool<byte>.Shared.Rent( initialCapacity );
			Position = 0;
		}

		/*
		===============
		Dispose
		===============
		*/
		/// <summary>
		/// Dumps the buffer to the stream.
		/// </summary>
		public override void Dispose() {
			if ( Buffer != null ) {
				Stream.Write( Buffer, 0, Position );
				ArrayPool<byte>.Shared.Return( Buffer );
				Buffer = null;
			}
			Stream?.Dispose();
			base.Dispose();
		}

		/*
		===============
		Write
		===============
		*/
		/// <summary>
		/// Writes a boolean value to the stream.s
		/// </summary>
		/// <param name="value">The boolean value to write to the stream.</param>
		public void Write( bool value ) {
			WritePrimitive( value );
		}

		/*
		===============
		Write
		===============
		*/
		/// <summary>
		/// Writes an 8-bit signed integer to the stream.
		/// </summary>
		/// <param name="value">The 8-bit signed value to write to the stream.</param>
		public void Write( sbyte value ) {
			WritePrimitive( value );
		}

		/*
		===============
		Write
		===============
		*/
		/// <summary>
		/// Writes a 16-bit signed integer to the stream.
		/// </summary>
		/// <param name="value">The 16-bit signed value to write to the stream.</param>
		public void Write( short value ) {
			WritePrimitive( value );
		}

		/*
		===============
		Write
		===============
		*/
		/// <summary>
		/// Writes a 32-bit signed integer to the stream.
		/// </summary>
		/// <param name="value">The 32-bit signed value to write to the stream.</param>
		public void Write( int value ) {
			WritePrimitive( value );
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
		public void Write( long value ) {
			WritePrimitive( value );
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
		public void Write( byte value ) {
			WritePrimitive( value );
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
		public void Write( ushort value ) {
			WritePrimitive( value );
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
		public void Write( uint value ) {
			WritePrimitive( value );
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
		public void Write( ulong value ) {
			WritePrimitive( value );
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
		public void Write( float value ) {
			WritePrimitive( value );
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
		public void Write( double value ) {
			WritePrimitive( value );
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
			ArgumentNullException.ThrowIfNull( Buffer );
			ArgumentNullException.ThrowIfNull( value );

			const int STACK_ALLOC_THRESHOLD = 256;
			int maxByteCount = Encoding.UTF8.GetMaxByteCount( value.Length );

			if ( maxByteCount <= STACK_ALLOC_THRESHOLD ) {
				Span<byte> tempBuffer = stackalloc byte[ maxByteCount ];
				int actualByteCount = Encoding.UTF8.GetBytes( value, tempBuffer );
				Write7BitEncodedInt( actualByteCount );
				EnsureCapacity( actualByteCount );
				tempBuffer[ ..actualByteCount ].CopyTo( Buffer.AsSpan( Position ) );
				Position += actualByteCount;
			} else {
				int byteCount = Encoding.UTF8.GetByteCount( value );
				Write7BitEncodedInt( byteCount );
				EnsureCapacity( byteCount );
				Position += Encoding.UTF8.GetBytes( value, 0, value.Length, Buffer, Position );
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
		public void Write( byte[] value ) {
			ArgumentNullException.ThrowIfNull( value );

			Write( value, 0, value.Length );
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
			ArgumentNullException.ThrowIfNull( Buffer );
			ArgumentNullException.ThrowIfNull( value );
			ArgumentOutOfRangeException.ThrowIfLessThan( offset, 0 );
			ArgumentOutOfRangeException.ThrowIfLessThan( length, 0 );
			ArgumentOutOfRangeException.ThrowIfGreaterThan( offset + length, value.Length );

			EnsureCapacity( length );
			System.Buffer.BlockCopy( value, offset, Buffer, Position, length );
			Position += length;
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
			ArgumentNullException.ThrowIfNull( Buffer );

			uint uValue = (uint)value;
			while ( uValue >= 0x80 ) {
				WritePrimitive( (byte)( uValue | 0x80 ) );
				uValue >>= 7;
			}
			WritePrimitive( (byte)uValue );
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
			ArgumentNullException.ThrowIfNull( Buffer );

			EnsureCapacity( buffer.Length );
			buffer.CopyTo( new Span<byte>( ref Buffer[ Position ] ) );
			Position += buffer.Length;
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
		/// We're already checking the <see cref="Buffer"/> is null from all the calling functions, so a null check isn't necessary here.
		/// </remarks>
		/// <param name="required">The bytes needed to write the data.</param>
		private void EnsureCapacity( int required ) {
			if ( Position + required > Buffer.Length ) {
				int newCapacity = Math.Max( Buffer.Length * 2, Position + required );
				byte[] newBuffer = ArrayPool<byte>.Shared.Rent( newCapacity );

				System.Buffer.BlockCopy( Buffer, 0, newBuffer, 0, Position );
				ArrayPool<byte>.Shared.Return( Buffer );

				Buffer = newBuffer;
			}
		}

		/*
		===============
		WritePrimitive
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private void WritePrimitive<T>( T value ) where T : unmanaged {
			ArgumentNullException.ThrowIfNull( Buffer );

			int sizeOfData = Marshal.SizeOf<T>();

			EnsureCapacity( sizeOfData );
			Unsafe.WriteUnaligned( ref Buffer[ Position ], value );
			Position += sizeOfData;
		}

		/*
		===============
		GetFreeDiskSpace
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		private static long GetFreeDiskSpace( string path ) {
			try {
				System.IO.DriveInfo driveInfo = new System.IO.DriveInfo( path );
				if ( driveInfo.IsReady ) {
					return driveInfo.AvailableFreeSpace;
				}
			} catch ( Exception ) {
				ConsoleSystem.Console.PrintError( $"SaveWriterStream.GetFreeDiskSpace: error getting remaining disk space in drive {path}" );
			}
			return -1;
		}
	};
};