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
using NomadCore.GameServices;
using NomadCore.Systems.Audio.Domain.Interfaces;
using NomadCore.Systems.Audio.Domain.Models.Aggregates;
using NomadCore.Systems.Audio.Domain.Models.ValueObjects;
using NomadCore.Systems.Audio.Infrastructure.Fmod.Repositories.Loaders;
using NomadCore.Systems.Audio.Infrastructure.Fmod.Services;
using NomadCore.Systems.ResourceCache.Common;
using System.Collections.Generic;

namespace NomadCore.Systems.Audio.Infrastructure.Fmod.Repositories {
	/*
	===================================================================================
	
	FMODBankRepository
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	internal sealed class FMODBankRepository : BaseCache<BankComposite, FilePath>, IBankCompositeFactory {
		private readonly ILoggerService _logger;
		private readonly Dictionary<FilePath, BankId> _bankCache;

		public FMODBankRepository( ILoggerService logger, IGameEventRegistryService eventFactory, FMODSystemService fmodSystem )
			: base( logger, eventFactory, new FMODBankLoader( fmodSystem ) )
		{
			_logger = logger;

			var files = System.IO.Directory.GetFiles( FilePath.FromResourcePath( "res://Assets/Audio/Banks" ) );
		}

		public IAudioBank Create( BankId id ) {
			if ( TryGetById( ) )
		}
	};
};