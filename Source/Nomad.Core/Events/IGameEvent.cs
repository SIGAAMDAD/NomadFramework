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
using System.Threading;
using System.Threading.Tasks;

namespace Nomad.Core.Events
{
    /// <summary>
    ///
    /// </summary>
    public interface IGameEvent : IDisposable
    {
        string DebugName { get; }
        int Id { get; }
    }

    /// <summary>
    /// The base game event type.
    /// </summary>
    /// <typeparam name="TArgs"></typeparam>
    public interface IGameEvent<TArgs> : IGameEvent
        where TArgs : struct
    {
        event EventCallback<TArgs> OnPublished;
        event AsyncEventCallback<TArgs> OnPublishedAsync;

        IDisposable SubscribeAsync(AsyncEventCallback<TArgs> asyncCallback, object? owner = null);
        IDisposable Subscribe(EventCallback<TArgs> callback, object? owner = null);
        Task PublishAsync(TArgs eventArgs, CancellationToken ct = default);
        void Publish(in TArgs eventArgs);
    }
}
