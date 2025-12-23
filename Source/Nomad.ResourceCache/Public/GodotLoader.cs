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
using System.Threading.Tasks;
using System.Threading;
using Nomad.Core.Util;
using Nomad.ResourceCache.Private.Errors;

namespace Nomad.ResourceCache
{
    /// <summary>
    /// A godot-focused resource loader.
    /// </summary>

    public sealed class GodotLoader<Resource> : IResourceLoader<Resource, FilePath>
        where Resource : Godot.Resource
    {
        public LoadCallback<Resource, FilePath> Load => LoadResource;
        public LoadAsyncCallback<Resource, FilePath> LoadAsync => LoadResourceAsync;

        /// <summary>
        ///
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private Result<Resource> LoadResource(FilePath path)
        {
            Godot.Resource resource = Godot.ResourceLoader.Load(path.GodotPath, String.Empty, Godot.ResourceLoader.CacheMode.ReplaceDeep);
            if (resource == null)
            {
                return Result<Resource>.Failure(LoadError.Create($"Error loading godot resource '{path.GodotPath}'"));
            }
            return Result<Resource>.Success(resource as Resource);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="path"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        private async Task<Result<Resource>> LoadResourceAsync(FilePath path, CancellationToken ct = default)
        {
            Godot.Error requestError = Godot.ResourceLoader.LoadThreadedRequest(path.GodotPath, String.Empty, true, Godot.ResourceLoader.CacheMode.ReplaceDeep);
            if (requestError != Godot.Error.Ok)
            {
                return Result<Resource>.Failure(LoadError.Create($"Error loading godot resource '{path.GodotPath}' - {requestError}"));
            }

            Godot.ResourceLoader.ThreadLoadStatus status = Godot.ResourceLoader.ThreadLoadStatus.Failed;
            var sceneTree = (Godot.SceneTree)Godot.Engine.GetMainLoop();

            do
            {
                if (status == Godot.ResourceLoader.ThreadLoadStatus.InProgress)
                {
                    await sceneTree.ToSignal(sceneTree, Godot.SceneTree.SignalName.ProcessFrame);
                }
                status = Godot.ResourceLoader.LoadThreadedGetStatus(path.GodotPath);
            } while (status == Godot.ResourceLoader.ThreadLoadStatus.InProgress);

            if (status == Godot.ResourceLoader.ThreadLoadStatus.Loaded)
            {
                return Result<Resource>.Success(Godot.ResourceLoader.LoadThreadedGet(path) as Resource);
            }
            return Result<Resource>.Failure(LoadError.Create($"godot resource '{path.GodotPath}' failed to load with thread status '{status}"));
        }
    };
};
