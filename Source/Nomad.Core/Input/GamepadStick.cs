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
    /// Represents the sticks (analog controls) on a gamepad controller.
    /// </summary>
    public enum GamepadStick : byte
    {
        /// <summary>
        /// The left analog stick.
        /// </summary>
        Left,

        /// <summary>
        /// The right analog stick.
        /// </summary>
        Right,
    }
}