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

using System.Collections.Generic;
using Nomad.Save.Interfaces;
using Nomad.Save.Private.Serialization.FieldSerializers;
using Nomad.Save.Private.Serialization.Streams;
using Nomad.Save.Private.ValueObjects;

namespace Nomad.Save.Private.Entities {
	/*
	===================================================================================

	SaveSectionWriter

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	internal sealed class SaveSectionWriter( string name, SaveStreamWriter writer ) : ISaveSectionWriter {
		public string Name => name;
		public int FieldCount => _fields.Count;

		private readonly Dictionary<string, SaveField> _fields = new();

		/*
		===============
		Dispose
		===============
		*/
		/// <summary>
		///
		/// </summary>
		public void Dispose() {
			var header = new SectionHeader( name, FieldCount, Checksum.Empty.Value );
			foreach ( var field in _fields ) {
				var serializer = FieldSerializerRegistry.GetSerializer( FieldValue.GetFieldType( field.Value.Type ) );
				serializer.Serialize( writer, field.Value.Value );
			}
		}

		/*
		===============
		AddField
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="fieldId"></param>
		/// <param name="value"></param>
		public void AddField<T>( string fieldId, T value ) {
			_fields[ fieldId ] = new SaveField(
				fieldId,
				FieldValue.GetFieldType<T>(),
				FieldValue.From( value )
			);
		}

		/*
		===============
		HasField
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="fieldId"></param>
		/// <returns></returns>
		public bool HasField<T>( string fieldId ) {
			return _fields.ContainsKey( fieldId );
		}
	};
};
