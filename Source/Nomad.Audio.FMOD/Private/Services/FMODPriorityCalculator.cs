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
using System.Numerics;
using Nomad.Audio.Fmod.ValueObjects;
using Nomad.Audio.Interfaces;
using Nomad.Core;
using Nomad.Core.CVars;
using Nomad.Core.Events;
using Nomad.CVars;

namespace Nomad.Audio.Fmod.Private.Services {
	/*
	===================================================================================

	FMODPriorityCalculator

	===================================================================================
	*/
	/// <summary>
	/// </summary>
	internal sealed class FMODPriorityCalculator : IDisposable {
		private float _distanceFalloffStart;
		private float _distanceFalloffEnd;
		private float _distanceFalloff;

		private float _frequencyPenalty = 0.4f;

		private const float TIME_PENALTY_MULTIPLIER = 0.5f;

		private readonly ISubscriptionHandle _distanceFalloffStartEvent;
		private readonly ISubscriptionHandle _distanceFalloffEndEvent;
		private readonly ISubscriptionHandle _distanceWeightEvent;

		private readonly IListenerService _listenerService;

		private bool _isDisposed = false;

		/*
		===============
		FMODPriorityCalculator
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="cvarSystem"></param>
		/// <param name="listenerService"></param>
		/// <exception cref="ArgumentNullException"></exception>
		public FMODPriorityCalculator( ICVarSystemService cvarSystem, IListenerService listenerService ) {
			_listenerService = listenerService ?? throw new ArgumentNullException( nameof( listenerService ) );

			var distanceFalloffStart = cvarSystem.GetCVarOrThrow<float>( Constants.CVars.EngineUtils.Audio.DISTANCE_FALLOFF_START );
			_distanceFalloffStart = distanceFalloffStart.Value;
			_distanceFalloffStartEvent = distanceFalloffStart.ValueChanged.Subscribe( OnDistanceFalloffStartValueChanged );

			var distanceFalloffEnd = cvarSystem.GetCVarOrThrow<float>( Constants.CVars.EngineUtils.Audio.DISTANCE_FALLOFF_END );
			_distanceFalloffEnd = distanceFalloffEnd.Value;
			_distanceFalloffEndEvent = distanceFalloffEnd.ValueChanged.Subscribe( OnDistanceFalloffEndValueChanged );

			var frequencyPenalty = cvarSystem.GetCVarOrThrow<float>( Constants.CVars.EngineUtils.Audio.FREQUENCY_PENALTY );
			_frequencyPenalty = frequencyPenalty.Value;
			frequencyPenalty.ValueChanged.Subscribe( OnFrequencyPenaltyValueChanged );

			_distanceFalloff = _distanceFalloffEnd / _distanceFalloffStart;
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
				_distanceFalloffStartEvent?.Dispose();
				_distanceFalloffEndEvent?.Dispose();
				_distanceWeightEvent?.Dispose();
			}
			GC.SuppressFinalize( this );
			_isDisposed = true;
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
		/// <param name="lastPlayTimes"></param>
		/// <param name="consecutiveStealCounts"></param>
		/// <returns></returns>
		public float CalculateActualPriority(
			float startTime,
			string id,
			Vector2 position,
			float basePriority,
			SoundCategory category,
			Dictionary<string, float> lastPlayTimes,
			Dictionary<string, int> consecutiveStealCounts )
		{
			float priority =
				basePriority *
				category.Config.PriorityScale *
				CalculateDistanceFactor( Vector2.Distance( position, _listenerService.ActiveListener ) );

			priority *= 1.0f - CalculateTimePenalty( startTime, id, lastPlayTimes ) * TIME_PENALTY_MULTIPLIER;
			priority *= 1.0f - CalculateFrequencyPenalty( id, consecutiveStealCounts ) * _frequencyPenalty;

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
		public float CalculateDistanceFactor( float distance ) {
			if ( distance <= _distanceFalloffStart ) {
				return 1.0f;
			}
			if ( distance >= _distanceFalloffEnd ) {
				return 0.1f;
			}
			float t = (distance - _distanceFalloffStart) / _distanceFalloff;
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
		/// <param name="startTime"></param>
		/// <param name="id"></param>
		/// <param name="lastPlayTimes"></param>
		/// <returns></returns>
		private static float CalculateTimePenalty( float startTime, string id, Dictionary<string, float> lastPlayTimes ) {
			if ( !lastPlayTimes.TryGetValue( id, out float lastTime ) ) {
				return 0.0f;
			}

			float timeSinceLast = startTime - lastTime;
			const float protectionTime = 0.5f;

			return timeSinceLast > protectionTime ?
					0.0f
				:
					1.0f - (timeSinceLast / protectionTime);
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
		/// <param name="consecutiveStealCounts"></param>
		/// <returns></returns>
		private static float CalculateFrequencyPenalty( string id, Dictionary<string, int> consecutiveStealCounts ) {
			return !consecutiveStealCounts.TryGetValue( id, out int stealCount ) ?
					0.0f
				:
					Math.Clamp( stealCount * 0.1f, 0.0f, 0.5f );
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
			_distanceFalloff = _distanceFalloffEnd / _distanceFalloffStart;
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
			_distanceFalloff = _distanceFalloffEnd / _distanceFalloffStart;
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
	};
};
