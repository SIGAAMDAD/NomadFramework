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
    /// Defines the different types of input sources supported by the input system.
    /// </summary>
    public enum InputType : byte
    {
        /// <summary>
        /// Input from a keyboard key press/release.
        /// </summary>
        Keyboard,

        /// <summary>
        /// Input from mouse movement (relative motion).
        /// </summary>
        MouseMotion,

        /// <summary>
        /// Input from mouse button press/release.
        /// </summary>
        MouseButton,

        /// <summary>
        /// Input from gamepad button press/release.
        /// </summary>
        GamepadButton,

        /// <summary>
        /// Input from gamepad analog stick/axis movement.
        /// </summary>
        GamepadAxis,

        /// <summary>
        /// Sentinel value representing the total number of input types.
        /// </summary>
        Count
    }
}
