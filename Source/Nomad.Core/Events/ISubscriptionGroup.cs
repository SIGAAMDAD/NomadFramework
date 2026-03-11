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
using System.Collections.Generic;

namespace Nomad.Core.Events
{
    /// <summary>
    ///
    /// </summary>
    public interface ISubscriptionGroup : IDisposable
    {
        /// <summary>
        /// The name of the group.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///
        /// </summary>
        IReadOnlyList<ISubscriptionHandle> Subscriptions { get; }

        /// <summary>
        /// Adds a new subscription handle to the group.
        /// </summary>
        /// <param name="gameEvent"></param>
        /// <param name="callback"></param>
        void Add<TArgs>(IGameEvent<TArgs> gameEvent, EventCallback<TArgs> callback)
            where TArgs : struct;

        /// <summary>
        ///
        /// </summary>
        void UnsubscribeAll();
    }
}
