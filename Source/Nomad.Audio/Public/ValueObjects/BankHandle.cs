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
    /// Represents a handle to an audio bank within the system. This struct encapsulates a 32-bit unsigned integer value that serves as a reference to a loaded audio bank, allowing for efficient management and access to audio resources.
    /// </summary>
    /// <param name="value">The 32-bit unsigned integer value that represents the handle.</param>
    public readonly struct BankHandle
    {
        /// <summary>
        /// An invalid bank handle. This value is used to represent an uninitialized or invalid handle, indicating that it does not reference a valid audio bank within the system. A valid handle will have a non-zero value in the lower 24 bits, while an invalid handle will have all bits set to zero.
        /// </summary>
        public static readonly BankHandle Invalid = default;

        private readonly uint _value;

        /// <summary>
        /// Determines whether this instance represents a valid bank handle. A valid handle is one that has a non-zero value in the lower 24 bits, indicating that it references an active audio bank within the system. This property allows for quick checks to ensure that operations are performed on valid handles, preventing errors and ensuring the integrity of audio resource management.
        /// </summary>
        public bool IsValid => (_value & 0x00FFFFFF) != 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="BankHandle"/> struct with the specified value. The value is a 32-bit unsigned integer that serves as a reference to a loaded audio bank within the system. This constructor allows for the creation of valid bank handles by providing the appropriate value, while also enabling the representation of invalid handles through the use of the default constructor.
        /// </summary>
        /// <param name="value">The 32-bit unsigned integer value that represents the bank handle.</param>
        public BankHandle(uint value)
        {
            _value = value;
        }

        /// <summary>
        /// Determines whether this instance and another specified <see cref="BankHandle"/> object have the same value. This method allows for efficient comparisons between bank handles, enabling the audio system to quickly determine if two handles reference the same audio bank. By comparing the underlying 32-bit unsigned integer values, this method provides a fast and reliable way to check for handle equality, which is essential for managing audio resources effectively.
        /// </summary>
        /// <param name="other">The bank handle to compare with this instance.</param>
        /// <returns>true if the value of the other parameter is the same as the value of this instance; otherwise, false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(BankHandle other) => other._value == _value;

        /// <summary>
        /// Defines an implicit conversion of a <see cref="BankHandle"/> to a <see cref="uint"/>. This allows for easy retrieval of the underlying value when needed, while still benefiting from the type safety and encapsulation provided by the <see cref="BankHandle"/> struct.
        /// </summary>
        /// <param name="handle">The bank handle to convert.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator uint(BankHandle handle) => handle._value;
    }
}
