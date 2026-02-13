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
using Nomad.Core.Abstractions;

namespace Nomad.Audio.ValueObjects
{
    /// <summary>
    /// Represents a handle to an audio channel within the Nomad audio system. This struct encapsulates a 32-bit integer value that serves as a reference to an active audio channel, allowing for efficient management and control of audio playback. The <see cref="ChannelHandle"/> struct provides type safety and abstraction over raw integer values, ensuring that operations on audio channels are performed correctly and safely. By implementing the <see cref="IValueObject{T}"/> interface, it also supports value-based equality comparisons, making it easier to manage collections of channel handles and perform operations such as lookups and comparisons within the audio system.
    /// </summary>
    /// <param name="value">The 32-bit integer value that represents the channel handle.</param>
    public readonly struct ChannelHandle
    {
        private readonly int _value;

        /// <summary>
        /// Determines whether this instance represents a valid channel handle. A valid handle is one that has a non-zero value, indicating that it references an active audio channel within the system. This property allows for quick checks to ensure that operations are performed on valid handles, preventing errors and ensuring the integrity of audio resource management.
        /// </summary>
        public bool IsValid => _value != 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelHandle"/> struct with the specified value. The value is a 32-bit integer that serves as a reference to an active audio channel within the system. This constructor allows for the creation of valid channel handles by providing the appropriate value, while also enabling the representation of invalid handles through the use of the default constructor.
        /// </summary>
        /// <param name="value">The 32-bit integer value that represents the channel handle.</param>
        public ChannelHandle(int value)
        {
            _value = value;
        }

        /// </summary>
        /// <param name="other">The channel handle to compare with this instance.</param>
        /// <returns>true if the value of the other parameter is the same as the value of this instance; otherwise, false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(ChannelHandle other)
            => other._value == _value;
    }
}
