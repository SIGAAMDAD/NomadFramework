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

	IntSerializer

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	internal sealed class IntSerializer : IFieldSerializer<int> {
		public FieldType FieldType => FieldType.Int32;
		public Type DataType => typeof( int );

		public void Serialize( IWriteStream stream, int value ) => stream.WriteInt32( value );
		public void Serialize( IWriteStream stream, FieldValue value ) => stream.WriteInt32( value.GetValue<int>() );
		public FieldValue Deserialize( IReadStream stream ) => new FieldValue( stream.ReadInt32() );
	};
};
