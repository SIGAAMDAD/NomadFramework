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
using Nomad.Core.FileSystem;
using Nomad.Save.Interfaces;
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

	internal sealed class SaveSectionWriter : ISaveSectionWriter {
		public string Name => _name;
		private readonly string _name;

		public int FieldCount => _fields.Count;
		private readonly Dictionary<string, SaveField> _fields;

		private readonly IWriteStream _writer;

		/*
		===============
		SaveSectionWriter
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="writer"></param>
		public SaveSectionWriter( string name, IWriteStream writer ) {
			_name = name;
			_fields = new Dictionary<string, SaveField>();
			_writer = writer;
		}

		/*
		===============
		Dispose
		===============
		*/
		/// <summary>
		///
		/// </summary>
		public void Dispose() {
			var header = new SectionHeader( _name, FieldCount, Checksum.Empty.Value );
			header.Save( _writer );
			foreach ( var field in _fields ) {
				SaveField.Write( _name, field.Value, _writer );
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
			if ( _fields.ContainsKey( fieldId ) ) {
				// FIXME: THROW
				return;
			}
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
