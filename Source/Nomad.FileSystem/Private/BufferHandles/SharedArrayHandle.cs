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
using Nomad.Core.Util;

namespace Nomad.FileSystem.Private.BufferHandles {
	internal sealed class SharedArrayHandle : IBufferHandle {
		public int Length => _length;
		private readonly int _length;

		public byte[] Buffer => _buffer;
		private readonly byte[] _buffer;

		/*
		===============
		SharedArrayHandle
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="length"></param>
		public SharedArrayHandle( int length ) {
			_length = length;
			_buffer = ArrayPool<byte>.Shared.Rent( length );
		}

		/*
		===============
		Dispose
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		public void Dispose() {
			if ( _buffer != null ) {
				ArrayPool<byte>.Shared.Return( _buffer );
			}
		}
	};
};