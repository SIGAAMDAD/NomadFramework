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
    /// Represents the arguments for a mouse position changed event.
    /// </summary>
    public readonly partial struct MousePositionChangedEventArgs : IEquatable<MousePositionChangedEventArgs>
    {
        /// <summary>
        /// Determines whether the specified <see cref="MousePositionChangedEventArgs"/> is equal to the current instance.
        /// </summary>
        /// <param name="other">The <see cref="MousePositionChangedEventArgs"/> to compare with the current instance.</param>
        /// <returns>True if the specified object is equal to the current instance; otherwise, false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(MousePositionChangedEventArgs other)
        {
            return PositionX == other.PositionX && PositionY == other.PositionY;
        }
    }
}
