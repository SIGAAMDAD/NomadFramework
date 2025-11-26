/*
===========================================================================
The Nomad AGPL Source Code
Copyright (C) 2025 Noah Van Til

The Nomad Source Code is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published
by the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

The Nomad Source Code is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with The Nomad Source Code.  If not, see <http://www.gnu.org/licenses/>.

If you have questions concerning this license or the applicable additional
terms, you may contact me via email at nyvantil@gmail.com.
===========================================================================
*/

using Godot;
using NomadCore.Abstractions.Services;
using NomadCore.Enums.Audio;
using NomadCore.Infrastructure;
using NomadCore.Infrastructure.Events;
using NomadCore.Interfaces.Audio;
using NomadCore.Interfaces.ConsoleSystem;
using NomadCore.Interfaces.EventSystem;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.Marshalling;

namespace NomadCore.Systems.Audio.Infrastructure {
	/*
	===================================================================================
	
	AudioSourceBase
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	internal class AudioSourceBase : PoolableObject, IAudioSource {
		/// <summary>
		/// The volume, in decibles, of the audio source.
		/// </summary>
		public float Volume => _volume;
		protected float _volume = 0.0f;

		/// <summary>
		/// The source's current playback state.
		/// </summary>
		public AudioSourceStatus Status => _status;
		protected AudioSourceStatus _status = AudioSourceStatus.Stopped;

		public SourceType Type => _type;
		protected readonly SourceType _type = SourceType.UI;

		public IAudioStream? Stream => _stream;
		protected IAudioStream? _stream;

		public Node StreamNode => GetStreamNode();

		protected readonly IAudioService? Service = ServiceRegistry.Get<IAudioService>();

		public IGameEvent<AudioStreamFinished> Finished => _finished;
		protected readonly IGameEvent<AudioStreamFinished> _finished;

		/*
		===============
		AudioSourceBase
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="type"></param>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public AudioSourceBase( SourceType type ) {
			ArgumentNullException.ThrowIfNull( Service );

			ICVar<float> volumeConfig = type switch {
				SourceType.UI or SourceType.Entity or SourceType.Ambience => Service.SoundVolume,
				SourceType.Music => Service.MusicVolume,
				_ => throw new ArgumentOutOfRangeException( $"Invalid SourceType '{type}' for IAudioSource" )
			};
			
			volumeConfig.ValueChanged.Subscribe( this, OnVolumeChanged );
			_volume = Mathf.LinearToDb( volumeConfig.Value );

			_type = type;
			_stream = null;

			var eventBus = ServiceRegistry.Get<IGameEventBusService>();
			_finished = eventBus.CreateEvent<AudioStreamFinished>( nameof( Finished ) );
		}

		/*
		===============
		PlaySound
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="looping"></param>
		public virtual void PlaySound( IAudioStream stream, bool looping = false ) {
			ArgumentNullException.ThrowIfNull( stream );

			_stream = stream;
			_status = looping ? AudioSourceStatus.Looping : AudioSourceStatus.Playing;
		}

		/*
		===============
		Stop
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		public virtual void Stop() {
			_status = AudioSourceStatus.Stopped;
			_stream = null;
		}

		/*
		===============
		Dispose
		===============
		*/
		public override void Dispose() {
			GC.SuppressFinalize( this );
		}

		/*
		===============
		GetStreamNode
		===============
		*/
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		protected virtual Node GetStreamNode() {
			throw new NotImplementedException();
		}

		/*
		===============
		OnVolumeChanged
		===============
		*/
		/// <summary>
		/// Sets the <see cref="_volume"/> based on the changed CVar value.
		/// </summary>
		/// <param name="args"></param>
		private void OnVolumeChanged( in ICVarValueChangedEventData<float> args ) {
			_volume = Mathf.LinearToDb( args.Value );
		}
	};
};