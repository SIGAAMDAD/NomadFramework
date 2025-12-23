/*
===========================================================================
The Nomad Framework
Copyright (C) 2025 Noah Van Til

This Source Code Form is subject to the terms of the Mozilla Public
License, v2. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.

This software is provided "as is", without warranty of any kind,
express or implied, including but not limited to the warranties
of merchantability, fitness for a particular purpose and noninfringement.
===========================================================================
*/

using System;
using Nomad.Audio.Interfaces;
using Nomad.Audio.ValueObjects;
using Nomad.Core.Abstractions;

namespace Nomad.Audio.Fmod.Private.ValueObjects {
	/*
	===================================================================================

	FMODBankMetadata

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	internal record FMODBankMetadata : IBankMetadata {
		public DateTime LoadedAt => _createdAt;

		public BankId Id => _id;
		private readonly BankId _id;

		public DateTime CreatedAt => _createdAt;
		private readonly DateTime _createdAt = DateTime.UtcNow;

		public DateTime? ModifiedAt => null;
		public int Version => 0;

		public FMODBankMetadata( BankId id ) {
			_id = id;
			_createdAt = DateTime.UtcNow;
		}

		public bool Equals( IEntity<BankId>? other ) {
			return other?.Id == Id;
		}
	};
};
