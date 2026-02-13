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

namespace Nomad.Core.Util.Math
{
    /// <summary>
    ///
    /// </summary>
    public struct Vector2 : IEquatable<Vector2>
    {
        /// <summary>
        /// Enumerated index values for the axes.
        /// </summary>
        public enum Axis
        {
            /// <summary>
            /// The vector's X axis.
            /// </summary>
            X,

            /// <summary>
            /// The vector's Y axis.
            /// </summary>
            Y
        }

        /// <summary>
        /// The vector's X component. Also accessible by using the index position [0].
        /// </summary>
        public float X;

        /// <summary>
        /// The vector's Y component. Also accessible by using the index position [0].
        /// </summary>
        public float Y;

        /// <summary>
        ///
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Vector2 other)
        {
            throw new NotImplementedException();
        }
    }
}
