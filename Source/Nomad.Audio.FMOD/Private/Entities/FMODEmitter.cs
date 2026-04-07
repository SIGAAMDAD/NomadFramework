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
			get => _channel != null ? _channel.Instance.Position : Vector2.Zero;
			set => _channel?.Instance.Position = value;
		}

		public float Volume {
			get => _channel != null ? _channel.Instance.Volume : 0.0f;
			set => _channel?.Instance.Volume = value;
		}

		public float Pitch {
			get => _channel != null ? _channel.Instance.Pitch : 0.0f;
			set => _channel?.Instance.Pitch = value;
		}

		public string Category => _category.Config.Name;
		private readonly SoundCategory _category;

		public ChannelStatus Status {
			get {
				throw new System.NotImplementedException();
			}
		}

		private FMODChannel? _channel;

		private readonly FMODChannelRepository _channelRepository;

		public FMODEmitter( FMODChannelRepository channelRepository, SoundCategory category ) {
			_category = category;
			_channelRepository = channelRepository;
		}

		public void PlaySound( string id, Vector2 position = default, float priority = 0.5f ) {
			_channel = _channelRepository.AllocateChannel( id, position, _category, priority );
		}
	};
};
