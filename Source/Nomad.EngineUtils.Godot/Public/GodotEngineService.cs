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

#if !UNITY_EDITOR
using System;
using Godot;
using Nomad.Core.Util;
using Nomad.Core.EngineUtils;
using Nomad.EngineUtils.Private;
using Nomad.Core.Logger;
using Nomad.Core.Events;
using Nomad.Core.ResourceCache;
using Nomad.Core.Input;
using Nomad.Core.Compatibility.Guards;
using Nomad.Core;

namespace Nomad.EngineUtils
{
    /// <summary>
    ///
    /// </summary>
    public sealed class GodotEngineService : IEngineService
    {
        private readonly SceneTree _sceneTree;
        private readonly Node _root;

        private readonly GodotLoader<Resource> _loader;
        private readonly GodotInputPump _inputPump;

        private readonly ILoggerService _logger;
        private readonly IGameEventRegistryService _eventFactory;

        private bool _isFocused = false;

        private bool _isDisposed = false;

        /// <summary>
        /// 
        /// </summary>
        public IGameEvent<WindowSizeChangedEventArgs> WindowSizeChanged => _windowSizeChanged;
        private readonly IGameEvent<WindowSizeChangedEventArgs> _windowSizeChanged;

        /*
        ===============
        GodotEngineService
        ===============
        */
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sceneTree"></param>
        /// <param name="inputSystem"></param>
        /// <param name="logger"></param>
        /// <param name="eventFactory"></param>
        public GodotEngineService(SceneTree sceneTree, IInputSystem inputSystem, ILoggerService logger, IGameEventRegistryService eventFactory)
        {
            ArgumentGuard.ThrowIfNull(sceneTree);
            ArgumentGuard.ThrowIfNull(inputSystem);
            ArgumentGuard.ThrowIfNull(logger);
            ArgumentGuard.ThrowIfNull(eventFactory);

            _sceneTree = sceneTree;
            _root = (Node)sceneTree.Get(SceneTree.PropertyName.Root).AsGodotObject();

            _inputPump = new GodotInputPump(inputSystem);
            _root.CallDeferred(Node.MethodName.AddChild, _inputPump);

            _loader = new GodotLoader<Resource>();

            _logger = logger;
            _eventFactory = eventFactory;

            _windowSizeChanged = eventFactory.GetEvent<WindowSizeChangedEventArgs>(Constants.Events.EngineUtils.WINDOW_SIZE_CHANGED, Constants.Events.EngineUtils.NAMESPACE);

            _root.GetWindow().SizeChanged += OnWindowSizeChanged;
            _root.GetWindow().FocusEntered += OnFocusEntered;
            _root.GetWindow().FocusExited += OnFocusExited;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (!_isDisposed)
            {
                _windowSizeChanged?.Dispose();
                _loader?.Dispose();
                _inputPump?.Dispose();
                _root?.Dispose();
            }
            GC.SuppressFinalize(this);
            _isDisposed = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ospath"></param>
        /// <returns></returns>
        public string GetLocalPath(string ospath)
        {
            return ProjectSettings.LocalizePath(ospath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="localpath"></param>
        /// <returns></returns>
        public string GetOSPath(string localpath)
        {
            return ProjectSettings.GlobalizePath(localpath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IConsoleObject CreateConsoleObject()
        {
            var console = new GodotConsole(_root, _logger, _eventFactory);
            return console;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IResourceLoader GetResourceLoader()
        {
            return _loader;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string Translate(InternString key)
        {
            return TranslationServer.Translate((string)key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsApplicationFocused()
        {
            return _isFocused;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetApplicationVersion()
        {
            return ProjectSettings.GetSetting("application/config/version").AsString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetEngineVersion()
        {
            var version = Engine.GetVersionInfo();
            return version["string"].AsString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exitCode"></param>
        public void Quit(int exitCode = 0)
        {
            OS.Kill(OS.GetProcessId());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        public string GetStoragePath(StorageScope scope) => scope switch
        {
            StorageScope.Install => ProjectSettings.GlobalizePath("res://"),
            StorageScope.StreamingAssets => ProjectSettings.GlobalizePath("res://Assets/"),
            StorageScope.UserData => ProjectSettings.GlobalizePath("user://"),
            _ => ProjectSettings.GlobalizePath("user://")
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="relativePath"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        public string GetStoragePath(string relativePath, StorageScope scope)
        {
            return $"{GetStoragePath(scope)}/{relativePath}";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetSystemRegion()
        {
            return System.Globalization.CultureInfo.CurrentCulture.Name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void GetScreenResolution(out int width, out int height)
        {
            Vector2I size = DisplayServer.WindowGetSize();
            width = size.X;
            height = size.Y;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SetScreenResolution(int width, int height)
        {
            DisplayServer.WindowSetSize(new Vector2I(width, height));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public IDisposable CreateImageRGBA(byte[] image, int width, int height)
        {
            return Image.CreateFromData(width, height, false, Image.Format.Rgba8, image);
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnWindowSizeChanged()
        {
            GetScreenResolution(out int width, out int height);
            _windowSizeChanged.Publish(new WindowSizeChangedEventArgs(width, height));
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnFocusEntered()
        {
            _isFocused = true;
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnFocusExited()
        {
            _isFocused = false;
        }
    }
}
#endif
