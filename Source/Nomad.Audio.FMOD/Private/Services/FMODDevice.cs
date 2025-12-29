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

#if DEBUG
#define FMOD_LOGGING
#endif

using Godot;
using System;
using System.Collections.Generic;
using Nomad.Audio.Fmod.Private.Registries;
using Nomad.Audio.Fmod.Private.Repositories;
using Nomad.Audio.Fmod.Private.ValueObjects;
using Nomad.Audio.Interfaces;
using Nomad.Audio.ValueObjects;
using Nomad.Core;
using Nomad.Core.Exceptions;
using Nomad.Core.Logger;
using Nomad.Core.ServiceRegistry.Interfaces;
using Nomad.CVars;

namespace Nomad.Audio.Fmod.Private.Services {
	/*
	===================================================================================

	FMODDevice

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	internal sealed class FMODDevice : IAudioDevice {
		public string AudioDriver => new( _driverRepository.Drivers[ _driverRepository.DriverIndex ].Name.ToArray() );

		public FMOD.Studio.System StudioSystem => _systemHandle.StudioSystem;
		public FMOD.System System => _systemHandle.System;

		private readonly FMODSystemHandle _systemHandle;
		private readonly ILoggerService _logger;

		public FMODEventRepository EventRepository => _eventRepository;
		private readonly FMODEventRepository _eventRepository;

		private readonly FMODBusRepository _busRepository;
		private readonly FMODBankRepository _bankRepository;

		public FMODGuidRepository GuidRepository => _guidRepository;
		private readonly FMODGuidRepository _guidRepository;

		private readonly FMODListenerService _listener;
		private readonly FMODDriverRepository _driverRepository;
		private readonly FMODChannelRepository _channelRepository;

		/*
		===============
		FMODDevice
		===============
		*/
		public FMODDevice( IServiceLocator locator, IServiceRegistry registry ) {
			_logger = locator.GetService<ILoggerService>();

			var cvarSystem = locator.GetService<ICVarSystemService>();

			FMODCVarRegistry.Register( cvarSystem );
			_systemHandle = new FMODSystemHandle( cvarSystem, _logger );
			ConfigureFMODDevice( cvarSystem );

			_driverRepository = new FMODDriverRepository( _logger, cvarSystem, _systemHandle.System );
			_listener = new FMODListenerService( _logger, this );

			_guidRepository = new FMODGuidRepository();
			_bankRepository = new FMODBankRepository( _logger, cvarSystem, _systemHandle.StudioSystem, _guidRepository );
			_eventRepository = new FMODEventRepository( _logger, _systemHandle.StudioSystem );
			_busRepository = new FMODBusRepository( _systemHandle.StudioSystem );
			_channelRepository = new FMODChannelRepository( _logger, cvarSystem, _listener, _eventRepository, _guidRepository, _busRepository );

			_logger.PrintLine( $"FMODDevice: initializing FMOD sound system..." );
		}

		/*
		===============
		Dispose
		===============
		*/
		public void Dispose() {
			_logger.PrintLine( "FMODDevice.Dispose: shutting down FMOD sound system..." );

			_listener?.Dispose();
			_channelRepository?.Dispose();
			_eventRepository?.Dispose();
			_bankRepository?.Dispose();
			_guidRepository?.Dispose();
			_busRepository?.Dispose();
			_systemHandle.Dispose();
		}

		/*
		===============
		GetAudioDrivers
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		public IEnumerable<string> GetAudioDrivers() {
			string[] drivers = new string[ _driverRepository.Drivers.Length ];
			for ( int i = 0; i < drivers.Length; i++ ) {
				drivers[ i ] = new( _driverRepository.Drivers[ i ].Name.Span );
			}
			return drivers;
		}

		/*
		===============
		Update
		===============
		*/
		public void Update( float deltaTime ) {
			_systemHandle.Update();
		}

		/*
		===============
		LoadBank
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="bankPath"></param>
		/// <param name="bank"></param>
		/// <returns></returns>
		public AudioResult LoadBank( string bankPath, out BankHandle bank ) {
			return _bankRepository.LoadBank( bankPath, out bank );
		}

		/*
		===============
		UnloadBank
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="bank"></param>
		/// <returns></returns>
		public AudioResult UnloadBank( BankHandle bank ) {
			return _bankRepository.UnloadBank( bank );
		}

		/*
		===============
		CreateEvent
		===============
		*/
		public AudioResult CreateEvent( string assetPath, out EventHandle eventHandle ) {
			return _eventRepository.CreateEvent( assetPath, out eventHandle );
		}

