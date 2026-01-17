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
using Nomad.Save.Private.Serialization.Streams;
using Nomad.Save.Private.ValueObjects;

namespace Nomad.Save.Private.Serialization.FieldSerializers {
	/*
	===================================================================================

	LongSerializer

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	internal sealed class LongSerializer : IFieldSerializer<long> {
		public FieldType FieldType => FieldType.Int64;
		public Type DataType => typeof( long );

		public void Serialize( SaveStreamWriter stream, long value ) => stream.Write( value );
		public void Serialize( SaveStreamWriter stream, FieldValue value ) => stream.Write( value.GetValue<long>() );
		public FieldValue Deserialize( SaveStreamReader stream ) => new FieldValue( stream.Read<long>() );
	};
};
