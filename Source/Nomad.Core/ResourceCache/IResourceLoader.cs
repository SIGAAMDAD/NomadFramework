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

namespace Nomad.Core.ResourceCache
{
    /// <summary>
    /// The base resource loader type.
    /// </summary>
    public interface IResourceLoader : IDisposable
    {
    }

    /// <summary>
    /// Interface for a custom loading procedure, give this to a BaseCache.
    /// </summary>
    /// <typeparam name="TResource"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public interface IResourceLoader<TResource, TId> : IResourceLoader
    {
        /// <summary>
        /// The default loading callback.
        /// </summary>
        LoadCallback<TResource, TId> Load { get; }

        /// <summary>
        /// The threaded loading callback.
        /// </summary>
        LoadAsyncCallback<TResource, TId> LoadAsync { get; }
    }
}
