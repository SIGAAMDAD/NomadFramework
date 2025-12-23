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

using Godot;
using System;

namespace Nomad.Core.Events
{
    /// <summary>
    ///
    /// </summary>
    public interface IGodotEventBusService : IDisposable
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="source"></param>
        /// <param name="signalName"></param>
        /// <param name="target"></param>
        /// <param name="method"></param>
        void ConnectSignal(GodotObject source, StringName signalName, GodotObject target, Action method);

        /// <summary>
        ///
        /// </summary>
        /// <param name="source"></param>
        /// <param name="signalName"></param>
        /// <param name="target"></param>
        /// <param name="method"></param>
        void ConnectSignal(GodotObject source, StringName signalName, GodotObject target, Callable method);

        /// <summary>
        ///
        /// </summary>
        /// <param name="source"></param>
        /// <param name="signalName"></param>
        void DisconnectSignal(GodotObject source, StringName signalName);

        /// <summary>
        ///
        /// </summary>
        /// <param name="source"></param>
        void CleanupSubscriber(GodotObject source);
    }
}
