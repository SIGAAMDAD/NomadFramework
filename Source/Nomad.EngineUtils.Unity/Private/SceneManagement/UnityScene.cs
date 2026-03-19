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
using Nomad.Core.Engine.SceneManagement;
using Nomad.Core.Scene.GameObjects;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Nomad.EngineUtils.Private.SceneManagement {
    /*
    ===================================================================================

    UnityScene

    ===================================================================================
    */
    /// <summary>
    ///
    /// </summary>
    
    internal sealed class UnityScene : IScene {
        /// <summary>
        ///
        /// </summary>
        public string Name {
            get => _root.Name;
            set => _root.Name = value;
        }

        /// <summary>
        ///
        /// </summary>
        public IGameObject Root => _root;
        private readonly UnitySceneRoot _root;

        internal UnityEngine.SceneManagement.Scene Scene => _scene;
        private readonly UnityEngine.SceneManagement.Scene _scene;

        internal string Path => _path;
        private readonly string _path;

        private bool _isDisposed;

        /// <summary>
        ///
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="path"></param>
        public UnityScene( UnityEngine.SceneManagement.Scene scene, string path ) {
            _scene = scene;
            _path = path;
            _root = new UnitySceneRoot( scene );
        }

        /// <summary>
        ///
        /// </summary>
        public void Dispose() {
            if ( _isDisposed ) {
                return;
            }

            _root.Dispose();
            _isDisposed = true;
            GC.SuppressFinalize( this );
        }

        /*
        ===================================================================================
        
        UnitySceneRoot
        
        ===================================================================================
        */
        /// <summary>
        /// 
        /// </summary>
        
        private sealed class UnitySceneRoot : IGameObject {
            public string Name { get; set; }

            public bool Enabled {
                get {
                    RefreshChildren();
                    for ( int i = 0; i < _children.Count; i++ ) {
                        if ( _children[i].Enabled ) {
                            return true;
                        }
                    }

                    return false;
                }
                set {
                    RefreshChildren();
                    for ( int i = 0; i < _children.Count; i++ ) {
                        _children[i].Enabled = value;
                    }
                }
            }

            public IGameObject? Parent {
                get => null;
                set {
                }
            }

            public IReadOnlyList<IGameObject> Children {
                get {
                    RefreshChildren();
                    return _children;
                }
            }

            private readonly UnityEngine.SceneManagement.Scene _scene;
            private readonly List<IGameObject> _children = new List<IGameObject>();
            private readonly ConcurrentDictionary<Type, IComponent> _components = new ConcurrentDictionary<Type, IComponent>();

            private bool _isDisposed;

            public UnitySceneRoot( UnityEngine.SceneManagement.Scene scene ) {
                _scene = scene;
                Name = scene.name;
                RefreshChildren();
            }

            public void Dispose() {
                if ( _isDisposed ) {
                    return;
                }

                _components.Clear();
                _children.Clear();

                _isDisposed = true;
                GC.SuppressFinalize( this );
            }

            public T AddComponent<T>( Action<T>? initializer = null )
                where T : IComponent, new() {
                var component = new T {
                    Object = this
                };
                initializer?.Invoke( component );
                component.OnInit();
                _components[typeof( T )] = component;
                return component;
            }

            public T? GetComponent<T>()
                where T : IComponent {
                return _components.TryGetValue( typeof( T ), out var component ) ? (T)component : default;
            }

            public bool HasComponent<T>()
                where T : IComponent {
                return _components.ContainsKey( typeof( T ) );
            }

            public void RemoveComponent<T>()
                where T : IComponent {
                _components.TryRemove( typeof( T ), out _ );
            }

            public T? FindChild<T>( string childName )
                where T : class, IGameObject {
                if ( string.IsNullOrWhiteSpace( childName ) ) {
                    return null;
                }

                RefreshChildren();

                string[] segments = childName.Split( '/' );
                IGameObject? current = null;
                for ( int i = 0; i < _children.Count; i++ ) {
                    if ( _children[i].Name == segments[0] ) {
                        current = _children[i];
                        break;
                    }
                }

                if ( current == null ) {
                    return null;
                }

                if ( segments.Length == 1 ) {
                    return current as T;
                }

                string remainingPath = string.Join( "/", segments, 1, segments.Length - 1 );
                return current.FindChild<T>( remainingPath );
            }

            public void AddChild( IGameObject child ) {
                if ( !UnityGameObject.TryGetBackingGameObject( child, out var childGameObject ) ) {
                    throw new InvalidCastException();
                }

                SceneManager.MoveGameObjectToScene( childGameObject, _scene );
                childGameObject.transform.SetParent( null, false );
                RefreshChildren();
            }

            public void RemoveChild( IGameObject child ) {
                if ( !UnityGameObject.TryGetBackingGameObject( child, out var childGameObject ) ) {
                    throw new InvalidCastException();
                }

                if ( childGameObject.transform.parent != null ) {
                    childGameObject.transform.SetParent( null, false );
                }

                RefreshChildren();
            }

            private void RefreshChildren() {
                _children.Clear();

                GameObject[] roots = _scene.GetRootGameObjects();
                for ( int i = 0; i < roots.Length; i++ ) {
                    _children.Add( UnityGameObject.WrapGameObject( roots[i] ) );
                }
            }
        }
    };
};
