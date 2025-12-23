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

namespace Nomad.ResourceCache
{
    /// <summary>
    /// Interface for a custom loading procedure, give this to a BaseCache.
    /// </summary>
    /// <typeparam name="TResource"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public interface IResourceLoader<TResource, TId>
        where TResource : IDisposable
    {
        LoadCallback<TResource, TId> Load { get; }
        LoadAsyncCallback<TResource, TId> LoadAsync { get; }
    }
}
