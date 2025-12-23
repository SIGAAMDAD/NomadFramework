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

namespace Nomad.Audio.Interfaces
{
    /// <summary>
    ///
    /// </summary>
    public interface IListenerService : IDisposable
    {
        /// <summary>
        /// The current number of active listeners
        /// </summary>
        int ListenerCount { get; }

        /// <summary>
        ///
        /// </summary>
        IListener? ActiveListener { get; }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        IListener AddListener();

        /// <summary>
        ///
        /// </summary>
        /// <param name="listener"></param>
        /// <returns></returns>
        IListener SetActiveListener(IListener listener);

        /// <summary>
        ///
        /// </summary>
        void ClearListeners();
    }
}
