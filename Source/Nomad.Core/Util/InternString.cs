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
    ///
    /// </summary>
    public readonly struct InternString : IEquatable<InternString>
    {
        private readonly int _id = 0;

        public InternString(int id) => _id = id;
        public InternString(ReadOnlySpan<char> value) => _id = StringPool.Intern(value);

        public static readonly InternString Empty = new InternString();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(InternString other) => other._id == _id;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object? obj) => obj is InternString other && other._id == _id;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => _id;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator string(InternString value) => StringPool.FromInterned(value) ?? String.Empty;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator int(InternString value) => value._id;

        public static bool operator ==(InternString left, InternString right) => left._id == right._id;
        public static bool operator !=(InternString left, InternString right) => left._id != right._id;
    }
}
