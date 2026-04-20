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
using System.Runtime.CompilerServices;

namespace Nomad.Core.Input
{
    /// <summary>
    /// Represents the arguments for a gamepad button input event.
    /// </summary>
    public readonly struct GamepadButtonEventArgs : IEquatable<GamepadButtonEventArgs>
    {
        /// <summary>
        /// Gets the timestamp when the gamepad button event occurred.
        /// </summary>
        public long TimeStamp { get; }

        /// <summary>
        /// Gets the device ID of the gamepad.
        /// </summary>
        public int DeviceId { get; }

        /// <summary>
        /// Gets the gamepad button that was pressed or released.
        /// </summary>
        public GamepadButton Button { get; }

        /// <summary>
        /// Gets a value indicating whether the button was pressed (true) or released (false).
        /// </summary>
        public bool Pressed { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GamepadButtonEventArgs"/> struct.
        /// </summary>
        /// <param name="button">The gamepad button that was pressed or released.</param>
        /// <param name="deviceId">The device ID of the gamepad.</param>
        /// <param name="timestamp">The timestamp of the event.</param>
        /// <param name="pressed">True if the button was pressed, false if released.</param>
        public GamepadButtonEventArgs(GamepadButton button, int deviceId, long timestamp, bool pressed)
        {
            TimeStamp = timestamp;
            DeviceId = deviceId;
            Button = button;
            Pressed = pressed;
        }

        /// <summary>
        /// Determines whether the specified <see cref="GamepadButtonEventArgs"/> is equal to the current instance.
        /// </summary>
        /// <param name="other">The <see cref="GamepadButtonEventArgs"/> to compare with the current instance.</param>
        /// <returns>True if the specified object is equal to the current instance; otherwise, false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(GamepadButtonEventArgs other)
        {
            return Button == other.Button && Pressed == other.Pressed;
        }
    }
}