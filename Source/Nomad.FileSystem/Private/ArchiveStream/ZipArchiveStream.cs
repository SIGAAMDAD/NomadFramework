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

using System.IO.Compression;

namespace Nomad.FileSystem.Private.ArchiveStream {
	internal sealed class ZipArchiveReadStream {
		private readonly ZipArchive _archive;

		public ZipArchiveReadStream( string path ) {
			using var stream = new System.IO.FileStream( path, System.IO.FileMode.Open, System.IO.FileAccess.Read );
			_archive = new ZipArchive( stream );
		}
	};
};