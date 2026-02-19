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

#if UNITY_EDITOR
using System;
using Nomad.Core.EngineUtils;
using Nomad.Core.ResourceCache;
using Nomad.Core.Util;
using UnityEngine;

namespace Nomad.EngineUtils
{
    /// <summary>
    ///
    /// </summary>
    public sealed class UnityEngineService : IEngineService
    {
        private readonly UnityLoader _loader;

        public UnityEngineService()
        {
            _loader = new UnityLoader();
        }

        public IResourceLoader GetResourceLoader()
        {
            return _loader;
        }

        public IConsoleObject CreateConsoleObject()
        {
            throw new NotImplementedException();
        }

        public IDisposable CreateImageRGBA(byte[] image, int width, int height)
        {
            throw new NotImplementedException();
        }

        public string GetApplicationVersion()
        {
            return Application.version;
        }

        public string GetEngineVersion()
        {
            return Application.unityVersion;
        }

        public string GetLocalPath(string ospath)
        {
            throw new NotImplementedException();
        }

        public string GetOSPath(string localpath)
        {
            throw new NotImplementedException();
        }

        public void GetScreenResolution(out int width, out int height)
        {
            width = Display.main.renderingWidth;
            height = Display.main.renderingHeight;
        }

        public string GetStoragePath(StorageScope scope) => scope switch
        {
            StorageScope.StreamingAssets => Application.dataPath,
            StorageScope.UserData => Application.persistentDataPath,
            StorageScope.Install => Application.dataPath,
            StorageScope.Documents => Application.persistentDataPath,
            StorageScope.Temporary => Application.persistentDataPath,
            _ => throw new ArgumentOutOfRangeException(nameof(scope))
        };

        public string GetStoragePath(string relativePath, StorageScope scope)
            => $"{GetStoragePath(scope)}/{relativePath}";

        public string GetSystemRegion()
        {
            return String.Empty;
        }

        public void Quit(int exitCode = 0)
        {
            Application.Quit(exitCode);
        }

        public void SetScreenResolution(int width, int height)
        {
            Display.main.SetRenderingResolution(width, height);
        }

        public string Translate(InternString key)
        {
            return String.Empty;
        }
    }
}
#endif
