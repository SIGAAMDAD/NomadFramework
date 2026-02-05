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

namespace Nomad.Core.Events
{
    /// <summary>
    /// Represents an event callback with arguments.
    /// </summary>
    /// <typeparam name="TArgs">The type of arguments for the event.</typeparam>
    public readonly struct GameEventArgs<TArgs>
        where TArgs : struct
    {
        /// <summary>
        /// The data for the event.
        /// </summary>
        public TArgs Data { get; }

        /// <summary>
        /// The timestamp of when the event was created.
        /// </summary>
        public DateTime Timestamp { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameEventArgs{TArgs}"/> struct.
        /// </summary>
        /// <param name="data">The data for the event.</param>
        public GameEventArgs(in TArgs data)
        {
            Data = data;
            Timestamp = DateTime.UtcNow;
        }
    }
}
