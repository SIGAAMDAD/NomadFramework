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

#if !UNITY_COMPATIBLE
using System;
using System.Threading.Tasks;
using System.Threading;
using Nomad.Core.ResourceCache;
using Nomad.Core.Util;

namespace Nomad.EngineUtils.Private {
    /*
    ===================================================================================
    
    GodotLoader
    
    ===================================================================================
    */
    /// <summary>
    /// A godot-focused resource loader.
    /// </summary>

    public sealed class GodotLoader<Resource> : IResourceLoader<Resource, string>
        where Resource : Godot.Resource
    {
        public LoadCallback<Resource, string> Load => LoadResource;
        public LoadAsyncCallback<Resource, string> LoadAsync => LoadResourceAsync;

        /*
        ===============
        Dispose
        ===============
        */
        /// <summary>
        /// 
        /// </summary>
        public void Dispose() {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private Result<Resource> LoadResource( string path ) {
            Godot.Resource resource = Godot.ResourceLoader.Load( path, String.Empty, Godot.ResourceLoader.CacheMode.ReplaceDeep );
            if ( resource == null ) {
                return Result<Resource>.Failure( Error.Create( $"Error loading godot resource '{path}'" ) );
            } else if ( resource is Resource loadedResource ) {
                return Result<Resource>.Success( loadedResource );
            }
            throw new InvalidCastException();
        }

        /*
        ===============
        LoadResourceAsync
        ===============
        */
        /// <summary>
        ///
        /// </summary>
        /// <param name="path"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        private async Task<Result<Resource>> LoadResourceAsync( string path, CancellationToken ct = default ) {
            ct.ThrowIfCancellationRequested();

            Godot.Error requestError = Godot.ResourceLoader.LoadThreadedRequest( path, String.Empty, true, Godot.ResourceLoader.CacheMode.ReplaceDeep );
            if ( requestError != Godot.Error.Ok ) {
                return Result<Resource>.Failure( Error.Create( $"Error loading godot resource '{path}' - {requestError}" ) );
            }

            Godot.ResourceLoader.ThreadLoadStatus status = Godot.ResourceLoader.ThreadLoadStatus.Failed;
            var sceneTree = ( Godot.SceneTree )Godot.Engine.GetMainLoop();

            do {
                if ( status == Godot.ResourceLoader.ThreadLoadStatus.InProgress ) {
                    await sceneTree.ToSignal( sceneTree, Godot.SceneTree.SignalName.ProcessFrame );
                }
                status = Godot.ResourceLoader.LoadThreadedGetStatus( path );
            } while ( status == Godot.ResourceLoader.ThreadLoadStatus.InProgress );

            if ( status == Godot.ResourceLoader.ThreadLoadStatus.Loaded ) {
                return Result<Resource>.Success( Godot.ResourceLoader.LoadThreadedGet( path ) as Resource );
            }
            return Result<Resource>.Failure( Error.Create( $"godot resource '{path}' failed to load with thread status '{status}" ) );
        }
    };
};
#endif
