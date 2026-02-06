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
using Nomad.Save.ValueObjects;

namespace Nomad.Save.Private.ValueObjects {
	/// <summary>
	/// 
	/// </summary>
	internal readonly struct BackupData {
		/// <summary>
		/// The backup's index in the chain.
		/// </summary>
		public readonly int Number;

		/// <summary>
		/// 
		/// </summary>
		public readonly DateTime CreatedAt;

		/// <summary>
		/// 
		/// </summary>
		public readonly SaveFileVersion Version;
	};
};