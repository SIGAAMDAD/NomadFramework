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
    public readonly struct GamepadButtonEvent : IEquatable<GamepadButtonEvent>
    {
        /// <summary>
        /// 
        /// </summary>
        public long TimeStamp => _timeStamp;
        private readonly long _timeStamp;

        /// <summary>
        /// 
        /// </summary>
        public int DeviceId => _deviceId;
        private readonly int _deviceId;

        /// <summary>
        /// 
        /// </summary>
        public GamepadButton Button => _button;
        private readonly GamepadButton _button;

        /// <summary>
        /// 
        /// </summary>
        public bool Pressed => _pressed;
        private readonly bool _pressed;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="button"></param>
        /// <param name="deviceId"></param>
        /// <param name="timestamp"></param>
        /// <param name="pressed"></param>
        public GamepadButtonEvent(GamepadButton button, int deviceId, long timestamp, bool pressed)
        {
            _timeStamp = timestamp;
            _deviceId = deviceId;
            _button = button;
            _pressed = pressed;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(GamepadButtonEvent other)
        {
            return _button == other._button && _pressed == other._pressed;
        }
    }
}