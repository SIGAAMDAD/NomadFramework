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
using Nomad.Audio.Fmod.Private.Repositories;
using Nomad.Audio.Fmod.ValueObjects;
using Nomad.Audio.Interfaces;
using Nomad.Audio.ValueObjects;

namespace Nomad.Audio.Fmod.Private.Entities {
	internal sealed class FMODEmitter( FMODChannelRepository channelRepository, SoundCategory category ) : IAudioEmitter {
		public Vector2 Positon {
			get {
				throw new System.NotImplementedException();
			}
			set {
				throw new System.NotImplementedException();
			}
		}
		private Vector2 _position = Vector2.Zero;

		public float Volume {
			get {
				throw new System.NotImplementedException();
			}
			set {
				throw new System.NotImplementedException();
			}
		}

		public float Pitch {
			get {
				throw new System.NotImplementedException();
			}

			set {
				throw new System.NotImplementedException();
			}
		}

		public string Category {
			get {
				throw new System.NotImplementedException();
			}
		}

		public ChannelStatus Status {
			get {
				throw new System.NotImplementedException();
			}
		}

		private FMODChannel? _channel;

		public void PlaySound( string id, float priority = 0.5F ) {
			_channel = channelRepository.AllocateChannel( id, _position, category, priority );
		}
	};
};
