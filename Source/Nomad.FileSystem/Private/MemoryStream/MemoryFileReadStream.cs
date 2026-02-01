/*
===========================================================================
The Nomad MPL Source Code
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

	public sealed class MemoryFileReadStream : MemoryReadStream, IFileReadStream {
		public string FilePath => _filepath;
		private readonly string _filepath;

		public bool IsOpen => true;

		/*
		===============
		MemoryFileReadStream
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="filepath"></param>
		public MemoryFileReadStream( string filepath ) {
			_filepath = filepath;
			_buffer = File.ReadAllBytes( _filepath );
			_length = _buffer.Length;
			_position = 0;
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
			await base.DisposeAsync();
		}

		public void Close() {
		}

		public bool Open( string filepath, FileMode openMode, FileAccess accessMode ) {
			return false;
		}
	};
};
