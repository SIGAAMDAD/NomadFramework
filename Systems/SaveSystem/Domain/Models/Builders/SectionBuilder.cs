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

using NomadCore.Systems.SaveSystem.Domain.Models.Aggregates;
using NomadCore.Systems.SaveSystem.Domain.Models.ValueObjects;
using System.Collections.Immutable;

namespace NomadCore.Systems.SaveSystem.Domain.Models.Builders {
	public sealed class SectionBuilder {
		private readonly SectionId _id;
		private readonly ImmutableDictionary<FieldId, Field>.Builder _fieldBuilder;

		internal SectionBuilder( SectionId id ) {
			_id = id;
			_fieldBuilder = ImmutableDictionary.CreateBuilder<FieldId, Field>();
		}

		public SectionBuilder AddField<T>( FieldId id, T value ) {
			_fieldBuilder[ id ] = new Field {
				Id = id,
				Type = FieldValue.GetFieldType<T>(),
				SerializedValue = FieldValue.From( value )
			};
			return this;
		}

		public Section Build() {
			var fields = _fieldBuilder.ToImmutable();
			return new Section( _id, fields );
		}
	};
};