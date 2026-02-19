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

namespace Nomad.Core
{
    /// <summary>
    /// 
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// 
        /// </summary>
        public static class CVars
        {
            /// <summary>
            /// 
            /// </summary>
            public static partial class Audio
            {
                /// <summary>
                /// 
                /// </summary>
                public const string EFFECTS_VOLUME = "audio.EffectsVolume";

                /// <summary>
                /// 
                /// </summary>
                public const string EFFECTS_ON = "audio.EffectsOn";

                /// <summary>
                /// 
                /// </summary>
                public const string MUSIC_VOLUME = "audio.MusicVolume";

                /// <summary>
                /// 
                /// </summary>
                public const string MUSIC_ON = "audio.MusicOn";

                /// <summary>
                /// 
                /// </summary>
                public const string MASTER_VOLUME = "audio.MasterVolume";

                /// <summary>
                /// 
                /// </summary>
                public const string MAX_CHANNELS = "audio.MaxChannels";

                /// <summary>
                /// 
                /// </summary>
                public const string MAX_ACTIVE_CHANNELS = "audio.MaxActiveChannels";

                /// <summary>
                /// 
                /// </summary>
                public const string DISTANCE_FALLOFF_START = "audio.DistanceFalloffStart";

                /// <summary>
                /// 
                /// </summary>
                public const string DISTANCE_FALLOFF_END = "audio.DistanceFalloffEnd";

                /// <summary>
                /// 
                /// </summary>
                public const string MIN_TIME_BETWEEN_CHANNEL_STEALS = "audio.MinTimeBetweenChannelSteals";

                /// <summary>
                /// 
                /// </summary>
                public const string FREQUENCY_PENALTY = "audio.FrequencyPenalty";

                /// <summary>
                /// 
                /// </summary>
                public const string VOLUME_WEIGHT = "audio.VolumeWeight";

                /// <summary>
                /// 
                /// </summary>
                public const string DISTANCE_WEIGHT = "audio.DistanceWeight";

                /// <summary>
                /// 
                /// </summary>
                public const string SPEAKER_MODE = "audio.SpeakerMode";

                /// <summary>
                /// 
                /// </summary>
                public const string OUTPUT_DEVICE_INDEX = "audio.OutputDeviceIndex";

                /// <summary>
                /// 
                /// </summary>
                public const string AUDIO_DRIVER = "audio.AudioDriver";

                /// <summary>
                /// 
                /// </summary>
                public static class FMOD
                {
                    /// <summary>
                    /// 
                    /// </summary>
                    public const string STREAM_BUFFER_SIZE = "audio.fmod.StreamBufferSize";

                    /// <summary>
                    /// 
                    /// </summary>
                    public const string DSP_BUFFER_SIZE = "audio.fmod.DSPBufferSize";

                    /// <summary>
                    /// 
                    /// </summary>
                    public const string DSP_BUFFER_COUNT = "audio.fmod.DSPBufferCount";

                    /// <summary>
                    /// 
                    /// </summary>
                    public const string LOGGING = "audio.fmod.LoggingEnabled";

                    /// <summary>
                    /// 
                    /// </summary>
                    public const string BANK_LOADING_STRATEGY = "audio.fmod.BankLoadingStrategy";
                }
            }

            /// <summary>
            /// 
            /// </summary>
            public static partial class Graphics
            {
                /// <summary>
                /// 
                /// </summary>
                public const string ANIMATION_QUALITY = "r.AnimationQuality";

                /// <summary>
                /// 
                /// </summary>
                public const string PARTICLE_QUALITY = "r.ParticleQuality";

                /// <summary>
                /// 
                /// </summary>
                public const string SHADOW_ATLAS_SIZE = "r.ShadowAtlasSize";

                /// <summary>
                /// 
                /// </summary>
                public const string SHADOW_FILTER_TYPE = "r.ShadowFilterType";

                /// <summary>
                /// 
                /// </summary>
                public const string SHADOW_FILTER_SMOOTH = "r.ShadowFilterSmooth";

                /// <summary>
                /// 
                /// </summary>
                public const string FORCE_VERTEX_SHADING = "r.ForceVertexShading";

