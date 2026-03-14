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

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Nomad.Core.Compatibility.Guards;
using Nomad.Core.Exceptions;
using Nomad.Core.Engine.SceneManagement;

namespace Nomad.Core.Engine.Globals
{
    /// <summary>
    ///
    /// </summary>
    public static class SceneManager
    {
        /// <summary>
        ///
        /// </summary>
        public static ISceneManager Instance => _instance ?? throw new SubsystemNotInitializedException();
        private static ISceneManager? _instance;

        /// <summary>
        ///
        /// </summary>
        public static IScene? ActiveScene => Instance.ActiveScene;

        /// <summary>
        ///
        /// </summary>
        public static IReadOnlyList<IScene> LoadedScenes => Instance.LoadedScenes;

        /// <summary>
        ///
        /// </summary>
        /// <param name="instance"></param>
        internal static void Initialize(ISceneManager instance)
        {
            ArgumentGuard.ThrowIfNull(instance);
            _instance = instance;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="path"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IScene LoadScene(string path, LoadSceneMode mode = LoadSceneMode.Single)
        {
            return Instance.LoadScene(path, mode);
        }

        /// <summary>
        ///
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnloadCurrentScene()
        {
            Instance.UnloadCurrentScene();
        }

        /// <summary>
        ///
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnloadAllScenes()
        {
            Instance.UnloadAllScenes();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="scene"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnloadScene(IScene scene)
        {
            Instance.UnloadScene(scene);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="scene"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetActiveScene(IScene scene)
        {
            Instance.SetActiveScene(scene);
        }
    }
}
