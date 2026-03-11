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
using System.Reflection;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.IO;

namespace Nomad.Core.Util
{
    /// <summary>
    ///
    /// </summary>
    public static class InteropAssemblyResolver
    {
        private static readonly Dictionary<Assembly, (string, string)> _hooks = new();

        /// <summary>
        ///
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="libraryNameLinux"></param>
        /// <param name="libraryNameWindows"></param>
        public static void Hook(Assembly assembly, string libraryNameLinux, string libraryNameWindows)
        {
            if (_hooks.ContainsKey(assembly))
            {
                return;
            }
            _hooks[assembly] = (libraryNameLinux, libraryNameWindows);
            NativeLibrary.SetDllImportResolver(assembly, Resolve);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="libraryName"></param>
        /// <param name="assembly"></param>
        /// <param name="searchPath"></param>
        /// <returns></returns>
        private static IntPtr Resolve(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
        {
            if (!_hooks.TryGetValue(assembly, out var value))
            {
                return IntPtr.Zero;
            }

            string fileName =
                OperatingSystem.IsWindows() ? $"{value.Item2}.dll" :
                OperatingSystem.IsLinux() ? $"{value.Item1}.so" :
                OperatingSystem.IsMacOS() ? $"{value.Item1}.dylib" :
                throw new PlatformNotSupportedException();

            string fullPath = Path.Combine(AppContext.BaseDirectory, fileName);
            if (File.Exists(fullPath))
            {
                return NativeLibrary.Load(fullPath);
            }

            fullPath = Path.Combine(Environment.CurrentDirectory, fileName);
            return File.Exists(fullPath) ? NativeLibrary.Load(fullPath) : IntPtr.Zero;
        }
    }
}
