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

using System.Numerics;
using Nomad.Audio.Fmod.Private.Entities;
using Nomad.Audio.Fmod.Private.Services;
using Nomad.Audio.Fmod.Private.ValueObjects;
using Nomad.Audio.Fmod.ValueObjects;
using Nomad.Audio.Interfaces;
using Nomad.Core;
using Nomad.Core.Exceptions;
using Nomad.Core.Logger;
using Nomad.Core.CVars;
using Nomad.ResourceCache;
using System;
using System.Collections.Generic;
using Nomad.Core.Memory;
using Nomad.Core.Util;
using Nomad.CVars;
using System.Collections.Concurrent;

namespace Nomad.Audio.Fmod.Private.Repositories {
	/*
	===================================================================================

	FMODChannelRepository

	FIXME: this violates SOLID

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	internal sealed class FMODChannelRepository : IChannelRepository {
		private const int INVALID_CHANNEL = -1;

		// TODO: add an index hash for faster operations

		private readonly List<FMODChannel> _allocatedChannels;
		private readonly Queue<int> _freeChannelIds;
		private readonly Dictionary<string, float> _lastPlayTimes = new Dictionary<string, float>();
		private readonly Dictionary<string, int> _consecutiveStealCounts = new Dictionary<string, int>();

		private readonly Dictionary<int, FMODChannel> _channelsById = new Dictionary<int, FMODChannel>();
		private readonly Dictionary<IntPtr, FMODChannelHandle> _handlesByInstance = new Dictionary<IntPtr, FMODChannelHandle>();
		private readonly ConcurrentQueue<IntPtr> _finishedInstances = new ConcurrentQueue<IntPtr>();

		private int _nextGeneration = 1;
		private float _elapsedSeconds = 0.0f;

		private readonly ILoggerCategory _fmodChannelCategory;

		private bool _shouldDecay = false;
		private float _minTimeBetweenChannelSteals = 0.1f;

		private float _effectsVolume = 0.0f;

		private readonly IResourceCacheService<IAudioResource, string> _eventRepository;
		private readonly IListenerService _listenerService;
		private readonly FMODPriorityCalculator _priorityCalculator;
		private readonly FMODChannelStealCalculator _channelStealCalculator;

		public FMODBusRepository BusRepository => _busRepository;
		private readonly FMODBusRepository _busRepository;

		private readonly ICVar<int> _maxChannels;
		private readonly ICVar<int> _maxActiveChannels;

		private readonly BasicObjectPool<FMODChannel> _channelPool = new BasicObjectPool<FMODChannel>( BasicObjectPool<FMODChannel>.DefaultFactory );

		private bool _isDisposed = false;

		/*
		===============
		FMODChannelRepository
		===============
		*/
		/// <summary>
		/// Creates an FMODChannelRepository
		/// </summary>
		/// <param name="logger"></param>
		/// <param name="cvarSystem"></param>
		/// <param name="listenerService"></param>
		/// <param name="fmodSystem"></param>
		/// <exception cref="CVarMissing"></exception>
		public FMODChannelRepository( ILoggerService logger, ICVarSystemService cvarSystem, IListenerService listenerService, FMODDevice fmodSystem ) {
			_listenerService = listenerService;

			_priorityCalculator = new FMODPriorityCalculator( cvarSystem, listenerService );
			_channelStealCalculator = new FMODChannelStealCalculator( cvarSystem, _priorityCalculator );

			_maxChannels = cvarSystem.GetCVarOrThrow<int>( Constants.CVars.EngineUtils.Audio.MAX_CHANNELS ) ?? throw new CVarMissing( Constants.CVars.EngineUtils.Audio.MAX_CHANNELS );

			_maxActiveChannels = cvarSystem.GetCVarOrThrow<int>( Constants.CVars.EngineUtils.Audio.MAX_ACTIVE_CHANNELS ) ?? throw new CVarMissing( Constants.CVars.EngineUtils.Audio.MAX_ACTIVE_CHANNELS );
			_maxChannels.Value = _maxActiveChannels.Value;
			_maxActiveChannels.ValueChanged.Subscribe( OnMaxActiveChannelsValueChanged );
			ValidateMaxChannels();
			InitConfig( cvarSystem );

			_allocatedChannels = new List<FMODChannel>( _maxChannels.Value );
			_freeChannelIds = new Queue<int>( _maxChannels.Value );

			for ( int i = 0; i < _maxChannels.Value; i++ ) {
				_freeChannelIds.Enqueue( i );
			}
			_eventRepository = fmodSystem.EventRepository;

			_busRepository = new FMODBusRepository( fmodSystem );
			_fmodChannelCategory = logger.CreateCategory( "FMODChannelAllocator", LogLevel.Debug, true );
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
			if ( !_isDisposed ) {
				_channelPool?.Dispose();
				_fmodChannelCategory?.Dispose();
			}
			GC.SuppressFinalize( this );
			_isDisposed = true;
		}

