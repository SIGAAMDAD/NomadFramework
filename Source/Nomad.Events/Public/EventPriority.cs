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

namespace Nomad.Events
{
    /// <summary>
    /// Defines the priority levels for event handling.
    /// </summary>
    public enum EventPriority : byte
    {
        /// <summary>
        /// Very low priority.
        /// </summary>
        VeryLow,

        /// <summary>
        /// Low priority.
        /// </summary>
        Low,

        /// <summary>
        /// Normal priority.
        /// </summary>
        Normal,

        /// <summary>
        /// High priority.
        /// </summary>
        High,

        /// <summary>
        /// Very high priority.
        /// </summary>
        VeryHigh,

        /// <summary>
        /// Critical priority.
        /// </summary>
        Critical,

        /// <summary>
        /// Number of priority levels.
        /// </summary>
        Count
    }
}