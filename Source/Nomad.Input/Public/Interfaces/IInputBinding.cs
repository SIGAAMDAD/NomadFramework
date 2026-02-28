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

using Nomad.Input.Private.ValueObjects;
using Nomad.Input.ValueObjects;

namespace Nomad.Input.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IInputBinding
    {
        /// <summary>
        /// The type of input (Keyboard, Mouse, Gamepad).
        /// </summary>
        InputDeviceType DeviceType { get; }

        /// <summary>
        /// Device‑specific identifier (e.g., KeyCode, MouseButton, GamepadButton).
        /// </summary>
        InputType Type { get; }

        /// <summary>
        /// Optional modifier (e.g., Shift, Ctrl). Null if none.
        /// </summary>
        InputModifier? Modifier { get; }

        /// <summary>
        /// For axes: which axis (X, Y) and whether inverted.
        /// </summary>
        AxisProperties? Axis { get; }

        /// <summary>
        /// Human‑readable description (e.g., "Space", "Left Mouse Button").
        /// </summary>
        string DisplayName { get; }
    }
}