                /// <summary>
                /// 
                /// </summary>
                public const string PHYSICALLY_BASED_RENDERING = "r.PhysicallyBasedRendering";

                /// <summary>
                /// 
                /// </summary>
                public const string BLOOM_ENABLED = "r.BloomEnabled";

                /// <summary>
                /// 
                /// </summary>
                public const string BAKED_LIGHTS = "r.BakedLights";
            }

            /// <summary>
            /// 
            /// </summary>
            public static partial class Display
            {
                /// <summary>
                /// 
                /// </summary>
                public const string ASPECT_RATIO = "display.AspectRatio";

                /// <summary>
                /// 
                /// </summary>
                public const string WINDOW_MODE = "display.WindowMode";

                /// <summary>
                /// 
                /// </summary>
                public const string WINDOW_RESOLUTION = "display.WindowResolution";

                /// <summary>
                /// 
                /// </summary>
                public const string ANTI_ALIASING = "display.AntiAliasing";

                /// <summary>
                /// 
                /// </summary>
                public const string MAX_FPS = "display.MaxFps";

                /// <summary>
                /// 
                /// </summary>
                public const string SEPARATE_RENDERING_THREAD = "r.SeparateRenderingThread";

                /// <summary>
                /// 
                /// </summary>
                public const string VSYNC_MODE = "display.VSyncMode";

                /// <summary>
                /// 
                /// </summary>
                public const string MONITOR = "display.Monitor";

                /// <summary>
                /// 
                /// </summary>
                public const string BRIGHTNESS = "display.Brightness";

                /// <summary>
                /// 
                /// </summary>
                public const string RESOLUTION_SCALE = "display.ResolutionScale";

                /// <summary>
                /// 
                /// </summary>
                public const string DRS_TARGET_FRAMES = "display.DRSTargetFrames";
            }

            /// <summary>
            /// 
            /// </summary>
            public static partial class Accessibility
            {
                /// <summary>
                /// 
                /// </summary>
                public const string HAPTIC_STRENGTH = "input.HapticStrength";

                /// <summary>
                /// 
                /// </summary>
                public const string HAPTIC_ENABLED = "input.HapticEnabled";

                /// <summary>
                /// 
                /// </summary>
                public const string COLORBLIND_MODE = "accessibility.ColorblindMode";

                /// <summary>
                /// 
                /// </summary>
                public const string DYSLEXIA_MODE = "accessibility.DyslexiaMode";

                /// <summary>
                /// 
                /// </summary>
                public const string UI_SCALE = "accessibility.UIScale";

                /// <summary>
                /// 
                /// </summary>
                public const string AUTO_AIM_MODE = "accessibility.AutoAimMode";

                /// <summary>
                /// 
                /// </summary>
                public const string TEXT_TO_SPEECH = "accessibility.TextToSpeech";
            }

            /// <summary>
            /// 
            /// </summary>
            public static partial class Console
            {
                /// <summary>
                /// 
                /// </summary>
                public const string DEFAULT_CONFIG_FILE = "console.DefaultConfig";

