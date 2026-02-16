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

namespace Nomad.EngineUtils
{
    /// <summary>
    ///
    /// </summary>
    public sealed class GodotEngineService : IEngineService
    {
        private readonly SceneTree _sceneTree;
        private readonly Node _root;

        private readonly string _installDirectory;
        private readonly string _assetDirectory;

        private readonly NotificationNode _notificationNode;
        private readonly GodotLoader<Resource> _loader;
        private readonly GodotInputPump _inputPump;

        private readonly ILoggerService _logger;
        private readonly IGameEventRegistryService _eventFactory;

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
            _sceneTree = sceneTree;
            _root = (Node)sceneTree.Get(SceneTree.PropertyName.Root);
            _notificationNode = new NotificationNode();

            _inputPump = new GodotInputPump(inputSystem);
            _root.CallDeferred(Node.MethodName.AddChild, _inputPump);

            _loader = new GodotLoader<Resource>();

            _logger = logger;
            _eventFactory = eventFactory;
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

        public bool IsApplicationFocused()
        {
            throw new NotImplementedException();
        }

        public bool IsApplicationPaused()
        {
            throw new NotImplementedException();
        }

        public string GetApplicationVersion()
        {
            throw new NotImplementedException();
        }

        public string GetEngineVersion()
        {
            throw new NotImplementedException();
        }

        public void Quit(int exitCode = 0)
        {
            OS.Kill(OS.GetProcessId());
        }

        public string GetStoragePath(StorageScope scope) => scope switch
        {
            StorageScope.Install => ProjectSettings.GlobalizePath("res://"),
            StorageScope.StreamingAssets => ProjectSettings.GlobalizePath("res://Assets/"),
            StorageScope.UserData => ProjectSettings.GlobalizePath("user://"),
            _ => ProjectSettings.GlobalizePath("user://")
        };

        public string GetStoragePath(string relativePath, StorageScope scope)
        {
            throw new NotImplementedException();
        }

        public string GetSystemRegion()
        {
            return OS.GetLocale();
        }

        public void GetScreenResolution(out int width, out int height)
        {
            Vector2I size = DisplayServer.WindowGetSize();
            width = size.X;
            height = size.Y;
        }

        public void SetScreenResolution(int width, int height)
        {
            DisplayServer.WindowSetSize(new Vector2I(width, height));
        }

        public IDisposable CreateImageRGBA(byte[] image, int width, int height)
        {
            return Image.CreateFromData(width, height, false, Image.Format.Rgba8, image);
        }
    }
}
#endif
