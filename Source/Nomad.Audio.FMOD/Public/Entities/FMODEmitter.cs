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
using Nomad.Audio.Fmod.Private.Repositories;
using Nomad.Audio.Interfaces;
using Nomad.Audio.ValueObjects;

namespace Nomad.Audio.Fmod.Entities
{
    /*
	===================================================================================

	FMODEmitter

	===================================================================================
	*/
    /// <summary>
    /// A source of audio, an "emitter" in a sense.
    /// </summary>

    internal sealed class FMODEmitter(FMODChannelRepository channelRepository, string category) : IAudioEmitter
    {
        public Vector2 Positon
        {
            get => _position;
            set
            {
                if (_position == value)
                {
                    return;
                }
                _position = value;
                _channel?.SetPosition(value);
            }
        }
        private Vector2 _position = Vector2.Zero;

        public float Volume
        {
            get => _volume;
            set
            {
                if (_volume == value)
                {
                    return;
                }
                _volume = value;
                _channel?.SetVolume(value);
            }
        }
        private float _volume = 0.0f;

        public string Category => category;

        public AudioSourceStatus Status => _status;
        private AudioSourceStatus _status = AudioSourceStatus.Stopped;

        private FMODChannel? _channel;

        public void PlaySound(EventId id, float priority = 0.5f)
        {
            _channel = channelRepository.AllocateChannel(id, _position, category, priority, false);
        }
    };
};
