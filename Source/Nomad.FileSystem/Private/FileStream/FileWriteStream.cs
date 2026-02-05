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
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Nomad.Core.Compatibility;
using Nomad.Core.FileSystem;

namespace Nomad.FileSystem.Private.FileStream {
	/*
	===================================================================================

	FileWriteStream

	===================================================================================
	*/
	/// <summary>
	/// Represents a write-only file stream.
	/// </summary>

	public sealed class FileWriteStream : FileStreamBase, IFileWriteStream {
		/// <summary>
		/// Indicates whether the stream supports reading.
		/// </summary>
		public override bool CanRead => false;

		/// <summary>
		/// Indicates whether the stream supports writing.
		/// </summary>
		public override bool CanWrite => true;

		/// <summary>
		/// The binary writer for the file stream.
		/// </summary>
		private readonly BinaryWriter _streamWriter;

		/*
		===============
		FileWriteStream
		===============
		*/
		/// <summary>
		/// Initializes a new instance of the FileWriteStream class with the specified file path, mode, and access.
		/// </summary>
		/// <param name="filepath">The path to the file to write to.</param>
		/// <param name="append">Whether to append to the file or overwrite it.</param>
		public FileWriteStream( string filepath, bool append = false )
			: base( filepath, append ? FileMode.Append : FileMode.Create, FileAccess.Write )
		{
			ExceptionCompat.ThrowIfNull( _fileStream );
			_streamWriter = new BinaryWriter( _fileStream );
		}

		/*
		===============
		Dispose
		===============
		*/
		/// <summary>
		/// Disposes the file write stream and releases all associated resources.
		/// </summary>
		public override void Dispose() {
			_streamWriter.Dispose();
			base.Dispose();
		}

		/*
		===============
		DisposeAsync
		===============
		*/
		/// <summary>
		/// Asynchronously disposes the file write stream and releases all associated resources.
		/// </summary>
		/// <returns>A task that represents the asynchronous dispose operation.</returns>
		public override async ValueTask DisposeAsync() {
			await _streamWriter.DisposeAsync();
			await base.DisposeAsync();
		}


		/*
		===============
		Write
		===============
		*/
		/// <summary>
		/// Writes a sequence of bytes to the file stream and advances the current position within this stream by the number of bytes written.
		/// </summary>
		/// <param name="buffer">An array of bytes. This method copies count bytes from buffer to the current stream.</param>
		/// <param name="offset">The zero-based byte offset in buffer at which to begin copying bytes to the current stream.</param>
		/// <param name="count">The number of bytes to be written to the current stream.</param>
		public void Write( byte[] buffer, int offset, int count ) {
			ExceptionCompat.ThrowIfNull( _fileStream );
			_fileStream.Write( buffer, offset, count );
		}

		/*
		===============
		Write
		===============
		*/
		/// <summary>
		/// Writes a sequence of bytes to the file stream and advances the current position within this stream by the number of bytes written.
		/// </summary>
		/// <param name="buffer">A read-only span of bytes. This method copies the contents of the span to the current stream.</param>
		public void Write( ReadOnlySpan<byte> buffer ) {
			ExceptionCompat.ThrowIfNull( _fileStream );
			_fileStream.Write( buffer );
		}

		/*
		===============
		WriteAsync
		===============
		*/
		/// <summary>
		/// Asynchronously writes a sequence of bytes to the file stream and advances the current position within this stream by the number of bytes written.
		/// </summary>
		/// <param name="buffer">An array of bytes. This method copies count bytes from buffer to the current stream.</param>
		/// <param name="offset">The zero-based byte offset in buffer at which to begin copying bytes to the current stream.</param>
		/// <param name="count">The number of bytes to be written to the current stream.</param>
		/// <param name="ct">A token to cancel the operation.</param>
		/// <returns>A task that represents the asynchronous write operation.</returns>
		public async ValueTask WriteAsync( byte[] buffer, int offset, int count, CancellationToken ct = default( CancellationToken ) ) {
			ExceptionCompat.ThrowIfNull( _fileStream );
			ct.ThrowIfCancellationRequested();
			await _fileStream.WriteAsync( buffer.AsMemory( offset, count ), ct );
		}

		/*
		===============
		WriteAsync
		===============
		*/
		/// <summary>
		/// Asynchronously writes a sequence of bytes to the file stream and advances the current position within this stream by the number of bytes written.
		/// </summary>
		/// <param name="buffer">A read-only memory buffer. This method copies the contents of the buffer to the current stream.</param>
		/// <param name="ct">A token to cancel the operation.</param>
		/// <returns>A task that represents the asynchronous write operation.</returns>
		public async ValueTask WriteAsync( ReadOnlyMemory<byte> buffer, CancellationToken ct = default( CancellationToken ) ) {
			ExceptionCompat.ThrowIfNull( _fileStream );
			ct.ThrowIfCancellationRequested();
			await _fileStream.WriteAsync( buffer, ct );
		}

