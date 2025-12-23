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

using System;
using System.Collections.Generic;
using Nomad.Audio.Interfaces;

namespace Nomad.Audio.Fmod.Private.Services {
	/*
	===================================================================================
	
	FMODSystemService
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	internal sealed class FMODSystemService : IAudioDevice {
		public string AudioDriver {
			get => _drivers[ _audioDriver ];
			set {
				if ( _drivers[ _audioDriver ].Equals( value ) ) {
					return;
				}
				SetAudioDriver( value );
			}
		}
		private int _audioDriver;

		public string OutputDevice {
			get => "Unknown";
			set => _outputDevice = value;
		}
		private string _outputDevice;

		public bool CanSetAudioDriver => true;

		private readonly List<string> _drivers = new List<string>();

		public global::FMOD.Studio.System StudioSystem {
			get {
				lock ( _systemLock ) {
					return _systemHandle.StudioSystem;
				}
			}
		}
		public global::FMOD.System System {
			get {
				lock ( _systemLock ) {
					return _systemHandle.System;
				}
			}
		}
		private readonly object _systemLock = new object();

		private readonly FMODSystemHandle _systemHandle;
		private readonly ILoggerService _logger;

		public IResourceCacheService<IEventResource, EventId> EventRepository => _eventRepository;
		private readonly FMODEventRepository _eventRepository;

		private readonly FMODBankRepository _bankRepository;
		
		public IDisposable GuidRepository => _guidRepository;
		private readonly FMODGuidRepository _guidRepository;



		/*
		===============
		FMODSystemService
		===============
		*/
		public FMODSystemService( IServiceLocator locator, IServiceRegistry registry ) {
			_logger = locator.GetService<ILoggerService>();

			var eventFactory = locator.GetService<IGameEventRegistryService>();
			var cvarSystem = locator.GetService<ICVarSystemService>();

			FMODCVarRegistry.Register( cvarSystem );
			_systemHandle = new FMODSystemHandle( cvarSystem, _logger );
			ConfigureFMODDevice( cvarSystem );

			_guidRepository = new FMODGuidRepository();
			_bankRepository = new FMODBankRepository( _logger, eventFactory, cvarSystem, this, _guidRepository );
			_eventRepository = new FMODEventRepository( _logger, eventFactory, this, _guidRepository );

			_logger.PrintLine( $"FMODSystemService: initializing FMOD sound system..." );

			FMODValidator.ValidateCall( System.setCallback( OnAudioOutputDeviceListChanged, FMOD.SYSTEM_CALLBACK_TYPE.DEVICELISTCHANGED ) );
		}

		/*
		===============
		Dispose
		===============
		*/
		public void Dispose() {
			_logger.PrintLine( "FMODSystemService.Dispose: shutting down FMOD sound system..." );

			_eventRepository?.Dispose();
			_bankRepository?.Dispose();
			_guidRepository?.Dispose();
			_systemHandle.Dispose();
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
		GetAudioDriverNames
		===============
		*/
		public IReadOnlyList<string> GetAudioDriverNames() {
			return _drivers;
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
		private RESULT OnAudioOutputDeviceListChanged( nint system, FMOD.SYSTEM_CALLBACK_TYPE type, nint commanddata1, nint commanddata2, nint userdata ) {
			GetAudioDrivers();
			return RESULT.OK;
		}

		/*
		===============
		GetAudioDrivers
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		private void GetAudioDrivers() {
			var system = System;
			
			FMODValidator.ValidateCall( system.getNumDrivers( out int numDrivers ) );
			_drivers.Clear();
			_drivers.EnsureCapacity( numDrivers );

			for ( int i = 0; i < numDrivers; i++ ) {
				FMODValidator.ValidateCall( system.getDriverInfo( i, out string name, 256, out _, out _, out FMOD.SPEAKERMODE speakerMode, out int speakerChannels ) );
				_drivers.Add( name );
				_logger.PrintLine( $"FMODSystemService.GetAudioDrivers: found audio driver '{name}' - speakerMode = '{speakerMode}', channelCount = '{speakerChannels}'" );
			}
			FMODValidator.ValidateCall( system.getDriver( out _audioDriver ) );
		}

		/*
		===============
		SetAudioDriver
		===============
		*/
		/// <summary>
		/// s
		/// </summary>
		/// <param name="driverName"></param>
		private void SetAudioDriver( string driverName ) {
			int driverIndex = -1;
			for ( int i = 0; i < _drivers.Count; i++ ) {
				if ( _drivers[ i ].Equals( driverName ) ) {
					driverIndex = i;
				}
			}
			if ( driverIndex == -1 ) {
				_logger.PrintError( $"FMODSystemService.SetAudioDriver: invalid audio driver id '{driverName}'" );
				return;
			}

			_logger.PrintLine( $"FMODSystemService.SetAudioDriver: setting audio driver to '{driverName}'..." );
			FMODValidator.ValidateCall( System.setDriver( driverIndex ) );
			_audioDriver = driverIndex;
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
			var maxChannels = cvarSystem.GetCVar<bool>( Constants.CVars.Audio.MAX_CHANNELS )
				?? throw new CVarMissing( Constants.CVars.Audio.MAX_CHANNELS );

			var flags = FMOD.INITFLAGS.CHANNEL_DISTANCEFILTER | FMOD.INITFLAGS.CHANNEL_LOWPASS | FMOD.INITFLAGS.VOL0_BECOMES_VIRTUAL;

			FMODValidator.ValidateCall( System.setStreamBufferSize( (uint)streamBufferSize.Value, FMOD.TIMEUNIT.MS ) );
			FMODValidator.ValidateCall( System.setDSPBufferSize( (uint) ) );
			FMODValidator.ValidateCall( StudioSystem.initialize( 512, FMOD.Studio.INITFLAGS.LIVEUPDATE | FMOD.Studio.INITFLAGS.SYNCHRONOUS_UPDATE, flags, 0 ) );

			GetAudioDrivers();
			_audioDriver = 0;
		}
	};
};