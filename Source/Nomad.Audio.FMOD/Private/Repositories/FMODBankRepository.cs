/*
===========================================================================
The Nomad Framework
Copyright (C) 2025 Noah Van Til

This Source Code Form is subject to the terms of the Mozilla Public
License, v2. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.

This software is provided "as is", without warranty of any kind,
express or implied, including but not limited to the warranties
of merchantability, fitness for a particular purpose and noninfringement.
===========================================================================
*/

using System;
using Nomad.Audio.Fmod.Private.Repositories.Loaders;
using Nomad.Audio.Fmod.ValueObjects;
using Nomad.Audio.ValueObjects;
using Nomad.Core.Logger;

namespace Nomad.Audio.Fmod.Private.Repositories {
	/*
	===================================================================================

	FMODBankRepository

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	internal sealed class FMODBankRepository : BaseCache<BankComposite, BankId> {
		private const string BANK_PATH = "res://Assets/Audio/Banks/";

		private readonly FMODBankLoadingStrategy _loadingStrategy;

		public FMODBankRepository( ILoggerService logger, IGameEventRegistryService eventFactory, ICVarSystemService cvarSystem, FMODSystemService fmodSystem, FMODGuidRepository guidRepository )
			: base( logger, eventFactory, new FMODBankLoader( fmodSystem, guidRepository, logger ) )
		{
			var bankLoadingStrategy = cvarSystem.GetCVar<FMODBankLoadingStrategy>( AudioConstants.CVars.FMOD.FMOD_BANK_LOADING_STRATEGY ) ?? throw new CVarMissing( AudioConstants.CVars.FMOD.FMOD_BANK_LOADING_STRATEGY );
			_loadingStrategy = bankLoadingStrategy.Value;

			if ( _loadingStrategy == FMODBankLoadingStrategy.Streaming ) {

			}
		}

		/*
		===============
		LoadBanks
		===============
		*/
		private void LoadBanks( ILoggerService logger ) {
			string path = FilePath.FromResourcePath( BANK_PATH ).OSPath;
			try {
				var files = System.IO.Directory.GetFiles( path, ".bank, .strings.bank" );
				for ( int i = 0; i < files.Length; i++ ) {
				}
			} catch ( Exception e ) {
				logger.PrintError( $"FMODBankRepository.LoadBanks: error loading banks from {path}\n{e}" );
				throw;
			}
		}
	};
};
