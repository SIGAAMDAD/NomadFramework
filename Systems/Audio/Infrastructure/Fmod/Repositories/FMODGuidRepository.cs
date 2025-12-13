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

using NomadCore.Domain.Models.ValueObjects;
using NomadCore.Systems.Audio.Domain.Models.ValueObjects;
using NomadCore.Systems.Audio.Infrastructure.Fmod.Models.ValueObjects;
using NomadCore.Systems.Audio.Infrastructure.Fmod.Services;
using System.Collections.Concurrent;

namespace NomadCore.Systems.Audio.Infrastructure.Fmod.Repositories {
	internal sealed class FMODGuidRepository {
		private readonly ConcurrentDictionary<BankId, FMODBankId> _bankGuids = new ConcurrentDictionary<BankId, FMODBankId>();
		private readonly ConcurrentDictionary<EventId, FMODEventId> _eventGuids = new ConcurrentDictionary<EventId, FMODEventId>();

		public FMODGuidRepository( FMODSystemService fmodSystem ) {
			FMODValidator.ValidateCall( fmodSystem.StudioSystem.getBankCount( out int bankCount ) );
			FMODValidator.ValidateCall( fmodSystem.StudioSystem.getBankList( out FMOD.Studio.Bank[] banks ) );
			for ( int i = 0; i < bankCount; i++ ) {
			}
		}

		public FMODEventId IdToFMOD( EventId id ) {
			return _eventGuids[ id ];
		}
	};
};