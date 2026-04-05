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
	/// Engine-agnostic collision layer mask.
	/// Uses 32 bits because both Unity and Godot can map cleanly to this.
	/// </summary>
	public readonly struct CollisionLayerMask : IEquatable<CollisionLayerMask>
	{
		public static readonly CollisionLayerMask None = new(0u);
		public static readonly CollisionLayerMask All = new(uint.MaxValue);

		public uint Value { get; }

		public CollisionLayerMask(uint value)
		{
			Value = value;
		}

		public static CollisionLayerMask FromLayer(int layer)
		{
			if ((uint)layer >= 32u)
				throw new ArgumentOutOfRangeException(nameof(layer));

			return new CollisionLayerMask(1u << layer);
		}

		public bool Contains(int layer)
		{
			if ((uint)layer >= 32u)
				return false;

			return ((Value >> layer) & 1u) != 0u;
		}

		public static CollisionLayerMask operator |(CollisionLayerMask left, CollisionLayerMask right)
			=> new(left.Value | right.Value);

		public static CollisionLayerMask operator &(CollisionLayerMask left, CollisionLayerMask right)
			=> new(left.Value & right.Value);

		public bool Equals(CollisionLayerMask other) => Value == other.Value;
		public override bool Equals(object? obj) => obj is CollisionLayerMask other && Equals(other);
		public override int GetHashCode() => (int)Value;
		public override string ToString() => $"0x{Value:X8}";
	}
}