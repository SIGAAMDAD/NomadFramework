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

namespace Nomad.Events
{
    /// <summary>
    /// Dictates how a <see cref="IGameEvent"/> behaves.
    /// </summary>
    [Flags]
    public enum EventFlags : uint
    {
        /// <summary>
        /// Notifies all synchronous subscribers.
        /// </summary>
        Synchronous = 1 << 0,

        /// <summary>
        /// Notifies all asynchronous subscribers.
        /// </summary>
        Asynchronous = 1 << 1,

        /// <summary>
        /// Don't use a WeakReference when allocating a new subscription.
        /// </summary>
        StrongSubscriptions = 1 << 2,

        /// <summary>
        /// Don't do any synchronization when publishing the event. This also disables asynchronous subscriptions.
        /// </summary>
        NoLock = 1 << 3,

        Default = Synchronous | Asynchronous
    };
};