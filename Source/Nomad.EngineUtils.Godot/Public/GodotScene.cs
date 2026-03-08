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
using Godot;
using Nomad.Core.EngineUtils;

namespace Nomad.EngineUtils
{
    /// <summary>
    ///
    /// </summary>
    internal sealed class GodotScene : IScene
    {
        public IReadOnlyList<IGameObject> Children => _children;
        private readonly List<IGameObject> _children;

        public string Path => _scene.SceneFilePath;

        /// <summary>
        ///
        /// </summary>
        public string Name
        {
            get => _scene.Name;
            set => _scene.Name = value;
        }

        public Node Scene => _scene;
        private readonly Node _scene;

        private bool _isDisposed = false;

        /// <summary>
        ///
        /// </summary>
        public GodotScene(Node scene)
        {
            _scene = scene;

            var children = scene.GetChildren();
            _children = new List<IGameObject>(children.Count);
            for (int i = 0; i < children.Count; i++)
            {
                if (children[i] is GodotGameObject gameObject)
                {
                    _children.Add(gameObject);
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        public void Dispose()
        {
            if (!_isDisposed)
            {
                for (int i = 0; i < _children.Count; i++)
                {
                    _children[i]?.Dispose();
                }
                _scene?.QueueFree();
            }
            GC.SuppressFinalize(this);
            _isDisposed = true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="gameObject"></param>
        public void AddChild(IGameObject gameObject)
        {
            if (gameObject is GodotGameObject godotObject)
            {
                _children.Add(godotObject);
                _scene.AddChild(godotObject);
            }
            else
            {
                throw new InvalidCastException();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="gameObject"></param>
        public void RemoveChild(IGameObject gameObject)
        {
            if (gameObject is GodotGameObject godotObject)
            {
                _children.Remove(godotObject);
                _scene.RemoveChild(godotObject);
            }
            else
            {
                throw new InvalidCastException();
            }
        }
    }
}
