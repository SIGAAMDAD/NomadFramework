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
using System.Collections.Generic;
using System.IO;
using Nomad.Core.Engine.Globals;
using Nomad.Core.Engine.SceneManagement;
using UScene = UnityEngine.SceneManagement.Scene;
using USceneLoadMode = UnityEngine.SceneManagement.LoadSceneMode;
using USceneManager = UnityEngine.SceneManagement.SceneManager;

namespace Nomad.EngineUtils.Private.SceneManagement {
    /*
    ===================================================================================

    UnitySceneManager

    ===================================================================================
    */
    /// <summary>
    ///
    /// </summary>
    
    internal sealed class UnitySceneManager : ISceneManager {
        /// <summary>
        ///
        /// </summary>
        public IScene? ActiveScene => _activeScene;
        private UnityScene? _activeScene;

        /// <summary>
        ///
        /// </summary>
        public IReadOnlyList<IScene> LoadedScenes => _additiveScenes;
        private readonly List<UnityScene> _additiveScenes = new List<UnityScene>();

        private readonly Dictionary<int, UnityScene> _sceneCache = new Dictionary<int, UnityScene>();

        private bool _isDisposed = false;

        /*
        ===============
        UnitySceneManager
        ===============
        */
        /// <summary>
        ///
        /// </summary>
        public UnitySceneManager() {
            USceneManager.sceneLoaded += OnSceneLoaded;
            USceneManager.sceneUnloaded += OnSceneUnloaded;
            USceneManager.activeSceneChanged += OnActiveSceneChanged;

            SyncLoadedScenes();

            SceneManager.Initialize( this );
        }

        /*
        ===============
        Dispose
        ===============
        */
        /// <summary>
        ///
        /// </summary>
        public void Dispose() {
            if ( _isDisposed ) {
                return;
            }

            USceneManager.sceneLoaded -= OnSceneLoaded;
            USceneManager.sceneUnloaded -= OnSceneUnloaded;
            USceneManager.activeSceneChanged -= OnActiveSceneChanged;

            foreach ( var scene in _sceneCache.Values ) {
                scene.Dispose();
            }

            _sceneCache.Clear();
            _additiveScenes.Clear();
            _activeScene = null;

            _isDisposed = true;
            GC.SuppressFinalize( this );
        }

        /*
        ===============
        LoadScene
        ===============
        */
        /// <summary>
        ///
        /// </summary>
        /// <param name="path"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public IScene LoadScene( string path, LoadSceneMode mode = LoadSceneMode.Single ) {
            USceneManager.LoadScene( path, mode == LoadSceneMode.Additive ? USceneLoadMode.Additive : USceneLoadMode.Single );
            SyncLoadedScenes();

            UnityScene scene = ResolveScene( path );
            if ( mode == LoadSceneMode.Single ) {
                _activeScene = scene;
            } else if ( !_additiveScenes.Contains( scene ) ) {
                _additiveScenes.Add( scene );
            }

            return scene;
        }
        
        /*
        ===============
        UnloadCurrentScene
        ===============
        */
        /// <summary>
        ///
        /// </summary>
        public void UnloadCurrentScene() {
            if ( _activeScene == null ) {
                return;
            }

            UnityScene scene = _activeScene;
            _activeScene = null;
            _sceneCache.Remove( scene.Scene.handle );
            scene.Dispose();

            USceneManager.UnloadSceneAsync( scene.Scene );
        }

        /*
        ===============
        UnloadAllScenes
        ===============
        */
        /// <summary>
        ///
        /// </summary>
        public void UnloadAllScenes() {
            UnloadCurrentScene();

            for ( int i = _additiveScenes.Count - 1; i >= 0; i-- ) {
                UnloadScene( _additiveScenes[i] );
            }

            _additiveScenes.Clear();
        }

