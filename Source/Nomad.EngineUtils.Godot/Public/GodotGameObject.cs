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

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Godot;
using Nomad.Core.ECS;
using Nomad.Core.Scene.GameObjects;

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

        /// <summary>
        ///
        /// </summary>
        /// <param name="node"></param>
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
        /// <param name="initializer"></param>
        public T AddComponent<T>(Action<T>? initializer = null)
            where T : IComponent, new()
        {
            var comp = new T()
            {
                Object = this
            };
            initializer?.Invoke(comp);
            comp.OnInit();
            _components[typeof(T)] = comp;
            return comp;
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
        /// <param name="childName"></param>
        /// <returns></returns>
        public T? FindChild<T>(string childName)
            where T : class, IGameObject
        {
            if (string.IsNullOrEmpty(childName))
            {
                return null;
            }

            // TODO: write a zero-allocation version of this
            ReadOnlySpan<string> segments = childName.Split('/');
            var current = _node;

            for (int i = 0; i < segments.Length; i++)
            {
                var segment = segments[i];
                if (string.IsNullOrEmpty(segment))
                {
                    continue;
                }
                bool found = false;
                foreach (var child in current.GetChildren())
                {
                    if (child.Name == segment)
                    {
                        current = child;
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    return null;
                }
            }
            return (T)WrapNode(current);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static IGameObject WrapNode(Node node)
        {
            if (node is IGameObject gameObject)
            {
                return gameObject;
            }
            return new GodotGameObject(node);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="child"></param>
        public void AddChild(IGameObject child)
        {
            _node.AddChild((child as GodotGameObject).Node);
            _children.Add(child);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="child"></param>
        public void RemoveChild(IGameObject child)
        {
            _node.RemoveChild((child as GodotGameObject).Node);
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
                _node.SetScript(Variant.CreateFrom<GodotObject>(null));
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
