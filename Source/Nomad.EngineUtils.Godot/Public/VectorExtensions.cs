/*
===========================================================================
The Nomad Framework
Copyright (C) 2025 Noah Van Til

This Source Code Form is subject to the terms of the Mozilla Public
License, v2. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.

This software is provided "as is", without warranty of any kind,
express or implied, including but not limited to the warranties
of merchantability, fitness for a particular purpose and noninfringement.
===========================================================================
*/

using System.Runtime.CompilerServices;

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
        public static Godot.Vector2 ToGodot(this System.Numerics.Vector2 from)
        {
            return new Godot.Vector2(from.X, from.Y);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Godot.Vector3 ToGodot(this System.Numerics.Vector3 from)
        {
            return new Godot.Vector3(from.X, from.Y, from.Z);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Numerics.Vector2 ToSystem(this Godot.Vector2 from)
        {
            return new System.Numerics.Vector2(from.X, from.Y);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Numerics.Vector3 ToSystem(this Godot.Vector3 from)
        {
            return new System.Numerics.Vector3(from.X, from.Y, from.Z);
        }
    }
}
