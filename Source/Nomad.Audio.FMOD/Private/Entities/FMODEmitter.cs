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
using Nomad.Audio.Fmod.Private.Repositories;
using Nomad.Audio.Fmod.ValueObjects;
using Nomad.Audio.Interfaces;
using Nomad.Audio.ValueObjects;

namespace Nomad.Audio.Fmod.Private.Entities {
	internal sealed class FMODEmitter : IAudioEmitter {
		public Vector2 Position {
			get => _position;
			set {
				_position = value;

				if ( TryGetCurrentChannel( out var channel ) ) {
					channel.Instance.Position = value;
				}
			}
		}
		private Vector2 _position = Vector2.Zero;

		public float Volume {
			get => _volume;
			set {
				_volume = value;

				if ( TryGetCurrentChannel( out var channel ) ) {
					channel.Volume = value;
				}
			}
		}
		private float _volume = 1.0f;

		public float Pitch {
			get => _pitch;
			set {
				_pitch = value;

				if ( TryGetCurrentChannel( out var channel ) ) {
					channel.Pitch = value;
				}
			}
		}
		private float _pitch = 1.0f;

		public string Category => _category.Config.Name;

		public ChannelStatus Status {
			get {
				if ( !TryGetCurrentChannel( out var channel ) ) {
					return ChannelStatus.Stopped;
				}

				return channel.IsPlaying
					? ChannelStatus.Playing
					: ChannelStatus.Stopped;
			}
		}

		private readonly SoundCategory _category;
		private readonly FMODChannelRepository _channelRepository;

		private FMODChannelHandle? _currentHandle;

		public FMODEmitter( FMODChannelRepository channelRepository, SoundCategory category ) {
			_category = category;
			_channelRepository = channelRepository;
		}

		public void PlaySound( string id, float priority = 0.5f ) {
			UnhookCurrentHandle();

			var handle = _channelRepository.AllocateChannel( id, _position, _category, priority );
			if ( handle == null ) {
				_currentHandle = null;
				return;
			}

			_currentHandle = handle;
			_currentHandle.OnEnded += OnCurrentHandleEnded;

			ApplyCachedState();
		}

		private void ApplyCachedState() {
			if ( !TryGetCurrentChannel( out var channel ) ) {
				return;
			}

			channel.Instance.Position = _position;
			channel.Volume = _volume;
			channel.Pitch = _pitch;
		}

		private bool TryGetCurrentChannel( out FMODChannel channel ) {
			if ( _currentHandle == null ) {
				channel = null!;
				return false;
			}

			return _channelRepository.TryGetChannel( _currentHandle, out channel );
		}

		private void OnCurrentHandleEnded( FMODChannelHandle handle ) {
			if ( !ReferenceEquals( _currentHandle, handle ) ) {
				return;
			}

			_currentHandle.OnEnded -= OnCurrentHandleEnded;
			_currentHandle = null;
		}

		private void UnhookCurrentHandle() {
			if ( _currentHandle != null ) {
				_currentHandle.OnEnded -= OnCurrentHandleEnded;
				_currentHandle = null;
			}
		}
	};
};
