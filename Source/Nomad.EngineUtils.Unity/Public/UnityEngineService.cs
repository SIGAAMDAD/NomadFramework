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
using Nomad.Core.EngineUtils;
using Nomad.Core.Util;

namespace Nomad.EngineUtils
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class UnityEngineService : IEngineService
    {
        public IDisposable CreateImageRGBA(byte[] image, int width, int height)
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
            throw new NotImplementedException();
        }

        public string GetStoragePath(StorageScope scope) => scope switch
        {
            StorageScope.StreamingAssets => Application.dataPath,
            StorageScope.UserData => Application.persistentDataPath
        };

        public string GetStoragePath(string relativePath, StorageScope scope)
        {
            throw new NotImplementedException();
        }

        public string GetSystemRegion()
        {
            throw new NotImplementedException();
        }

        public void Quit(int exitCode = 0)
        {
            Application.Quit(exitCode);
            throw new NotImplementedException();
        }

        public void SetScreenResolution(int width, int height)
        {
            throw new NotImplementedException();
        }

        public string Translate(InternString key)
        {
            throw new NotImplementedException();
        }
    }
}