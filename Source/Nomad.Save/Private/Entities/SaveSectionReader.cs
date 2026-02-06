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
using System.Collections.Generic;
using Nomad.Core.Compatibility;
using Nomad.Core.FileSystem;
using Nomad.Core.Logger;
using Nomad.Save.Interfaces;
using Nomad.Save.Private.ValueObjects;

namespace Nomad.Save.Private.Entities {
	/*
	===================================================================================

	SaveSectionReader

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	internal sealed class SaveSectionReader : ISaveSectionReader {
		public string Name { get; private set; }

		public int FieldCount => _fields.Count;

		private readonly Dictionary<string, SaveField> _fields;

		private readonly ILoggerService _logger;

		/*
		===============
		SaveSectionReader
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="logger"></param>
		public SaveSectionReader( in IReadStream reader, ILoggerService logger ) {
			_fields = new Dictionary<string, SaveField>();
			
			logger.PrintLine( "...Loading save section" );

			var header = SectionHeader.Load( in reader );
			logger.PrintLine( $"...Got name {header.Name}, field count {header.FieldCount}" );

			int fieldCount = header.FieldCount;
			Name = header.Name;

			_fields.EnsureCapacity( fieldCount );
			for ( int i = 0; i < fieldCount; i++ ) {
				var field = SaveField.Read( Name, i, in reader );
				_fields[ field.Name ] = field;

				logger.PrintLine( $"- Got field {field.Name}, value {field.Value}" );
			}

			_logger = logger;
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
			_fields.Clear();
		}

		/*
		===============
		GetField
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		public T GetField<T>( string fieldName ) {
			ExceptionCompat.ThrowIfNullOrEmpty( fieldName );
			
			T value = default;
			if ( _fields.TryGetValue( fieldName, out var field ) ) {
				if ( FieldValue.GetFieldType<T>() != field.Type ) {
					throw new InvalidCastException( "Field type found in file does not match the requested type" );
				}
				value = field.Value.GetValue<T>();
			}
			return value;
		}
	};
};
