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
using System.Collections.Generic;
using Nomad.Audio.Fmod.Entities;
using Nomad.Audio.Fmod.Private.Entities;
using Nomad.Audio.Fmod.Private.ValueObjects;
using Nomad.Audio.Interfaces;
using Nomad.Core;
using Nomad.Core.Util;
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

		private float _musicVolume = 0.0f;
		private bool _musicOn = true;

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

			var musicVolume = cvarSystem.GetCVar<float>( Constants.CVars.Audio.MUSIC_VOLUME ) ?? throw new Exception( "Missing CVar 'audio.MusicVolume'" );
			musicVolume.ValueChanged.Subscribe( this, OnMusicVolumeChanged );
			_musicVolume = musicVolume.Value;

			var musicOn = cvarSystem.GetCVar<bool>( Constants.CVars.Audio.MUSIC_ON ) ?? throw new Exception( "Missing CVar 'audio.MusicOn'" );
			musicOn.ValueChanged.Subscribe( this, OnMusicOnChanged );
			_musicOn = musicOn.Value;
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
			if ( !_musicOn ) {
				QueueTheme( name );
				// TODO: queue the requested theme to play if we enable music
				return;
			}

			_eventRepository.GetCached( name ).Get( out var handle );
			if ( handle is not FMODEventResource resource ) {
				return;
			}
			_musicHandle = resource;
			_musicHandle.CreateInstance( out _musicInstance );
			_musicInstance.Volume = _musicVolume / 100.0f;
			FMODValidator.ValidateCall( _musicInstance.instance.start() );
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
			if ( !_musicOn || !IsPlaying ) {
				return;
			}
			FMODValidator.ValidateCall( _musicInstance.instance.stop( FMOD.Studio.STOP_MODE.ALLOWFADEOUT ) );
		}

		/*
		===============
		QueueTheme
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="eventId"></param>
		/// <exception cref="InvalidCastException"></exception>
		private void QueueTheme( string eventId ) {
			_eventRepository.GetCached( eventId ).Get( out var handle );
			if ( handle is not FMODEventResource resource ) {
				return;
			}
			resource.CreateInstance( out _queuedTheme );
		}

		/*
		===============
		OnMusicOnChanged
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="args"></param>
		private void OnMusicOnChanged( in CVarValueChangedEventArgs<bool> args ) {
			_musicOn = args.NewValue;
			if ( !_musicOn ) {
				FMODValidator.ValidateCall( _musicInstance.instance.stop( FMOD.Studio.STOP_MODE.IMMEDIATE ) );
			}
		}

		/*
		===============
		OnMusicVolumeChanged
		===============
		*/
		private void OnMusicVolumeChanged( in CVarValueChangedEventArgs<float> args ) {
			_musicVolume = args.NewValue;
			if ( IsPlaying ) {
				_musicInstance.Volume = _musicVolume / 100.0f;
			}
		}
	};
};
