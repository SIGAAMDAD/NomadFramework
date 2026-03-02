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

using System.Threading;
using System.Threading.Tasks;
using Nomad.Audio.Fmod.Private.Services;
using Nomad.Audio.Fmod.Private.ValueObjects;
using Nomad.Audio.Interfaces;
using Nomad.Core.Logger;
using Nomad.Core.Util;
using Nomad.Core.ResourceCache;

namespace Nomad.Audio.Fmod.Private.Repositories.Loaders {
	/*
	===================================================================================

	FMODEventLoader

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	internal sealed class FMODEventLoader : IResourceLoader<IAudioResource, string> {
		public LoadCallback<IAudioResource, string> Load => LoadEvent;
		public LoadAsyncCallback<IAudioResource, string> LoadAsync => LoadEventAsync;

		private readonly FMODDevice _fmodSystem;
		private readonly FMODGuidRepository _guidRepository;
		private readonly ILoggerService _logger;

		/*
		===============
		FMODEventLoader
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="fmodSystem"></param>
		/// <param name="guidRepository"></param>
		/// <param name="logger"></param>
		public FMODEventLoader( FMODDevice fmodSystem, FMODGuidRepository guidRepository, ILoggerService logger ) {
			_fmodSystem = fmodSystem;
			_guidRepository = guidRepository;
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
		}

		/*
		===============
		LoadEvent
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		private Result<IAudioResource> LoadEvent( string id ) {
			try {
				FMODValidator.ValidateCall( _fmodSystem.StudioSystem.getEvent( id, out var eventDescription ) );
				FMODValidator.ValidateCall( eventDescription.getID( out var guid ) );
				_guidRepository.AddEventId( id, guid );
				_logger.PrintLine( $"FMODEventLoader.LoadEvent: loaded event '{id}'" );

				return Result<IAudioResource>.Success( new FMODEventResource( eventDescription ) );
			} catch ( FMODException e ) {
				_logger.PrintError( $"FMODEventLoader.LoadEvent: failed to load event '{id}' - {e.Error}" );
				throw;
			}
		}

		/*
		===============
		LoadEventAsync
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="id"></param>
		/// <param name="ct"></param>
		/// <returns></returns>
		private async Task<Result<IAudioResource>> LoadEventAsync( string id, CancellationToken ct = default ) {
			try {
				ct.ThrowIfCancellationRequested();

				FMODValidator.ValidateCall( _fmodSystem.StudioSystem.getEvent( id, out var eventDescription ) );
				FMODValidator.ValidateCall( eventDescription.getID( out var guid ) );
				_guidRepository.AddEventId( id, guid );
				_logger.PrintLine( $"FMODEventLoader.LoadEventAsync: loaded event '{id}'" );

				return Result<IAudioResource>.Success( new FMODEventResource( eventDescription ) );
			} catch ( FMODException e ) {
				_logger.PrintError( $"FMODEventLoader.LoadEventAsync: failed to load event '{id}' - {e.Error}" );
				throw;
			}
		}
	};
};
