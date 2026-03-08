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

namespace Nomad.Core.EngineUtils
{
    /// <summary>
    ///
    /// </summary>
    public readonly struct Monitor
    {
        /// <summary>
        ///
        /// </summary>
        public int Index => _index;
        private readonly int _index;

        /// <summary>
        ///
        /// </summary>
        public int RefreshRate => _refreshRate;
        private readonly int _refreshRate;

        /// <summary>
        ///
        /// </summary>
        public WindowResolution ScreenSize => _screenSize;
        private readonly WindowResolution _screenSize;

        /// <summary>
        ///
        /// </summary>
        /// <param name="index"></param>
        /// <param name="refreshRate"></param>
        /// <param name="screenSize"></param>
        public Monitor(int index, int refreshRate, WindowResolution screenSize)
        {
            _index = index;
            _refreshRate = refreshRate;
            _screenSize = screenSize;
        }
    }
}
