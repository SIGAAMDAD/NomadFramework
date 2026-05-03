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

namespace Nomad.Input.ValueObjects
{
    /// <summary>
    /// Defines the types of values that input actions can produce.
    /// </summary>
    public enum InputValueType : byte
    {
        /// <summary>
        /// A discrete button press/release value.
        /// </summary>
        Button,

        /// <summary>
        /// A single floating-point value (1D axis).
        /// </summary>
        Float,

        /// <summary>
        /// A 2D vector value (2D axis like joystick).
        /// </summary>
        Vector2,

        /// <summary>
        /// Sentinel value representing the total number of input value types.
        /// </summary>
        Count
    }
}
