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
using Nomad.Core.FileSystem;
using Nomad.Save.Private.ValueObjects;

namespace Nomad.Save.Private.Serialization.FieldSerializers {
	/*
	===================================================================================

	ULongSerializer

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	internal sealed class ULongSerializer : IFieldSerializer<ulong> {
		public FieldType FieldType => FieldType.UInt64;
		public Type DataType => typeof( ulong );

		public void Serialize( IWriteStream stream, ulong value ) => stream.WriteUInt64( value );
		public void Serialize( IWriteStream stream, FieldValue value ) => stream.WriteUInt64( value.GetValue<ulong>() );
		public FieldValue Deserialize( IReadStream stream ) => new FieldValue( stream.ReadUInt64() );
	};
};
