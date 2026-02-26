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

namespace Nomad.Core.EngineUtils
{
    /// <summary>
    /// 
    /// </summary>
    public readonly struct WindowSizeChangedEventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public int Width => _width;
        private readonly int _width;

        /// <summary>
        /// 
        /// </summary>
        public int Height => _height;
        private readonly int _height;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public WindowSizeChangedEventArgs(int width, int height)
        {
            _width = width;
            _height = height;
        }
    }
}