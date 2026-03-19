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
    public static class ColorExtensions
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color ToUnity(this System.Drawing.Color color)
        {
            return new Color32(color.R, color.G, color.B, color.A);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Drawing.Color ToSystem(this Color color)
        {
            Color32 value = color;
            return System.Drawing.Color.FromArgb(value.a, value.r, value.g, value.b);
        }
    }
}
