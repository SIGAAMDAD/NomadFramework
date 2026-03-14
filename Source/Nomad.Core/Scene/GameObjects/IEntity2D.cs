/*
===========================================================================
The Nomad Framework
Copyright (C) 2025 Noah Van Til

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
    ///
    /// </summary>
    public interface IEntity2D : ISceneObject
    {
        /// <summary>
        ///
        /// </summary>
        bool IsVisible { get; set; }

        /// <summary>
        ///
        /// </summary>
        Vector2 Position { get; set; }

        /// <summary>
        ///
        /// </summary>
        Vector2 Scale { get; set; }

        /// <summary>
        ///
        /// </summary>
        float RotationRadians { get; set; }

        /// <summary>
        ///
        /// </summary>
        float RotationDegrees { get; set; }

        /// <summary>
        ///
        /// </summary>
        int RenderOrder { get; set; }
    }
}
