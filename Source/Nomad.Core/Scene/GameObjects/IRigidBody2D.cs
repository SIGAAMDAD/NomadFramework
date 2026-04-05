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

namespace Nomad.Core.Scene.GameObjects
{
    /// <summary>
    /// Represents a simulated 2D rigid body.
    /// </summary>
    public interface IRigidBody2D : ICollisionBody2D
    {
        /// <summary>
        /// Gets or sets the current linear velocity.
        /// </summary>
        Vector2 LinearVelocity { get; set; }

        /// <summary>
        /// Gets or sets the current angular velocity.
        /// </summary>
        float AngularVelocity { get; set; }

        /// <summary>
        /// Gets or sets the body mass.
        /// </summary>
        float Mass { get; set; }

        /// <summary>
        /// Gets or sets the gravity multiplier applied to the body.
        /// </summary>
        float GravityScale { get; set; }

        /// <summary>
        /// Gets or sets the linear damping applied to the body.
        /// </summary>
        float LinearDamping { get; set; }

        /// <summary>
        /// Gets or sets the angular damping applied to the body.
        /// </summary>
        float AngularDamping { get; set; }

        /// <summary>
        /// Gets or sets whether the body is sleeping.
        /// </summary>
        bool Sleeping { get; set; }
    }
}
