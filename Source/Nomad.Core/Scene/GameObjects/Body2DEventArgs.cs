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

namespace Nomad.Core.Scene.GameObjects
{
    /// <summary>
    /// Describes an overlap event involving another collision body.
    /// </summary>
    public readonly struct Body2DEventArgs
    {
        /// <summary>
        /// The overlapping collision body.
        /// </summary>
        public ICollisionBody2D Body => _body;
        private readonly ICollisionBody2D _body;

        /// <summary>
        /// Creates a new set of body overlap event arguments.
        /// </summary>
        public Body2DEventArgs(ICollisionBody2D body)
        {
            _body = body;
        }
    }
}
