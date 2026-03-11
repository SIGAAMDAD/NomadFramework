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
using Nomad.Core.Compatibility.Guards;
using Nomad.Core.EngineUtils;
using Nomad.Core.EngineUtils.Globals;
using Nomad.Core.Util;
using Nomad.EngineUtils.UserInterface;
using Nomad.ResourceCache;

namespace Nomad.EngineUtils
{
    /// <summary>
    ///
    /// </summary>
    public sealed class GodotSceneManager : ISceneManager
    {
        /// <summary>
        ///
        /// </summary>
        public IScene? ActiveScene => _activeScene;
        private GodotScene? _activeScene;

        /// <summary>
        ///
        /// </summary>
        public IReadOnlyList<IScene> LoadedScenes => _additiveScenes;
        private readonly List<GodotScene> _additiveScenes = new();

        private readonly IResourceCacheService<PackedScene, string> _sceneCache;
        private readonly SceneTree _sceneTree;

        private readonly Dictionary<Node, GodotGameObject> _gameObjects = new();

        private bool _isDisposed = false;

        /// <summary>
        ///
        /// </summary>
        /// <param name="sceneTree"></param>
        /// <param name="sceneCache"></param>
        public GodotSceneManager(SceneTree sceneTree, IResourceCacheService<PackedScene, string> sceneCache)
        {
            _sceneTree = sceneTree;
            _sceneCache = sceneCache;

            SceneManager.Initialize(this);
        }

        /// <summary>
        ///
        /// </summary>
        public void Dispose()
        {
            if (!_isDisposed)
            {
                for (int i = 0; i < _additiveScenes.Count; i++)
                {
                    _sceneCache?.Unload(_additiveScenes[i].Path);
                }
                if (_activeScene != null)
                {
                    _sceneCache?.Unload(_activeScene.Path);
                }
                _sceneCache?.Dispose();
            }
            GC.SuppressFinalize(this);
            _isDisposed = true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="path"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public IScene LoadScene(string path, LoadSceneMode mode = LoadSceneMode.Single)
        {
            _sceneCache.GetCached(path).Get(out var resource);
            var scene = WalkSceneTree(resource);
            switch (mode)
            {
                case LoadSceneMode.Additive:
                    _sceneTree.Root.AddChild(scene.Root.Node);
                    _additiveScenes.Add(scene);
                    break;
                case LoadSceneMode.Single:
                    SetActiveScene(scene);
                    break;
            }
            return scene;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="scene"></param>
        /// <exception cref="InvalidCastException"></exception>
        public void SetActiveScene(IScene scene)
        {
            ArgumentGuard.ThrowIfNull(scene);
            if (scene is not GodotScene godotScene)
            {
                throw new InvalidCastException();
            }

            // clear any existing scenes
            UnloadAllScenes();
            _sceneTree.CallDeferred(SceneTree.MethodName.ChangeSceneToNode, godotScene.Root.Node);
            _activeScene = godotScene;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="scene"></param>
        public void UnloadScene(IScene scene)
        {
            if (scene is not GodotScene godotScene)
            {
                throw new InvalidCastException();
            }
            if (_additiveScenes.Contains(godotScene))
            {
                _sceneTree.Root.CallDeferred(Node.MethodName.RemoveChild, godotScene.Root.Node);
            }
            _additiveScenes.Remove(godotScene);
            _sceneCache.Unload(godotScene.Path);
        }

        /// <summary>
        ///
        /// </summary>
        public void UnloadAllScenes()
        {
            UnloadCurrentScene();
            for (int i = 0; i < _additiveScenes.Count; i++)
            {
                _sceneTree.Root.CallDeferred(Node.MethodName.RemoveChild, _additiveScenes[i].Root.Node);
                _sceneCache.Unload(_additiveScenes[i].Path);
            }
            _additiveScenes.Clear();
        }

        /// <summary>
        ///
        /// </summary>
        public void UnloadCurrentScene()
        {
            if (_activeScene == null)
            {
                return;
            }
            _sceneTree.CurrentScene = null;

            _sceneTree.Root.CallDeferred(Node.MethodName.RemoveChild, _activeScene.Root.Node);
            _sceneCache.Unload(_activeScene.Path);
            _activeScene = null;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="resource"></param>
        /// <returns></returns>
        private GodotScene WalkSceneTree(PackedScene resource)
        {
            var root = new GodotGameObject(resource.Instantiate());
            _gameObjects[root.Node] = root;
            WalkNode(root.Node);
            return new GodotScene(root);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="parentObject"></param>
        private void WalkNode(Node parentObject)
        {
            var children = parentObject.GetChildren();
            for (int i = 0; i < children.Count; i++)
            {
                var childNode = children[i];
                _gameObjects[childNode] = new GodotGameObject(childNode);
                WalkNode(childNode);
            }
        }
    }
}
