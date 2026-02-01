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
using Nomad.Core.Util;
using Nomad.Core.EngineUtils;
using Nomad.EngineUtils.Private;

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

        public GodotEngineService(SceneTree sceneTree)
        {
            _sceneTree = sceneTree;
            _root = (Node)sceneTree.Get(SceneTree.PropertyName.Root);
            _notificationNode = new NotificationNode();
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
