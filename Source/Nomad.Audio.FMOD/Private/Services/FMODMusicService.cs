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
using Nomad.Audio.Fmod.Private.Entities;
using Nomad.Audio.Fmod.Private.ValueObjects;
using Nomad.Audio.Interfaces;
using Nomad.Core;
using Nomad.Core.CVars;
using Nomad.Core.Events;
using Nomad.Core.Exceptions;
using Nomad.CVars;
using Nomad.ResourceCache;

namespace Nomad.Audio.Fmod.Private.Services {
	/*
	===================================================================================

	FMODMusicService

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	internal sealed class FMODMusicService : IMusicService {
		public bool IsPlaying => _musicInstance.PlaybackState == FMOD.Studio.PLAYBACK_STATE.PLAYING;

		private FMODEventResource _musicHandle;
		private FMODChannelResource _musicInstance;

		private FMODChannelResource _queuedTheme;

		private readonly IResourceCacheService<IAudioResource, string> _eventRepository;
		private readonly Queue<FMODChannel> _loopingTracks = new Queue<FMODChannel>();

		/*
		===============
		FMODMusicService
		===============
		*/
		/// <summary>
		/// Creates an FMODMusicService
		/// </summary>
		/// <param name="eventRepository"></param>
		/// <param name="cvarSystem"></param>
		/// <exception cref="Exception"></exception>
		public FMODMusicService( IResourceCacheService<IAudioResource, string> eventRepository, ICVarSystemService cvarSystem ) {
			_eventRepository = eventRepository;
		}

		/*
		===============
		PlayTheme
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="name"></param>
		/// <exception cref="InvalidCastException"></exception>
		public void PlayTheme( string name ) {
			_eventRepository.GetCached( name ).Get( out var handle );
			if ( handle is not FMODEventResource resource ) {
				return;
			}
			_musicHandle = resource;
			_musicHandle.CreateInstance( out _musicInstance );
			FMODValidator.ValidateCall( _musicInstance.Start() );
		}

		/*
		===============
		StopTheme
		===============
		*/
		/// <summary>
		///
		/// </summary>
		public void StopTheme() {
			if ( !IsPlaying ) {
				return;
			}
			FMODValidator.ValidateCall( _musicInstance.Stop( FMOD.Studio.STOP_MODE.ALLOWFADEOUT ) );
		}
	};
};
