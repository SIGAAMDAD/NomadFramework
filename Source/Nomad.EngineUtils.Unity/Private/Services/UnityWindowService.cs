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

using System;
using System.Collections.Generic;
using Nomad.Core.Engine.Globals;
using Nomad.Core.Engine.Services;
using Nomad.Core.Engine.Windowing;
using Nomad.Core.Events;
using UnityEngine;

namespace Nomad.EngineUtils.Private.Services {
    /*
    ===================================================================================

    UnityWindowService

    ===================================================================================
    */
    /// <summary>
    ///
    /// </summary>
    
    internal sealed class UnityWindowService : IWindowService {
        /// <summary>
        ///
        /// </summary>
        public string Title {
            get => _title;
            set => _title = value;
        }
        private string _title;

        /// <summary>
        ///
        /// </summary>
        public int Width {
            get => Screen.width;
            set => ApplyResolution( value, Height );
        }

        /// <summary>
        ///
        /// </summary>
        public int Height {
            get => Screen.height;
            set => ApplyResolution( Width, value );
        }

        /// <summary>
        ///
        /// </summary>
        public int ScreenIndex {
            get => _screenIndex;
            set {
                if ( value < 0 || value >= _monitors.Length ) {
                    throw new ArgumentOutOfRangeException( nameof( value ) );
                }

                _screenIndex = value;
                if ( _screenIndex > 0 && _screenIndex < Display.displays.Length ) {
                    Display.displays[_screenIndex].Activate();
                }
            }
        }
        private int _screenIndex;

        /// <summary>
        ///
        /// </summary>
        public bool IsFocused => _isFocused;
        private bool _isFocused;

        /// <summary>
        ///
        /// </summary>
        public float RefreshRate => _monitors.Length == 0 ? 0.0f : _monitors[ScreenIndex].RefreshRate;

        /// <summary>
        ///
        /// </summary>
        public IReadOnlyList<Monitor> Monitors => _monitors;
        private Monitor[] _monitors;

        /// <summary>
        ///
        /// </summary>
        public WindowMode Mode {
            get => _mode;
            set => SetWindowMode( value );
        }
        private WindowMode _mode;

        /// <summary>
        ///
        /// </summary>
        public IGameEvent<WindowSizeChangedEventArgs> SizeChanged => _sizeChanged;
        private readonly IGameEvent<WindowSizeChangedEventArgs> _sizeChanged;

        /// <summary>
        ///
        /// </summary>
        public IGameEvent<bool> FocusChanged => _focusChanged;
        private readonly IGameEvent<bool> _focusChanged;

        /// <summary>
        ///
        /// </summary>
        public IGameEvent<EmptyEventArgs> CloseRequested => _closeRequested;
        private readonly IGameEvent<EmptyEventArgs> _closeRequested;

        private readonly UnityRuntimeDriver _runtimeDriver;

        private bool _isDisposed;

        /// <summary>
        ///
        /// </summary>
        /// <param name="eventFactory"></param>
        public UnityWindowService( IGameEventRegistryService eventFactory ) {
            _title = Application.productName;
            _screenIndex = 0;
            _isFocused = Application.isFocused;

            _sizeChanged = eventFactory.GetEvent<WindowSizeChangedEventArgs>( Core.Constants.Events.EngineUtils.WINDOW_SIZE_CHANGED, Core.Constants.Events.EngineUtils.NAMESPACE );
            _closeRequested = eventFactory.GetEvent<EmptyEventArgs>( Core.Constants.Events.EngineUtils.CLOSE_REQUESTED, Core.Constants.Events.EngineUtils.NAMESPACE );
            _focusChanged = eventFactory.GetEvent<bool>( Core.Constants.Events.EngineUtils.FOCUS_CHANGED, Core.Constants.Events.EngineUtils.NAMESPACE );

            _runtimeDriver = UnityRuntimeDriver.Create();
            _runtimeDriver.ScreenSizeChanged += OnScreenSizeChanged;

            Application.focusChanged += OnFocusChanged;
            Application.quitting += OnApplicationQuitting;

            _monitors = GetScreenList();
            _mode = FromUnityMode( Screen.fullScreenMode );

            WindowService.Initialize( this );
        }

        /// <summary>
        ///
        /// </summary>
        public void Dispose() {
            if ( _isDisposed ) {
                return;
            }

            Application.focusChanged -= OnFocusChanged;
            Application.quitting -= OnApplicationQuitting;

            if ( _runtimeDriver != null ) {
                _runtimeDriver.ScreenSizeChanged -= OnScreenSizeChanged;
                UnityEngine.Object.Destroy( _runtimeDriver.gameObject );
            }

            _sizeChanged.Dispose();
            _closeRequested.Dispose();
            _focusChanged.Dispose();

            _isDisposed = true;
            GC.SuppressFinalize( this );
        }

        /*
        ===============
        GetScreenResolution
        ===============
        */
        /// <summary>
        ///
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void GetScreenResolution( out int width, out int height ) {
            if ( ScreenIndex < Display.displays.Length ) {
                width = Display.displays[ScreenIndex].systemWidth;
                height = Display.displays[ScreenIndex].systemHeight;
                return;
            }

            width = Screen.currentResolution.width;
            height = Screen.currentResolution.height;
        }

