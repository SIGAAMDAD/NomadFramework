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

using System.Drawing;
using System.Numerics;

namespace Nomad.Core.EngineUtils.GameObjects
{
    /// <summary>
    ///
    /// </summary>
    public interface ILight2D : IGameObject
    {
        /// <summary>
        ///
        /// </summary>
        bool Show { get; set; }

        /// <summary>
        ///
        /// </summary>
        Vector2 Position { get; set; }

        /// <summary>
        ///
        /// </summary>
        Color Color { get; set; }

        /// <summary>
        ///
        /// </summary>
        float Intensity { get; set; }

        /// <summary>
        ///
        /// </summary>
        float Range { get; set; }

        /// <summary>
        ///
        /// </summary>
        bool CastShadows { get; set; }
    }
}
