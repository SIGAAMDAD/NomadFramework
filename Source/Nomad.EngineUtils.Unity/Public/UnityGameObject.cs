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
using UnityEngine;
using Nomad.Core.ECS;
using Nomad.Core.EngineUtils;

namespace Nomad.EngineUtils
{
    /// <summary>
    /// Unity implementation of IGameObject for Unity 6000+.
    /// Wraps a Unity GameObject and provides Nomad ECS component management.
    /// </summary>
    public sealed class UnityGameObject : IGameObject
    {
        /// <summary>
        /// Gets or sets the wrapped GameObject's name.
        /// </summary>
        public string Name
        {
            get => _gameObject.name;
            set => _gameObject.name = value;
        }

        /// <summary>
        /// Gets or sets the wrapped GameObject's local active state.
        /// </summary>
        public bool Enabled
        {
            get => _gameObject.activeSelf;
            set => _gameObject.SetActive(value);
        }

        /// <summary>
        /// Gets or sets the parent game object.
        /// If the parent is another UnityGameObject, the Unity transform hierarchy is updated too.
        /// </summary>
        public IGameObject? Parent
        {
            get
            {
                return _transform.parent == null ? _parent : WrapGameObject(_transform.parent.gameObject);
            }
            set
            {
                _parent = value;

                if (value is UnityGameObject unityParent)
                {
                    _transform.SetParent(unityParent.Transform, false);
                }
                else if (value == null)
                {
                    _transform.SetParent(null, false);
                }
            }
        }
        private IGameObject? _parent;

        /// <summary>
        /// Gets the wrapped Unity GameObject.
        /// </summary>
        public GameObject GameObject => _gameObject;
        private readonly GameObject _gameObject;

        /// <summary>
        /// Gets the wrapped Unity Transform.
        /// </summary>
        public Transform Transform => _transform;
        private readonly Transform _transform;

        /// <summary>
        /// Gets the wrapped children as IGameObject instances.
        /// </summary>
        public IReadOnlyList<IGameObject> Children
        {
            get
            {
                RefreshChildren();
                return _children;
            }
        }
        private readonly List<IGameObject> _children = new();

        private readonly ConcurrentDictionary<Type, IComponent> _components = new();
        private bool _isDisposed;

        /// <summary>
        /// Creates a new UnityGameObject wrapper.
        /// </summary>
        /// <param name="gameObject">The Unity GameObject to wrap.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public UnityGameObject(GameObject gameObject)
        {
            _gameObject = gameObject ?? throw new ArgumentNullException(nameof(gameObject));
            _transform = gameObject.transform;
            RefreshChildren();
        }

        /// <summary>
        /// Clears managed ECS components.
        /// Does not destroy the Unity GameObject.
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _components.Clear();
            _children.Clear();

            _isDisposed = true;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Adds an ECS component instance to the wrapper.
        /// </summary>
        public void AddComponent<T>(T component)
            where T : IComponent
        {
            if (component == null)
            {
                throw new ArgumentNullException(nameof(component));
            }

            _components.TryAdd(typeof(T), component);
        }

        /// <summary>
        /// Adds a new ECS component instance using its default constructor.
        /// </summary>
        public T AddComponent<T>()
            where T : IComponent, new()
        {
            var component = _components.GetOrAdd(typeof(T), _ => new T());
            return (T)component;
        }

        /// <summary>
        /// Gets an ECS component from the wrapper.
        /// </summary>
        public T? GetComponent<T>()
            where T : IComponent
        {
            return _components.TryGetValue(typeof(T), out var component)
                ? (T)component
                : default;
        }

        /// <summary>
        /// Checks whether an ECS component of the given type exists.
        /// </summary>
        public bool HasComponent<T>()
            where T : IComponent
        {
            return _components.ContainsKey(typeof(T));
        }

        /// <summary>
        /// Removes an ECS component of the given type from the wrapper.
        /// </summary>
        public void RemoveComponent<T>()
            where T : IComponent
        {
            _components.TryRemove(typeof(T), out _);
        }

        /// <summary>
        /// Finds a child by relative path, e.g. "Weapon/Muzzle".
        /// Uses Unity's Transform.Find semantics.
        /// </summary>
        public T? FindChild<T>(string childName)
            where T : class, IGameObject
        {
            if (string.IsNullOrWhiteSpace(childName))
            {
                return null;
            }

            Transform? childTransform = _transform.Find(childName);
            if (childTransform == null)
            {
                return null;
            }

            return WrapGameObject(childTransform.gameObject) as T;
        }

        /// <summary>
        /// Adds a child to this object.
        /// If the child is a UnityGameObject, the Unity transform hierarchy is updated too.
        /// </summary>
        public void AddChild(IGameObject child)
        {
            if (child == null)
            {
                throw new ArgumentNullException(nameof(child));
            }

            if (child is UnityGameObject unityChild)
            {
                unityChild.Transform.SetParent(_transform, false);
                unityChild._parent = this;
            }

            RefreshChildren();
        }

        /// <summary>
        /// Removes a child from this object.
        /// If the child is a UnityGameObject and is parented here, it is unparented.
        /// </summary>
        public void RemoveChild(IGameObject child)
        {
            if (child == null)
            {
                throw new ArgumentNullException(nameof(child));
            }

            if (child is UnityGameObject unityChild && unityChild.Transform.parent == _transform)
            {
                unityChild.Transform.SetParent(null, false);
                unityChild._parent = null;
            }

            RefreshChildren();
        }

        /// <summary>
        /// Initializes wrapper-managed components.
        /// Also auto-registers attached MonoBehaviours that implement ECS IComponent.
        /// </summary>
        public void OnInit()
        {
            RegisterAttachedUnityComponents();

            foreach (var component in _components.Values)
            {
                component.OnInit();
            }
        }

        /// <summary>
        /// Shuts down wrapper-managed components.
        /// </summary>
        public void OnShutdown()
        {
            foreach (var component in _components.Values)
            {
                component.OnShutdown();
            }
        }

        /// <summary>
        /// Updates wrapper-managed components.
        /// </summary>
        public void OnUpdate(float delta)
        {
            foreach (var component in _components.Values)
            {
                component.OnUpdate(delta);
            }
        }

        /// <summary>
        /// Fixed-updates wrapper-managed components.
        /// </summary>
        public void OnPhysicsUpdate(float delta)
        {
            foreach (var component in _components.Values)
            {
                component.OnPhysicsUpdate(delta);
            }
        }

        /// <summary>
        /// Registers all attached MonoBehaviours that also implement ECS IComponent.
        /// </summary>
        private void RegisterAttachedUnityComponents()
        {
            var monoBehaviours = _gameObject.GetComponents<MonoBehaviour>();

            foreach (var behaviour in monoBehaviours)
            {
                if (behaviour is IComponent IComponent)
                {
                    _components.TryAdd(behaviour.GetType(), IComponent);
                }
            }
        }

        /// <summary>
        /// Refreshes the cached child wrapper list from the Unity hierarchy.
        /// </summary>
        private void RefreshChildren()
        {
            _children.Clear();

            foreach (Transform child in _transform)
            {
                _children.Add(WrapGameObject(child.gameObject));
            }
        }

        /// <summary>
        /// Wraps a Unity GameObject as an IGameObject.
        /// </summary>
        private static IGameObject WrapGameObject(GameObject gameObject)
        {
            if (gameObject is IGameObject wrapped)
            {
                return wrapped;
            }

            return new UnityGameObject(gameObject);
        }
    }
}
