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

namespace Nomad.Core.Engine.Rendering
{
    /// <summary>
    ///
    /// </summary>
    public enum VSyncMode : byte
    {
        /// <summary>
        /// 
        /// </summary>
        Enabled,

        /// <summary>
        /// 
        /// </summary>
        Disabled,

        /// <summary>
        /// 
        /// </summary>
        Adaptive,

        /// <summary>
        /// 
        /// </summary>
        TripleBuffered,

        /// <summary>
        /// 
        /// </summary>
        Count
    }

    /// <summary>
    /// 
    /// </summary>
    public static class VSyncModeExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vsyncMode"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static string ToDisplayString(this VSyncMode vsyncMode) => vsyncMode switch
        {
            VSyncMode.Disabled => "Off",
            VSyncMode.Enabled => "On",
            VSyncMode.Adaptive => "Adaptive",
            VSyncMode.TripleBuffered => "Triple Buffered",
            _ => throw new ArgumentOutOfRangeException(nameof(vsyncMode))
        };
    }
}
