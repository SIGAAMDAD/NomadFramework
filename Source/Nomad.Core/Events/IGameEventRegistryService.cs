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

namespace Nomad.Core.Events
{
    /// <summary>
    ///
    /// </summary>
    public interface IGameEventRegistryService : IDisposable
    {
        /// <summary>
        /// Disposes of all events found in provided namespace.
        /// </summary>
        /// <param name="nameSpace"></param>
        void ClearEventsInNamespace(string nameSpace);

        /// <summary>
        /// Creates a new <see cref="IGameEvent"/> with the provided namespace and id.
        /// </summary>
        /// <typeparam name="TArgs"></typeparam>
        /// <param name="nameSpace"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        IGameEvent<TArgs> GetEvent<TArgs>(string nameSpace, string name)
            where TArgs : struct;
    }
}
