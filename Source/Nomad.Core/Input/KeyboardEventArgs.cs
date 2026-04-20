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
using Nomad.Core.Input.ValueObjects;

namespace Nomad.Core.Input
{
    /// <summary>
    /// Represents the arguments for a keyboard input event.
    /// </summary>
    public readonly struct KeyboardEventArgs : IEquatable<KeyboardEventArgs>
    {
        /// <summary>
        /// Gets the timestamp when the keyboard event occurred.
        /// </summary>
        public long TimeStamp { get; }

        /// <summary>
        /// Gets the key that was pressed or released.
        /// </summary>
        public KeyNum KeyNum { get; }

        /// <summary>
        /// Gets a value indicating whether the key was pressed (true) or released (false).
        /// </summary>
        public bool Pressed { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyboardEventArgs"/> struct.
        /// </summary>
        /// <param name="keyNum">The key that was pressed or released.</param>
        /// <param name="timeStamp">The timestamp of the event.</param>
        /// <param name="pressed">True if the key was pressed, false if released.</param>
        public KeyboardEventArgs(KeyNum keyNum, long timeStamp, bool pressed)
        {
            KeyNum = keyNum;
            TimeStamp = timeStamp;
            Pressed = pressed;
        }

        /// <summary>
        /// Determines whether the specified <see cref="KeyboardEventArgs"/> is equal to the current instance.
        /// </summary>
        /// <param name="other">The <see cref="KeyboardEventArgs"/> to compare with the current instance.</param>
        /// <returns>True if the specified object is equal to the current instance; otherwise, false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(KeyboardEventArgs other)
        {
            return KeyNum == other.KeyNum && Pressed == other.Pressed;
        }
    }
}
