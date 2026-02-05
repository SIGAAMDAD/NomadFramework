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

	SByteSerializer

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	internal sealed class SByteSerializer : IFieldSerializer<sbyte> {
		public FieldType FieldType => FieldType.Int8;
		public Type DataType => typeof( sbyte );

		public void Serialize( IWriteStream stream, sbyte value ) => stream.WriteInt8( value );
		public void Serialize( IWriteStream stream, FieldValue value ) => stream.WriteInt8( value.GetValue<sbyte>() );
		public FieldValue Deserialize( IReadStream stream ) => new FieldValue( stream.ReadInt8() );
	};
};