		/*
		===============
		TriggerEvent
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="eventHandle"></param>
		/// <param name="config"></param>
		/// <param name="channel"></param>
		/// <returns></returns>
		public AudioResult TriggerEvent( EventHandle eventHandle, ChannelGroupHandle group, out ChannelHandle channel ) {
			var result = _eventRepository.GetEventDescription( eventHandle, out var resource );
			if ( result != AudioResult.Success ) {
				channel = new( 0 );
				return result;
			}
			var soundConfig = _busRepository.GetSoundCategory( group );
			return _channelRepository.TriggerEvent( resource, Vector2.Zero, soundConfig, out channel );
		}

		/*
		===============
		SetChannelVolume
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="channel"></param>
		/// <param name="volume"></param>
		/// <returns></returns>
		public AudioResult SetChannelVolume( ChannelHandle channel, float volume ) {
			var resource = _channelRepository.GetChannel( channel );
			resource.Volume = volume;
			return AudioResult.Success;
		}

		/*
		===============
		SetChannelPitch
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="channel"></param>
		/// <param name="pitch"></param>
		/// <returns></returns>
		public AudioResult SetChannelPitch( ChannelHandle channel, float pitch ) {
			var resource = _channelRepository.GetChannel( channel );
			resource.Pitch = pitch;
			return AudioResult.Success;
		}

		/*
		===============
		GetChannelStatus
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="channel"></param>
		/// <param name="status"></param>
		/// <returns></returns>
		public AudioResult GetChannelStatus( ChannelHandle channel, out ChannelStatus status ) {
			var resource = _channelRepository.GetChannel( channel );
			status = resource.Instance.PlaybackState switch {
				FMOD.Studio.PLAYBACK_STATE.PLAYING or FMOD.Studio.PLAYBACK_STATE.STARTING => ChannelStatus.Playing,
				FMOD.Studio.PLAYBACK_STATE.STOPPED or FMOD.Studio.PLAYBACK_STATE.STOPPING => ChannelStatus.Stopped,
				FMOD.Studio.PLAYBACK_STATE.SUSTAINING => ChannelStatus.Looping
			};
			return AudioResult.Success;
		}

		/*
		===============
		StopChannel
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="channel"></param>
		/// <returns></returns>
		public AudioResult StopChannel( ChannelHandle channel ) {
			var resource = _channelRepository.GetChannel( channel );
			resource.Instance.instance.stop( FMOD.Studio.STOP_MODE.ALLOWFADEOUT );
			return AudioResult.Success;
		}

		/*
		===============
		SetParameterValue
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="eventHandle"></param>
		/// <param name="parameterName"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public AudioResult SetParameterValue( EventHandle eventHandle, string parameterName, float value ) {
			return AudioResult.Success;
		}

		/*
		===============
		CreateChannelGroup
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="category"></param>
		/// <param name="group"></param>
		/// <returns></returns>
		public AudioResult CreateChannelGroup( SoundCategoryCreateInfo category, out ChannelGroupHandle group ) {
			return _busRepository.CreateChannelGroup( category, out group );
		}

		/*
		===============
		GetChannelGroup
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="groupname"></param>
		/// <param name="group"></param>
		/// <returns></returns>
		public AudioResult GetChannelGroup( string groupname, out ChannelGroupHandle group ) {
			return _busRepository.GetChannelGroup( groupname, out group );
		}

		/*
		===============
		StopChannelGroup
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="group"></param>
		/// <returns></returns>
		public AudioResult StopChannelGroup( ChannelGroupHandle group ) {
			return _busRepository.StopChannelGroup( group );
		}

		/*
		===============
		SetChannelGroupVolume
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="group"></param>
		/// <param name="volume"></param>
		/// <returns></returns>
		public AudioResult SetChannelGroupVolume( ChannelGroupHandle group, float volume ) {
			return _busRepository.SetChannelGroupVolume( group, volume );
		}

		/*
		===============
		SetChannelGroupMute
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="group"></param>
		/// <param name="mute"></param>
		/// <returns></returns>
		public AudioResult SetChannelGroupMute( ChannelGroupHandle group, bool mute ) {
			return _busRepository.SetChannelGroupMute( group, mute );
		}

		/*
		===============
		SetChannelGroupPitch
		===============
		*/
		public AudioResult SetChannelGroupPitch( ChannelGroupHandle group, float pitch ) {
			return _busRepository.SetChannelGroupPitch( group, pitch );
		}

		/*
		===============
		SetListenerPosition
		===============
		*/
		public AudioResult SetListenerPosition( int listenerIndex, Vector2 position ) {
			_listener.SetListenerPosition( listenerIndex, position );
			return AudioResult.Success;
		}

		/*
		===============
		ConfigureFMODDevice
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="cvarSystem"></param>
		/// <exception cref="Exception"></exception>
		private void ConfigureFMODDevice( ICVarSystemService cvarSystem ) {
			var streamBufferSize = cvarSystem.GetCVar<int>( Constants.CVars.Audio.FMOD.STREAM_BUFFER_SIZE )
				?? throw new CVarMissing( Constants.CVars.Audio.FMOD.STREAM_BUFFER_SIZE );
			var maxChannels = cvarSystem.GetCVar<int>( Constants.CVars.Audio.MAX_CHANNELS )
				?? throw new CVarMissing( Constants.CVars.Audio.MAX_CHANNELS );
			var dspBufferSize = cvarSystem.GetCVar<uint>( Constants.CVars.Audio.FMOD.DSP_BUFFER_SIZE ) ?? throw new CVarMissing( Constants.CVars.Audio.FMOD.DSP_BUFFER_SIZE );
			var dspBufferCount = cvarSystem.GetCVar<int>( Constants.CVars.Audio.FMOD.DSP_BUFFER_COUNT ) ?? throw new CVarMissing( Constants.CVars.Audio.FMOD.DSP_BUFFER_COUNT );

			var flags = FMOD.INITFLAGS.CHANNEL_DISTANCEFILTER | FMOD.INITFLAGS.CHANNEL_LOWPASS | FMOD.INITFLAGS.VOL0_BECOMES_VIRTUAL;

			FMODValidator.ValidateCall( System.setStreamBufferSize( ( uint )streamBufferSize.Value, FMOD.TIMEUNIT.MS ) );
			FMODValidator.ValidateCall( System.setDSPBufferSize( dspBufferSize.Value * 1024, dspBufferCount.Value ) );
			FMODValidator.ValidateCall( StudioSystem.initialize( maxChannels.Value, FMOD.Studio.INITFLAGS.LIVEUPDATE | FMOD.Studio.INITFLAGS.SYNCHRONOUS_UPDATE, flags, 0 ) );
		}
	};
};
