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
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Nomad.Core.Input
{
    /// <summary>
    /// 
    /// </summary>
    public readonly struct GamepadAxisEvent : IEquatable<GamepadAxisEvent>
    {
        /// <summary>
        /// 
        /// </summary>
        public long Timestamp => _timeStamp;
        private readonly long _timeStamp;

        /// <summary>
        /// 
        /// </summary>
        public int DeviceId => _deviceId;
        private readonly int _deviceId;

        /// <summary>
        /// 
        /// </summary>
        public GamepadStick Stick => _stick;
        private readonly GamepadStick _stick;

        /// <summary>
        /// 
        /// </summary>
        public Vector2 Value => _value;
        private readonly Vector2 _value;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stick"></param>
        /// <param name="timestamp"></param>
        /// <param name="deviceId"></param>
        /// <param name="value"></param>
        public GamepadAxisEvent(GamepadStick stick, long timestamp, int deviceId, Vector2 value)
        {
            _stick = stick;
            _timeStamp = timestamp;
            _deviceId = deviceId;
            _value = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(GamepadAxisEvent other)
        {
            return _stick == other._stick && _value == other._value;
        }
    }
}
