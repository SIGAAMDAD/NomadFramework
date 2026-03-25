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
    public readonly struct MouseButtonEventArgs : IEquatable<MouseButtonEventArgs>
    {
        /// <summary>
        /// 
        /// </summary>
        public long TimeStamp => _timeStamp;
        private readonly long _timeStamp;

        /// <summary>
        /// 
        /// </summary>
        public MouseButton Button => _button;
        private readonly MouseButton _button;

        /// <summary>
        /// 
        /// </summary>
        public bool Pressed => _pressed;
        private readonly bool _pressed;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="button"></param>
        /// <param name="timestamp"></param>
        /// <param name="pressed"></param>
        public MouseButtonEventArgs(MouseButton button, long timestamp, bool pressed)
        {
            _button = button;
            _timeStamp = timestamp;
            _pressed = pressed;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(MouseButtonEventArgs other)
        {
            return _button == other._button && _pressed == other._pressed;
        }
    }
}
