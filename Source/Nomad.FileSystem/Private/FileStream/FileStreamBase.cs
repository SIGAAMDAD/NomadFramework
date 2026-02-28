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
using Nomad.Core.Compatibility.Guards;
using Nomad.Core.FileSystem.Streams;

namespace Nomad.FileSystem.Private.FileStream {
	/*
	===================================================================================

	FileStreamBase

	===================================================================================
	*/
	/// <summary>
	/// Base implementation of a file stream.
	/// </summary>

	internal abstract class FileStreamBase : BaseStream, IFileStream {
		/// <summary>
		/// Indicates whether the stream supports seeking.
		/// </summary>
		public override bool CanSeek => fileStream?.CanSeek ?? false;

		/// <summary>
		/// Gets or sets the current position within the file stream.
		/// </summary>
		public override long Position {
			get => !isDisposed ? fileStream!.Position : throw new ObjectDisposedException( nameof( FileStreamBase ) );
			set {
				fileStream!.Seek( value, SeekOrigin.Begin );
			}
		}

		/// <summary>
		/// Gets the length of the file stream in bytes.
		/// </summary>
		public override long Length {
			get => !isDisposed ? fileStream!.Length : throw new ObjectDisposedException( nameof( FileStreamBase ) );
			set {
				fileStream!.SetLength( value );
			}
		}

		/// <summary>
		/// Gets the file path associated with the file stream.
		/// </summary>
		public string FilePath => fileStream!.Name ?? String.Empty;

		/// <summary>
		/// Indicates whether the file stream is currently open.
		/// </summary>
		public bool IsOpen => fileStream != null;

		/// <summary>
		/// The timestamp of which this file was last accessed.
		/// </summary>
		public DateTime LastAccessTime => _info.LastAccessTime;

		/// <summary>
		/// The time of the file's initial creation.
		/// </summary>
		public DateTime CreationTime => _info.CreationTime;

		/// <summary>
		/// The underlying file stream.
		/// </summary>
		protected System.IO.FileStream? fileStream;

		/// <summary>
		/// The file metadata, fetched at object construction.
		/// </summary>
		private readonly FileInfo _info;

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
			ArgumentGuard.ThrowIfNullOrEmpty( filepath );
			fileStream = new System.IO.FileStream( filepath, fileMode, fileAccess );
			_info = new FileInfo( filepath );
		}

		/*
		===============
		Dispose
		===============
		*/
		/// <summary>
		/// Releases all resources used by the file stream.
		/// </summary>
		public sealed override void Dispose() {
			Dispose( true );
			GC.SuppressFinalize( this );
		}

		/*
		===============
		Dispose
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose( bool disposing ) {
			if ( isDisposed ) {
				return;
			}
			if ( disposing ) {
				fileStream?.Dispose();
			}
			isDisposed = true;
		}

		/*
		===============
		DisposeAsync
		===============
		*/
		/// <summary>
		/// Asynchronously releases all resources used by the file stream.
		/// </summary>
		public sealed override async ValueTask DisposeAsync() {
			await DisposeAsyncCore().ConfigureAwait( false );
			Dispose( false );
			GC.SuppressFinalize( this );
		}

		/*
		===============
		DisposeAsyncCore
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		protected virtual async ValueTask DisposeAsyncCore() {
			if ( isDisposed ) {
				return;
			}
			if ( fileStream is IAsyncDisposable asyncDisposable ) {
				await asyncDisposable.DisposeAsync().ConfigureAwait( false );
			} else {
				fileStream?.Dispose();
			}
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
			fileStream?.Close();
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
			ArgumentGuard.ThrowIfNull( fileStream );
			fileStream.Flush();
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
		public override async ValueTask FlushAsync( CancellationToken ct = default ) {
			ArgumentGuard.ThrowIfNull( fileStream );
			ct.ThrowIfCancellationRequested();
			await fileStream.FlushAsync( ct );
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
		public bool Open( string filepath, FileMode openMode, FileAccess accessMode ) {
			fileStream = new System.IO.FileStream( filepath, openMode, accessMode );
			return fileStream != null;
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
		/// <returns>The new position in the file stream.</returns>
		public override long Seek( long offset, SeekOrigin origin ) {
			StateGuard.ThrowIfDisposed( isDisposed, this );
			return fileStream!.Seek( offset, origin );
		}

		/*
		===============
		SetLength
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="length"></param>
		public override void SetLength( long length ) {
			StateGuard.ThrowIfDisposed( isDisposed, this );
			fileStream!.SetLength( length );
		}
	};
};
