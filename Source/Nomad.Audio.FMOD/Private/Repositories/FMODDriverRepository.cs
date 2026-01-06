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
using System.Collections.Immutable;
using Nomad.Audio.Fmod.Private.ValueObjects;
using Nomad.Core;
using Nomad.Core.Exceptions;
using Nomad.Core.Logger;
using Nomad.CVars;

namespace Nomad.Audio.Fmod.Private.Repositories {
	/*
	===================================================================================

	FMODDriverRepository

	===================================================================================
	*/
	/// <summary>
	/// Holds FMOD audio driver data.
	/// </summary>

	internal sealed class FMODDriverRepository : IDisposable {
		public FMODDeviceInfo[] Devices => _devices;
		private FMODDeviceInfo[] _devices;

		public int OutputDeviceIndex {
			get => _outputDeviceIndex;
			set {
				SetOutputDevice( value );
			}
		}
		private int _outputDeviceIndex = 0;

		public string[] Drivers => [ .. _supportedAudioDrivers.Values ];
		public string Driver => _supportedAudioDrivers[ _audioDriver ];
		private FMOD.OUTPUTTYPE _audioDriver;

		private readonly ImmutableDictionary<FMOD.OUTPUTTYPE, string> _supportedAudioDrivers;

		private readonly ILoggerService _logger;
		private readonly FMOD.System _system;

		/*
		===============
		FMODDriverRepository
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="logger"></param>
		/// <param name="cvarSystem"></param>
		/// <param name="system"></param>
		public FMODDriverRepository( ILoggerService logger, ICVarSystemService cvarSystem, FMOD.System system ) {
			_logger = logger;
			_system = system;

			_supportedAudioDrivers = new Dictionary<FMOD.OUTPUTTYPE, string>() {
				[ FMOD.OUTPUTTYPE.AUTODETECT ] = "Auto Detect",
			#if WINDOWS
				[ FMOD.OUTPUTTYPE.ASIO ] = "ASIO",
				[ FMOD.OUTPUTTYPE.WASAPI ] = "WasAPI",
				[ FMOD.OUTPUTTYPE.WINSONIC ] = "WinSonic",
			#elif LINUX
				[ FMOD.OUTPUTTYPE.ALSA ] = "ALSA",
				[ FMOD.OUTPUTTYPE.PULSEAUDIO ] = "PulseAudio"
			#endif
			}.ToImmutableDictionary();

			var audioDriver = cvarSystem.GetCVar<string>( Constants.CVars.Audio.AUDIO_DRIVER ) ?? throw new CVarMissing( Constants.CVars.Audio.AUDIO_DRIVER );
			audioDriver.ValueChanged.Subscribe( this, OnAudioDriverValueChanged );

			var outputDeviceIndex = cvarSystem.GetCVar<int>( Constants.CVars.Audio.OUTPUT_DEVICE_INDEX ) ?? throw new CVarMissing( Constants.CVars.Audio.OUTPUT_DEVICE_INDEX );
			outputDeviceIndex.ValueChanged.Subscribe( this, OnAudioDeviceValueChanged );

			FMODValidator.ValidateCall( _system.setCallback( OnAudioOutputDeviceListChanged, FMOD.SYSTEM_CALLBACK_TYPE.DEVICELISTCHANGED ) );

			GetAudioDeviceData();
			FMODValidator.ValidateCall( _system.getOutput( out _audioDriver ) );
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
			_devices = null;
		}

		/*
		===============
		SetOutputDevice
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="driverIndex"></param>
		public void SetOutputDevice( int deviceIndex ) {
			if ( deviceIndex < 0 || deviceIndex >= _devices.Length ) {
				throw new ArgumentOutOfRangeException( nameof( deviceIndex ) );
			}
			var device = _devices[ deviceIndex ];

			_logger.PrintLine( $"FMODDriverRepository.SetOutputDevice: setting output audio device to '{device.Name}'..." );
			FMODValidator.ValidateCall( _system.setDriver( deviceIndex ) );
			_outputDeviceIndex = deviceIndex;
		}

		/*
		===============
		GetAudioDeviceData
		===============
		*/
		/// <summary>
		///
		/// </summary>
		private void GetAudioDeviceData() {
			FMODValidator.ValidateCall( _system.getNumDrivers( out int numDrivers ) );

			_devices = new FMODDeviceInfo[ numDrivers ];
			for ( int i = 0; i < numDrivers; i++ ) {
				FMODValidator.ValidateCall( _system.getDriverInfo( i, out string name, 256, out var guid, out int systemRate, out FMOD.SPEAKERMODE speakerMode, out int speakerChannels ) );
				_devices[ i ] = new FMODDeviceInfo( name, guid, systemRate, speakerMode, speakerChannels );
				_logger.PrintLine( $"FMODDevice.GetAudioDeviceData: found audio output device '{name}' - speakerMode = '{speakerMode}', channelCount = '{speakerChannels}'" );
			}

			_system.getDriver( out _outputDeviceIndex );
		}

		/*
		===============
		OnAudioDriverValueChanged
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="args"></param>
		private void OnAudioDriverValueChanged( in CVarValueChangedEventArgs<string> args ) {
			FMOD.OUTPUTTYPE outputType = FMOD.OUTPUTTYPE.MAX;

			foreach ( var drivers in _supportedAudioDrivers ) {
				if ( drivers.Value.Equals( args.NewValue ) ) {
					outputType = drivers.Key;
					break;
				}
			}
			if ( outputType == FMOD.OUTPUTTYPE.MAX ) {
				_logger.PrintError( $"FMODDriverRepository.OnAudioDriverValueChanged: invalid audio driver name '{args.NewValue}'" );
				return;
			}

			_audioDriver = outputType;
			_logger.PrintLine( $"FMODDriverRepository.OnAudioDriverValueChanged: setting audio driver API to '{_audioDriver}'..." );
			FMODValidator.ValidateCall( _system.setOutput( _audioDriver ) );
		}

		/*
		===============
		OnAudioDeviceValueChanged
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="args"></param>
		private void OnAudioDeviceValueChanged( in CVarValueChangedEventArgs<int> args ) {
			SetOutputDevice( args.NewValue );
		}

		/*
		===============
		OnAudioOutputDeviceListChanged
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="system"></param>
		/// <param name="type"></param>
		/// <param name="commanddata1"></param>
		/// <param name="commanddata2"></param>
		/// <param name="userdata"></param>
		/// <returns></returns>
		private FMOD.RESULT OnAudioOutputDeviceListChanged( nint system, FMOD.SYSTEM_CALLBACK_TYPE type, nint commanddata1, nint commanddata2, nint userdata ) {
			GetAudioDeviceData();
			return FMOD.RESULT.OK;
		}
	};
};
