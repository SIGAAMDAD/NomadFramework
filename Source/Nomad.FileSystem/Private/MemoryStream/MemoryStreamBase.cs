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
using System.Threading.Tasks;

namespace Nomad.FileSystem.Private.MemoryStream {
	/*
	===================================================================================

	MemoryStreamBase

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	public abstract class MemoryStreamBase : BaseStream {
		public override int Length => _length;
		protected int _length = 0;

		public override int Position {
			get => _position;
			set => Seek( value, SeekOrigin.Begin );
		}
		protected int _position = 0;

		public override bool CanSeek => true;

		/*
		===============
		Dispose
		===============
		*/
		/// <summary>
		///
		/// </summary>
		public override void Dispose() {
			GC.SuppressFinalize( this );
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
			GC.SuppressFinalize( this );
		}

		/*
		===============
		Seek
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="offset"></param>
		/// <param name="origin"></param>
		/// <returns></returns>
		/// <exception cref="IOException"></exception>
		public override int Seek( int offset, SeekOrigin origin ) {
			switch ( origin ) {
				case SeekOrigin.Begin:
					if ( offset >= _length ) {
						throw new IOException( "seek (begin) offset is larger than the stream's size!" );
					}
					_position = offset;
					break;
				case SeekOrigin.Current:
					if ( _position + offset >= _length ) {
						throw new IOException( "seek (current) offset + position is larger than the stream's size!" );
					}
					_position += offset;
					break;
				case SeekOrigin.End:
					if ( offset > 0 ) {
						throw new IOException( "seek (end) offset is greater than 0!" );
					}
					_position = _length;
					break;
				default:
					return -1;
			}
			return _position;
		}
	};
};
