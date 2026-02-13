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
using System.Threading;
using System.Threading.Tasks;

namespace Nomad.Core.Events
{
    /// <summary>
    /// The base game event type.
    /// </summary>
    public interface IGameEvent : IDisposable
    {
        /// <summary>
        /// The event's debugger name.
        /// </summary>
        string DebugName { get; }

        /// <summary>
        /// The event's namespace.
        /// </summary>
        string NameSpace { get; }

        /// <summary>
        /// The event's hash code.
        /// </summary>
        int Id { get; }

        /// <summary>
        ///
        /// </summary>
        void CleanupSubscriptions();
    }

    /// <summary>
    /// The base game event type.
    /// </summary>
    /// <typeparam name="TArgs">The argument structure to use when publishing the event.</typeparam>
    public interface IGameEvent<TArgs> : IGameEvent
        where TArgs : struct
    {
        /// <summary>
        ///
        /// </summary>
        event EventCallback<TArgs> OnPublished;

        /// <summary>
        ///
        /// </summary>
        event AsyncEventCallback<TArgs> OnPublishedAsync;

        /// <summary>
        ///
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task PublishAsync(TArgs eventArgs, CancellationToken ct = default);

        /// <summary>
        ///
        /// </summary>
        /// <param name="eventArgs"></param>
        void Publish(in TArgs eventArgs);

        /// <summary>
        ///
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="asyncCallback"></param>
        ISubscriptionHandle SubscribeAsync(object owner, AsyncEventCallback<TArgs> asyncCallback);

        /// <summary>
        ///
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="callback"></param>
        ISubscriptionHandle Subscribe(object owner, EventCallback<TArgs> callback);

        /// <summary>
        ///
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="asyncCallback"></param>
        void UnsubscribeAsync(object owner, AsyncEventCallback<TArgs> asyncCallback);

        /// <summary>
        ///
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="callback"></param>
        void Unsubscribe(object owner, EventCallback<TArgs> callback);

        /// <summary>
        ///
        /// </summary>
        /// <param name="owner"></param>
        void UnsubscribeAll(object owner);

#if NET7_OR_GREATER
        /// <summary>
        /// Event subscription operator.
        /// </summary>
        /// <param name="callback"></param>
        void operator +=(EventCallback<TArgs> other);

        /// <summary>
        /// Event subscription operator.
        /// </summary>
        /// <param name="callback"></param>
        void operator +=(AsyncEventCallback<TArgs> other);

        /// <summary>
        /// Event subscription operator.
        /// </summary>
        /// <param name="callback"></param>
        void operator -=(EventCallback<TArgs> other);

        /// <summary>
        /// Event subscription operator.
        /// </summary>
        /// <param name="callback"></param>
        void operator -=(AsyncEventCallback<TArgs> other);
#endif
    }
}
