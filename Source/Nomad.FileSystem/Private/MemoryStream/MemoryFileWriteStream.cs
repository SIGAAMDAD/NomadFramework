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
using System.Threading;
using System.Threading.Tasks;
using Nomad.Core.FileSystem;

namespace Nomad.FileSystem.Private.MemoryStream {
	/*
	===================================================================================

	MemoryFileWriteStream

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>
	/// <param name="filepath"></param>
	/// <param name="length"></param>
	/// <param name="fixedSize"></param>

	public sealed class MemoryFileWriteStream
		: MemoryWriteStream, IMemoryFileWriteStream
	{
		public string FilePath => _filepath;
		private string _filepath;

		public bool IsOpen => _buffer != null;

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
				using var stream = new System.IO.FileStream( _filepath, System.IO.FileMode.Create, System.IO.FileAccess.Write );
				stream.Write( _buffer.AsSpan( 0, _position ) );
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
				using var stream = new System.IO.FileStream( _filepath, System.IO.FileMode.Create, System.IO.FileAccess.Write );
				await stream.WriteAsync( _buffer.AsMemory( 0, _position ), ct );
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
		/// <param name="filepath">The path to the file to open.</param>
		/// <param name="openMode">The mode in which to open the file.</param>
		/// <param name="accessMode">The access mode for the file.</param>
		/// <returns><c>true</c> if the file was opened successfully; otherwise, <c>false.</c></returns>
		public bool Open( string filepath, System.IO.FileMode openMode, System.IO.FileAccess accessMode ) {
			_filepath = filepath;
			
			if ( openMode == System.IO.FileMode.Append ) {
				throw new NotImplementedException();
			} else {
				_length = DEFAULT_CAPACITY;
				_buffer = ArrayPool<byte>.Shared.Rent( _length );
			}

			return true;
		}
	};
};
