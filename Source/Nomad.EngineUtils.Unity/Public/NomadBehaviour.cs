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

using Nomad.Core.ECS;
using Nomad.Core.Scene.GameObjects;
using UnityEngine;

namespace Nomad.EngineUtils
{
    /// <summary>
    ///
    /// </summary>
    public abstract class NomadBehaviour : MonoBehaviour, IComponent
    {
        /// <summary>
        ///
        /// </summary>
        public IGameObject Object { get; set; } = default!;

        /// <summary>
        /// Initializes the component.
        /// </summary>
        public virtual void OnInit()
        {
        }

        /// <summary>
        /// Updates the component each physics frame.
        /// </summary>
        /// <param name="delta"></param>
        public virtual void OnPhysicsUpdate(float delta)
        {
        }

        /// <summary>
        /// Terminates and releases the component.
        /// </summary>
        public virtual void OnShutdown()
        {
        }

        /// <summary>
        /// Updates the component each frame.
        /// </summary>
        /// <param name="delta"></param>
        public virtual void OnUpdate(float delta)
        {
        }
    }
}
