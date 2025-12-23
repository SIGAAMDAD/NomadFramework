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

using System.Threading;
using System.Threading.Tasks;
using Nomad.Audio.Entities;
using Nomad.Audio.Fmod.Private.Entities;
using Nomad.Audio.Fmod.Private.Services;
using Nomad.Audio.Fmod.Private.ValueObjects;
using Nomad.Audio.ValueObjects;
using Nomad.Core.Logger;
using Nomad.Core.Util;
using Nomad.ResourceCache;

namespace Nomad.Audio.Fmod.Private.Repositories.Loaders {
	/*
	===================================================================================

	FMODBankLoader

	===================================================================================
	*/
	/// <summary>
	/// Loads FMOD banks from disk.
	/// </summary>

	internal sealed class FMODBankLoader( FMODSystemService fmodSystem, FMODGuidRepository guidRepository, ILoggerService logger ) : IResourceLoader<BankComposite, BankId> {
		public LoadCallback<BankComposite, BankId> Load => LoadBank;
		public LoadAsyncCallback<BankComposite, BankId> LoadAsync => LoadBankAsync;

		/*
		===============
		LoadBank
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		private Result<BankComposite> LoadBank( BankId id ) {
			try {
				var filePath = FilePath.FromResourcePath( $"res://Assets/Audio/Banks/{id.Name}" );

				FMODValidator.ValidateCall( fmodSystem.StudioSystem.loadBankFile( filePath.OSPath, FMOD.Studio.LOAD_BANK_FLAGS.NORMAL, out var bank ) );
				FMODValidator.ValidateCall( bank.getEventList( out var events ) );
				FMODValidator.ValidateCall( bank.getID( out var bankId ) );

				logger.PrintLine( $"FMODBankLoader.LoadBank: loaded bank '{id.Name}'" );

				var eventCollection = new FMODEventCollection( events, guidRepository, logger );
				var resource = new FMODBankResource( bank );
				var metadata = new FMODBankMetadata( id );

				guidRepository.AddBankId( id.Name, new FMODBankId( bankId ) );

				return Result<BankComposite>.Success( new BankComposite( eventCollection, resource, metadata ) );
			} catch ( FMODException e ) {
				logger.PrintError( $"FMODBankLoader.LoadBank: failed to load bank '{id.Name}' - {e.Error}" );
				throw;
			}
		}

		/*
		===============
		LoadBankAsync
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="id"></param>
		/// <param name="ct"></param>
		/// <returns></returns>
		private async Task<Result<BankComposite>> LoadBankAsync( BankId id, CancellationToken ct = default ) {
			try {
				ct.ThrowIfCancellationRequested();

				var filePath = FilePath.FromResourcePath( $"res://Assets/Audio/Banks/{id.Name}" );

				FMODValidator.ValidateCall( fmodSystem.StudioSystem.loadBankFile( filePath.OSPath, FMOD.Studio.LOAD_BANK_FLAGS.NONBLOCKING, out var bank ) );
				FMODValidator.ValidateCall( bank.getEventList( out var events ) );
				FMODValidator.ValidateCall( bank.getID( out var bankId ) );

				var eventCollection = new FMODEventCollection( events, guidRepository, logger );
				var resource = new FMODBankResource( bank );
				var metadata = new FMODBankMetadata( id );

				guidRepository.AddBankId( id.Name, new FMODBankId( bankId ) );

				logger.PrintLine( $"FMODBankLoader.LoadBankAsync: loaded bank '{id.Name}'" );

				return Result<BankComposite>.Success( new BankComposite( eventCollection, resource, metadata ) );
			} catch ( FMODException e ) {
				logger.PrintError( $"FMODBankLoader.LoadBank: failed to load bank '{id.Name}' - {e.Error}" );
				throw;
			}
		}
	};
};
