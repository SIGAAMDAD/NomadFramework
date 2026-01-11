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
using Nomad.Audio.Fmod.Private.Registries;
using Nomad.Audio.Fmod.Private.Repositories;
using Nomad.Audio.Fmod.Private.ValueObjects;
using Nomad.Audio.Interfaces;
using Nomad.Core;
using Nomad.Core.Exceptions;
using Nomad.Core.Logger;
using Nomad.Core.ServiceRegistry.Interfaces;
using Nomad.CVars;
using Nomad.Core.Events;

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
		public int OutputDevice => _driverRepository.OutputDeviceIndex;
		public string AudioDriver => _driverRepository.Driver;

		public FMOD.Studio.System StudioSystem => _systemHandle.StudioSystem;
		public FMOD.System System => _systemHandle.System;

		private readonly FMODSystemHandle _systemHandle;
		private readonly ILoggerService _logger;
		private readonly ILoggerCategory _fmodCategory;

		public FMODEventRepository EventRepository => _eventRepository;
		private readonly FMODEventRepository _eventRepository;

		private readonly FMODBankRepository _bankRepository;

		public FMODGuidRepository GuidRepository => _guidRepository;
		private readonly FMODGuidRepository _guidRepository;

		private readonly FMODListenerService _listener;
		private readonly FMODDriverRepository _driverRepository;

		/*
		===============
		FMODDevice
		===============
		*/
		public FMODDevice( IServiceLocator locator, IServiceRegistry registry ) {
			_logger = locator.GetService<ILoggerService>();

			var cvarSystem = locator.GetService<ICVarSystemService>();
			var eventFactory = locator.GetService<IGameEventRegistryService>();

			_fmodCategory = _logger.CreateCategory( "FMOD", LogLevel.Info, true );

			FMODCVarRegistry.Register( cvarSystem );
			_systemHandle = new FMODSystemHandle( cvarSystem, _logger );
			ConfigureFMODDevice( cvarSystem );

			_driverRepository = new FMODDriverRepository( _logger, cvarSystem, _systemHandle.System );
			_listener = new FMODListenerService( _logger, this );

			_guidRepository = new FMODGuidRepository();
			_bankRepository = new FMODBankRepository( _logger, eventFactory, this );
			_eventRepository = new FMODEventRepository( _logger, eventFactory, this );

			// preload the strings bank so we get all the human-readable event names cached
			_bankRepository.GetCached( "res://Assets/Audio/Banks/Desktop/Master.strings.bank" );
			_bankRepository.GetCached( "res://Assets/Audio/Banks/Desktop/Master.bank" );

			_logger.PrintLine( in _fmodCategory, $"FMODDevice: initializing FMOD sound system..." );
		}

		/*
		===============
		Dispose
		===============
		*/
		public void Dispose() {
			_logger.PrintLine( in _fmodCategory, "FMODDevice.Dispose: shutting down FMOD sound system..." );

			_listener?.Dispose();
			_eventRepository?.Dispose();
			_bankRepository?.Dispose();
			_guidRepository?.Dispose();
			_systemHandle.Dispose();
		}

		/*
		===============
		LoadBank
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="assetPath"></param>
		public void LoadBank( string assetPath ) {
			_bankRepository.GetCached( assetPath );
		}

		/*
		===============
		UnloadBank
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="assetPath"></param>
		public void UnloadBank( string assetPath ) {
			_bankRepository.Unload( assetPath );
		}

		/*
		===============
		GetOutputDevices
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		public IEnumerable<string> GetOutputDevices() {
			var devices = new string[ _driverRepository.Devices.Length ];

			for ( int i = 0; i < devices.Length; i++ ) {
				devices[ i ] = _driverRepository.Devices[ i ].Name;
			}

			return devices;
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
		public IEnumerable<string> GetAudioDrivers()
			=> _driverRepository.Drivers;

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
			var dspBufferSize = cvarSystem.GetCVar<uint>( Constants.CVars.Audio.FMOD.DSP_BUFFER_SIZE )
				?? throw new CVarMissing( Constants.CVars.Audio.FMOD.DSP_BUFFER_SIZE );
			var dspBufferCount = cvarSystem.GetCVar<int>( Constants.CVars.Audio.FMOD.DSP_BUFFER_COUNT )
				?? throw new CVarMissing( Constants.CVars.Audio.FMOD.DSP_BUFFER_COUNT );

			var flags = FMOD.INITFLAGS.CHANNEL_DISTANCEFILTER | FMOD.INITFLAGS.CHANNEL_LOWPASS | FMOD.INITFLAGS.VOL0_BECOMES_VIRTUAL;

			FMODValidator.ValidateCall( System.setStreamBufferSize( (uint)streamBufferSize.Value, FMOD.TIMEUNIT.MS ) );
			FMODValidator.ValidateCall( System.setDSPBufferSize( dspBufferSize.Value, dspBufferCount.Value ) );
			FMODValidator.ValidateCall( StudioSystem.initialize( maxChannels.Value, FMOD.Studio.INITFLAGS.LIVEUPDATE | FMOD.Studio.INITFLAGS.SYNCHRONOUS_UPDATE, flags, 0 ) );
		}
	};
};