                /// <summary>
                /// 
                /// </summary>
                public const string CONSOLE_LOG_LEVEL = "console.LogLevel";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static class Events
        {
            /// <summary>
            /// 
            /// </summary>
            public static partial class Console
            {
                /// <summary>
                /// 
                /// </summary>
                public const string NAMESPACE = nameof(Console);

                /// <summary>
                /// 
                /// </summary>
                public const string CONSOLE_OPENED_EVENT = NAMESPACE + ":ConsoleOpened";

                /// <summary>
                /// 
                /// </summary>
                public const string CONSOLE_CLOSED_EVENT = NAMESPACE + ":ConsoleClosed";

                /// <summary>
                /// 
                /// </summary>
                public const string TEXT_ENTERED_EVENT = NAMESPACE + ":TextEntered";

                /// <summary>
                /// 
                /// </summary>
                public const string HISTORY_PREV_EVENT = NAMESPACE + ":HistoryPrev";

                /// <summary>
                /// 
                /// </summary>
                public const string HISTORY_NEXT_EVENT = NAMESPACE + ":HistoryNext";

                /// <summary>
                /// 
                /// </summary>
                public const string AUTOCOMPLETE_EVENT = NAMESPACE + ":AutoComplete";

                /// <summary>
                /// 
                /// </summary>
                public const string PAGE_UP_EVENT = NAMESPACE + ":PageUp";

                /// <summary>
                /// 
                /// </summary>
                public const string PAGE_DOWN_EVENT = NAMESPACE + ":PageDown";

                /// <summary>
                /// 
                /// </summary>
                public const string UNKNOWN_COMMAND_EVENT = NAMESPACE + ":UnknownCommand";

                /// <summary>
                /// 
                /// </summary>
                public const string COMMAND_EXECUTED_EVENT = NAMESPACE + ":CommandExecuted";
            }

            /// <summary>
            /// 
            /// </summary>
            public static partial class ResourceCache
            {
                /// <summary>
                /// 
                /// </summary>
                public const string NAMESPACE = nameof(ResourceCache);

                /// <summary>
                /// 
                /// </summary>
                public const string RESOURCE_LOADED_EVENT = NAMESPACE + ":ResourceLoaded";

                /// <summary>
                /// 
                /// </summary>
                public const string RESOURCE_LOAD_FAILED_EVENT = NAMESPACE + ":ResourceLoadFailed";

                /// <summary>
                /// 
                /// </summary>
                public const string RESOURCE_LOAD_PROGRESS_EVENT = NAMESPACE + ":ResourceLoadProgress";

                /// <summary>
                /// 
                /// </summary>
                public const string RESOURCE_UNLOADED_EVENT = NAMESPACE + ":ResourceUnloaded";
            }

            /// <summary>
            /// 
            /// </summary>
            public static partial class CVars
            {
                private const string NAMESPACE = nameof(CVars);

                /// <summary>
                /// 
                /// </summary>
                public const string CVAR_VALUE_CHANGED_EVENT = NAMESPACE + ":CVarValueChanged";
            }

            /// <summary>
            /// 
            /// </summary>
            public static partial class Rendering
            {
                private const string NAMESPACE = nameof(Rendering);

                /// <summary>
                /// 
                /// </summary>
                public const string ENTITY_VISIBILITY_CHANGED_EVENT = NAMESPACE + ":EntityVisibilityChanged";

                /// <summary>
                /// 
                /// </summary>
                public const string ANIMATION_FINISHED_EVENT = NAMESPACE + ":AnimationFinished";

                /// <summary>
                /// 
                /// </summary>
                public const string ANIMATION_LOOPED_EVENT = NAMESPACE + ":AnimationLooped";

                /// <summary>
                /// 
                /// </summary>
                public const string ANIMATION_CHANGED_EVENT = NAMESPACE + ":AnimationChanged";

                /// <summary>
                /// 
                /// </summary>
                public const string FRAME_CHANGED_EVENT = NAMESPACE + ":FrameChanged";

                /// <summary>
                /// 
                /// </summary>
                public const string SPRITE_FRAMES_CHANGED_EVENT = NAMESPACE + ":SpriteFramesChanged";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static class Commands
        {
            /// <summary>
            /// 
            /// </summary>
            public static partial class Console
            {
                /// <summary>
                /// 
                /// </summary>
                public const string ECHO_COMMAND = "echo";

                /// <summary>
                /// 
                /// </summary>
                public const string CLEAR_COMMAND = "clear";

                /// <summary>
                /// 
                /// </summary>
                public const string EXIT_COMMAND = "exit";

                /// <summary>
                /// 
                /// </summary>
                public const string QUIT_COMMAND = "quit";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static class Audio
        {
            /// <summary>
            /// 
            /// </summary>
            public const int MAX_AUDIO_CHANNELS = 512;

            /// <summary>
            /// 
            /// </summary>
            public const int MIN_AUDIO_CHANNELS = 64;
        }

        /// <summary>
        /// 
        /// </summary>
        public static class Multiplayer
        {
            /// <summary>
            /// 
            /// </summary>
            public const int MAX_PLAYERS = 16;
        }
    }
}
