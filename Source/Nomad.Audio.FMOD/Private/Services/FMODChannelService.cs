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
using System.Numerics;
using Nomad.Audio.Fmod.Private.Entities;
using Nomad.Audio.Fmod.Private.Repositories;
using Nomad.Audio.Fmod.Private.ValueObjects;
using Nomad.Audio.Fmod.ValueObjects;
using Nomad.Audio.Interfaces;
using Nomad.Core;
using Nomad.Core.CVars;
using Nomad.Core.Exceptions;
using Nomad.Core.Logger;
using Nomad.Core.Memory;
using Nomad.CVars;
using Nomad.ResourceCache;

namespace Nomad.Audio.Fmod.Private.Services {
	/*
	===================================================================================
	
	FMODChannelService
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	internal sealed class FMODChannelService : IChannelRepository {
		private const int INVALID_CHANNEL = -1;

		private readonly IResourceCacheService<IAudioResource, string> _eventRepository;
		private readonly IListenerService _listenerService;

		private readonly FMODPriorityCalculator _priorityCalculator;
		private readonly FMODChannelStealCalculator _channelStealCalculator;
		private readonly FMODChannelTracker _tracker;
		private readonly FMODChannelStealService _channelStealService;
		private readonly FMODBusRepository _busRepository;

		private readonly BasicObjectPool<FMODChannel> _channelPool =
			new BasicObjectPool<FMODChannel>( BasicObjectPool<FMODChannel>.DefaultFactory );

		private readonly ILoggerCategory _category;
		private readonly FMOD.Studio.EVENT_CALLBACK _soundFinishedCallback;

		private readonly ICVar<int> _maxChannels;
		private readonly ICVar<int> _maxActiveChannels;

		private readonly int _capacity;
		private int _nextGeneration = 1;

		private float _elapsedSeconds;
		private float _effectsVolume;
		private float _minTimeBetweenChannelSteals;

		private bool _isDisposed;

		public FMODBusRepository BusRepository => _busRepository;

		public FMODChannelService(
			ILoggerService logger,
			ICVarSystemService cvarSystem,
			IListenerService listenerService,
			FMODDevice fmodSystem ) {

			_listenerService = listenerService;

			_priorityCalculator = new FMODPriorityCalculator( cvarSystem, listenerService );
			_channelStealCalculator = new FMODChannelStealCalculator( cvarSystem, _priorityCalculator );

			_maxChannels = cvarSystem.GetCVarOrThrow<int>( Constants.CVars.EngineUtils.Audio.MAX_CHANNELS );
			_maxActiveChannels = cvarSystem.GetCVarOrThrow<int>( Constants.CVars.EngineUtils.Audio.MAX_ACTIVE_CHANNELS );

			ValidateInitialChannelConfig();

			_capacity = _maxChannels.Value;
			_tracker = new FMODChannelTracker( _capacity );
			_channelStealService = new FMODChannelStealService(
				listenerService,
				_priorityCalculator,
				_channelStealCalculator,
				_capacity );

			_eventRepository = fmodSystem.EventRepository;
			_busRepository = new FMODBusRepository( fmodSystem );

			_category = logger.CreateCategory( nameof( FMODChannelService ), LogLevel.Debug, true );
			_soundFinishedCallback = SoundFinishedCallback;

			_maxActiveChannels.ValueChanged.Subscribe( OnMaxActiveChannelsValueChanged );
			InitConfig( cvarSystem );
		}

		public void Dispose() {
			if ( _isDisposed ) {
				return;
			}

			for ( int i = _tracker.ActiveCount - 1; i >= 0; i-- ) {
				FMODChannel channel = _tracker.GetActiveChannel( i );
				StopSound( _elapsedSeconds, channel, false );
			}

			_channelPool.Dispose();
			_category.Dispose();

			_isDisposed = true;
			GC.SuppressFinalize( this );
		}

		public FMODChannelHandle? AllocateChannel(
			string id,
			Vector2 position,
			SoundCategory config,
			float basePriority = 0.5f,
			bool isEssential = false ) {

			// Keep counts/handles clean before making admission decisions.
			ProcessFinishedSounds();

			int soundsInCategory = _tracker.CountSoundsInCategory( config.Config.Name );
			if ( soundsInCategory >= config.Config.MaxSimultaneous &&
			     !config.Config.AllowStealingFromSameCategory ) {
				return null;
			}

			float startTime = _elapsedSeconds;
			FMODEventResource resource = CreateSoundInstance( id );

			float actualPriority = _channelStealService.CalculateActualPriority(
				startTime,
				id,
				position,
				basePriority,
				config );

			int channelId = AllocateChannelId(
				startTime,
				actualPriority,
				config,
				isEssential );

			if ( channelId == INVALID_CHANNEL ) {
				_category.PrintError( $"Couldn't allocate a channel for sound event '{id}'." );
				return null;
			}

			resource.CreateInstance( out var eventInstance );
			eventInstance.Position = position;
			eventInstance.Volume = _effectsVolume * 0.1f;

			FMODValidator.ValidateCall( eventInstance.SetFinishedCallback( _soundFinishedCallback ) );
			FMODValidator.ValidateCall( eventInstance.Start() );

			FMODChannel channel = _channelPool.Rent();
			channel.Instance = eventInstance;
			channel.Category = config;
			channel.BasePriority = basePriority;
			channel.CurrentPriority = actualPriority;
			channel.StartTimeSeconds = startTime;
			channel.IsEssential = isEssential;
			channel.ChannelId = channelId;
			channel.Generation = _nextGeneration++;
			channel.EventId = id;
			channel.Volume = 1.0f;

			var handle = new FMODChannelHandle( channelId, channel.Generation );
			channel.Handle = handle;

			_tracker.Register( channel, handle );
			_channelStealService.UpdateLastPlayTime( startTime, id );

			return handle;
		}

		public void Update( float deltaTime ) {
			_elapsedSeconds += deltaTime;

			ProcessFinishedSounds();
			_channelStealService.DecayStealCountsIfNeeded();

			UpdateChannelPrioritiesAndVolumes();
			CleanupFinishedSounds( _elapsedSeconds );
			EnforceCategoryLimits( _elapsedSeconds );
		}

		public bool TryGetChannel( FMODChannelHandle? handle, out FMODChannel channel ) {
			return _tracker.TryGetChannel( handle, out channel );
		}

		public bool IsAlive( FMODChannelHandle handle ) {
			return _tracker.TryGetChannel( handle, out _ );
		}

		private int AllocateChannelId(
			float currentTime,
			float incomingPriority,
			SoundCategory incomingCategory,
			bool isEssential ) {

			if ( !isEssential && _tracker.ActiveCount >= _maxActiveChannels.Value ) {
				return TryStealChannel( currentTime, incomingPriority, incomingCategory );
			}

			if ( _tracker.TryAllocateChannelId( out int freeChannelId ) ) {
				return freeChannelId;
			}

			return TryStealChannel( currentTime, incomingPriority, incomingCategory );
		}

		private int TryStealChannel( float currentTime, float incomingPriority, SoundCategory incomingCategory ) {
			if ( _channelStealService.TryFindStealCandidate(
				currentTime,
				incomingPriority,
				incomingCategory,
				_tracker,
				out FMODChannel? candidate
			) && candidate != null ) {

				int stolenChannelId = candidate.ChannelId;
				string stolenEventId = candidate.EventId;

				StopSound( currentTime, candidate, true );
				_channelStealService.UpdateStealStatistics( stolenEventId );

				return stolenChannelId;
			}

			return INVALID_CHANNEL;
		}

		private void ProcessFinishedSounds() {
			while ( _tracker.TryDequeueFinishedInstance( out nint instanceHandle ) ) {
				if ( _tracker.TryGetChannelByInstance( instanceHandle, out FMODChannel channel ) ) {
					StopSound( _elapsedSeconds, channel, false );
				}
			}
		}

		private void UpdateChannelPrioritiesAndVolumes() {
			Vector2 listenerPosition = _listenerService.ActiveListener;
			float globalVolume = _effectsVolume * 0.1f;

			for ( int i = 0; i < _tracker.ActiveCount; i++ ) {
				FMODChannel channel = _tracker.GetActiveChannel( i );

				if ( !channel.IsPlaying ) {
					continue;
				}

				float distanceFactor = _priorityCalculator.CalculateDistanceFactor(
					Vector2.Distance( channel.Instance.Position, listenerPosition ) );

				channel.Volume = distanceFactor;
				channel.CurrentPriority =
					channel.BasePriority *
					channel.Category.Config.PriorityScale *
					distanceFactor;

				channel.Instance.Volume = globalVolume * distanceFactor;
			}
		}

		private void CleanupFinishedSounds( float currentTime ) {
			for ( int i = _tracker.ActiveCount - 1; i >= 0; i-- ) {
				FMODChannel channel = _tracker.GetActiveChannel( i );
				if ( !channel.IsPlaying ) {
					StopSound( currentTime, channel, false );
				}
			}
		}

		private void EnforceCategoryLimits( float currentTime ) {
			foreach ( var category in _busRepository.Categories ) {
				string categoryName = category.Key;
				int maxSimultaneous = category.Value.Config.MaxSimultaneous;

				while ( _tracker.CountSoundsInCategory( categoryName ) > maxSimultaneous ) {
					if ( !_tracker.TryFindLowestPriorityInCategory( categoryName, out FMODChannel? victim ) ||
					     victim == null ) {
						break;
					}

					StopSound( currentTime, victim, true );
				}
			}
		}

		private void StopSound( float currentTime, FMODChannel channel, bool wasStolen ) {
			FMODChannelHandle? handle = channel.Handle;

			if ( wasStolen ) {
				channel.LastStolenTime = currentTime;
			}

			if ( channel.Instance.IsValid ) {
				channel.Instance.Unload();
			}

			_tracker.Unregister( channel );

			channel.Handle = null;
			channel.Reset();
			_channelPool.Return( channel );

			handle?.Invalidate();
		}

		private FMODEventResource CreateSoundInstance( string id ) {
			var cached = _eventRepository.GetCached( id )
				?? throw new Exception( $"Couldn't find event description for '{id}'." );

			cached.Get( out var description );

			if ( description is not FMODEventResource eventResource ) {
				throw new InvalidCastException(
					$"Cached audio resource '{id}' is not an {nameof( FMODEventResource )}." );
			}

			return eventResource;
		}

		private FMOD.RESULT SoundFinishedCallback(
			FMOD.Studio.EVENT_CALLBACK_TYPE type,
			nint instance,
			IntPtr parameters ) {

			// Never mutate channel state on the FMOD callback thread.
			_tracker.EnqueueFinishedInstance( instance );
			return FMOD.RESULT.OK;
		}

		private void InitConfig( ICVarSystemService cvarSystem ) {
			var effectsVolume = cvarSystem.GetCVarOrThrow<float>(
				Constants.CVars.EngineUtils.Audio.EFFECTS_VOLUME );
			_effectsVolume = effectsVolume.Value;
			effectsVolume.ValueChanged.Subscribe( OnEffectsVolumeChanged );

			var minTimeBetweenChannelSteals = cvarSystem.GetCVarOrThrow<float>(
				Constants.CVars.EngineUtils.Audio.MIN_TIME_BETWEEN_CHANNEL_STEALS );
			_minTimeBetweenChannelSteals = minTimeBetweenChannelSteals.Value;
			minTimeBetweenChannelSteals.ValueChanged.Subscribe( OnMinTimeBetweenChannelStealsValueChanged );

			_channelStealService.SetMinTimeBetweenChannelSteals( _minTimeBetweenChannelSteals );
		}

		private void ValidateInitialChannelConfig() {
			if ( _maxChannels.Value <= 0 ) {
				throw new InvalidOperationException( "MAX_CHANNELS must be > 0." );
			}

			if ( _maxActiveChannels.Value <= 0 ) {
				throw new InvalidOperationException( "MAX_ACTIVE_CHANNELS must be > 0." );
			}

			if ( _maxActiveChannels.Value > _maxChannels.Value ) {
				throw new InvalidOperationException(
					"MAX_ACTIVE_CHANNELS cannot be larger than MAX_CHANNELS for FMODChannelService." );
			}
		}

		private void OnMaxActiveChannelsValueChanged( in CVarValueChangedEventArgs<int> args ) {
			if ( args.NewValue <= 0 ) {
				_category.PrintError(
					$"OnMaxActiveChannelsValueChanged: attempted to set MAX_ACTIVE_CHANNELS to {args.NewValue}. Reverting." );
				_maxActiveChannels.Value = Math.Max( 1, args.OldValue );
				return;
			}

			if ( args.NewValue > _capacity ) {
				_category.PrintError(
					$"OnMaxActiveChannelsValueChanged: attempted to set MAX_ACTIVE_CHANNELS to {args.NewValue}, but service capacity is fixed at {_capacity}. Clamping." );
				_maxActiveChannels.Value = _capacity;
			}
		}

		private void OnEffectsVolumeChanged( in CVarValueChangedEventArgs<float> args ) {
			_effectsVolume = args.NewValue;
		}

		private void OnMinTimeBetweenChannelStealsValueChanged( in CVarValueChangedEventArgs<float> args ) {
			_minTimeBetweenChannelSteals = args.NewValue;
			_channelStealService.SetMinTimeBetweenChannelSteals( args.NewValue );
		}
	}
}