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
using Nomad.Core.Events;
using Nomad.Events.Global;

namespace Nomad.EngineUtils
{
    public partial class EngineButton : Button, IGameObject
    {
        /// <summary>
        ///
        /// </summary>
        string IGameObject.Name {
            get => Name;
            set => Name = value;
        }

        /// <summary>
        ///
        /// </summary>
        public IGameObject Parent {
            get => _parent;
            set => _parent = value;
        }
        private IGameObject _parent;

        /// <summary>
        ///
        /// </summary>
        public IReadOnlyList<IGameObject> Children => _children;
        private readonly List<IGameObject> _children = new();

        private readonly ConcurrentDictionary<Type, IComponent> _components = new();

        private bool _isDisposed = false;

        /// <summary>
        ///
        /// </summary>
        public IGameEvent<EmptyEventArgs> Clicked => GameEventRegistry.GetEvent<EmptyEventArgs>($"{Name}:{Constants.Events.BUTTON_CLICKED}", Constants.Events.NAMESPACE);

        /// <summary>
        ///
        /// </summary>
        public IGameEvent<EmptyEventArgs> Focused => GameEventRegistry.GetEvent<EmptyEventArgs>($"{Name}:{Constants.Events.BUTTON_FOCUSED}", Constants.Events.NAMESPACE);

        /// <summary>
        ///
        /// </summary>
        public IGameEvent<EmptyEventArgs> Unfocused => GameEventRegistry.GetEvent<EmptyEventArgs>($"{Name}:{Constants.Events.BUTTON_UNFOCUSED}", Constants.Events.NAMESPACE);

        /// <summary>
        ///
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                _parent?.Dispose();
            }
            base.Dispose(disposing);
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
        public sealed override void _Ready()
        {
            base._Ready();

            if (GetParent() is GodotGameObject parentObject)
            {
                _parent = parentObject;
            }
            foreach (var component in _components)
            {
                component.Value.OnInit();
            }

            Pressed += () => Clicked.Publish(default);
            FocusEntered += () => Focused.Publish(default);
            FocusExited += () => Unfocused.Publish(default);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="delta"></param>
        public sealed override void _Process(double delta)
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
        public sealed override void _PhysicsProcess(double delta)
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
        public sealed override void _ExitTree()
        {
            base._ExitTree();

            foreach (var component in _components)
            {
                component.Value.OnShutdown();
            }
        }
    }
}
