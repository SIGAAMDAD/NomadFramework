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
    /// Defines the different input schemes supported by the input system.
    /// </summary>
    public enum InputScheme : byte
    {
        /// <summary>
        /// Input scheme using keyboard and mouse.
        /// </summary>
        KeyboardAndMouse,

        /// <summary>
        /// Input scheme using a gamepad controller.
        /// </summary>
        Gamepad,

        /// <summary>
        /// Input scheme using touch input (for mobile devices).
        /// </summary>
        Touch,

        /// <summary>
        /// Sentinel value representing the total number of input schemes.
        /// </summary>
        Count
    }
}
