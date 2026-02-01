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

	public sealed class MemoryFileWriteStream( string filepath, int length, bool fixedSize = false )
		: MemoryWriteStream( length, fixedSize ), IFileWriteStream
	{
		public string FilePath => _filepath;
		private readonly string _filepath = filepath;

		public bool IsOpen => false;

		private readonly bool _fixedSize = fixedSize;

		/*
		===============
		Dispose
		===============
		*/
		/// <summary>
		///
		/// </summary>
		public override void Dispose() {
			if ( _buffer != null ) {
				using var stream = new System.IO.FileStream( _filepath, System.IO.FileMode.Create, System.IO.FileAccess.Write );
				stream.Write( _buffer, 0, _position );
			}
			base.Dispose();
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
		Open
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="filepath"></param>
		/// <param name="openMode"></param>
		/// <param name="accessMode"></param>
		/// <returns></returns>
		public bool Open( string filepath, System.IO.FileMode openMode, System.IO.FileAccess accessMode ) {
			return false;
		}
	};
};
