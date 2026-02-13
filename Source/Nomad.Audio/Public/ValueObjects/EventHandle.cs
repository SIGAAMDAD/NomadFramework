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

using System.Runtime.CompilerServices;

namespace Nomad.Audio.ValueObjects
{
    /// <summary>
    /// A handle that uniquely identifies an audio event instance. This struct is used to track and manage active audio events within the system.
    /// </summary>
    /// <param name="value"></param>
    public readonly struct EventHandle
    {
        private readonly uint _value;

        /// <summary>
        /// An invalid event handle. This value is used to represent an uninitialized or invalid handle.
        /// </summary>
        public static readonly EventHandle Invalid = default;

        public bool IsValid => (_value & 0x00FFFFFF) != 0;

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        public EventHandle(uint value)
        {
            _value = value;
        }

        /// <summary>
        /// Determines whether this instance and another specified <see cref="EventHandle"/> object have the same value.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(EventHandle other) => other._value == _value;

        /// <summary>
        /// Defines an implicit conversion of an <see cref="EventHandle"/> to a <see cref="uint"/>.
        /// </summary>
        /// <param name="handle"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator uint(EventHandle handle) => handle._value;
    }
}
