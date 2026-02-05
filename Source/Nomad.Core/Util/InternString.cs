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

using Nomad.Core.Memory;
using System;
using System.Runtime.CompilerServices;

namespace Nomad.Core.Util
{
    /// <summary>
    /// Represents an interned string, providing efficient storage and comparison of string values.
    /// </summary>
    /// <remarks>
    /// <para>
    /// An <see cref="InternString"/> is a lightweight value type that stores an integer identifier
    /// pointing to a unique string instance in the global string pool. This enables:
    /// <list type="bullet">
    /// <item><description>Reduced memory usage for duplicate strings</description></item>
    /// <item><description>Fast equality comparisons (integer comparison instead of string comparison)</description></item>
    /// <item><description>Efficient hashing (the identifier itself serves as the hash code)</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// Interned strings are managed by the <see cref="StringPool"/> class, which ensures that
    /// identical strings share the same storage and identifier.
    /// </para>
    /// <para>
    /// This struct implements <see cref="IEquatable{T}"/> for efficient equality comparisons
    /// without boxing. See the remarks on the <see cref="Equals(InternString)"/> method for details.
    /// </para>
    /// </remarks>
    public readonly struct InternString : IEquatable<InternString>
    {
        private readonly int _id;

        /// <summary>
        /// Initializes a new instance of the <see cref="InternString"/> struct with a specific identifier.
        /// </summary>
        /// <param name="id">The identifier of the interned string in the string pool.</param>
        /// <remarks>
        /// <para>
        /// This constructor is typically used internally by the string interning system.
        /// For user code, prefer the constructor that takes a <see cref="ReadOnlySpan{T}"/> of characters
        /// or use implicit conversion from strings.
        /// </para>
        /// <para>
        /// Note: This constructor does not validate that the identifier corresponds to
        /// an existing interned string. Using an invalid identifier may result in
        /// undefined behavior when the string is accessed.
        /// </para>
        /// </remarks>
        public InternString(int id) => _id = id;

        /// <summary>
        /// Initializes a new instance of the <see cref="InternString"/> struct from a character span.
        /// </summary>
        /// <param name="value">The character span containing the string value to intern.</param>
        /// <remarks>
        /// <para>
        /// This constructor interns the specified string value in the global string pool.
        /// If an identical string is already interned, the existing identifier is reused.
        /// </para>
        /// <para>
        /// The interning process may allocate memory in the string pool if the string
        /// is not already present.
        /// </para>
        /// </remarks>
        public InternString(ReadOnlySpan<char> value) => _id = StringPool.Intern(value);

        /// <summary>
        /// Gets an empty <see cref="InternString"/> instance.
        /// </summary>
        /// <value>
        /// An <see cref="InternString"/> instance representing an empty string.
        /// </value>
        /// <remarks>
        /// The empty <see cref="InternString"/> has an identifier of 0, which is
        /// reserved for the empty string in the string pool.
        /// </remarks>
        public static readonly InternString Empty = new InternString();

        /// <summary>
        /// Determines whether this instance and another specified <see cref="InternString"/> are equal.
        /// </summary>
        /// <param name="other">The <see cref="InternString"/> to compare with this instance.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="other"/> has the same identifier as this instance;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method provides an efficient equality comparison by comparing only the
        /// integer identifiers, avoiding string comparison entirely.
        /// </para>
        /// <para>
        /// This method is aggressively inlined for performance.
        /// </para>
        /// <para>
        /// Implementing <see cref="IEquatable{T}"/> prevents boxing when comparing
        /// <see cref="InternString"/> instances in generic contexts or collections
        /// that use the interface.
        /// </para>
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(InternString other) => other._id == _id;

        /// <summary>
        /// Determines whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">The object to compare with this instance.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="obj"/> is an <see cref="InternString"/>
        /// and has the same identifier as this instance; otherwise, <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// This method is aggressively inlined for performance.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object? obj) => obj is InternString other && other._id == _id;

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// The identifier of this interned string, which serves as its hash code.
        /// </returns>
        /// <remarks>
        /// <para>
        /// The hash code is simply the integer identifier, which provides:
        /// <list type="bullet">
        /// <item><description>Perfect distribution (each interned string has a unique identifier)</description></item>
        /// <item><description>Consistency with equality (equal instances have the same hash code)</description></item>
        /// <item><description>Minimal computation (the identifier is already available)</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// This method is aggressively inlined for performance.
        /// </para>
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => _id;

        /// <summary>
        /// Converts an <see cref="InternString"/> to a regular string.
        /// </summary>
        /// <param name="value">The <see cref="InternString"/> to convert.</param>
        /// <returns>
        /// The string value represented by the <see cref="InternString"/>, or
        /// <see cref="string.Empty"/> if the identifier is invalid or represents an empty string.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This conversion looks up the string value in the string pool using the identifier.
        /// If the identifier is invalid (does not correspond to an interned string),
        /// <see cref="string.Empty"/> is returned.
        /// </para>
        /// <para>
        /// This method is aggressively inlined for performance, though the underlying
        /// string pool lookup may involve a dictionary lookup.
        /// </para>
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator string(InternString value) => StringPool.FromInterned(value) ?? String.Empty;

        /// <summary>
        /// Converts an <see cref="InternString"/> to its integer identifier.
        /// </summary>
        /// <param name="value">The <see cref="InternString"/> to convert.</param>
        /// <returns>The integer identifier of the interned string.</returns>
        /// <remarks>
        /// This conversion allows direct access to the underlying identifier,
        /// which can be useful for serialization or low-level operations.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator int(InternString value) => value._id;

        /// <summary>
        /// Determines whether two <see cref="InternString"/> instances are equal.
        /// </summary>
        /// <param name="left">The first <see cref="InternString"/> to compare.</param>
        /// <param name="right">The second <see cref="InternString"/> to compare.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/>
        /// have the same identifier; otherwise, <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// This operator performs the same comparison as <see cref="Equals(InternString)"/>
        /// but with a more convenient syntax for direct comparisons.
        /// </remarks>
        public static bool operator ==(InternString left, InternString right) => left._id == right._id;

        /// <summary>
        /// Determines whether two <see cref="InternString"/> instances are not equal.
        /// </summary>
        /// <param name="left">The first <see cref="InternString"/> to compare.</param>
        /// <param name="right">The second <see cref="InternString"/> to compare.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/>
        /// have different identifiers; otherwise, <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// This operator is the logical negation of the equality operator.
        /// </remarks>
        public static bool operator !=(InternString left, InternString right) => left._id != right._id;
    }
}