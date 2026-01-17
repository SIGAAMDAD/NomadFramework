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

namespace Nomad.Save.Private.Serialization.Streams {
	/*
	===================================================================================

	SaveFileStream

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	internal interface ISaveFileStream : IDisposable {
		/// <summary>
		/// The current offset of the buffer. Basically a pointer without the pointer.
		/// </summary>
		public int Position { get; }

		/// <summary>
		/// The length of the stream.
		/// </summary>
		public int Length { get; }

		/// <summary>
		/// The buffer that's being operated on.
		/// </summary>
		public byte[]? Buffer { get; }
	};
};