		/*
		===============
		WriteByte
		===============
		*/
		/// <summary>
		/// Writes a byte to the file stream.
		/// </summary>
		/// <param name="value">The byte value to write.</param>
		public void WriteByte( byte value ) {
			ExceptionCompat.ThrowIfNull( _streamWriter );
			_streamWriter.Write( value );
		}

		/*
		===============
		WriteDouble
		===============
		*/
		/// <summary>
		/// Writes a double-precision floating-point number to the file stream.
		/// </summary>
		/// <param name="value">The double value to write.</param>
		public void WriteDouble( double value ) {
			ExceptionCompat.ThrowIfNull( _streamWriter );
			_streamWriter.Write( value );
		}

		/*
		===============
		WriteFloat
		===============
		*/
		/// <summary>
		/// Writes a single-precision floating-point number to the file stream.
		/// </summary>
		/// <param name="value">The float value to write.</param>
		public void WriteFloat( float value ) {
			ExceptionCompat.ThrowIfNull( _streamWriter );
			_streamWriter.Write( value );
		}

		/*
		===============
		WriteFloat32
		===============
		*/
		/// <summary>
		/// Writes a 32-bit floating-point number to the file stream.
		/// </summary>
		/// <param name="value">The 32-bit float value to write.</param>
		public void WriteFloat32( float value ) {
			ExceptionCompat.ThrowIfNull( _streamWriter );
			_streamWriter.Write( value );
		}

		/*
		===============
		WriteFloat64
		===============
		*/
		/// <summary>
		/// Writes a 64-bit floating-point number to the file stream.
		/// </summary>
		/// <param name="value">The 64-bit double value to write.</param>
		public void WriteFloat64( double value ) {
			ExceptionCompat.ThrowIfNull( _streamWriter );
			_streamWriter.Write( value );
		}

		/*
		===============
		WriteFromStream
		===============
		*/
		/// <summary>
		/// Reads all bytes from the specified read stream and writes them to the file stream.
		/// </summary>
		/// <param name="stream">The read stream to copy from.</param>
		public void WriteFromStream( IReadStream stream ) {
			ExceptionCompat.ThrowIfNull( stream );
			ExceptionCompat.ThrowIfNull( _fileStream );

			byte[] buffer = ArrayPool<byte>.Shared.Rent( 4096 );
			int bytesRead;
			while ( ( bytesRead = stream.Read( buffer, 0, buffer.Length ) ) > 0 ) {
				_fileStream.Write( buffer, 0, bytesRead );
			}
		}

		/*
		===============
		WriteFromStreamAsync
		===============
		*/
		/// <summary>
		/// Asynchronously reads all bytes from the specified read stream and writes them to the file stream.
		/// </summary>
		/// <param name="stream">The read stream to copy from.</param>
		/// <param name="ct">A token to cancel the operation.</param>
		/// <returns>A task that represents the asynchronous copy operation.</returns>
		public async ValueTask WriteFromStreamAsync( IReadStream stream, CancellationToken ct = default( CancellationToken ) ) {
			ExceptionCompat.ThrowIfNull( stream );
			ExceptionCompat.ThrowIfNull( _fileStream );

			byte[] buffer = ArrayPool<byte>.Shared.Rent( 4096 );
			int bytesRead;
			while ( ( bytesRead = await stream.ReadAsync( buffer, 0, buffer.Length, ct ) ) > 0 ) {
				await _fileStream.WriteAsync( buffer.AsMemory( 0, bytesRead ), ct );
			}
		}

		/*
		===============
		WriteInt
		===============
		*/
		/// <summary>
		/// Writes a 32-bit signed integer to the file stream.
		/// </summary>
		/// <param name="value">The 32-bit signed integer value to write.</param>
		public void WriteInt( int value ) {
			ExceptionCompat.ThrowIfNull( _streamWriter );
			_streamWriter.Write( value );
		}

		/*
		===============
		WriteInt16
		===============
		*/
		/// <summary>
		/// Writes a 16-bit signed integer to the file stream.
		/// </summary>
		/// <param name="value">The 16-bit signed integer value to write.</param>
		public void WriteInt16( short value ) {
			ExceptionCompat.ThrowIfNull( _streamWriter );
			_streamWriter.Write( value );
		}

		/*
		===============
		WriteInt32
		===============
		*/
		/// <summary>
		/// Writes a 32-bit signed integer to the file stream.
		/// </summary>
		/// <param name="value">The 32-bit signed integer value to write.</param>
		public void WriteInt32( int value ) {
			ExceptionCompat.ThrowIfNull( _streamWriter );
			_streamWriter.Write( value );
		}

		/*
		===============
		WriteInt64
		===============
		*/
		/// <summary>
		/// Writes a 64-bit signed integer to the file stream.
		/// </summary>
		/// <param name="value">The 64-bit signed integer value to write.</param>
		public void WriteInt64( long value ) {
			ExceptionCompat.ThrowIfNull( _streamWriter );
			_streamWriter.Write( value );
		}

