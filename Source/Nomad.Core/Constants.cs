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
        public static class Events
        {
            public static partial class Console
            {
                public const string NAMESPACE = nameof(Console);

#if USE_COMPATIBILITY_EXTENSIONS || UNITY_EDITOR
                public const string CONSOLE_OPENED_EVENT = NAMESPACE + ":ConsoleOpened";
                public const string CONSOLE_CLOSED_EVENT = NAMESPACE + ":ConsoleClosed";
                public const string TEXT_ENTERED_EVENT = NAMESPACE + ":TextEntered";
                public const string HISTORY_PREV_EVENT = NAMESPACE + ":HistoryPrev";
                public const string HISTORY_NEXT_EVENT = NAMESPACE + ":HistoryNext";
                public const string AUTOCOMPLETE_EVENT = NAMESPACE + ":AutoComplete";
                public const string PAGE_UP_EVENT = NAMESPACE + ":PageUp";
                public const string PAGE_DOWN_EVENT = NAMESPACE + ":PageDown";
                public const string UNKNOWN_COMMAND_EVENT = NAMESPACE + ":UnknownCommand";
                public const string COMMAND_EXECUTED_EVENT = NAMESPACE + ":CommandExecuted";
#else
                public const string CONSOLE_OPENED_EVENT = $"{NAMESPACE}:ConsoleOpened";
                public const string CONSOLE_CLOSED_EVENT = $"{NAMESPACE}:ConsoleClosed";
                public const string TEXT_ENTERED_EVENT = $"{NAMESPACE}:TextEntered";
                public const string HISTORY_PREV_EVENT = $"{NAMESPACE}:HistoryPrev";
                public const string HISTORY_NEXT_EVENT = $"{NAMESPACE}:HistoryNext";
                public const string AUTOCOMPLETE_EVENT = $"{NAMESPACE}:AutoComplete";
                public const string PAGE_UP_EVENT = $"{NAMESPACE}:PageUp";
                public const string PAGE_DOWN_EVENT = $"{NAMESPACE}:PageDown";
                public const string UNKNOWN_COMMAND_EVENT = $"{NAMESPACE}:UnknownCommand";
                public const string COMMAND_EXECUTED_EVENT = $"{NAMESPACE}:CommandExecuted";
#endif
            }
            public static partial class ResourceCache
            {
                public const string NAMESPACE = nameof(ResourceCache);

#if USE_COMPATIBILITY_EXTENSIONS || UNITY_EDITOR
                public const string RESOURCE_LOADED_EVENT = NAMESPACE + ":ResourceLoaded";
                public const string RESOURCE_LOAD_FAILED_EVENT = NAMESPACE + ":ResourceLoadFailed";
                public const string RESOURCE_LOAD_PROGRESS_EVENT = NAMESPACE + ":ResourceLoadProgress";
                public const string RESOURCE_UNLOADED_EVENT = NAMESPACE + ":ResourceUnloaded";
#else
                public const string RESOURCE_LOADED_EVENT = $"{NAMESPACE}:ResourceLoaded";
                public const string RESOURCE_LOAD_FAILED_EVENT = $"{NAMESPACE}:ResourceLoadFailed";
                public const string RESOURCE_LOAD_PROGRESS_EVENT = $"{NAMESPACE}:ResourceLoadProgress";
                public const string RESOURCE_UNLOADED_EVENT = $"{NAMESPACE}:ResourceUnloaded";
#endif
            }
            public static partial class CVars
            {
                public const string NAMESPACE = nameof(CVars);

#if USE_COMPATIBILITY_EXTENSIONS || UNITY_EDITOR
                public const string CVAR_VALUE_CHANGED_EVENT = NAMESPACE + ":CVarValueChanged";
#else
                public const string CVAR_VALUE_CHANGED_EVENT = $"{NAMESPACE}:CVarValueChanged";
#endif
            }
            public static partial class Rendering
            {
                public const string NAMESPACE = nameof(Rendering);

                public const string ENTITY_VISIBILITY_CHANGED_EVENT = "Rendering:EntityVisibilityChanged";
                public const string ANIMATION_FINISHED_EVENT = "Rendering:AnimationFinished";
                public const string ANIMATION_LOOPED_EVENT = "Rendering:AnimationLooped";
                public const string ANIMATION_CHANGED_EVENT = "Rendering:AnimationChanged";
                public const string FRAME_CHANGED_EVENT = "Rendering:FrameChanged";
                public const string SPRITE_FRAMES_CHANGED_EVENT = "Rendering:SpriteFramesChanged";
            }
        }
        public static class Commands
        {
            public static partial class Console
            {
                public const string ECHO_COMMAND = "echo";
                public const string CLEAR_COMMAND = "clear";
                public const string EXIT_COMMAND = "exit";
                public const string QUIT_COMMAND = "quit";
            }
        }
        public static class Audio
        {
            public const int MAX_AUDIO_CHANNELS = 512;
            public const int MIN_AUDIO_CHANNELS = 64;
        }
        public static class Multiplayer
        {
            public const int MAX_PLAYERS = 16;
        }
    }
}
