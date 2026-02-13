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
    /// A handle that uniquely identifies a channel group within the audio system. This struct is used to manage and reference channel groups for audio routing and processing.
    /// </summary>
    public readonly struct ChannelGroupHandle
    {
        private readonly uint _value;

        /// <summary>
        /// Defines an invalid <see cref="ChannelGroupHandle"/> with a value of 0. This can be used to represent an uninitialized or non-existent channel group handle.
        /// </summary>
        public static readonly ChannelGroupHandle Invalid = default;

        /// <summary>
        /// Determines whether this instance represents a valid channel group handle. A valid handle is one that has a non-zero value in the lower 24 bits, indicating that it references an active channel group within the system.
        /// </summary>
        public bool IsValid => (_value & 0x00FFFFFF) != 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelGroupHandle"/> struct with the specified value.
        /// </summary>
        /// <param name="value">The value to initialize the handle with.</param>
        public ChannelGroupHandle(uint value)
        {
            _value = value;
        }

        /// <summary>
        /// Determines whether this instance and another specified <see cref="ChannelGroupHandle"/> object have the same value.
        /// </summary>
        /// <param name="handle">The handle to compare with this instance.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator uint(ChannelGroupHandle handle) => handle._value;
    }
}
