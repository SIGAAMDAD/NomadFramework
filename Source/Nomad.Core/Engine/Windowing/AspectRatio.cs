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

using System;

namespace Nomad.Core.Engine.Windowing
{
    public enum AspectRatio : byte
    {
        Aspect_4_3,
        Aspect_16_9,
        Aspect_16_10,
        Aspect_21_9,

        Aspect_Automatic,

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
    }
}
