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

using Godot;
using Nomad.Audio.Fmod.Private.Entities;
using Nomad.Audio.Fmod.Private.Services;
using Nomad.Audio.Fmod.Private.ValueObjects;
using Nomad.Audio.Fmod.ValueObjects;
using Nomad.Audio.Interfaces;
using Nomad.Core;
using Nomad.Core.Exceptions;
using Nomad.Core.Logger;
using Nomad.Core.Util;
using Nomad.CVars;
using Nomad.ResourceCache;
using NomadCore.Utilities;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

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
		private readonly Dictionary<IntPtr, float> _lastPlayTimes = new Dictionary<IntPtr, float>();
		private readonly Dictionary<IntPtr, int> _consecutiveStealCounts = new Dictionary<IntPtr, int>();

		private readonly ILoggerCategory _fmodChannelCategory;

		private bool _shouldDecay = false;
		private float _timePenaltyMultiplier = 0.5f;
		private float _distanceWeight = 0.3f;
		private float _volumeWeight = 0.2f;
		private float _frequencyPenalty = 0.4f;
		private float _minTimeBetweenChannelSteals = 0.1f;

		private float _distanceFalloffStart = 50.0f;
		private float _distanceFalloffEnd = 100.0f;
		private float _effectsVolume = 0.0f;

		private readonly IResourceCacheService<IAudioResource, string> _eventRepository;
		private readonly ILoggerService _logger;
		private readonly IListenerService _listenerService;

		public FMODBusRepository BusRepository => _busRepository;
		private readonly FMODBusRepository _busRepository;

		private readonly ICVar<int> _maxChannels;
		private readonly ICVar<int> _maxActiveChannels;

		private readonly ObjectPool<FMODChannel> _channelPool = new ObjectPool<FMODChannel>();

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
		public FMODChannelRepository( ILoggerService logger, ICVarSystemService cvarSystem, IListenerService listenerService,
			FMODDevice fmodSystem ) {
			_logger = logger;
			_listenerService = listenerService;

			_maxChannels = cvarSystem.GetCVar<int>( Constants.CVars.Audio.MAX_CHANNELS ) ?? throw new CVarMissing( Constants.CVars.Audio.MAX_CHANNELS );

			_maxActiveChannels = cvarSystem.GetCVar<int>( Constants.CVars.Audio.MAX_ACTIVE_CHANNELS ) ?? throw new CVarMissing( Constants.CVars.Audio.MAX_ACTIVE_CHANNELS );
			_maxChannels.Value = _maxActiveChannels.Value;
			_maxActiveChannels.ValueChanged.Subscribe( this, OnMaxActiveChannelsValueChanged );
			ValidateMaxChannels();
			InitConfig( cvarSystem );

			_allocatedChannels = new List<FMODChannel>( _maxChannels.Value );
			_freeChannelIds = new Queue<int>( _maxChannels.Value );

			for ( int i = 0; i < _maxChannels.Value; i++ ) {
				_freeChannelIds.Enqueue( i );
			}
			_eventRepository = fmodSystem.EventRepository;

			_busRepository = new FMODBusRepository( fmodSystem );

			_fmodChannelCategory = _logger.CreateCategory( "FMODChannelAllocator", LogLevel.Debug, true );
		}

		/*
		===============
		Dispose
		===============
		*/
		public void Dispose() {
			_freeChannelIds.Clear();
			_lastPlayTimes.Clear();
			_consecutiveStealCounts.Clear();
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
		/// <param name="category"></param>
		/// <param name="basePriority"></param>
		/// <param name="isEssential"></param>
		/// <returns></returns>
		public FMODChannel? AllocateChannel( string id, Vector2 position,
			SoundCategory config, float basePriority = 0.5f,
			bool isEssential = false ) {
			int soundsInCategory = CountSoundsInCategory( config.Config.Name );
			if ( soundsInCategory >= config.Config.MaxSimultaneous ) {
				if ( !config.Config.AllowStealingFromSameCategory ) {
					// not allowed to steal from ourselves, most unfortunate :(
					return null;
				}
			}

			float startTime = Time.GetTicksMsec() / 1000.0f;
			var instance = CreateSoundInstance( id, position );

			float actualPriority = CalculateActualPriority( startTime, instance.Handle.handle, position, basePriority, config );
			int channelId = AllocateChannelId( startTime, instance.Handle.handle, position, actualPriority, config, isEssential );

			if ( channelId == INVALID_CHANNEL ) {
				_logger.PrintError( in _fmodChannelCategory, $"Couldn't allocate a channel for sound event '{id}'!" );
				return null;
			}

			instance.CreateInstance( out var eventInstance );
			eventInstance.Volume = _effectsVolume / 10.0f;
			FMODValidator.ValidateCall( eventInstance.instance.setCallback( SoundFinishedCallback, FMOD.Studio.EVENT_CALLBACK_TYPE.STOPPED | FMOD.Studio.EVENT_CALLBACK_TYPE.START_FAILED ) );
			FMODValidator.ValidateCall( eventInstance.instance.start() );

			_logger.PrintDebug( in _fmodChannelCategory, $"AllocateChannel: allocating new channel with id {channelId}..." );

			var channel = _channelPool.Rent();
			channel.Instance = eventInstance;
			channel.Category = config;
			channel.BasePriority = basePriority;
			channel.CurrentPriority = actualPriority;
			channel.StartTime = startTime;
			channel.IsEssential = isEssential;
			channel.ChannelId = channelId;

			_allocatedChannels.Add( channel );
			UpdateLastPlayTime( startTime, eventInstance.instance.handle );

			_logger.PrintDebug( in _fmodChannelCategory, $"AllocateChannel: freeChannelIds.Count - {_freeChannelIds.Count}" );

			return channel;
		}

		/*
		===============
		UpdateLastPlayTime
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="id"></param>
		public void UpdateLastPlayTime( float startTime, IntPtr id ) {
			_lastPlayTimes[ id ] = startTime;
		}

		/*
		===============
		Update
		===============
		*/
		public void Update( float deltaTime ) {
			if ( _shouldDecay ) {
				DecayStealCounts();
				_shouldDecay = false;
			}
			UpdatePriorities();

			float startTime = Time.GetTicksMsec() / 1000.0f;
			CleanupFinishedSounds( startTime );
			EnforceCategoryLimits( startTime );
		}

		/*
		===============
		UpdatePriorities
		===============
		*/
		private void UpdatePriorities() {
			Vector2 listenerPos = _listenerService.ActiveListener;

			for ( int i = 0; i < _allocatedChannels.Count; i++ ) {
				var channel = _allocatedChannels[ i ];

				if ( !channel.IsPlaying ) {
					continue;
				}

				float distanceFactor = CalculateDistanceFactor( channel.Instance.Position.DistanceTo( listenerPos ) );
				channel.Volume = distanceFactor;
				channel.CurrentPriority = channel.BasePriority * channel.Category.Config.PriorityScale * distanceFactor;
			}
		}

		/*
		===============
		CleanupFinishedSounds
		===============
		*/
		private void CleanupFinishedSounds( float startTime ) {
			for ( int i = _allocatedChannels.Count - 1; i >= 0; i-- ) {
				if ( !_allocatedChannels[ i ].IsPlaying ) {
					StopSound( startTime, _allocatedChannels[ i ], false );
				}
			}
		}

		/*
		===============
		EnforceCategoryLimits
		===============
		*/
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
		private void StealFromCategory( float startTime, string category ) {
			FMODChannel? lowestPriority = null;
			float lowestPriorityValue = float.MaxValue;

			for ( int i = 0; i < _allocatedChannels.Count; i++ ) {
				var channel = _allocatedChannels[ i ];
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
		/// <param name="id"></param>
		/// <param name="position"></param>
		/// <param name="basePriority"></param>
		/// <param name="category"></param>
		/// <returns></returns>
		private float CalculateActualPriority( float startTime, IntPtr id, Vector2 position, float basePriority, SoundCategory category ) {
			Vector2 listenerPos = _listenerService.ActiveListener;

			float priority =
				basePriority *
				category.Config.PriorityScale * // category multiplier
				CalculateDistanceFactor( position.DistanceTo( listenerPos ) );

			priority *= 1.0f - CalculateTimePenalty( startTime, id ) * _timePenaltyMultiplier;
			priority *= 1.0f - CalculateFrequencyPenalty( id ) * _frequencyPenalty;

			return Math.Clamp( priority, 0.01f, 1.0f );
		}


		/*
		===============
		CalculateDistanceFactor
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="distance"></param>
		/// <returns></returns>
		private float CalculateDistanceFactor( float distance ) {
			if ( distance <= _distanceFalloffStart ) {
				return 1.0f;
			}
			if ( distance >= _distanceFalloffEnd ) {
				return 0.1f;
			}

			// TODO: precompute

			// LERP it
			float t = (distance - _distanceFalloffStart) / (_distanceFalloffEnd / _distanceFalloffStart);
			return 1.0f - t * 0.5f;
		}

		/*
		===============
		CalculateTimePenalty
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		private float CalculateTimePenalty( float startTime, IntPtr id ) {
			if ( !_lastPlayTimes.TryGetValue( id, out float lastTime ) ) {
				return 0.0f;
			}
			float timeSinceLast = startTime - lastTime;
			float protectionTime = 0.5f;

			// less penality if enough time has passed
			if ( timeSinceLast > protectionTime ) {
				return 0.0f;
			}
			return 1.0f - (timeSinceLast / protectionTime);
		}

		/*
		===============
		CalculateFrequencyPenalty
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private float CalculateFrequencyPenalty( IntPtr id ) {
			if ( !_consecutiveStealCounts.TryGetValue( id, out int stealCount ) ) {
				return 0.0f;
			}
			// reduce priority if this sound keeps getting stolen
			return Math.Clamp( stealCount * 0.1f, 0.0f, 0.5f );
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
				_logger.PrintDebug( in _fmodChannelCategory, $"AllocateChannelId: reusing released channel id {channelId}..." );
				return channelId;
			}
			_logger.PrintDebug( in _fmodChannelCategory, $"AllocateChannelId: stealing a channel..." );
			return StealChannel( startTime, id, position, priority, category, isEssential );
		}

		/*
		===============
		StealChannel
		===============
		*/
		/// <summary>
		///
		/// </summary>
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
				var sound = _allocatedChannels[ i ];
				if ( !sound.IsPlaying || sound.IsEssential ) {
					continue;
				}

				// check steal protection (can't steal brand new sounds)
				if ( sound.Age < sound.Category.Config.StealProtectionTime ) {
					continue;
				}

				// don't steal if recently stolen (prevents thrashing)
				if ( currentTime - sound.LastStolenTime < _minTimeBetweenChannelSteals ) {
					continue;
				}

				// calc steal score
				float stealScore = CalculateStealScore( currentTime, sound, priority, category );
				if ( stealScore > bestStealScore ) {
					bestStealScore = stealScore;
					bestCandidate = sound;
				}
			}
			if ( bestCandidate != null && bestStealScore > 0.0f ) {
				int stolenChannelId = bestCandidate.ChannelId;
				StopSound( currentTime, bestCandidate, true );

				_logger.PrintDebug( in _fmodChannelCategory, $"Channel {stolenChannelId} stolen" );

				UpdateStealStatistics( bestCandidate.Id );

				return stolenChannelId;
			}
			return -1;
		}

		/*
		===============
		CalculateStealScore
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="currentTime"></param>
		/// <param name="candidate"></param>
		/// <param name="newPriority"></param>
		/// <param name="category"></param>
		/// <returns></returns>
		private float CalculateStealScore( float currentTime, FMODChannel candidate, float newPriority, SoundCategory category ) {
			Vector2 listenerPos = _listenerService.ActiveListener;
			float distance = candidate.Instance.Position.DistanceTo( listenerPos );

			float priorityDiff = newPriority - candidate.CurrentPriority;
			float ageFactor = Math.Min( candidate.Age / 5.0f, 1.0f );
			float distanceFactor = 1.0f - CalculateDistanceFactor( distance );
			float volumeFactor = 1.0f - candidate.Volume;

			float score =
				priorityDiff * 2.0f +
				ageFactor * 0.5f +
				distanceFactor * _distanceWeight +
				volumeFactor * _volumeWeight;

			score *= category.Config.Name == candidate.Category.Config.Name ? 0.5f : 1.0f;

			float timeSinceLastStolen = currentTime - candidate.LastStolenTime;
			if ( timeSinceLastStolen < 1.0f ) {
				score *= timeSinceLastStolen;
			}

			return score;

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
		private void UpdateStealStatistics( IntPtr stolenEventId ) {
			if ( !_consecutiveStealCounts.TryGetValue( stolenEventId, out int count ) ) {
				count = 0;
			} else {
				count++;
			}
			_consecutiveStealCounts[ stolenEventId ] = count;
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
			foreach ( var kvp in _consecutiveStealCounts ) {
				int stealCount = kvp.Value;
				stealCount = Math.Min( 0, stealCount - 1 );
				if ( stealCount == 0 ) {
					_consecutiveStealCounts.Remove( kvp.Key );
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
			_logger.PrintDebug( in _fmodChannelCategory, $"StopSound: unloading channel {channel.ChannelId}..." );
			channel.Instance.Unload();

			if ( wasStolen ) {
				channel.LastStolenTime = startTime;
			}

			_channelPool.Return( channel );

			// FIXME: this is slow
			_allocatedChannels.Remove( channel );
			_freeChannelIds.Enqueue( channel.ChannelId );
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
				var sound = _allocatedChannels[ i ];
				if ( sound.Instance.instance.handle == eventInstance.handle ) {
					channel = sound;
					break;
				}
			}
			if ( channel != null ) {
				// ensure we unhook the callback (causes a seggy)
				StopSound( Time.GetTicksMsec() / 1000.0f, channel, false );
			}
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
				var channel = _allocatedChannels[ i ];
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
			var effectsVolume = cvarSystem.GetCVar<float>( Constants.CVars.Audio.EFFECTS_VOLUME ) ?? throw new CVarMissing( Constants.CVars.Audio.EFFECTS_VOLUME );
			_effectsVolume = effectsVolume.Value;
			effectsVolume.ValueChanged.Subscribe( this, OnEffectsVolumeChanged );

			var distanceFalloffStart = cvarSystem.GetCVar<float>( Constants.CVars.Audio.DISTANCE_FALLOFF_START ) ?? throw new CVarMissing( Constants.CVars.Audio.DISTANCE_FALLOFF_START );
			_distanceFalloffStart = distanceFalloffStart.Value;
			distanceFalloffStart.ValueChanged.Subscribe( this, OnDistanceFalloffStartValueChanged );

			var distanceFalloffEnd = cvarSystem.GetCVar<float>( Constants.CVars.Audio.DISTANCE_FALLOFF_END ) ?? throw new CVarMissing( Constants.CVars.Audio.DISTANCE_FALLOFF_END );
			_distanceFalloffEnd = distanceFalloffEnd.Value;
			distanceFalloffEnd.ValueChanged.Subscribe( this, OnDistanceFalloffEndValueChanged );

			var minTimeBetweenChannelSteals = cvarSystem.GetCVar<float>( Constants.CVars.Audio.MIN_TIME_BETWEEN_CHANNEL_STEALS ) ?? throw new CVarMissing( Constants.CVars.Audio.MIN_TIME_BETWEEN_CHANNEL_STEALS );
			_minTimeBetweenChannelSteals = minTimeBetweenChannelSteals.Value;
			minTimeBetweenChannelSteals.ValueChanged.Subscribe( this, OnMinTimeBetweenChannelStealsValueChanged );

			var frequencyPenalty = cvarSystem.GetCVar<float>( Constants.CVars.Audio.FREQUENCY_PENALTY ) ?? throw new CVarMissing( Constants.CVars.Audio.FREQUENCY_PENALTY );
			_frequencyPenalty = frequencyPenalty.Value;
			frequencyPenalty.ValueChanged.Subscribe( this, OnFrequencyPenaltyValueChanged );

			var volumeWeight = cvarSystem.GetCVar<float>( Constants.CVars.Audio.VOLUME_WEIGHT ) ?? throw new CVarMissing( Constants.CVars.Audio.VOLUME_WEIGHT );
			_volumeWeight = volumeWeight.Value;
			volumeWeight.ValueChanged.Subscribe( this, OnVolumeWeightValueChanged );

			var distanceWeight = cvarSystem.GetCVar<float>( Constants.CVars.Audio.DISTANCE_WEIGHT ) ?? throw new CVarMissing( Constants.CVars.Audio.DISTANCE_WEIGHT );
			_distanceWeight = distanceWeight.Value;
			distanceWeight.ValueChanged.Subscribe( this, OnDistanceWeightValueChanged );
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
				_logger.PrintError( in _fmodChannelCategory, $"ValidateMaxChannels: maxActiveChannels cannot be larger than maxChannels, resetting both." );
				_maxActiveChannels.Reset();
				_maxChannels.Reset();
			}
		}

		/*
		===============
		OnMaxActiveChannelsValueChanged
		===============
		*/
		private void OnMaxActiveChannelsValueChanged( in CVarValueChangedEventArgs<int> args ) {
			_maxChannels.Value = args.NewValue;
			ValidateMaxChannels();
		}

		/*
		===============
		OnMaxChannelsValueChanged
		===============
		*/
		private void OnEffectsVolumeChanged( in CVarValueChangedEventArgs<float> args ) {
			_effectsVolume = args.NewValue;
		}

		/*
		===============
		OnDistanceWeightValueChanged
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="args"></param>
		private void OnDistanceWeightValueChanged( in CVarValueChangedEventArgs<float> args ) {
			_distanceWeight = args.NewValue;
		}

		/*
		===============
		OnDistanceFalloffEndValueChanged
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="args"></param>
		private void OnDistanceFalloffEndValueChanged( in CVarValueChangedEventArgs<float> args ) {
			_distanceFalloffEnd = args.NewValue;
		}

		/*
		===============
		OnDistanceFalloffStartValueChanged
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="args"></param>
		private void OnDistanceFalloffStartValueChanged( in CVarValueChangedEventArgs<float> args ) {
			_distanceFalloffStart = args.NewValue;
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

		/*
		===============
		OnFrequencyPenaltyValueChanged
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="args"></param>
		private void OnFrequencyPenaltyValueChanged( in CVarValueChangedEventArgs<float> args ) {
			_frequencyPenalty = args.NewValue;
		}

		/*
		===============
		OnVolumeWeightValueChanged
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="args"></param>
		private void OnVolumeWeightValueChanged( in CVarValueChangedEventArgs<float> args ) {
			_volumeWeight = args.NewValue;
		}
	};
};
