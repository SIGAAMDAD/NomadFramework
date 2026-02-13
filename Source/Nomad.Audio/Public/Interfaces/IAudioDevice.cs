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
using System.Collections.Immutable;

namespace Nomad.Audio.Interfaces
{
    /// <summary>
    ///
    /// </summary>
    public interface IAudioDevice : IDisposable
    {
        /// <summary>
        ///
        /// </summary>
        string AudioDriver { get; }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        IImmutableList<string> GetOutputDevices();

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>s
        IImmutableList<string> GetAudioDrivers();

        /// <summary>
        ///
        /// </summary>
        /// <param name="deltaTime"></param>
        void Update(float deltaTime);

        /// <summary>
        ///
        /// </summary>
        /// <param name="assetPath"></param>
        void LoadBank(string assetPath);

        /// <summary>
        ///
        /// </summary>
        /// <param name="assetPath"></param>
        void UnloadBank(string assetPath);

        /// <summary>
        ///
        /// </summary>
        void UnloadBanks();
    }
}
