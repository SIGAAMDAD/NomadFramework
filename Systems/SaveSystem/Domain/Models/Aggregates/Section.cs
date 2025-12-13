/*
===========================================================================
The Nomad AGPL Source Code
Copyright (C) 2025 Noah Van Til

The Nomad Source Code is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published
by the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

The Nomad Source Code is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with The Nomad Source Code.  If not, see <http://www.gnu.org/licenses/>.

If you have questions concerning this license or the applicable additional
terms, you may contact me via email at nyvantil@gmail.com.
===========================================================================
*/

using NomadCore.Interfaces.Common;
using NomadCore.Systems.SaveSystem.Domain.Models.ValueObjects;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace NomadCore.Systems.SaveSystem.Domain.Models.Aggregates {
	/*
	===================================================================================
	
	Section
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	public sealed class Section : ISection {
		public int FieldCount => _fields.Count;

		public SectionId Id => _id;
		private readonly SectionId _id;

		public DateTime CreatedAt => _createdAt;
		private readonly DateTime _createdAt = DateTime.UtcNow;

		public DateTime? ModifiedAt => _saveTime;
		private readonly DateTime? _saveTime = DateTime.UtcNow;

		public int Version => 0;

		public IReadOnlyDictionary<FieldId, Field> Fields => _fields;
		private readonly ImmutableDictionary<FieldId, Field> _fields;

		/*
		===============
		Section
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <param name="fields"></param>
		internal Section( SectionId id, ImmutableDictionary<FieldId, Field> fields ) {
			_id = id;
			_fields = fields;
		}

		/*
		===============
		Equals
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public bool Equals( IEntity<SectionId>? other ) {
			return other is not null && other.Id == _id;
		}

		/*
		===============
		TryGetField
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="fieldId"></param>
		/// <param name="field"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public bool TryGetField( FieldId fieldId, out Field field ) {
			return _fields.TryGetValue( fieldId, out field );
		}

		/*
		===============
		TryGetValue
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="fieldId"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool TryGetValue<T>( FieldId fieldId, out T value ) {
			if ( _fields.TryGetValue( fieldId, out var field ) && FieldValue.GetFieldType<T>() == field.Type ) {
				value = field.As<T>().Value;
				return true;
			}

			value = default;
			return false;
		}
	};
};