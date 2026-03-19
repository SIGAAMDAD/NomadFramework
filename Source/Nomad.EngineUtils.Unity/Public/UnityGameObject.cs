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
using Nomad.Core.ECS;
using Nomad.Core.Scene.GameObjects;
using UnityEngine;

namespace Nomad.EngineUtils
{
    /// <summary>
    /// Unity implementation of <see cref="IGameObject"/>.
    /// </summary>
    public sealed class UnityGameObject : IGameObject
    {
        /// <summary>
        ///
        /// </summary>
        public string Name
        {
            get => _gameObject.name;
            set => _gameObject.name = value;
        }

        /// <summary>
        ///
        /// </summary>
        public bool Enabled
        {
            get => _gameObject.activeSelf;
            set => _gameObject.SetActive(value);
        }

        /// <summary>
        ///
        /// </summary>
        public IGameObject? Parent
        {
            get => _transform.parent == null ? _parent : WrapGameObject(_transform.parent.gameObject);
            set
            {
                if (value == null)
                {
                    _parent = null;
                    _transform.SetParent(null, false);
                    return;
                }

                if (!TryGetBackingGameObject(value, out GameObject? parentGameObject))
                {
                    throw new InvalidCastException();
                }

                _parent = WrapGameObject(parentGameObject);
                _transform.SetParent(parentGameObject.transform, false);
            }
        }
        private IGameObject? _parent;

        /// <summary>
        ///
        /// </summary>
        public GameObject GameObject => _gameObject;
        private readonly GameObject _gameObject;

        /// <summary>
        ///
        /// </summary>
        public Transform Transform => _transform;
        private readonly Transform _transform;

        /// <summary>
        ///
        /// </summary>
        public IReadOnlyList<IGameObject> Children
        {
            get
            {
                RefreshChildren();
                return _children;
            }
        }
        private readonly List<IGameObject> _children = new List<IGameObject>();

        private readonly ConcurrentDictionary<Type, IComponent> _components = new ConcurrentDictionary<Type, IComponent>();

        private bool _isDisposed;

        /// <summary>
        ///
        /// </summary>
        /// <param name="gameObject"></param>
        public UnityGameObject(GameObject gameObject)
        {
            if (gameObject == null)
            {
                throw new ArgumentNullException(nameof(gameObject));
            }

            _gameObject = gameObject;
            _transform = gameObject.transform;
            RefreshChildren();
        }

        /// <summary>
        ///
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
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="initializer"></param>
        /// <returns></returns>
        public T AddComponent<T>(Action<T>? initializer = null)
            where T : IComponent, new()
        {
            T component;
            if (typeof(Component).IsAssignableFrom(typeof(T)))
            {
                component = (T)(IComponent)_gameObject.AddComponent(typeof(T));
            }
            else
            {
                component = new T();
            }

            component.Object = this;
            initializer?.Invoke(component);
            component.OnInit();
            _components[typeof(T)] = component;
            return component;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T? GetComponent<T>()
            where T : IComponent
        {
            return _components.TryGetValue(typeof(T), out IComponent? component) ? (T)component : default;
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
        /// <typeparam name="T"></typeparam>
        /// <param name="childName"></param>
        /// <returns></returns>
        public T? FindChild<T>(string childName)
            where T : class, IGameObject
        {
            if (string.IsNullOrWhiteSpace(childName))
            {
                return null;
            }

            Transform childTransform = _transform.Find(childName);
            if (childTransform == null)
            {
                return null;
            }

            return WrapGameObject(childTransform.gameObject) as T;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="child"></param>
        public void AddChild(IGameObject child)
        {
            if (child == null)
            {
                throw new ArgumentNullException(nameof(child));
            }

            if (!TryGetBackingGameObject(child, out GameObject? childGameObject))
            {
                throw new InvalidCastException();
            }

            childGameObject.transform.SetParent(_transform, false);
            RefreshChildren();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="child"></param>
        public void RemoveChild(IGameObject child)
        {
            if (child == null)
            {
                throw new ArgumentNullException(nameof(child));
            }

            if (!TryGetBackingGameObject(child, out GameObject? childGameObject))
            {
                throw new InvalidCastException();
            }

            if (childGameObject.transform.parent == _transform)
            {
                childGameObject.transform.SetParent(null, false);
            }

            RefreshChildren();
        }

        /// <summary>
        ///
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
        ///
        /// </summary>
        public void OnShutdown()
        {
            foreach (var component in _components.Values)
            {
                component.OnShutdown();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="delta"></param>
        public void OnUpdate(float delta)
        {
            foreach (var component in _components.Values)
            {
                component.OnUpdate(delta);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="delta"></param>
        public void OnPhysicsUpdate(float delta)
        {
            foreach (var component in _components.Values)
            {
                component.OnPhysicsUpdate(delta);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void RegisterAttachedUnityComponents()
        {
            MonoBehaviour[] behaviours = _gameObject.GetComponents<MonoBehaviour>();
            for (int i = 0; i < behaviours.Length; i++)
            {
                if (behaviours[i] is IComponent component)
                {
                    component.Object = this;
                    _components.TryAdd(behaviours[i].GetType(), component);
                }
            }
        }

        /// <summary>
        /// 
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
        /// 
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        internal static IGameObject WrapGameObject(GameObject gameObject)
        {
            MonoBehaviour[] behaviours = gameObject.GetComponents<MonoBehaviour>();
            for (int i = 0; i < behaviours.Length; i++)
            {
                if (behaviours[i] is IGameObject wrapped)
                {
                    return wrapped;
                }
            }

            return new UnityGameObject(gameObject);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="unityGameObject"></param>
        /// <returns></returns>
        internal static bool TryGetBackingGameObject(IGameObject? gameObject, out GameObject unityGameObject)
        {
            if (gameObject is UnityGameObject wrapper)
            {
                unityGameObject = wrapper.GameObject;
                return true;
            }

            if (gameObject is Component component)
            {
                unityGameObject = component.gameObject;
                return true;
            }

            unityGameObject = null!;
            return false;
        }
    }
}
