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
using Nomad.Core.Input.ValueObjects;

namespace Nomad.Core.Input
{
    /// <summary>
    /// 
    /// </summary>
    public readonly struct KeyboardEventArgs : IEquatable<KeyboardEventArgs>
    {
        /// <summary>
        /// 
        /// </summary>
        public long TimeStamp => _timeStamp;
        private readonly long _timeStamp;

        /// <summary>
        /// 
        /// </summary>
        public KeyNum KeyNum => _keyNum;
        private readonly KeyNum _keyNum;

        /// <summary>
        /// 
        /// </summary>
        public bool Pressed => _pressed;
        private readonly bool _pressed;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyNum"></param>
        /// <param name="timeStamp"></param>
        /// <param name="pressed"></param>
        public KeyboardEventArgs(KeyNum keyNum, long timeStamp, bool pressed)
        {
            _keyNum = keyNum;
            _timeStamp = timeStamp;
            _pressed = pressed;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(KeyboardEventArgs other)
        {
            return _keyNum == other._keyNum && _pressed == other._pressed;
        }
    }
}
