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
using Nomad.Core.Events;

namespace Nomad.Core.Scene.GameObjects
{
    /// <summary>
    /// Represents a 2D in-game object.
    /// </summary>
    public interface IObject2D : IGameObject
    {
        /// <summary>
        ///
        /// </summary>
        IGameEvent<bool> DisplayStateChanged { get; }

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
        Vector2 Scale { get; set; }

        /// <summary>
        ///
        /// </summary>
        float Rotation { get; set; }
    }
}
