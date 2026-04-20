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

namespace Nomad.Core.Input
{
    /// <summary>
    /// Defines the mouse buttons supported by the input system.
    /// </summary>
    public enum MouseButton : byte
    {
        /// <summary>
        /// No mouse button.
        /// </summary>
        None = 0,

        /// <summary>
        /// The left mouse button.
        /// </summary>
        Left = 1 << 0,

        /// <summary>
        /// The right mouse button.
        /// </summary>
        Right = 1 << 1,

        /// <summary>
        /// The middle mouse button (wheel click).
        /// </summary>
        Middle = 1 << 2,

        /// <summary>
        /// Mouse wheel scroll down.
        /// </summary>
        WheelDown = 1 << 3,

        /// <summary>
        /// Mouse wheel scroll up.
        /// </summary>
        WheelUp = 1 << 4,

        /// <summary>
        /// The first extra mouse button (X1).
        /// </summary>
        X1 = 1 << 5,

        /// <summary>
        /// The second extra mouse button (X2).
        /// </summary>
        X2 = 1 << 6
    }
}