        /*
        ===============
        UnloadScene
        ===============
        */
        /// <summary>
        ///
        /// </summary>
        /// <param name="scene"></param>
        public void UnloadScene( IScene scene ) {
            if ( scene is not UnityScene unityScene ) {
                throw new InvalidCastException();
            }

            _additiveScenes.Remove( unityScene );
            _sceneCache.Remove( unityScene.Scene.handle );
            unityScene.Dispose();

            USceneManager.UnloadSceneAsync( unityScene.Scene );
        }

        /*
        ===============
        SetActiveScene
        ===============
        */
        /// <summary>
        ///
        /// </summary>
        /// <param name="scene"></param>
        public void SetActiveScene( IScene scene ) {
            if ( scene is not UnityScene unityScene ) {
                throw new InvalidCastException();
            }

            USceneManager.SetActiveScene( unityScene.Scene );
            _activeScene = unityScene;
            SyncLoadedScenes();
        }

        /*
        ===============
        ResolveScene
        ===============
        */
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private UnityScene ResolveScene( string path ) {
            string normalizedPath = NormalizeScenePath( path );

            UScene scene = USceneManager.GetSceneByPath( normalizedPath );
            if ( !scene.IsValid() ) {
                scene = USceneManager.GetSceneByName( Path.GetFileNameWithoutExtension( normalizedPath ) );
            }

            return GetOrCreateScene( scene, normalizedPath );
        }

        /*
        ===============
        SyncLoadedScenes
        ===============
        */
        /// <summary>
        /// 
        /// </summary>
        private void SyncLoadedScenes() {
            _additiveScenes.Clear();

            UScene activeScene = USceneManager.GetActiveScene();
            _activeScene = activeScene.IsValid() ? GetOrCreateScene( activeScene, activeScene.path ) : null;

            for ( int i = 0; i < USceneManager.sceneCount; i++ ) {
                UScene scene = USceneManager.GetSceneAt( i );
                if ( !scene.IsValid() ) {
                    continue;
                }

                UnityScene unityScene = GetOrCreateScene( scene, scene.path );
                if ( _activeScene == null || scene.handle != _activeScene.Scene.handle ) {
                    _additiveScenes.Add( unityScene );
                }
            }
        }

        /*
        ===============
        GetOrCreateScene
        ===============
        */
        /// <summary>
        /// 
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private UnityScene GetOrCreateScene( UScene scene, string path ) {
            if ( !scene.IsValid() ) {
                throw new InvalidOperationException( "The requested Unity scene is invalid." );
            }
            if ( _sceneCache.TryGetValue( scene.handle, out var cachedScene ) ) {
                return cachedScene;
            }

            var createdScene = new UnityScene( scene, path );
            _sceneCache[scene.handle] = createdScene;
            return createdScene;
        }

        /*
        ===============
        NormalizeScenePath
        ===============
        */
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string NormalizeScenePath( string path ) {
            return path.Replace( '\\', '/' );
        }

        /*
        ===============
        OnSceneLoaded
        ===============
        */
        /// <summary>
        /// 
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="mode"></param>
        private void OnSceneLoaded( UScene scene, USceneLoadMode mode ) {
            GetOrCreateScene( scene, scene.path );
            SyncLoadedScenes();
        }

        /*
        ===============
        OnSceneUnloaded
        ===============
        */
        /// <summary>
        /// 
        /// </summary>
        /// <param name="scene"></param>
        private void OnSceneUnloaded( UScene scene ) {
            if ( _sceneCache.TryGetValue( scene.handle, out var unityScene ) ) {
                unityScene.Dispose();
                _sceneCache.Remove( scene.handle );
            }
            SyncLoadedScenes();
        }

        /*
        ===============
        OnActiveSceneChanged
        ===============
        */
        /// <summary>
        /// 
        /// </summary>
        /// <param name="previousScene"></param>
        /// <param name="nextScene"></param>
        private void OnActiveSceneChanged( UScene previousScene, UScene nextScene ) {
            if ( nextScene.IsValid() ) {
                _activeScene = GetOrCreateScene( nextScene, nextScene.path );
            } else {
                _activeScene = null;
            }
            SyncLoadedScenes();
        }
    };
};