		/*
		===============
		AllocateChannel
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <param name="position"></param>
		/// <param name="config"></param>
		/// <param name="basePriority"></param>
		/// <param name="isEssential"></param>
		/// <returns></returns>
		public FMODChannelHandle? AllocateChannel( string id, Vector2 position, SoundCategory config, float basePriority = 0.5f, bool isEssential = false ) {
			int soundsInCategory = CountSoundsInCategory( config.Config.Name );
			if ( soundsInCategory >= config.Config.MaxSimultaneous ) {
				if ( !config.Config.AllowStealingFromSameCategory ) {
					// not allowed to steal from ourselves, most unfortunate :(
					return null;
				}
			}

			float startTime = _elapsedSeconds;

			var instance = CreateSoundInstance( id, position );
			float actualPriority = CalculateActualPriority( startTime, id, position, basePriority, config );

			int channelId = AllocateChannelId( startTime, instance, position, actualPriority, config, isEssential );
			if ( channelId == INVALID_CHANNEL ) {
				_fmodChannelCategory.PrintError( $"Couldn't allocate a channel for sound event '{id}'!" );
				return null;
			}

			instance.CreateInstance( out var eventInstance );
			eventInstance.Volume = _effectsVolume / 10.0f;
			eventInstance.Position = position;

			FMODValidator.ValidateCall( eventInstance.SetFinishedCallback( SoundFinishedCallback ) );
			FMODValidator.ValidateCall( eventInstance.Start() );

			var channel = _channelPool.Rent();
			channel.Instance = eventInstance;
			channel.Category = config;
			channel.BasePriority = basePriority;
			channel.CurrentPriority = actualPriority;
			channel.StartTimeSeconds = startTime;
			channel.IsEssential = isEssential;
			channel.ChannelId = channelId;
			channel.Generation = _nextGeneration++;
			channel.EventId = id;

			var handle = new FMODChannelHandle( channelId, channel.Generation );
			channel.Handle = handle;

			_allocatedChannels.Add( channel );
			_channelsById[ channelId ] = channel;
			_handlesByInstance[ eventInstance ] = handle;

			UpdateLastPlayTime( startTime, id );

			return handle;
		}

		/*
		===============
		UpdateLastPlayTime
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="startTime"></param>
		/// <param name="id"></param>
		public void UpdateLastPlayTime( float startTime, string id ) {
			_lastPlayTimes[id] = startTime;
		}

		/*
		===============
		Update
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="deltaTime"></param>
		public void Update( float deltaTime ) {
			_elapsedSeconds += deltaTime;

			ProcessFinishedSounds();

			if ( _shouldDecay ) {
				DecayStealCounts();
				_shouldDecay = false;
			}
			UpdatePriorities();
			float startTime = DateTime.Now.Millisecond / 1000.0f;
			CleanupFinishedSounds( startTime );
			EnforceCategoryLimits( startTime );
		}

		private void ProcessFinishedSounds() {
			while ( _finishedInstances.TryDequeue( out var instanceHandle ) ) {
				if ( !_handlesByInstance.TryGetValue( instanceHandle, out var handle ) ) {
					continue;
				}
				if ( !TryGetChannel( handle, out var channel ) ) {
					continue;
				}
				StopSound( _elapsedSeconds, channel, false );
			}
		}

		/*
		===============
		UpdatePriorities
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		private void UpdatePriorities() {
			Vector2 listenerPos = _listenerService.ActiveListener;

			for ( int i = 0; i < _allocatedChannels.Count; i++ ) {
				var channel = _allocatedChannels[i];

				if ( !channel.IsPlaying ) {
					continue;
				}

				float distanceFactor = _priorityCalculator.CalculateDistanceFactor( channel.Instance.Position.DistanceTo( listenerPos ) );
				channel.Volume = distanceFactor;
				channel.CurrentPriority = channel.BasePriority * channel.Category.Config.PriorityScale * distanceFactor;
			}
		}

		/*
		===============
		CleanupFinishedSounds
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="startTime"></param>
		private void CleanupFinishedSounds( float startTime ) {
			for ( int i = _allocatedChannels.Count - 1; i >= 0; i-- ) {
				if ( !_allocatedChannels[i].IsPlaying ) {
					StopSound( startTime, _allocatedChannels[i], false );
				}
			}
		}

		/*
		===============
		EnforceCategoryLimits
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="startTime"></param>
		private void EnforceCategoryLimits( float startTime ) {
			foreach ( var category in _busRepository.Categories ) {
				int count = CountSoundsInCategory( category.Key );
				if ( count > category.Value.Config.MaxSimultaneous ) {
					// find the lowest priority sound in this category to steal
					StealFromCategory( startTime, category.Key );
				}
			}
		}

		/*
		===============
		StealFromCategory
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="startTime"></param>
		/// <param name="category"></param>
		private void StealFromCategory( float startTime, string category ) {
			FMODChannel? lowestPriority = null;
			float lowestPriorityValue = float.MaxValue;

			for ( int i = 0; i < _allocatedChannels.Count; i++ ) {
				var channel = _allocatedChannels[i];
				if ( channel.Category.Config.Name == category && channel.IsPlaying && !channel.IsEssential ) {
					if ( channel.CurrentPriority < lowestPriorityValue ) {
						lowestPriorityValue = channel.CurrentPriority;
						lowestPriority = channel;
					}
				}
			}
			if ( lowestPriority != null ) {
				StopSound( startTime, lowestPriority, true );
			}
		}

		/*
		===============
		CalculateActualPriority
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="startTime"></param>
		/// <param name="id"></param>
		/// <param name="position"></param>
		/// <param name="basePriority"></param>
		/// <param name="category"></param>
		/// <returns></returns>
		private float CalculateActualPriority( float startTime, string id, Vector2 position, float basePriority, SoundCategory category ) {
			return _priorityCalculator.CalculateActualPriority(
				startTime,
				id,
				position,
				basePriority,
				category,
				_lastPlayTimes,
				_consecutiveStealCounts
			);
		}

		/*
		===============
		AllocateChannel
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="startTime"></param>
		/// <param name="id"></param>
		/// <param name="position"></param>
		/// <param name="priority"></param>
		/// <param name="category"></param>
		/// <param name="isEssential"></param>
		/// <returns></returns>
		private int AllocateChannelId( float startTime, IntPtr id, Vector2 position, float priority, SoundCategory category, bool isEssential ) {
			if ( _freeChannelIds.TryDequeue( out int channelId ) ) {
				return channelId;
			}
			return StealChannel( startTime, id, position, priority, category, isEssential );
		}

		/*
		===============
		TryGetChannel
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="handle"></param>
		/// <param name="channel"></param>
		/// <returns></returns>
		public bool TryGetChannel( FMODChannelHandle? handle, out FMODChannel channel ) {
			channel = null;

			if ( handle == null || !handle.IsValid ) {
				return false;
			}
			if ( !_channelsById.TryGetValue( handle.ChannelId, out var existing ) ) {
				return false;
			}
			if ( existing.Generation != handle.Generation ) {
				return false;
			}

			if ( !existing.Instance.IsValid ) {
				return false;
			}

			channel = existing;
			return true;
		}

		public bool IsAlive( FMODChannelHandle handle ) {
			return TryGetChannel( handle, out _ );
		}

		/*
		===============
		StealChannel
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="currentTime"></param>
		/// <param name="id"></param>
		/// <param name="position"></param>
		/// <param name="priority"></param>
		/// <param name="category"></param>
		/// <param name="isEssential"></param>
		/// <returns></returns>
		private int StealChannel( float currentTime, IntPtr id, Vector2 position, float priority, SoundCategory category, bool isEssential ) {
			float bestStealScore = float.MinValue;
			FMODChannel? bestCandidate = null;

			for ( int i = 0; i < _allocatedChannels.Count; i++ ) {
				var sound = _allocatedChannels[i];
				if ( !sound.IsPlaying || sound.IsEssential ) {
					continue;
				}

				// check steal protection (can't steal brand new sounds)
				if ( sound.AgeSeconds( currentTime ) < sound.Category.Config.StealProtectionTime ) {
					continue;
				}

				// don't steal if recently stolen (prevents thrashing)
				if ( currentTime - sound.LastStolenTime < _minTimeBetweenChannelSteals ) {
					continue;
				}

				// calc steal score
				float stealScore = _channelStealCalculator.CalculateStealScore( currentTime, sound, priority, category, _listenerService );
				if ( stealScore > bestStealScore ) {
					bestStealScore = stealScore;
					bestCandidate = sound;
				}
			}
			if ( bestCandidate != null && bestStealScore > 0.0f ) {
				int stolenChannelId = bestCandidate.ChannelId;
				StopSound( currentTime, bestCandidate, true );

				UpdateStealStatistics( bestCandidate.EventId );

				return stolenChannelId;
			}
			return -1;
		}

		/*
		===============
		UpdateStealStatistics
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="stolenEventId"></param>
		private void UpdateStealStatistics( string stolenEventId ) {
			if ( !_consecutiveStealCounts.TryGetValue( stolenEventId, out int count ) ) {
				count = 0;
			} else {
				count++;
			}
			_consecutiveStealCounts[stolenEventId] = count;
			_shouldDecay = true;
		}

		/*
		===============
		DecayStealCounts
		===============
		*/
		/// <summary>
		///
		/// </summary>
		private void DecayStealCounts() {
			foreach ( var key in _consecutiveStealCounts.Keys ) {
				int stealCount = Math.Min( 0, _consecutiveStealCounts[key] - 1 );
				if ( stealCount == 0 ) {
					_consecutiveStealCounts.Remove( key );
				} else {
					_consecutiveStealCounts[key] = stealCount;
				}
			}
		}

