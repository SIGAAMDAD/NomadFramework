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

using System.Runtime.CompilerServices;
using UnityEngine;

namespace Nomad.EngineUtils
{
    /// <summary>
    ///
    /// </summary>
    public static class VectorExtensions
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ToUnity(this System.Numerics.Vector2 from)
        {
            return new Vector2(from.X, from.Y);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ToUnity(this System.Numerics.Vector3 from)
        {
            return new Vector3(from.X, from.Y, from.Z);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Numerics.Vector2 ToSystem(this Vector2 from)
        {
            return new System.Numerics.Vector2(from.x, from.y);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Numerics.Vector3 ToSystem(this Vector3 from)
        {
            return new System.Numerics.Vector3(from.x, from.y, from.z);
        }
    }
}
