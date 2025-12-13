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
using NomadCore.Systems.Audio.Domain.Interfaces;
using NomadCore.Systems.Audio.Domain.Models.ValueObjects;
using System;

namespace NomadCore.Systems.Audio.Domain.Models.Aggregates {
	public sealed class EventComposite( IEventMetadata metadata, IEventResource resource ) : IAudioEvent {
		public bool IsPlaying => throw new NotImplementedException();

		public EventId Id => throw new NotImplementedException();

		public DateTime CreatedAt => throw new NotImplementedException();

		public DateTime? ModifiedAt => throw new NotImplementedException();

		public int Version => throw new NotImplementedException();

		private readonly IEventMetadata _metadata = metadata;
		private readonly IEventResource _resource = resource;

		public bool Equals( IEntity<EventId>? other ) {
			return other?.Id == Id;
		}
		
		/*
		===============
		Dispose
		===============
		*/
		public void Dispose() {
			_resource.Dispose();
		}

		public IAudioParameter GetAudioParameter( ParameterId id ) {
			
		}
	};
};