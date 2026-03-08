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
using Godot;
using Nomad.Core.Compatibility.Guards;
using Nomad.Core.CVars;
using Nomad.Core.EngineUtils;
using Nomad.Core.Events;
using Nomad.Core.Input;
using Nomad.Core.Logger;
using Nomad.Core.ResourceCache;
using Nomad.EngineUtils.Globals;
using Nomad.EngineUtils.Private;
using Nomad.ResourceCache;

namespace Nomad.EngineUtils
{
    /// <summary>
    ///
    /// </summary>
    public sealed class GodotEngineService : IEngineService
    {
        private readonly SceneTree _sceneTree;
        private readonly Node _root;

        private readonly GodotLoader _loader;
        private readonly GodotInputPump _inputPump;

        private readonly IWindowService _windowService;
        private readonly ILocalizationService _localizationService;
        private readonly IRenderingService _renderingService;
        private readonly ISceneManager _sceneManager;

        private readonly ILoggerService _logger;
        private readonly IGameEventRegistryService _eventFactory;

        private bool _isDisposed = false;

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
        /// <param name="cvarSystem"></param>
        /// <param name="eventFactory"></param>
        public GodotEngineService(SceneTree sceneTree, IInputSystem inputSystem, ILoggerService logger, ICVarSystemService cvarSystem, IGameEventRegistryService eventFactory)
        {
            ArgumentGuard.ThrowIfNull(sceneTree);
            ArgumentGuard.ThrowIfNull(inputSystem);
            ArgumentGuard.ThrowIfNull(logger);
            ArgumentGuard.ThrowIfNull(eventFactory);

            _sceneTree = sceneTree;
            _root = (Node)sceneTree.Get(SceneTree.PropertyName.Root).AsGodotObject();

            //_inputPump = new GodotInputPump(inputSystem);
            //_root.CallDeferred(Node.MethodName.AddChild, _inputPump);

            _loader = new GodotLoader();

            _logger = logger;
            _eventFactory = eventFactory;

            _windowService = new GodotWindowService(_sceneTree, eventFactory);
            _localizationService = new GodotLocalizationService();
            _renderingService = new GodotRenderService(_root.GetViewport().GetViewportRid(), _windowService, cvarSystem);
            _sceneManager = new GodotSceneManager(_sceneTree, new BaseCache<PackedScene, string>(logger, eventFactory, _loader));

            EngineService.Initialize(this);
        }

        /// <summary>
        ///
        /// </summary>
        public void Dispose()
        {
            if (!_isDisposed)
            {
                _windowService?.Dispose();
                _inputPump?.Dispose();
                _sceneManager?.Dispose();
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
        /// <returns></returns>
        public bool IsApplicationFocused()
        {
            return _windowService.IsFocused;
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
        /// <param name="image"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public IDisposable CreateImageRGBA(byte[] image, int width, int height)
        {
            return Image.CreateFromData(width, height, false, Image.Format.Rgba8, image);
        }
    }
}
