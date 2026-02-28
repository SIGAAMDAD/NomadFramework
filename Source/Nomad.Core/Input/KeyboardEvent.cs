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
using Nomad.Core.Input.ValueObjects;

namespace Nomad.Core.Input
{
    /// <summary>
    /// 
    /// </summary>
    public readonly struct KeyboardEvent
    {
        /// <summary>
        /// 
        /// </summary>
        public DateTime TimeStamp => _timeStamp;
        private readonly DateTime _timeStamp;

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
        public KeyboardEvent(KeyNum keyNum, DateTime timeStamp, bool pressed)
        {
            _keyNum = keyNum;
            _timeStamp = timeStamp;
            _pressed = pressed;
        }
    }
}
