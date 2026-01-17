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

using Nomad.Save.Private.Exceptions;
using Nomad.Save.Private.Serialization.FieldSerializers;
using Nomad.Save.Private.Serialization.Streams;

namespace Nomad.Save.Private.ValueObjects {
	/*
	===================================================================================

	SaveField

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	internal readonly record struct SaveField( string Name, FieldType Type, FieldValue Value ) {
		public static readonly SaveField Empty = new();

		private const int MAX_FIELD_NAME_LENGTH = 256;

		/*
		===============
		Write
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="section"></param>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <param name="stream"></param>
		public static void Write<T>( string section, string name, T value, SaveStreamWriter stream ) {
			stream.Write( name );
			stream.Write( FieldValue.GetFieldType<T>() );
			FieldSerializerRegistry.GetSerializer<T>().Serialize( stream, value );
		}

		/*
		===============
		Read
		===============
		*/
		/// <summary>
		/// Loads a SaveField object into memory from a save file stream.
		/// </summary>
		/// <param name="section">The section the field is bound to.</param>
		/// <param name="index">The index of the field.</param>
		/// <param name="stream">The file stream to read from.</param>
		/// <returns>A new SaveField object.</returns>
		/// <exception cref="FieldCorruptException">Thrown if the field's data is invalid.</exception>
		public static SaveField Read( string section, int index, SaveStreamReader stream ) {
			string name = stream.ReadString();
			if ( name.Length <= 0 || name.Length > MAX_FIELD_NAME_LENGTH ) {
				throw new FieldCorruptException( section, index, stream.Position, $"Field name length corrupted (0 or string overflow, {name.Length} bytes)" );
			}

			FieldType type = stream.Read<FieldType>();
			if ( type < FieldType.Int8 || type >= FieldType.Count ) {
				throw new FieldCorruptException( section, index, stream.Position, $"Field type '{type}' isn't valid" );
			}

			FieldValue value = FieldSerializerRegistry.GetSerializer( FieldValue.GetFieldType( type ) ).Deserialize( stream );

			return new SaveField( name, type, value );
		}
	};
};
