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

namespace Nomad.Core.Input
{
    /// <summary>
    /// 
    /// </summary>
    public enum MouseButton : byte
    {
        /// <summary>
        /// 
        /// </summary>
        None = 0,

        /// <summary>
        /// 
        /// </summary>
        Left = 1 << 0,

        /// <summary>
        /// 
        /// </summary>
        Right = 1 << 1,

        /// <summary>
        /// 
        /// </summary>
        Middle = 1 << 2,

        /// <summary>
        /// 
        /// </summary>
        WheelDown = 1 << 3,

        /// <summary>
        /// 
        /// </summary>
        WheelUp = 1 << 4,

        /// <summary>
        /// 
        /// </summary>
        X1 = 1 << 5,

        /// <summary>
        /// 
        /// </summary>
        X2 = 1 << 6,

        /// <summary>
        /// 
        /// </summary>
        Count
    }
}
