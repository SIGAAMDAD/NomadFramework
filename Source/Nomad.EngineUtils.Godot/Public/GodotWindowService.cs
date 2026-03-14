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
using System.Collections.Generic;
using Nomad.Core.Compatibility.Guards;
using Nomad.Core.Events;
using Godot;
using Nomad.Core.Engine.Windowing;
using Nomad.Core.Engine.Globals;
using Nomad.Core.Engine.Services;

namespace Nomad.EngineUtils
{
    /// <summary>
    ///
    /// </summary>
    public sealed class GodotWindowService : IWindowService
    {
        /// <summary>
        ///
        /// </summary>
        public string Title
        {
            get => _window.Title;
            set => _window.Title = value;
        }

        /// <summary>
        ///
        /// </summary>
        public int Width
        {
            get => _window.Size.X;
            set => _window.Size = new Vector2I(value, _window.Size.Y);
        }

        /// <summary>
        ///
        /// </summary>
        public int Height
        {
            get => _window.Size.Y;
            set => _window.Size = new Vector2I(_window.Size.X, value);
        }

        /// <summary>
        ///
        /// </summary>
        public int ScreenIndex
        {
            get => _window.CurrentScreen;
            set
            {
                _window.CurrentScreen = value;
                var size = DisplayServer.ScreenGetSize(ScreenIndex);
                _screenWidth = size.X;
                _screenHeight = size.Y;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public float RefreshRate => _monitors[_window.CurrentScreen].RefreshRate;

        /// <summary>
        ///
        /// </summary>
        public IReadOnlyList<Monitor> Monitors => _monitors;
        private readonly Monitor[] _monitors;

        /// <summary>
        ///
        /// </summary>
        public WindowMode Mode
        {
            get => _mode;
            set => SetWindowMode(value);
        }
        private WindowMode _mode;

        /// <summary>
        ///
        /// </summary>
        public bool IsFocused => _isFocused;
        private bool _isFocused = true;

        /// <summary>
        ///
        /// </summary>
        public int MonitorIndex
        {
            get => _window.CurrentScreen;
            set
            {
                RangeGuard.ThrowIfOutOfRange(value, 0, _monitors.Length - 1);
                _window.CurrentScreen = value;
            }
        }

        private int _screenWidth = 0;
        private int _screenHeight = 0;

        private readonly Window _window;

        private bool _isDisposed = false;

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

        /// <summary>
        ///
        /// </summary>
        /// <param name="sceneTree"></param>
        /// <param name="eventFactory"></param>
        internal GodotWindowService(SceneTree sceneTree, IGameEventRegistryService eventFactory)
        {
            _window = sceneTree.Root.GetWindow();
            _window.FocusEntered += OnFocusEntered;
            _window.FocusExited += OnFocusExited;
            _window.SizeChanged += OnSizeChanged;
            _window.CloseRequested += OnCloseRequested;
            _window.TitleChanged += OnTitleChanged;

            _sizeChanged = eventFactory.GetEvent<WindowSizeChangedEventArgs>(Core.Constants.Events.EngineUtils.WINDOW_SIZE_CHANGED, Core.Constants.Events.EngineUtils.NAMESPACE);
            _closeRequested = eventFactory.GetEvent<EmptyEventArgs>(Core.Constants.Events.EngineUtils.CLOSE_REQUESTED, Core.Constants.Events.EngineUtils.NAMESPACE);
            _focusChanged = eventFactory.GetEvent<bool>(Core.Constants.Events.EngineUtils.FOCUS_CHANGED, Core.Constants.Events.EngineUtils.NAMESPACE);

            _monitors = GetScreenList();

            WindowService.Initialize(this);
        }

        /// <summary>
        ///
        /// </summary>
        public void Dispose()
        {
            if (!_isDisposed)
            {
                _sizeChanged?.Dispose();
                _closeRequested?.Dispose();
                _focusChanged?.Dispose();
                _window?.Dispose();
            }
            GC.SuppressFinalize(this);
            _isDisposed = true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void GetScreenResolution(out int width, out int height)
        {
            width = _screenWidth;
            height = _screenHeight;
        }

        /// <summary>
		/// Returns a list of monitor supported window resolutions.
		/// </summary>
		/// <param name="monitorIndex"></param>
		/// <returns></returns>
		public IReadOnlyList<WindowResolution> GetSupportedResolutions(int monitorIndex)
        {
            var monitor = _monitors[monitorIndex];
            var resolutions = new List<WindowResolution>();

            for (var resolution = WindowResolution.Min; resolution <= WindowResolution.Max; resolution++)
            {
                if (monitor.ScreenSize >= resolution)
                {
                    resolutions.Add(resolution);
                }
            }

            return [.. resolutions];
        }

        /// <summary>
		///
		/// </summary>
		/// <param name="monitorIndex"></param>
        /// <param name="nativeSize"></param>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public void GetNativeResolutionForMonitor(int monitorIndex, out WindowSize nativeSize)
        {
            RangeGuard.ThrowIfOutOfRange(monitorIndex, 0, _monitors.Length - 1, nameof(monitorIndex));
            nativeSize = _monitors[monitorIndex].ScreenSize;
        }

        /// <summary>
		/// Gets a list of all the currently available monitors and fetches their sizes and other various data.
		/// </summary>u
		/// <returns></returns>
		private static Monitor[] GetScreenList()
        {
            int screenCount = DisplayServer.GetScreenCount();
            var screens = new Monitor[screenCount];

            for (int i = 0; i < screenCount; i++)
            {
                var screenSize = DisplayServer.ScreenGetSize(i);
                var size = new WindowSize(screenSize.X, screenSize.Y);
                screens[i] = new Monitor(i, (int)DisplayServer.ScreenGetRefreshRate(i), (WindowResolution)size);
            }

            return screens;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="mode"></param>
        private void SetWindowMode(WindowMode mode)
        {
            bool borderless = false;
            var modeEnum = Window.ModeEnum.Windowed;
            switch (mode)
            {
                case WindowMode.Windowed:
                    borderless = false;
                    modeEnum = Window.ModeEnum.Windowed;
                    break;
                case WindowMode.BorderlessWindowed:
                    borderless = true;
                    modeEnum = Window.ModeEnum.Windowed;
                    break;
                case WindowMode.Fullscreen:
                    borderless = false;
                    modeEnum = Window.ModeEnum.Fullscreen;
                    break;
                case WindowMode.BorderlessFullscreen:
                    borderless = true;
                    modeEnum = Window.ModeEnum.Fullscreen;
                    break;
                case WindowMode.ExclusiveFullscreen:
                    borderless = true;
                    modeEnum = Window.ModeEnum.ExclusiveFullscreen;
                    break;
            }
            _window.Borderless = borderless;
            _window.Mode = modeEnum;
            _mode = mode;
        }

        /// <summary>
        ///
        /// </summary>
        private void OnFocusEntered()
        {
            _isFocused = true;
            _focusChanged.Publish(_isFocused);
        }

        /// <summary>
        ///
        /// </summary>
        private void OnFocusExited()
        {
            _isFocused = false;
            _focusChanged.Publish(_isFocused);
        }

        /// <summary>
        ///
        /// </summary>
        private void OnSizeChanged()
        {
            var size = _window.Size;
            _sizeChanged.Publish(new WindowSizeChangedEventArgs(size.X, size.Y));
        }

        /// <summary>
        ///
        /// </summary>
        private void OnCloseRequested()
        {
            _closeRequested.Publish(default);
        }

        /// <summary>
        ///
        /// </summary>
        private void OnTitleChanged()
        {
        }
    }
}
