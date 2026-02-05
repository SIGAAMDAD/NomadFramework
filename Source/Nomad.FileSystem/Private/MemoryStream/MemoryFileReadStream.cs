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

using System.Buffers;
using System.IO;
using System.Threading.Tasks;
using Nomad.Core.FileSystem;

namespace Nomad.FileSystem.Private.MemoryStream {
	/*
	===================================================================================

	MemoryFileReadStream

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	public sealed class MemoryFileReadStream : MemoryReadStream, IMemoryFileReadStream {
		public string FilePath => _filepath;
		private string _filepath;

		public bool IsOpen => _buffer != null;

		/*
		===============
		MemoryFileReadStream
		===============
		*/
		/// <summary>
		/// Initializes a new instance of the MemoryFileReadStream class with the specified file path.
		/// </summary>
		/// <param name="filepath">The path to the file to read from.</param>
		public MemoryFileReadStream( string filepath ) {
			Open( filepath, FileMode.Open, FileAccess.Read );
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
			Close();
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
			Close();
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
			if ( _buffer == null ) {
				return;
			}
			ArrayPool<byte>.Shared.Return( _buffer );
			_buffer = null;
		}

		/*
		===============
		Open
		===============
		*/
		/// <summary>
		/// Opens the memory file read stream with the specified file path, open mode, and access mode.
		/// </summary>
		/// <param name="filepath">The path to the file to open.</param>
		/// <param name="openMode">The mode in which to open the file.</param>
		/// <param name="accessMode">The access mode for the file.</param>
		/// <returns><c>true</c> if the file was opened successfully; otherwise, <c>false.</c></returns>
		public bool Open( string filepath, FileMode openMode, FileAccess accessMode ) {
			_filepath = filepath;
			_position = 0;

			try {
				using var stream = new System.IO.FileStream( filepath, openMode, accessMode );
				if ( stream == null ) {
					return false;
				}

				_buffer = ArrayPool<byte>.Shared.Rent( ( int )stream.Length );
#if USE_COMPATIBILITY_EXTENSIONS
				stream.Read( _buffer, 0, (int)stream.Length );
#else
				stream.ReadExactly( _buffer, 0, ( int )stream.Length );
#endif

				_length = ( int )stream.Length;

				stream.Dispose();

				return true;
			} catch ( IOException ) {
				return false;
			}
		}
	};
};