		/*
		===============
		WriteInt8
		===============
		*/
		/// <summary>
		/// Writes an 8-bit signed integer to the file stream.
		/// </summary>
		/// <param name="value">The 8-bit signed integer value to write.</param>
		public void WriteInt8( sbyte value ) {
			ExceptionCompat.ThrowIfNull( _streamWriter );
			_streamWriter.Write( value );
		}

		/*
		===============
		WriteLong
		===============
		*/
		/// <summary>
		/// Writes a 64-bit signed integer to the file stream.
		/// </summary>
		/// <param name="value">The 64-bit signed integer value to write.</param>
		public void WriteLong( long value ) {
			ExceptionCompat.ThrowIfNull( _streamWriter );
			_streamWriter.Write( value );
		}

		/*
		===============
		WriteSByte
		===============
		*/
		/// <summary>
		/// Writes a signed byte to the file stream.
		/// </summary>
		/// <param name="value">The signed byte value to write.</param>
		public void WriteSByte( sbyte value ) {
			ExceptionCompat.ThrowIfNull( _streamWriter );
			_streamWriter.Write( value );
		}

		/*
		===============
		WriteShort
		===============
		*/
		/// <summary>
		/// Writes a 16-bit signed integer to the file stream.
		/// </summary>
		/// <param name="value">The 16-bit signed integer value to write.</param>
		public void WriteShort( short value ) {
			ExceptionCompat.ThrowIfNull( _streamWriter );
			_streamWriter.Write( value );
		}

		/*
		===============
		WriteSingle
		===============
		*/
		/// <summary>
		/// Writes a single-precision floating-point number to the file stream.
		/// </summary>
		/// <param name="value">The single-precision float value to write.</param>
		public void WriteSingle( float value ) {
			ExceptionCompat.ThrowIfNull( _streamWriter );
			_streamWriter.Write( value );
		}

		/*
		===============
		WriteString
		===============
		*/
		/// <summary>
		/// Writes a string to the file stream.
		/// </summary>
		/// <param name="value">The string value to write.</param>
		public void WriteString( string value ) {
			ExceptionCompat.ThrowIfNull( _streamWriter );
			_streamWriter.Write( value );
		}

		/*
		===============
		WriteUInt
		===============
		*/
		/// <summary>
		/// Writes a 32-bit unsigned integer to the file stream.
		/// </summary>
		/// <param name="value">The 32-bit unsigned integer value to write.</param>
		public void WriteUInt( uint value ) {
			ExceptionCompat.ThrowIfNull( _streamWriter );
			_streamWriter.Write( value );
		}

		/*
		===============
		WriteUInt16
		===============
		*/
		/// <summary>
		/// Writes a 16-bit unsigned integer to the file stream.
		/// </summary>
		/// <param name="value">The 16-bit unsigned integer value to write.</param>
		public void WriteUInt16( ushort value ) {
			ExceptionCompat.ThrowIfNull( _streamWriter );
			_streamWriter.Write( value );
		}

		/*
		===============
		WriteUInt32
		===============
		*/
		/// <summary>
		/// Writes a 32-bit unsigned integer to the file stream.
		/// </summary>
		/// <param name="value">The 32-bit unsigned integer value to write.</param>
		public void WriteUInt32( uint value ) {
			ExceptionCompat.ThrowIfNull( _streamWriter );
			_streamWriter.Write( value );
		}

		/*
		===============
		WriteUInt64
		===============
		*/
		/// <summary>
		/// Writes a 64-bit unsigned integer to the file stream.
		/// </summary>
		/// <param name="value">The 64-bit unsigned integer value to write.</param>
		public void WriteUInt64( ulong value ) {
			ExceptionCompat.ThrowIfNull( _streamWriter );
			_streamWriter.Write( value );
		}

		/*
		===============
		WriteUInt8
		===============
		*/
		/// <summary>
		/// Writes an 8-bit unsigned integer to the file stream.
		/// </summary>
		/// <param name="value">The 8-bit unsigned integer value to write.</param>
		public void WriteUInt8( byte value ) {
			ExceptionCompat.ThrowIfNull( _streamWriter );
			_streamWriter.Write( value );
		}

		/*
		===============
		WriteULong
		===============
		*/
		/// <summary>
		/// Writes a 64-bit unsigned integer to the file stream.
		/// </summary>
		/// <param name="value">The 64-bit unsigned integer value to write.</param>
		public void WriteULong( ulong value ) {
			ExceptionCompat.ThrowIfNull( _streamWriter );
			_streamWriter.Write( value );
		}

		/*
		===============
		WriteUShort
		===============
		*/
		/// <summary>
		/// Writes a 16-bit unsigned integer to the file stream.
		/// </summary>
		/// <param name="value">The 16-bit unsigned integer value to write.</param>
		public void WriteUShort( ushort value ) {
			ExceptionCompat.ThrowIfNull( _streamWriter );
			_streamWriter.Write( value );
		}
	};
};
