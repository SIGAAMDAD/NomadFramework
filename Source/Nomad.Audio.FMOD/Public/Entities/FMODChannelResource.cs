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
using Godot;
using Nomad.Audio.Interfaces;
using Nomad.Core.Abstractions;

namespace Nomad.Audio.Fmod.Entities
{
    /// <summary>
    ///
    /// </summary>
    internal record struct FMODChannelResource(FMOD.Studio.EventInstance instance) : IAudioResource, IValueObject<FMODChannelResource>
    {
        public readonly FMOD.Studio.PLAYBACK_STATE PlaybackState
        {
            get
            {
                FMODValidator.ValidateCall(instance.getPlaybackState(out FMOD.Studio.PLAYBACK_STATE state));
                return state;
            }
        }
        public readonly uint ListenerMask
        {
            get
            {
                FMODValidator.ValidateCall(instance.getListenerMask(out uint mask));
                return mask;
            }
            set
            {
                FMODValidator.ValidateCall(instance.setListenerMask(value));
            }
        }
        public Vector2 Position
        {
            get
            {
                FMODValidator.ValidateCall(instance.get3DAttributes(out FMOD.ATTRIBUTES_3D attributes));
                return new Vector2() { X = attributes.position.x, Y = attributes.position.y };
            }
            set
            {
                FMODValidator.ValidateCall(instance.set3DAttributes(
                    new FMOD.ATTRIBUTES_3D
                    {
                        position = new FMOD.VECTOR
                        {
                            x = value.X,
                            y = value.Y,
                            z = 1.0f,
                        }
                    }
                ));
            }
        }
        public readonly float Volume
        {
            get
            {
                FMODValidator.ValidateCall(instance.getVolume(out float volume));
                return volume;
            }
            set
            {
                FMODValidator.ValidateCall(instance.setVolume(value));
            }
        }
        public readonly float Pitch
        {
            get
            {
                FMODValidator.ValidateCall(instance.getPitch(out float pitch));
                return pitch;
            }
            set
            {
                FMODValidator.ValidateCall(instance.setPitch(value));
            }
        }

        public readonly FMOD.Studio.MEMORY_USAGE MemoryUsage
        {
            get
            {
                FMODValidator.ValidateCall(instance.getMemoryUsage(out FMOD.Studio.MEMORY_USAGE memoryUsage));
                return memoryUsage;
            }
        }
        public readonly FMOD.Studio.EventDescription Description
        {
            get
            {
                FMODValidator.ValidateCall(instance.getDescription(out FMOD.Studio.EventDescription description));
                return description;
            }
        }
        public readonly bool IsPlaying => PlaybackState == FMOD.Studio.PLAYBACK_STATE.PLAYING;
        public readonly bool IsValid => instance.isValid();

        /// <summary>
        ///
        /// </summary>
        public readonly void Dispose()
        {
            Unload();
        }

        /// <summary>
        /// Clears the unmanaged FMOD EventInstance.
        /// </summary>
        public readonly void Unload()
        {
            if (instance.isValid())
            {
                FMODValidator.ValidateCall(instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE));

                // ensure we unhook the callback (causes a seggy if its not done)
                FMODValidator.ValidateCall(instance.setCallback(null, FMOD.Studio.EVENT_CALLBACK_TYPE.STOPPED | FMOD.Studio.EVENT_CALLBACK_TYPE.START_FAILED));

                FMODValidator.ValidateCall(instance.release());
                instance.clearHandle();
            }
        }

        public static bool operator ==(FMODChannelResource resource, FMOD.Studio.EventInstance eventInstance) => resource.instance.handle == eventInstance.handle;
        public static bool operator !=(FMODChannelResource resource, FMOD.Studio.EventInstance eventInstance) => resource.instance.handle != eventInstance.handle;
    };
};
