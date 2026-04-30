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
using Nomad.Core.Events;

namespace Nomad.Core.Input
{
    /// <summary>
    /// Represents the arguments for a mouse motion input event.
    /// </summary>
    [Event(
        name: nameof(MouseMotionEventArgs),
        nameSpace: "Nomad.Core.Input"
    )]
    public readonly partial struct MouseMotionEventArgs : IEquatable<MouseMotionEventArgs>
    {
        /// <summary>
        /// Gets the timestamp when the mouse motion event occurred.
        /// </summary>
        public long TimeStamp { get; }

        /// <summary>
        /// Gets the relative X movement of the mouse.
        /// </summary>
        public int RelativeX { get; }

        /// <summary>
        /// Gets the relative Y movement of the mouse.
        /// </summary>
        public int RelativeY { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MouseMotionEventArgs"/> struct.
        /// </summary>
        /// <param name="timestamp">The timestamp of the event.</param>
        /// <param name="relativeX">The relative X movement.</param>
        /// <param name="relativeY">The relative Y movement.</param>
        public MouseMotionEventArgs(long timestamp, int relativeX, int relativeY)
        {
            TimeStamp = timestamp;
            RelativeX = relativeX;
            RelativeY = relativeY;
        }

        /// <summary>
        /// Determines whether the specified <see cref="MouseMotionEventArgs"/> is equal to the current instance.
        /// </summary>
        /// <param name="other">The <see cref="MouseMotionEventArgs"/> to compare with the current instance.</param>
        /// <returns>True if the specified object is equal to the current instance; otherwise, false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(MouseMotionEventArgs other)
        {
            return RelativeX == other.RelativeX && RelativeY == other.RelativeY;
        }
    }
}
