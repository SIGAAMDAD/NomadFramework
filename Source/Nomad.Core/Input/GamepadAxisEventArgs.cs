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
using System.Numerics;
using System.Runtime.CompilerServices;
using Nomad.Core.Events;

namespace Nomad.Core.Input
{
    /// <summary>
    /// Represents the arguments for a gamepad axis input event.
    /// </summary>
    [Event(
        name: nameof(GamepadAxisEventArgs),
        nameSpace: "Nomad.Core.Input"
    )]
    public readonly partial struct GamepadAxisEventArgs : IEquatable<GamepadAxisEventArgs>
    {
        /// <summary>
        /// Gets the timestamp when the gamepad axis event occurred.
        /// </summary>
        public long TimeStamp { get; }

        /// <summary>
        /// Gets the device ID of the gamepad.
        /// </summary>
        public int DeviceId { get; }

        /// <summary>
        /// Gets the gamepad stick that was moved.
        /// </summary>
        public GamepadStick Stick { get; }

        /// <summary>
        /// Gets the axis values as a 2D vector (X and Y components).
        /// </summary>
        public Vector2 Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GamepadAxisEventArgs"/> struct.
        /// </summary>
        /// <param name="stick">The gamepad stick that was moved.</param>
        /// <param name="timestamp">The timestamp of the event.</param>
        /// <param name="deviceId">The device ID of the gamepad.</param>
        /// <param name="value">The axis values as a 2D vector.</param>
        public GamepadAxisEventArgs(GamepadStick stick, long timestamp, int deviceId, Vector2 value)
        {
            Stick = stick;
            TimeStamp = timestamp;
            DeviceId = deviceId;
            Value = value;
        }

        /// <summary>
        /// Determines whether the specified <see cref="GamepadAxisEventArgs"/> is equal to the current instance.
        /// </summary>
        /// <param name="other">The <see cref="GamepadAxisEventArgs"/> to compare with the current instance.</param>
        /// <returns>True if the specified object is equal to the current instance; otherwise, false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(GamepadAxisEventArgs other)
        {
            return Stick == other.Stick && Value == other.Value;
        }
    }
}
