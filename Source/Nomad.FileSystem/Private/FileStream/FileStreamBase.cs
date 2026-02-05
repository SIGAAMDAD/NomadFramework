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
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Nomad.Core.Compatibility;

namespace Nomad.FileSystem.Private.FileStream {
	/*
	===================================================================================

	FileStreamBase

	===================================================================================
	*/
	/// <summary>
	/// Base implementation of a file stream.
	/// </summary>

	public abstract class FileStreamBase : BaseStream {
		/// <summary>
		/// Indicates whether the stream supports seeking.
		/// </summary>
		public override bool CanSeek => _fileStream?.CanSeek ?? false;

		/// <summary>
		/// Gets or sets the current position within the file stream.
		/// </summary>
		public override int Position {
			get => _fileStream != null ? (int)_fileStream.Position : 0;
			set => Seek( value, SeekOrigin.Begin );
		}

		/// <summary>
		/// Gets the length of the file stream in bytes.
		/// </summary>
		public override int Length => _fileStream != null ? (int)_fileStream.Length : 0;

		/// <summary>
		/// Gets the file path associated with the file stream.
		/// </summary>
		public string FilePath => _fileStream?.Name ?? String.Empty;

		/// <summary>
		/// Indicates whether the file stream is currently open.
		/// </summary>
		public bool IsOpen => _fileStream != null;

		/// <summary>
		/// The underlying file stream.
		/// </summary>
		protected System.IO.FileStream? _fileStream;

		/*
		===============
		FileStreamBase
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="filepath"></param>
		/// <param name="fileMode"></param>
		/// <param name="fileAccess"></param>
		public FileStreamBase( string filepath, FileMode fileMode, FileAccess fileAccess ) {
			_fileStream = new System.IO.FileStream( filepath, fileMode, fileAccess );
		}

		/*
		===============
		Dispose
		===============
		*/
		/// <summary>
		/// Releases all resources used by the file stream.
		/// </summary>
		public override void Dispose() {
			if ( _fileStream == null ) {
				return;
			}
			_fileStream.Dispose();
			_fileStream = null;
			GC.SuppressFinalize( this );
		}

		/*
		===============
		DisposeAsync
		===============
		*/
		/// <summary>
		/// Asynchronously releases all resources used by the file stream.
		/// </summary>
		/// <returns></returns>
		public override async ValueTask DisposeAsync() {
			if ( _fileStream == null ) {
				return;
			}
			await _fileStream.DisposeAsync();
			_fileStream = null;
			GC.SuppressFinalize( this );
		}

		/*
		===============
		Close
		===============
		*/
		/// <summary>
		/// Closes the file stream.
		/// </summary>
		public void Close() {
			_fileStream?.Close();
		}

		/*
		===============
		Flush
		===============
		*/
		/// <summary>
		/// Flushes the file stream's buffer to the underlying file.
		/// </summary>
		public override void Flush() {
			ExceptionCompat.ThrowIfNull( _fileStream );
			_fileStream.Flush();
		}

		/*
		===============
		FlushAsync
		===============
		*/
		/// <summary>
		/// Asynchronously flushes the file stream's buffer to the underlying file.
		/// </summary>
		/// <param name="ct"></param>
		/// <returns></returns>
		/// <exception cref="NotImplementedException"></exception>
		public override async ValueTask FlushAsync( CancellationToken ct = default( CancellationToken ) ) {
			ExceptionCompat.ThrowIfNull( _fileStream );
			await _fileStream.FlushAsync( ct );
		}

		/*
		===============
		Open
		===============
		*/
		/// <summary>
		/// Opens a file stream with the specified file path, open mode, and access mode.
		/// </summary>
		/// <param name="filepath"></param>
		/// <param name="openMode"></param>
		/// <param name="accessMode"></param>
		/// <exception cref="NotImplementedException"></exception>
		public bool Open( string filepath, FileMode openMode, FileAccess accessMode ) {
			try {
				_fileStream = new System.IO.FileStream( filepath, openMode, accessMode );
			} catch ( Exception ex ) {
				return false;
			}
			return true;
		}

		/*
		===============
		Seek
		===============
		*/
		/// <summary>
		/// Sets the file stream's position to the specified offset based on the given origin.
		/// </summary>
		/// <param name="offset"></param>
		/// <param name="origin"></param>
		/// <returns></returns>
		/// <exception cref="NotImplementedException"></exception>
		public override int Seek( int offset, SeekOrigin origin ) {
			ExceptionCompat.ThrowIfNull( _fileStream );
			return (int)_fileStream.Seek( offset, origin );
		}
	};
};
