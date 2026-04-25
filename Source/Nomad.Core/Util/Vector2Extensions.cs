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

namespace Nomad.Core.Util
{
    /// <summary>
    /// Provides extension methods for <see cref="Vector2"/> operations.
    /// </summary>
    public static class Vector2Extensions
    {
        /// <summary>
        /// Calculates the Euclidean distance between two vectors.
        /// </summary>
        /// <remarks>
        /// This method computes the straight-line distance between two points in 2D space.
        /// This is an extension method for convenience and is marked with <see cref="MethodImplOptions.AggressiveInlining"/> for performance.
        /// </remarks>
        /// <param name="from">The starting vector.</param>
        /// <param name="to">The ending vector.</param>
        /// <returns>The Euclidean distance between the two vectors.</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static float DistanceTo(this Vector2 from, Vector2 to)
        {
            return (float)Math.Sqrt((from.X - to.X) * (from.X - to.X) + (from.Y - to.Y) * (from.Y - to.Y));
        }
    }
}
