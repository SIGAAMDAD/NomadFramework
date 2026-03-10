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
    public partial class GodotGameObject : Node, IGameObject
    {
        /// <summary>
        ///
        /// </summary>
        string IGameObject.Name
        {
            get => Name;
            set => Name = value;
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
        public IReadOnlyList<IGameObject> Children => _children;
        private readonly List<IGameObject> _children = new();

        private readonly ConcurrentDictionary<Type, IComponent> _components = new();

        protected bool isDisposed = false;

        /// <summary>
        ///
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (isDisposed)
            {
                return;
            }
            if (disposing)
            {
                _components.Clear();
            }
            base.Dispose(disposing);
            isDisposed = true;
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
        public sealed override void _Ready()
        {
            base._Ready();

            if (GetScript().AsGodotObject() is GodotObject script && script is IComponent scriptComponent)
            {
                _components.TryAdd(script.GetType(), scriptComponent);
                SetScript(Variant.CreateFrom<GodotObject>(null!));
            }
            foreach (var component in _components)
            {
                component.Value.OnInit();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="delta"></param>
        public override void _Process(double delta)
        {
            base._Process(delta);

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
        public override void _PhysicsProcess(double delta)
        {
            base._PhysicsProcess(delta);

            float deltaTime = (float)delta;
            foreach (var component in _components)
            {
                component.Value.OnPhysicsUpdate(deltaTime);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public override void _ExitTree()
        {
            base._ExitTree();

            foreach (var component in _components)
            {
                component.Value.OnShutdown();
            }
        }
    }
}
