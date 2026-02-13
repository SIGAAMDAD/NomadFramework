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
using Nomad.Core.Exceptions;

namespace Nomad.Save.Private.Exceptions {
	/*
	===================================================================================

	FailedSectionLoadException

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	public sealed class FailedSectionLoadException : NomadError {
		public readonly string? SectionName;
		public readonly Exception Error;

		/*
		===============
		FailedSectionLoadException
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sectionName"></param>
		/// <param name="exception"></param>
		public FailedSectionLoadException( string sectionName, Exception exception )
			: base( $"Failed to load save section {sectionName}, exception - {exception}" )
		{
			SectionName = sectionName;
			Error = exception;
		}
	};
};
