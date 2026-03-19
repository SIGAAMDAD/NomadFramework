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
using System.Runtime.CompilerServices;

namespace Nomad.Core.Input
{
    /// <summary>
    /// 
    /// </summary>
    public readonly struct MouseMotionEvent : IEquatable<MouseMotionEvent>
    {
        /// <summary>
        /// 
        /// </summary>
        public long TimeStamp => _timeStamp;
        private readonly long _timeStamp;

        /// <summary>
        /// 
        /// </summary>
        public int RelativeX => _relativeX;
        private readonly int _relativeX;

        /// <summary>
        /// 
        /// </summary>
        public int RelativeY => _relativeY;
        private readonly int _relativeY;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timestamp"></param>
        /// <param name="relativeX"></param>
        /// <param name="relativeY"></param>
        public MouseMotionEvent(long timestamp, int relativeX, int relativeY)
        {
            _timeStamp = timestamp;
            _relativeX = relativeX;
            _relativeY = relativeY;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(MouseMotionEvent other)
        {
            return _relativeX == other._relativeX && _relativeY == other._relativeY;
        }
    }
}
