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

namespace Nomad.Core.Engine.Windowing
{
    /// <summary>
    ///
    /// </summary>
    public readonly struct Monitor
    {
        /// <summary>
        ///
        /// </summary>
        public int Index { get; }

        /// <summary>
        ///
        /// </summary>
        public int RefreshRate { get; }

        /// <summary>
        ///
        /// </summary>
        public WindowResolution ScreenSize { get; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="index"></param>
        /// <param name="refreshRate"></param>
        /// <param name="screenSize"></param>
        public Monitor(int index, int refreshRate, WindowResolution screenSize)
        {
            Index = index;
            RefreshRate = refreshRate;
            ScreenSize = screenSize;
        }
    }
}