        /*
        ===============
        GetSupportedResolutions
        ===============
        */
        /// <summary>
        ///
        /// </summary>
        /// <param name="monitorIndex"></param>
        /// <returns></returns>
        public IReadOnlyList<WindowResolution> GetSupportedResolutions( int monitorIndex ) {
            if ( monitorIndex < 0 || monitorIndex >= _monitors.Length ) {
                throw new ArgumentOutOfRangeException( nameof( monitorIndex ) );
            }

            var resolutions = new List<WindowResolution>();
            Monitor monitor = _monitors[monitorIndex];

            for ( var resolution = WindowResolution.Min; resolution <= WindowResolution.Max; resolution++ ) {
                if ( monitor.ScreenSize >= resolution ) {
                    resolutions.Add( resolution );
                }
            }

            return resolutions;
        }

        /*
        ===============
        GetNativeResolutionForMonitor
        ===============
        */
        /// <summary>
        ///
        /// </summary>
        /// <param name="monitorIndex"></param>
        /// <param name="nativeSize"></param>
        public void GetNativeResolutionForMonitor( int monitorIndex, out WindowSize nativeSize ) {
            if ( monitorIndex < 0 || monitorIndex >= _monitors.Length ) {
                throw new ArgumentOutOfRangeException( nameof( monitorIndex ) );
            }

            nativeSize = _monitors[monitorIndex].ScreenSize;
        }

        /*
        ===============
        GetScreenList
        ===============
        */
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static Monitor[] GetScreenList() {
            Display[] displays = Display.displays;
            if ( displays == null || displays.Length == 0 ) {
                return new[]
                {
                    new Monitor(0, (int)Screen.currentResolution.refreshRateRatio.value, (WindowResolution)new WindowSize(Screen.currentResolution.width, Screen.currentResolution.height))
                };
            }

            var screens = new Monitor[displays.Length];
            int refreshRate = (int)Screen.currentResolution.refreshRateRatio.value;

            for ( int i = 0; i < displays.Length; i++ ) {
                int width = displays[i].systemWidth > 0 ? displays[i].systemWidth : Screen.currentResolution.width;
                int height = displays[i].systemHeight > 0 ? displays[i].systemHeight : Screen.currentResolution.height;
                screens[i] = new Monitor( i, refreshRate, (WindowResolution)new WindowSize( width, height ) );
            }

            return screens;
        }

        /*
        ===============
        SetWindowMode
        ===============
        */
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode"></param>
        private void SetWindowMode( WindowMode mode ) {
            Screen.fullScreenMode = ToUnityMode( mode );
            _mode = mode;
        }

        /*
        ===============
        ApplyResolution
        ===============
        */
        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private void ApplyResolution( int width, int height ) {
            if ( ScreenIndex == 0 ) {
                Screen.SetResolution( width, height, ToUnityMode( _mode ) );
                return;
            }

            if ( ScreenIndex < Display.displays.Length ) {
                RefreshRate refreshRate = new RefreshRate { numerator = (uint)RefreshRate, denominator = 1 };
                Display.displays[ScreenIndex].Activate( width, height, refreshRate );
            }
        }

        /*
        ===============
        ToUnityMode
        ===============
        */
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        private static FullScreenMode ToUnityMode( WindowMode mode ) {
            switch ( mode ) {
                case WindowMode.Windowed:
                    return FullScreenMode.Windowed;
                case WindowMode.BorderlessWindowed:
                case WindowMode.BorderlessFullscreen:
                    return FullScreenMode.FullScreenWindow;
                case WindowMode.Fullscreen:
                    return FullScreenMode.MaximizedWindow;
                case WindowMode.ExclusiveFullscreen:
                    return FullScreenMode.ExclusiveFullScreen;
                default:
                    return FullScreenMode.FullScreenWindow;
            }
        }

        /*
        ===============
        FromUnityMode
        ===============
        */
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        private static WindowMode FromUnityMode( FullScreenMode mode ) {
            switch ( mode ) {
                case FullScreenMode.Windowed:
                    return WindowMode.Windowed;
                case FullScreenMode.MaximizedWindow:
                    return WindowMode.Fullscreen;
                case FullScreenMode.ExclusiveFullScreen:
                    return WindowMode.ExclusiveFullscreen;
                case FullScreenMode.FullScreenWindow:
                default:
                    return WindowMode.BorderlessFullscreen;
            }
        }

        /*
        ===============
        OnFocusChanged
        ===============
        */
        /// <summary>
        /// 
        /// </summary>
        /// <param name="focused"></param>
        private void OnFocusChanged( bool focused ) {
            _isFocused = focused;
            _focusChanged.Publish( focused );
        }

        /*
        ===============
        OnScreenSizeChanged
        ===============
        */
        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private void OnScreenSizeChanged( int width, int height ) {
            _sizeChanged.Publish( new WindowSizeChangedEventArgs( width, height ) );
        }

        /*
        ===============
        OnApplicationQuitting
        ===============
        */
        /// <summary>
        /// 
        /// </summary>
        private void OnApplicationQuitting() {
            _closeRequested.Publish( default );
        }
    };
};
