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

namespace Nomad.Core.Physics.ValueObjects
{
    public readonly struct RaycastQuery
    {
        public Vector2 Origin { get; }
        public Vector2 Direction { get; } // normalized by constructor
        public float MaxDistance { get; }
        public CollisionLayerMask LayerMask { get; }
        public RaycastFlags Flags { get; }

        public RaycastQuery(
            Vector2 origin,
            Vector2 direction,
            float maxDistance,
            CollisionLayerMask layerMask,
            RaycastFlags flags = RaycastFlags.None)
        {
            float directionLengthSquared = direction.LengthSquared();
            if (directionLengthSquared <= 0.0f)
            {
                throw new ArgumentException("Direction must be non-zero.", nameof(direction));
            }
            if (maxDistance <= 0.0f && !float.IsPositiveInfinity(maxDistance))
            {
                throw new ArgumentOutOfRangeException(nameof(maxDistance));
            }

            Origin = origin;
            Direction = Vector2.Normalize(direction);
            MaxDistance = maxDistance;
            LayerMask = layerMask;
            Flags = flags;
        }

        public Vector2 GetEndPoint() => Origin + (Direction * MaxDistance);
    }
}
