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

namespace Nomad.Core.Engine.Windowing
{
    /// <summary>
    /// 
    /// </summary>
    public enum AspectRatio : byte
    {
        /// <summary>
        /// 
        /// </summary>
        Aspect_4_3,

        /// <summary>
        /// 
        /// </summary>
        Aspect_16_9,

        /// <summary>
        /// 
        /// </summary>
        Aspect_16_10,

        /// <summary>
        /// 
        /// </summary>
        Aspect_21_9,

        /// <summary>
        /// 
        /// </summary>
        Aspect_Automatic,

        /// <summary>
        /// 
        /// </summary>
        Count
    }

    /// <summary>
    ///
    /// </summary>
    public static class AspectRatioExtensions
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="aspectRatio"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static float GetRatio(this AspectRatio aspectRatio) => aspectRatio switch
        {
            AspectRatio.Aspect_Automatic => 1.0f,
            AspectRatio.Aspect_4_3 => 4.0f / 3.0f,
            AspectRatio.Aspect_16_10 => 16.0f / 10.0f,
            AspectRatio.Aspect_16_9 => 16.0f / 9.0f,
            AspectRatio.Aspect_21_9 => 21.0f / 9.0f,
            _ => throw new ArgumentOutOfRangeException(nameof(aspectRatio))
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aspectRatio"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static string ToDisplayString(this AspectRatio aspectRatio) => aspectRatio switch
        {
            AspectRatio.Aspect_Automatic => "Automatic",
            AspectRatio.Aspect_4_3 => "4:3",
            AspectRatio.Aspect_16_10 => "16:10",
            AspectRatio.Aspect_16_9 => "16:9",
            AspectRatio.Aspect_21_9 => "21:9",
            _ => throw new ArgumentOutOfRangeException(nameof(aspectRatio))
        };
    }
}
