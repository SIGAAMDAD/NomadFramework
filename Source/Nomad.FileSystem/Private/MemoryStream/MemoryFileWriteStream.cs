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
using System.Threading;
using System.Threading.Tasks;
using Nomad.Core.FileSystem;
using Nomad.Core.Util.BufferHandles;

namespace Nomad.FileSystem.Private.MemoryStream {
	/*
	===================================================================================

	MemoryFileWriteStream

	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>

	internal sealed class MemoryFileWriteStream : MemoryWriteStream, IMemoryFileWriteStream
	{
		/// <summary>
		/// 
		/// </summary>
		public string FilePath => _filepath;
		private readonly string _filepath;

		/// <summary>
		/// 
		/// </summary>
		public bool IsOpen => _buffer != null;

		/// <summary>
		/// 
		/// </summary>
		public DateTime LastAccessTime => _creationTime;

		/// <summary>
		/// 
		/// </summary>
		public DateTime CreationTime => _creationTime;
		private readonly DateTime _creationTime;

		/*
		===============
		MemoryFileWriteStream
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="filepath"></param>
		/// <param name="length"></param>
		/// <param name="fixedSize"></param>
		public MemoryFileWriteStream( string filepath, int length, bool fixedSize = false )
			: base( length, fixedSize )
		{
			_filepath = filepath;
			_creationTime = DateTime.Now;
			Open( System.IO.FileMode.CreateNew, System.IO.FileAccess.Write );
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
			Flush();
			base.Dispose();
		}

		/*
		===============
		DisposeAsync
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override async ValueTask DisposeAsync() {
			await FlushAsync();
			await base.DisposeAsync();
		}

		/*
		===============
		Close
		===============
		*/
		/// <summary>
		///
		/// </summary>
		public void Close() {
			Dispose();
		}

		/*
		===============
		Flush
		===============
		*/
		/// <summary>
		/// Flushes the memory file write stream to the underlying file.
		/// </summary>
		public override void Flush() {
			if ( _buffer != null ) {
				using var stream = new System.IO.FileStream( _filepath, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.None );
				stream.Write( _buffer.Buffer.AsSpan( 0, _position ) );
				stream.Close();
			}
		}
	
		/*
		===============
		FlushAsync
		===============
		*/
		/// <summary>
		/// Asynchronously flushes the memory file write stream to the underlying file.
		/// </summary>
		/// <param name="ct">The cancellation token to use for the operation.</param>
		/// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
		public override async ValueTask FlushAsync( CancellationToken ct = default ) {
			if ( _buffer != null ) {
				using var stream = new System.IO.FileStream( _filepath, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.None );
				await stream.WriteAsync( _buffer.Buffer.AsMemory( 0, _position ), ct );
				stream.Close();
			}
		}

		/*
		===============
		Open
		===============
		*/
		/// <summary>
		/// Opens the memory file write stream with the specified file path, open mode, and access mode.
		/// </summary>
		/// <param name="openMode">The mode in which to open the file.</param>
		/// <param name="accessMode">The access mode for the file.</param>
		/// <returns><c>true</c> if the file was opened successfully; otherwise, <c>false.</c></returns>
		private bool Open( System.IO.FileMode openMode, System.IO.FileAccess accessMode ) {
			if ( openMode == System.IO.FileMode.Append ) {
				throw new NotImplementedException();
			} else {
				_length = DEFAULT_CAPACITY;
				_buffer = new PooledBufferHandle( _length );
			}
			return true;
		}

		/*
		===============
		WriteLine
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="line"></param>
		public void WriteLine( string line )
			=> Write( $"{line}\n" );

		/*
		===============
		WriteLine
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="line"></param>
		public void WriteLine( ReadOnlySpan<char> line )
			=> WriteLine( line );

		/*
		===============
		WriteLineAsync
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="line"></param>
		public async ValueTask WriteLineAsync( string line )
			=> WriteLine( line );

		/*
		===============
		WriteLineAsync
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="line"></param>
		/// <returns></returns>
		public async ValueTask WriteLineAsync( ReadOnlyMemory<char> line )
			=> WriteLine( line.Span );
	};
};