		/*
		===============
		StopSound
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="startTime"></param>
		/// <param name="channel"></param>
		/// <param name="wasStolen"></param>
		private void StopSound( float startTime, FMODChannel channel, bool wasStolen = false ) {
			var handle = channel.Handle;
			var instanceHandle = (IntPtr)channel.Instance;

			if ( wasStolen ) {
				channel.LastStolenTime = startTime;
			}
			channel.Instance.Unload();

			_handlesByInstance.Remove( instanceHandle );
			_channelsById.Remove( channel.ChannelId );

			// FIXME: this is slow
			_allocatedChannels.Remove( channel );
			_freeChannelIds.Enqueue( channel.ChannelId );

			channel.Handle = null;
			channel.Reset();
			_channelPool.Return( channel );
			
			handle?.Invalidate();
		}

		/*
		===============
		CreateSoundInstance
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="id"></param>
		/// <param name="position"></param>
		/// <returns></returns>
		private FMODEventResource CreateSoundInstance( string id, Vector2 position ) {
			var cached = _eventRepository.GetCached( id ) ?? throw new Exception( $"Couldn't find event description for '{id}'" );
			cached.Get( out var description );

			if ( description is not FMODEventResource eventResource ) {
				throw new InvalidCastException();
			}

			return eventResource;
		}

		/*
		===============
		SoundFinishedCallback
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="type"></param>
		/// <param name="instance"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		private FMOD.RESULT SoundFinishedCallback( FMOD.Studio.EVENT_CALLBACK_TYPE type, nint instance, IntPtr parameters ) {
			FMOD.Studio.EventInstance eventInstance = new FMOD.Studio.EventInstance( instance );
			FMODChannel? channel = null;

			for ( int i = 0; i < _allocatedChannels.Count; i++ ) {
				var sound = _allocatedChannels[i];
				if ( sound.Instance == eventInstance ) {
					channel = sound;
					break;
				}
			}
			if ( channel != null ) {
				// ensure we unhook the callback (causes a seggy)
				StopSound( DateTime.Now.Millisecond / 1000.0f, channel, false );
			}
			_finishedInstances.Enqueue( instance );
			return FMOD.RESULT.OK;
		}

		/*
		===============
		CountSoundsInCategory
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="category"></param>
		/// <returns></returns>
		private int CountSoundsInCategory( string category ) {
			int count = 0;
			for ( int i = 0; i < _allocatedChannels.Count; i++ ) {
				var channel = _allocatedChannels[i];
				if ( channel.Category.Config.Name == category && channel.IsPlaying ) {
					count++;
				}
			}
			return count;
		}

		/*
		===============
		InitConfig
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="cvarSystem"></param>
		/// <exception cref="CVarMissing"></exception>
		private void InitConfig( ICVarSystemService cvarSystem ) {
			var effectsVolume = cvarSystem.GetCVarOrThrow<float>( Constants.CVars.EngineUtils.Audio.EFFECTS_VOLUME );
			_effectsVolume = effectsVolume.Value;
			effectsVolume.ValueChanged.Subscribe( OnEffectsVolumeChanged );

			var minTimeBetweenChannelSteals = cvarSystem.GetCVarOrThrow<float>( Constants.CVars.EngineUtils.Audio.MIN_TIME_BETWEEN_CHANNEL_STEALS );
			_minTimeBetweenChannelSteals = minTimeBetweenChannelSteals.Value;
			minTimeBetweenChannelSteals.ValueChanged.Subscribe( OnMinTimeBetweenChannelStealsValueChanged );
		}

		/*
		===============
		UpdateChannelVolumes
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		private static void UpdateChannelVolumes() {
		}

		/*
		===============
		ValidateMaxChannels
		===============
		*/
		/// <summary>
		///
		/// </summary>
		private void ValidateMaxChannels() {
			if ( _maxActiveChannels.Value > _maxChannels.Value ) {
				_fmodChannelCategory.PrintError( $"ValidateMaxChannels: maxActiveChannels cannot be larger than maxChannels, resetting both." );
				_maxActiveChannels.Reset();
				_maxChannels.Reset();
			}
		}

		/*
		===============
		OnMaxActiveChannelsValueChanged
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="args"></param>
		private void OnMaxActiveChannelsValueChanged( in CVarValueChangedEventArgs<int> args ) {
			_maxChannels.Value = args.NewValue;
			ValidateMaxChannels();
		}

		/*
		===============
		OnEffectsVolumeChanged
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="args"></param>
		private void OnEffectsVolumeChanged( in CVarValueChangedEventArgs<float> args ) {
			_effectsVolume = args.NewValue;
			if ( args.OldValue != args.NewValue ) {
				UpdateChannelVolumes();
			}
		}

		/*
		===============
		OnMinTimeBetweenChannelStealsValueChanged
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="args"></param>
		private void OnMinTimeBetweenChannelStealsValueChanged( in CVarValueChangedEventArgs<float> args ) {
			_minTimeBetweenChannelSteals = args.NewValue;
		}
	};
};
