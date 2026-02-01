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
using Nomad.Core.Util;

namespace Nomad.Core.EngineUtils
{
    /// <summary>
    ///
    /// </summary>
    public interface IEngineService
    {
        /*
        /// <summary>
        /// Check if the application is currently focused.
        /// </summary>
        /// <returns></returns>
        bool IsApplicationFocused();

        /// <summary>
        /// Check if the application is currently paused.
        /// </summary>
        /// <returns></returns>
        bool IsApplicationPaused();
        */

        /// <summary>
        /// Gets the application version string.
        /// </summary>
        /// <returns></returns>
        string GetApplicationVersion();

        /// <summary>
        /// Gets the engine version string.
        /// </summary>
        /// <returns></returns>
        string GetEngineVersion();

        /// <summary>
        /// Gracefully quit the application.
        /// </summary>
        /// <param name="exitCode">Optional exit code for the process.</param>
        void Quit(int exitCode = 0);

        /// <summary>
        /// Get a platform-specific path for the given storage scope
        /// </summary>
        /// <param name="scope">The type of storage location requested</param>
        /// <returns>Absolute path to the directory</returns>
        string GetStoragePath(StorageScope scope);

        /// <summary>
        /// Get a path relative to a specific storage scope
        /// </summary>
        /// <param name="relativePath">Path relative to the scope root</param>
        /// <param name="scope">Storage scope to resolve from</param>
        /// <returns>Absolute combined path</returns>
        string GetStoragePath(string relativePath, StorageScope scope);

        /// <summary>
        /// Get system region/country.
        /// </summary>
        /// <returns></returns>
        string GetSystemRegion();

        /// <summary>
        /// Gets the native/desktop resolution.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        void GetScreenResolution(out int width, out int height);

        /// <summary>
        ///
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        void SetScreenResolution(int width, int height);

        IDisposable CreateImageRGBA(byte[] image, int width, int height);

        string Translate(InternString key);
    }
}
