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
using System.Runtime.CompilerServices;
using Nomad.Core.CVars;
using Nomad.Core.FileSystem;
using Nomad.Core.Exceptions;

namespace Nomad.CVars.Global
{
    /// <summary>
    /// 
    /// </summary>
    public static class CVarSystem
    {
        /// <summary>
        /// The global cvar system instance.
        /// </summary>
        public static ICVarSystemService Instance => _instance ?? throw new SubsystemNotInitializedException();
        private static ICVarSystemService? _instance;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <exception cref="ArgumentNullException"></exception>
        internal static void Initialize(ICVarSystemService instance)
        {
            _instance = instance ?? throw new ArgumentNullException(nameof(instance));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="createInfo"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ICVar<T> Register<T>(CVarCreateInfo<T> createInfo)
        {
            return Instance.Register(createInfo);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cvar"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Unregister(ICVar cvar)
        {
            Instance.Unregister(cvar);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CVarExists(string name)
        {
            return Instance.CVarExists(name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CVarExists<T>(string name)
        {
            return Instance.CVarExists<T>(name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ICVar<T>? GetCVar<T>(string name)
        {
            return Instance.GetCVar<T>(name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ICVar? GetCVar(string name)
        {
            return Instance.GetCVar(name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ICVar[]? GetCVars()
        {
            return Instance.GetCVars();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ICVar<T>[]? GetCVarsWithValueType<T>()
        {
            return Instance.GetCVarsWithValueType<T>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ICVar[]? GetCVarsInGroup(string groupName)
        {
            return Instance.GetCVarsInGroup(groupName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GroupExists(string groupName)
        {
            return Instance.GroupExists(groupName);
        }

        /// <summary>
        /// 
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Restart()
        {
            Instance.Restart();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cvar"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryFind(string name, out ICVar? cvar)
        {
            return Instance.TryFind(name, out cvar);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="cvar"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryFind<T>(string name, out ICVar<T>? cvar)
        {
            return Instance.TryFind(name, out cvar);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileSystem"></param>
        /// <param name="configFile"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Save(IFileSystem fileSystem, string configFile)
        {
            Instance.Save(fileSystem, configFile);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileSystem"></param>
        /// <param name="configFile"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Load(IFileSystem fileSystem, string configFile)
        {
            Instance.Load(fileSystem, configFile);
        }
    }
}