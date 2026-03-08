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
using System.Collections.Generic;

namespace Nomad.Core.EngineUtils
{
    /// <summary>
    ///
    /// </summary>
    public interface ISceneManager : IDisposable
    {
        /// <summary>
        ///
        /// </summary>
        IScene? ActiveScene { get; }

        /// <summary>
        ///
        /// </summary>
        IReadOnlyList<IScene> LoadedScenes { get; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="path"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        IScene LoadScene(string path, LoadSceneMode mode = LoadSceneMode.Single);

        /// <summary>
        ///
        /// </summary>
        void UnloadCurrentScene();

        /// <summary>
        ///
        /// </summary>
        void UnloadAllScenes();

        /// <summary>
        ///
        /// </summary>
        /// <param name="scene"></param>
        void UnloadScene(IScene scene);

        /// <summary>
        ///
        /// </summary>
        /// <param name="scene"></param>
        void SetActiveScene(IScene scene);
    }
}
