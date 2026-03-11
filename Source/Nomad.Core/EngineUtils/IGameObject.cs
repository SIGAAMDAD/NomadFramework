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

using System;
using System.Collections.Generic;
using Nomad.Core.ECS;

namespace Nomad.Core.EngineUtils
{
    /// <summary>
    ///
    /// </summary>
    public interface IGameObject : IDisposable
    {
        /// <summary>
        ///
        /// </summary>
        string Name { get; set; }

        /// <summary>
        ///
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        ///
        /// </summary>
        IGameObject? Parent { get; set; }

        /// <summary>
        ///
        /// </summary>
        IReadOnlyList<IGameObject> Children { get; }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T AddComponent<T>() where T : IComponent, new();

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T? GetComponent<T>() where T : IComponent;

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        bool HasComponent<T>() where T : IComponent;

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void RemoveComponent<T>() where T : IComponent;

        /// <summary>
        ///
        /// </summary>
        /// <param name="child"></param>
        void AddChild(IGameObject child);

        /// <summary>
        ///
        /// </summary>
        /// <param name="child"></param>
        void RemoveChild(IGameObject child);
    }
}
