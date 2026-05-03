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

using System.Numerics;

namespace Nomad.Core.Physics.ValueObjects
{
    public readonly struct RaycastHit
    {
        public Vector2 Point { get; }
        public Vector2 Normal { get; }
        public float Distance { get; }
        public int Layer { get; }

        public PhysicsObjectHandle Collider { get; }
        public PhysicsObjectHandle Body { get; }

        public RaycastHit(
            Vector2 point,
            Vector2 normal,
            float distance,
            int layer,
            PhysicsObjectHandle collider,
            PhysicsObjectHandle body)
        {
            Point = point;
            Normal = normal;
            Distance = distance;
            Layer = layer;
            Collider = collider;
            Body = body;
        }
    }
}
