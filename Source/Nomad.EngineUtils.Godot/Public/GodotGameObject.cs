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
using System.Collections.Concurrent;
using System.Collections.Generic;
using Godot;
using Nomad.Core.ECS;
using Nomad.Core.EngineUtils;

namespace Nomad.EngineUtils
{
    /// <summary>
    ///
    /// </summary>
    public sealed class GodotGameObject : IGameObject
    {
        /// <summary>
        ///
        /// </summary>
        public string Name
        {
            get => _node.Name;
            set => _node.Name = value;
        }

        /// <summary>
        ///
        /// </summary>
        public bool Enabled
        {
            get => _node.ProcessMode != Node.ProcessModeEnum.Disabled;
            set => _node.ProcessMode = value ? Node.ProcessModeEnum.Inherit : Node.ProcessModeEnum.Disabled;
        }

        /// <summary>
        ///
        /// </summary>
        public IGameObject? Parent
        {
            get => _parent;
            set => _parent = value;
        }
        private IGameObject? _parent;

        /// <summary>
        ///
        /// </summary>
        public Node Node => _node;
        private readonly Node _node;

        /// <summary>
        ///
        /// </summary>
        public IReadOnlyList<IGameObject> Children => _children;
        private readonly List<IGameObject> _children = new();

        private readonly ConcurrentDictionary<Type, IComponent> _components = new();

        private bool _isDisposed = false;

        public GodotGameObject(Node node)
        {
            _node = node;
        }

        /// <summary>
        ///
        /// </summary>
        public void Dispose()
        {
            if (!_isDisposed)
            {
                _components.Clear();
            }
            GC.SuppressFinalize(this);
            _isDisposed = true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T AddComponent<T>()
            where T : IComponent, new()
        {
            var component = _components.GetOrAdd(typeof(T), f => new T());
            return (T)component;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T? GetComponent<T>()
            where T : IComponent
        {
            return _components.TryGetValue(typeof(T), out var component) ? (T)component : default;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool HasComponent<T>()
            where T : IComponent
        {
            return _components.ContainsKey(typeof(T));
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void RemoveComponent<T>()
            where T : IComponent
        {
            _components.TryRemove(typeof(T), out _);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="child"></param>
        public void AddChild(IGameObject child)
        {
            _children.Add(child);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="child"></param>
        public void RemoveChild(IGameObject child)
        {
            _children.Remove(child);
        }

        /// <summary>
        ///
        /// </summary>
        public void OnInit()
        {
            if (_node.GetScript().AsGodotObject() is GodotObject script && script is IComponent scriptComponent)
            {
                _components.TryAdd(script.GetType(), scriptComponent);
                _node.SetScript(Variant.CreateFrom<GodotObject>(null!));
            }
            foreach (var component in _components)
            {
                component.Value.OnInit();
            }
        }

        /// <summary>
        ///
        /// </summary>
        public void OnShutdown()
        {
            foreach (var component in _components)
            {
                component.Value.OnShutdown();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="delta"></param>
        public void OnUpdate(float delta)
        {
            float deltaTime = (float)delta;
            foreach (var component in _components)
            {
                component.Value.OnUpdate(deltaTime);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="delta"></param>
        public void OnPhysicsUpdate(float delta)
        {
            float deltaTime = (float)delta;
            foreach (var component in _components)
            {
                component.Value.OnPhysicsUpdate(deltaTime);
            }
        }
    }
}
