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
    /// Represents an in-game viewpoint.
    /// </summary>
    public interface ICamera2D : IEntity2D
    {
        /// <summary>
        /// 
        /// </summary>
        Vector2 Zoom { get; set; }

        /// <summary>
        /// 
        /// </summary>
        IGameObject FollowTarget { get; set; }
    }
}
