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
using Nomad.Save.Exceptions;
using Nomad.Save.Private.ValueObjects;
using Nomad.Core.Util;
using Nomad.Core.Logger;
using System.Collections.Concurrent;

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
		/// <summary>
		/// This section's name.
		/// </summary>
		public string Name => _name;
		private readonly string _name;

		/// <summary>
		/// The number of fields in this section.
		/// </summary>
		public int FieldCount => _fields.Count;
		private readonly ConcurrentDictionary<string, SaveField> _fields;

		private readonly IMemoryFileWriteStream _writer;

		private readonly SaveConfig _config;

		private readonly ILoggerService _logger;
		private readonly ILoggerCategory _category;

		/*
		===============
		SaveSectionWriter
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="config"></param>
		/// <param name="logger"></param>
		/// <param name="category"></param>
		/// <param name="name"></param>
		/// <param name="writer"></param>
		public SaveSectionWriter( in SaveConfig config, ILoggerService logger, ILoggerCategory category, string name, IMemoryFileWriteStream writer ) {
			_name = name;
			_fields = new ConcurrentDictionary<string, SaveField>();
			_writer = writer;
			_config = config;

			_logger = logger;
			_category = category;
		}

		/*
		===============
		Dispose
		===============
		*/
		/// <summary>
		/// Writes the entire section to the save stream and clears all fields.
		/// </summary>
		public void Dispose() {
			int offset = _writer.Position;
			{
				var header = new SectionHeader( _name, 0, FieldCount, Checksum.Empty );
				header.Save( _writer );
			}

			int start = _writer.Position;
			foreach ( var field in _fields ) {
				SaveField.Write( _name, field.Value, _writer );
			}

			int position = _writer.Position;
			int length = position - start;

			_writer.Seek( offset, System.IO.SeekOrigin.Begin );
			{
				var header = new SectionHeader( _name, length, FieldCount, Checksum.Compute( _writer.Buffer.GetSlice( start, length ) ) );
				header.Save( _writer );

				if ( _config.LogSerializationTree ) {
					_logger.PrintLine( in _category, "Finalized section data:" );
					_logger.PrintLine( in _category, $"\tName: {header.Name}" );
					_logger.PrintLine( in _category, $"\tByteLength: {header.ByteLength}" );
					_logger.PrintLine( in _category, $"\tFieldCount: {header.FieldCount}" );
					_logger.PrintLine( in _category, $"\tChecksum64: {header.Checksum}" );
				}
			}
			_writer.Seek( position, System.IO.SeekOrigin.Begin );

			_fields.Clear();
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
				throw new DuplicateFieldException( $"Field '{fieldId}' added twice!" );
			}

			var type = Any.GetType<T>();

			_fields[ fieldId ] = new SaveField(
				fieldId,
				type,
				Any.From( value )
			);
			if ( _config.LogSerializationTree ) {
				_logger.PrintLine( in _category, $"\t\t[Field] (NAME) {fieldId}, (TYPE) {type}, (VALUE) {value}" );
			}
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
		public bool HasField<T>( string fieldId )
			=> _fields.ContainsKey( fieldId );
	};
};
