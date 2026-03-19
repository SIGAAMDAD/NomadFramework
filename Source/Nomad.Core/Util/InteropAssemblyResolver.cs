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

#if NET5_0_OR_GREATER
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
        private static readonly Dictionary<Assembly, Dictionary<string, (string, string)>> _hooks = new();

        /// <summary>
        ///
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="libraryName"></param>
        /// <param name="libraryNameLinux"></param>
        /// <param name="libraryNameWindows"></param>
        public static void Hook(Assembly assembly, string libraryName, string libraryNameLinux, string libraryNameWindows)
        {
            if (!_hooks.TryGetValue(assembly, out var resolvers))
            {
                resolvers = new Dictionary<string, (string, string)>();
                _hooks[assembly] = resolvers;
                NativeLibrary.SetDllImportResolver(assembly, Resolve);
            }
            resolvers.Add(libraryName, (libraryNameLinux, libraryNameWindows));
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
            if (!_hooks.TryGetValue(assembly, out var dict))
            {
                return IntPtr.Zero;
            }
            if (!dict.TryGetValue(libraryName, out var library))
            {
                return IntPtr.Zero;
            }

            string fileName =
                OperatingSystem.IsWindows() ? $"{library.Item2}.dll" :
                OperatingSystem.IsLinux() ? $"{library.Item1}.so" :
                OperatingSystem.IsMacOS() ? $"{library.Item1}.dylib" :
                throw new PlatformNotSupportedException();

            string fullPath = Path.Combine(AppContext.BaseDirectory, fileName);
            if (File.Exists(fullPath))
            {
                return GetLibrary(fullPath);
            }

            fullPath = Path.Combine(Environment.CurrentDirectory, fileName);
            return File.Exists(fullPath) ? GetLibrary(fullPath) : IntPtr.Zero;
        }

        private static IntPtr GetLibrary(string path)
        {
            return NativeLibrary.Load(path);
        }
    }
}
#endif
