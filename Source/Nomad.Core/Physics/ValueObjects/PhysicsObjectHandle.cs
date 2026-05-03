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

namespace Nomad.Core.Physics.ValueObjects
{
    /// <summary>
    /// Opaque engine-agnostic handle for a physics object.
    /// Backends map this to engine-native collider/body instance IDs.
    /// </summary>
    public readonly struct PhysicsObjectHandle : IEquatable<PhysicsObjectHandle>
    {
        public static readonly PhysicsObjectHandle Invalid = new(0);

        public long Value { get; }

        public PhysicsObjectHandle(long value)
        {
            Value = value;
        }

        public bool IsValid => Value != 0;

        public bool Equals(PhysicsObjectHandle other) => Value == other.Value;
        public override bool Equals(object? obj) => obj is PhysicsObjectHandle other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator ==(PhysicsObjectHandle left, PhysicsObjectHandle right) => left.Equals(right);
        public static bool operator !=(PhysicsObjectHandle left, PhysicsObjectHandle right) => !left.Equals(right);

        public override string ToString() => Value.ToString();
    }
}
