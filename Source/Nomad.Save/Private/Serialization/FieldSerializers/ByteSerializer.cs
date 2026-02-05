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
	
	ByteSerializer
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	internal sealed class ByteSerializer : IFieldSerializer<byte> {
		public FieldType FieldType => FieldType.UInt8;
		public Type DataType => typeof( byte );

		public void Serialize( IWriteStream stream, byte value ) => stream.WriteUInt8( value );
		public void Serialize( IWriteStream stream, FieldValue value ) => stream.WriteUInt8( value.GetValue<byte>() );
		public FieldValue Deserialize( IReadStream stream ) => new FieldValue( stream.ReadUInt8() );
	};
};
