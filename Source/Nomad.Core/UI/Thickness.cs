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

namespace Nomad.Core.UI
{
    /// <summary>
    /// 
    /// </summary>
    public readonly struct Thickness
    {
        /// <summary>
        /// 
        /// </summary>
        public float Left => _left;
        private readonly float _left;

        /// <summary>
        /// 
        /// </summary>
        public float Top => _top;
        private readonly float _top;

        /// <summary>
        /// 
        /// </summary>
        public float Right => _right;
        private readonly float _right;

        /// <summary>
        /// 
        /// </summary>
        public float Bottom => _bottom;
        private readonly float _bottom;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="right"></param>
        /// <param name="bottom"></param>
        public Thickness(float left, float top, float right, float bottom)
        {
            _left = left;
            _top = top;
            _right = right;
            _bottom = bottom;
        }
    }
}
