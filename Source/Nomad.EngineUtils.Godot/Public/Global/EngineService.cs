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

using System.Runtime.CompilerServices;
using Nomad.Core.Compatibility.Guards;
using Nomad.Core.EngineUtils;
using Nomad.Core.Exceptions;
using Nomad.Core.ResourceCache;
using Nomad.Core.Util;

namespace Nomad.EngineUtils.Globals
{
    /// <summary>
    /// 
    /// </summary>
    public static class EngineService
    {
        private static GodotEngineService Instance => _instance ?? throw new SubsystemNotInitializedException();
        private static GodotEngineService? _instance;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        internal static void Initialize(GodotEngineService instance)
        {
            ArgumentGuard.ThrowIfNull(instance);
            _instance = instance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IResourceLoader GetResourceLoader()
        {
            return Instance.GetResourceLoader();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IConsoleObject CreateConsoleObject()
        {
            return Instance.CreateConsoleObject();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetApplicationVersion()
        {
            return Instance.GetApplicationVersion();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetEngineVersion()
        {
            return Instance.GetEngineVersion();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ospath"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetLocalPath(string ospath)
        {
            return Instance.GetLocalPath(ospath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="localpath"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetOSPath(string localpath)
        {
            return Instance.GetOSPath(localpath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetScreenResolution(out int width, out int height)
        {
            Instance.GetScreenResolution(out width, out height);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetStoragePath(StorageScope scope)
        {
            return Instance.GetStoragePath(scope);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="relativePath"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetStoragePath(string relativePath, StorageScope scope)
        {
            return Instance.GetStoragePath(relativePath, scope);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetSystemRegion()
        {
            return Instance.GetSystemRegion();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exitCode"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Quit(int exitCode = 0)
        {
            Instance.Quit(exitCode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetScreenResolution(int width, int height)
        {
            Instance.SetScreenResolution(width, height);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Translate(InternString key)
        {
            return Instance.Translate(key);
        }
    }
}
