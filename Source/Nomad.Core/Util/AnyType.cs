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

namespace Nomad.Core.Util
{
    /// <summary>
    /// Represents the type of value stored in an <see cref="Any"/> instance.
    /// </summary>
    /// <remarks>
    /// This enumeration is used internally by the <see cref="Any"/> struct to track
    /// the type of value stored, enabling type-safe operations and conversions.
    /// </remarks>
    public enum AnyType : byte
    {
        /// <summary>
        /// Represents a boolean value.
        /// </summary>
        Boolean,

        /// <summary>
        /// Represents a signed 8-bit integer.
        /// </summary>
        Int8,

        /// <summary>
        /// Represents a signed 16-bit integer.
        /// </summary>
        Int16,

        /// <summary>
        /// Represents a signed 32-bit integer.
        /// </summary>
        Int32,

        /// <summary>
        /// Represents a signed 64-bit integer.
        /// </summary>
        Int64,

        /// <summary>
        /// Represents an unsigned 8-bit integer.
        /// </summary>
        UInt8,

        /// <summary>
        /// Represents an unsigned 16-bit integer.
        /// </summary>
        UInt16,

        /// <summary>
        /// Represents an unsigned 32-bit integer.
        /// </summary>
        UInt32,

        /// <summary>
        /// Represents an unsigned 64-bit integer.
        /// </summary>
        UInt64,

        /// <summary>
        /// Represents a 32-bit floating-point number.
        /// </summary>
        Float32,

        /// <summary>
        /// Represents a 64-bit floating-point number.
        /// </summary>
        Float64,

        /// <summary>
        /// Represents an interned string.
        /// </summary>
        InternString,

        /// <summary>
        /// Represents the total count of valid <see cref="AnyType"/> values.
        /// </summary>
        /// <remarks>
        /// This value is useful for array allocations and bounds checking.
        /// It is not a valid type for storing values in an <see cref="Any"/> instance.
        /// </remarks>
        Count
    };
}