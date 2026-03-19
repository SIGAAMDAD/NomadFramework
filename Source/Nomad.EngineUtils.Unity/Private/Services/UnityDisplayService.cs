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

using System.Collections.Generic;
using Nomad.CVars;
using Nomad.Core.CVars;
using Nomad.Core.Engine.Rendering;
using Nomad.Core.Engine.Services;
using Nomad.Core.Engine.Windowing;
using Nomad.Core.Util;
using UnityEngine;

namespace Nomad.EngineUtils.Private.Services {
    /*
    ===================================================================================

    UnityDisplayService

    ===================================================================================
    */
    /// <summary>
    ///
    /// </summary>
    
    internal sealed class UnityDisplayService : IDisplayService {
        /// <summary>
        ///
        /// </summary>
        public bool SupportsBloom => true;

        /// <summary>
        ///
        /// </summary>
        public bool SupportsTAA => true;

        /// <summary>
        ///
        /// </summary>
        public Dictionary<string, Any> CustomSettings => _customSettings;
        private readonly Dictionary<string, Any> _customSettings = new Dictionary<string, Any>();

        private readonly ICVarSystemService _cvarSystem;
        private readonly IWindowService _windowService;

        /// <summary>
        ///
        /// </summary>
        /// <param name="windowService"></param>
        /// <param name="cvarSystem"></param>
        public UnityDisplayService( IWindowService windowService, ICVarSystemService cvarSystem ) {
            _windowService = windowService;
            _cvarSystem = cvarSystem;

            DisplayCVars.Register( cvarSystem );

            _cvarSystem.GetCVarOrThrow<WindowMode>( Core.Constants.CVars.EngineUtils.Display.WINDOW_MODE ).ValueChanged.Subscribe( OnWindowModeChanged );
            _cvarSystem.GetCVarOrThrow<int>( Core.Constants.CVars.EngineUtils.Display.MONITOR ).ValueChanged.Subscribe( OnMonitorIndexChanged );
            _cvarSystem.GetCVarOrThrow<int>( Core.Constants.CVars.EngineUtils.Display.MAX_FPS ).ValueChanged.Subscribe( OnMaxFpsChanged );
            _cvarSystem.GetCVarOrThrow<float>( Core.Constants.CVars.EngineUtils.Display.BRIGHTNESS ).ValueChanged.Subscribe( OnBrightnessChanged );
            _cvarSystem.GetCVarOrThrow<VSyncMode>( Core.Constants.CVars.EngineUtils.Display.VSYNC_MODE ).ValueChanged.Subscribe( OnVSyncModeChanged );
            _cvarSystem.GetCVarOrThrow<WindowResolution>( Core.Constants.CVars.EngineUtils.Display.WINDOW_RESOLUTION ).ValueChanged.Subscribe( OnWindowResolutionChanged );
            _cvarSystem.GetCVarOrThrow<AntiAliasingMode>( Core.Constants.CVars.EngineUtils.Display.ANTI_ALIASING ).ValueChanged.Subscribe( OnAntiAliasingChanged );
            _cvarSystem.GetCVarOrThrow<AspectRatio>( Core.Constants.CVars.EngineUtils.Display.ASPECT_RATIO ).ValueChanged.Subscribe( OnAspectRatioChanged );
            _cvarSystem.GetCVarOrThrow<float>( Core.Constants.CVars.EngineUtils.Display.RESOLUTION_SCALE ).ValueChanged.Subscribe( OnResolutionScaleChanged );
        }

        /*
        ===============
        OnBrightnessChanged
        ===============
        */
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        private void OnBrightnessChanged( in CVarValueChangedEventArgs<float> args ) {
            _customSettings[Core.Constants.CVars.EngineUtils.Display.BRIGHTNESS] = new Any( args.NewValue );
        }

        /*
        ===============
        OnWindowResolutionChanged
        ===============
        */
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        private void OnWindowResolutionChanged( in CVarValueChangedEventArgs<WindowResolution> args ) {
            WindowSize size = args.NewValue;
            _windowService.Width = size.Width;
            _windowService.Height = size.Height;
        }

        /*
        ===============
        OnMonitorIndexChanged
        ===============
        */
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        private void OnMonitorIndexChanged( in CVarValueChangedEventArgs<int> args ) {
            _windowService.ScreenIndex = args.NewValue;
        }

        /*
        ===============
        OnAspectRatioChanged
        ===============
        */
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        private void OnAspectRatioChanged( in CVarValueChangedEventArgs<AspectRatio> args ) {
            _customSettings[Core.Constants.CVars.EngineUtils.Display.ASPECT_RATIO] = new Any( args.NewValue.GetRatio() );
        }

        /*
        ===============
        OnWindowModeChanged
        ===============
        */
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        private void OnWindowModeChanged( in CVarValueChangedEventArgs<WindowMode> args ) {
            _windowService.Mode = args.NewValue;
        }

        /*
        ===============
        OnMaxFpsChanged
        ===============
        */
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        private void OnMaxFpsChanged( in CVarValueChangedEventArgs<int> args ) {
            Application.targetFrameRate = args.NewValue;
        }

        /*
        ===============
        OnVSyncModeChanged
        ===============
        */
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        private void OnVSyncModeChanged( in CVarValueChangedEventArgs<VSyncMode> args ) {
            switch ( args.NewValue ) {
                case VSyncMode.Disabled:
                    QualitySettings.vSyncCount = 0;
                    break;
                case VSyncMode.Enabled:
                case VSyncMode.Adaptive:
                    QualitySettings.vSyncCount = 1;
                    break;
                case VSyncMode.TripleBuffered:
                    QualitySettings.vSyncCount = 1;
                    _customSettings[Core.Constants.CVars.EngineUtils.Display.VSYNC_MODE] = new Any( "TripleBuffered" );
                    break;
            }
        }

        /*
        ===============
        OnAntiAliasingChanged
        ===============
        */
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        private void OnAntiAliasingChanged( in CVarValueChangedEventArgs<AntiAliasingMode> args ) {
            switch ( args.NewValue ) {
                case AntiAliasingMode.None:
                    QualitySettings.antiAliasing = 0;
                    break;
                case AntiAliasingMode.MSAA_2x:
                    QualitySettings.antiAliasing = 2;
                    break;
                case AntiAliasingMode.MSAA_4x:
                    QualitySettings.antiAliasing = 4;
                    break;
                case AntiAliasingMode.MSAA_8x:
                    QualitySettings.antiAliasing = 8;
                    break;
                default:
                    QualitySettings.antiAliasing = 0;
                    _customSettings[Core.Constants.CVars.EngineUtils.Display.ANTI_ALIASING] = new Any( args.NewValue.ToString() );
                    break;
            }
        }

        /*
        ===============
        OnResolutionScaleChanged
        ===============
        */
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        private void OnResolutionScaleChanged( in CVarValueChangedEventArgs<float> args ) {
            _customSettings[Core.Constants.CVars.EngineUtils.Display.RESOLUTION_SCALE] = new Any( args.NewValue );
        }
    };
};
