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
using System.Collections.Concurrent;
using Nomad.Core.Logger;
using Nomad.Save.Private.Entities;
using Nomad.Save.Private.Serialization.Streams;
using Nomad.Save.Private.ValueObjects;

namespace Nomad.Save.Private.Services {
	/*
	===================================================================================

	SaveReaderService

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	internal sealed class SaveReaderService : IDisposable {
		public int SectionCount => _sections.Count;
		private readonly ConcurrentDictionary<string, SaveSectionReader> _sections = new();

		/*
		===============
		SaveReaderService
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="filepath"></param>
		/// <param name="logger"></param>
		public SaveReaderService( string filepath, ILoggerService logger ) {
			logger.PrintLine( $"Loading save data..." );

			using var reader = new SaveStreamReader( filepath );
			var header = SaveHeader.Deserialize( reader );

			logger.PrintLine( $"...Section Count: {header.SectionCount}" );
			logger.PrintLine( $"...Version: {header.Version}" );

			for ( int i = 0; i < header.SectionCount; i++ ) {
				var section = new SaveSectionReader( in reader, logger );
				_sections[ section.Name ] = section;
			}

			logger.PrintLine( "...Finished loading save data" );
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
			_sections.Clear();
		}
	};
};
