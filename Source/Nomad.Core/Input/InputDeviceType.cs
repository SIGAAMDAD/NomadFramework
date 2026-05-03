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
    /// Defines the types of input devices supported by the input system.
    /// </summary>
    public enum InputDeviceType : byte
    {
        /// <summary>
        /// Input coming from a keyboard, virtual or physical.
        /// </summary>
        Keyboard,

        /// <summary>
        /// Input coming from a mouse.
        /// </summary>
        Mouse,

        /// <summary>
        /// Input coming from a gamepad/joystick/game controller.
        /// </summary>
        Gamepad,

        /// <summary>
        /// Sentinel value representing the total number of input device types.
        /// </summary>
        Count
    }
}
