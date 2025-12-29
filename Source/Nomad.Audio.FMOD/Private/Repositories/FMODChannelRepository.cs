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
using Nomad.Audio.Fmod.Entities;
using Nomad.Audio.Fmod.Private.Entities;
using Nomad.Audio.Fmod.Private.ValueObjects;
using Nomad.Audio.Fmod.ValueObjects;
using Nomad.Audio.Interfaces;
using Nomad.Audio.ValueObjects;
using Nomad.Core;
using Nomad.Core.Exceptions;
using Nomad.Core.Logger;
using Nomad.CVars;
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

		private readonly Dictionary<ChannelHandle, int> _channelsIndexMap;
		private readonly List<FMODChannel> _allocatedChannels;
		private readonly Queue<int> _freeChannelIds;
		private readonly Dictionary<IntPtr, float> _lastPlayTimes = new Dictionary<IntPtr, float>();
		private readonly Dictionary<IntPtr, int> _consecutiveStealCounts = new Dictionary<IntPtr, int>();

		private readonly FMODBusRepository _busRepository;

		private bool _shouldDecay = false;
		private float _timePenaltyMultiplier = 0.5f;
		private float _distanceWeight = 0.3f;
		private float _volumeWeight = 0.2f;
		private float _frequencyPenalty = 0.4f;
		private float _minTimeBetweenChannelSteals = 0.1f;

		private float _distanceFalloffStart = 50.0f;
		private float _distanceFalloffEnd = 100.0f;
		private float _distanceFalloffLerp = 0.0f;
		private float _effectsVolume = 0.0f;

		private readonly ICVar<int> _maxChannels;
		private readonly ICVar<int> _maxActiveChannels;

		private readonly FMODEventRepository _eventRepository;
		private readonly FMODGuidRepository _guidRepository;
		private readonly ILoggerService _logger;
		private readonly IListenerService _listenerService;

		private readonly ObjectPool<FMODChannel> _channelPool = new ObjectPool<FMODChannel>();

		/*
		===============
		FMODChannelRepository
		===============
		*/
		public FMODChannelRepository( ILoggerService logger, ICVarSystemService cvarSystem, IListenerService listenerService,
			FMODEventRepository eventRepository, FMODGuidRepository guidRepository, FMODBusRepository busRepository ) {
			_logger = logger;
			_listenerService = listenerService;
			_busRepository = busRepository;

			_maxActiveChannels = cvarSystem.GetCVar<int>( Constants.CVars.Audio.MAX_ACTIVE_CHANNELS ) ?? throw new CVarMissing( Constants.CVars.Audio.MAX_ACTIVE_CHANNELS );
			_maxChannels = cvarSystem.GetCVar<int>( Constants.CVars.Audio.MAX_CHANNELS ) ?? throw new CVarMissing( Constants.CVars.Audio.MAX_CHANNELS );
			InitConfig( cvarSystem );

			_allocatedChannels = new List<FMODChannel>( _maxActiveChannels.Value );
			_freeChannelIds = new Queue<int>( _maxActiveChannels.Value );
			_channelsIndexMap = new Dictionary<ChannelHandle, int>( _maxActiveChannels.Value );

			for ( int i = 0; i < _maxActiveChannels.Value; i++ ) {
				_freeChannelIds.Enqueue( i );
			}
			_eventRepository = eventRepository;
			_guidRepository = guidRepository;
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
			_busRepository.Dispose();
		}

		/*
		===============
		GetChannel
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="channel"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public FMODChannel GetChannel( ChannelHandle channel ) {
			return _allocatedChannels[ _channelsIndexMap[ channel ] ];
		}

		/*
		===============
		TriggerEvent
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="eventDescription"></param>
		/// <param name="position"></param>
		/// <param name="config"></param>
		/// <param name="channel"></param>
		/// <returns></returns>
		public AudioResult TriggerEvent( FMODEventResource eventDescription, Vector2 position, SoundCategory config, out ChannelHandle channel ) {
			int soundsInCategory = CountSoundsInCategory( config.Handle );
			if ( soundsInCategory >= config.Config.MaxSimultaneous ) {
				if ( !config.Config.AllowStealingFromSameCategory ) {
					// not allowed to steal from ourselves, most unfortunate :(
					channel = new( 0 );
					return AudioResult.Success;
				}
			}
			IntPtr eventHandle = eventDescription.Handle.handle;
			float startTime = Time.GetTicksMsec() / 1000.0f;

			float actualPriority = CalculateActualPriority( startTime, eventHandle, position, 0.5f, config );
			int channelId = AllocateChannelId( startTime, actualPriority, config.Handle );
			if ( channelId == INVALID_CHANNEL ) {
				FMODValidator.ValidateCall( eventDescription.Handle.getPath( out var path ) );
				_logger.PrintError( $"Couldn't allocate a channel for sound event '{path}'!" );
				channel = new( 0 );
				return AudioResult.Error_DeviceError;
			}

			CreateSoundInstance( eventDescription, position, out var instance );
			channel = new( channelId );
			_channelsIndexMap[ channel ] = _allocatedChannels.Count;

			FMODChannel resource = _channelPool.Rent();
			resource.Instance = instance;
			resource.StartTime = startTime;
			resource.BasePriority = 0.5f;
			resource.CurrentPriority = actualPriority;
			resource.ChannelId = channelId;
			resource.IsEssential = false;

			_allocatedChannels.Add( resource );
			UpdateLastPlayTime( eventHandle, startTime );

			return AudioResult.Success;
		}

		/*
		===============
		UpdateLastPlayTime
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="eventHandle"></param>
		public void UpdateLastPlayTime( IntPtr eventHandle, float startTime ) {
			_lastPlayTimes[ eventHandle ] = startTime;
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
			CleanupFinishedSounds();
			EnforceCategoryLimits();
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
		private void CleanupFinishedSounds() {
			for ( int i = _allocatedChannels.Count - 1; i >= 0; i-- ) {
				if ( !_allocatedChannels[ i ].IsPlaying ) {
					StopSound( _allocatedChannels[ i ], false );
				}
			}
		}

		/*
		===============
		EnforceCategoryLimits
		===============
		*/
		private void EnforceCategoryLimits() {
			foreach ( var category in _busRepository.Categories ) {
				int count = CountSoundsInCategory( category.Value.Handle );
				if ( count > category.Value.Config.MaxSimultaneous ) {
					// find the lowest priority sound in this category to steal
					StealFromCategory( category.Value.Handle );
				}
			}
		}

		/*
		===============
		StealFromCategory
		===============
		*/
		private void StealFromCategory( ChannelGroupHandle category ) {
			FMODChannel? lowestPriority = null;
			float lowestPriorityValue = float.MaxValue;

			for ( int i = 0; i < _allocatedChannels.Count; i++ ) {
				var channel = _allocatedChannels[ i ];
				if ( channel.Category.Handle == category && channel.IsPlaying && !channel.IsEssential ) {
					if ( channel.CurrentPriority < lowestPriorityValue ) {
						lowestPriorityValue = channel.CurrentPriority;
						lowestPriority = channel;
					}
				}
			}
			if ( lowestPriority != null ) {
				StopSound( lowestPriority, true );
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
		/// <param name="eventHandle"></param>
		/// <param name="position"></param>
		/// <param name="basePriority"></param>
		/// <param name="category"></param>
		/// <returns></returns>
		private float CalculateActualPriority( float startTime, IntPtr eventHandle, Vector2 position, float basePriority, SoundCategory category ) {
			float distance = position.DistanceTo( _listenerService.ActiveListener );
			float distanceFactor = CalculateDistanceFactor( distance );

			// prevent spamming
			float timePenalty = CalculateTimePenalty( eventHandle, startTime );

			float frequencyPenalty = CalculateFrequencyPenalty( eventHandle );

			float categoryMultiplier = category.Config.PriorityScale;

			float priority = basePriority * categoryMultiplier * distanceFactor;
			priority *= 1.0f - timePenalty * _timePenaltyMultiplier;
			priority *= 1.0f - frequencyPenalty * _frequencyPenalty;

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
			float t = ( distance - _distanceFalloffStart ) / _distanceFalloffLerp;
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
		private float CalculateTimePenalty( IntPtr eventHandle, float startTime ) {
			if ( !_lastPlayTimes.TryGetValue( eventHandle, out float lastTime ) ) {
				return 0.0f;
			}
			float timeSinceLast = startTime - lastTime;
			float protectionTime = 0.5f;

			// less penality if enough time has passed
			if ( timeSinceLast > protectionTime ) {
				return 0.0f;
			}
			return 1.0f - ( timeSinceLast / protectionTime );
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
		private float CalculateFrequencyPenalty( IntPtr eventHandle ) {
			if ( !_consecutiveStealCounts.TryGetValue( eventHandle, out int stealCount ) ) {
				return 0.0f;
			}
			// reduce priority if this sound keeps getting stolen
			return Math.Clamp( stealCount * 0.1f, 0.0f, 0.5f );
		}

		/*
		===============
		AllocateChannelId
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="currentTime"></param>
		/// <param name="priority"></param>
		/// <param name="category"></param>
		/// <returns></returns>
		private int AllocateChannelId( float currentTime, float priority, ChannelGroupHandle category ) {
			if ( _freeChannelIds.Count > 0 ) {
				return _freeChannelIds.Dequeue();
			}
			return StealChannel( currentTime, priority, category );
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
		/// <param name="priority"></param>
		/// <param name="category"></param>
		/// <returns></returns>
		private int StealChannel( float currentTime, float priority, ChannelGroupHandle category ) {
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
				StopSound( bestCandidate, true );

				UpdateStealStatistics( bestCandidate.Instance.instance.handle );

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
		private float CalculateStealScore( float currentTime, FMODChannel candidate, float newPriority, ChannelGroupHandle category ) {
			float distance = candidate.Instance.Position.DistanceTo( _listenerService.ActiveListener );
			float priorityDiff = newPriority - candidate.CurrentPriority;
			float ageFactor = Math.Min( candidate.Age / 5.0f, 1.0f );
			float distanceFactor = 1.0f - CalculateDistanceFactor( distance );
			float volumeFactor = 1.0f - candidate.Volume;

			float score =
				priorityDiff * 2.0f +
				ageFactor * 0.5f +
				distanceFactor * _distanceWeight +
				volumeFactor * _volumeWeight;

			score *= category == candidate.Category.Handle ? 0.5f : 1.0f;

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
			if ( _consecutiveStealCounts.TryGetValue( stolenEventId, out int stealCount ) ) {
				stealCount++;
			} else {
				stealCount = 0;
			}
			_consecutiveStealCounts[ stolenEventId ] = stealCount;
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
				int stealCount = _consecutiveStealCounts[ kvp.Key ];
				if ( stealCount-- <= 0 ) {
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
		/// <param name="channel"></param>
		/// <param name="wasStolen"></param>
		private void StopSound( FMODChannel channel, bool wasStolen = false ) {
			channel.Instance.Dispose();
			if ( wasStolen ) {
				channel.LastStolenTime = Time.GetTicksMsec() / 1000.0f;
			}

			_channelPool.Return( channel );

			// FIXME: this is slow
			_channelsIndexMap.Remove( new( channel.ChannelId ) );
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
		/// <param name="resource"></param>
		/// <param name="position"></param>
		/// <param name="instance"></param>
		private void CreateSoundInstance( FMODEventResource resource, Vector2 position, out FMODChannelResource instance ) {
			resource.CreateInstance( out instance );
			instance.Position = position;
			instance.Volume = _effectsVolume;
			FMODValidator.ValidateCall( instance.instance.setCallback( SoundFinishedCallback, FMOD.Studio.EVENT_CALLBACK_TYPE.STOPPED | FMOD.Studio.EVENT_CALLBACK_TYPE.START_FAILED ) );
			FMODValidator.ValidateCall( instance.instance.start() );
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
		private FMOD.RESULT SoundFinishedCallback( FMOD.Studio.EVENT_CALLBACK_TYPE type, nint instance, nint parameters ) {
			FMOD.Studio.EventInstance eventInstance = new FMOD.Studio.EventInstance( instance );
			FMODChannel? channel = null;

			for ( int i = 0; i < _allocatedChannels.Count; i++ ) {
				var sound = _allocatedChannels[ i ];
				if ( sound.Instance == eventInstance ) {
					channel = sound;
					break;
				}
			}
			if ( channel != null ) {
				StopSound( channel, false );
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
		private int CountSoundsInCategory( ChannelGroupHandle category ) {
			int count = 0;
			for ( int i = 0; i < _allocatedChannels.Count; i++ ) {
				var channel = _allocatedChannels[ i ];
				if ( channel.Category.Handle == category && channel.IsPlaying ) {
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
			_effectsVolume = effectsVolume.Value / 10.0f;
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

			_maxActiveChannels.ValueChanged.Subscribe( this, OnMaxChannelsValueChanged );
			ValidateMaxActiveChannels();
		}

		/*
		===============
		ValidateMaxActiveChannels
		===============
		*/
		/// <summary>
		///
		/// </summary>
		private void ValidateMaxActiveChannels() {
			if ( _maxActiveChannels.Value > _maxChannels.Value ) {
				_logger.PrintError( $"FMODChannelRepository.ValidateMaxActiveChannels: maxActiveChannels cannot be larger than maxChannels, resetting both..." );
				_maxActiveChannels.Reset();
				_maxChannels.Reset();
			}
		}

		/*
		===============
		OnMaxChannelsValueChanged
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="args"></param>
		private void OnMaxChannelsValueChanged( in CVarValueChangedEventArgs<int> args ) {
			ValidateMaxActiveChannels();
		}

		/*
		===============
		OnMaxChannelsValueChanged
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="args"></param>
		private void OnEffectsVolumeChanged( in CVarValueChangedEventArgs<float> args ) {
			_effectsVolume = args.NewValue / 10.0f;
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
			_distanceFalloffEnd = args.NewValue;_distanceFalloffLerp = _distanceFalloffEnd / _distanceFalloffStart;
			_distanceFalloffLerp = _distanceFalloffEnd / _distanceFalloffStart;
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
			_distanceFalloffLerp = _distanceFalloffEnd / _distanceFalloffStart;
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
