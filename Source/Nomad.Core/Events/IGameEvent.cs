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
    /// Represents the base type for all game events.
    /// </summary>
    /// <remarks>
    /// Provides identifying metadata and manual subscription cleanup.
    /// Typically inherited by generic game events that carry an event payload.
    /// </remarks>
    public interface IGameEvent : IDisposable
    {
        /// <summary>
        /// Gets the debugger-friendly display name for this event.
        /// </summary>
        string DebugName { get; }

        /// <summary>
        /// Gets the logical namespace/category associated with this event.
        /// </summary>
        string NameSpace { get; }

        /// <summary>
        /// Gets the unique identifier or hash code for this event.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Removes all active subscriptions.
        /// </summary>
        /// <remarks>
        /// Useful when resetting systems or shutting down the event entirely.
        /// </remarks>
        void CleanupSubscriptions();
    }

    /// <summary>
    /// Represents a typed game event capable of publishing payload data to subscribers.
    /// </summary>
    /// <typeparam name="TArgs">The struct payload type used when publishing the event.</typeparam>
    /// <remarks>
    /// Subscriptions are tied to an owner object. Subscriptions automatically clean
    /// themselves up when the owner goes out of scope or is destroyed.
    /// </remarks>
    /// # Examples
    /// <example>
    /// ## Godot
    /// Here's an example of how to subscribe to an IGameEvent in Godot:
    /// \include Nomad.Events/Godot/Player_Subscribe.cs
    /// </example>
    /// <example>
    /// ## Unity
    /// ### Declaring an event variable
    /// \snippet Nomad.Events/Unity/Player_Subscribe.cs declare
    /// ### Retrieving an event
    /// \snippet Nomad.Events/Unity/Player_Subscribe.cs get_event
    /// ### Subscribing to an event
    /// \snippet Nomad.Events/Unity/Player_Subscribe.cs subscribe
    /// ### Publishing an event
    /// \snippet Nomad.Events/Unity/Enemy_Publish.cs publish
    /// This will allow you to send messages between scripts without ever directly calling into them.
    /// </example>
    /// 
    /// @module Nomad.Events
    /// @stability Stable
    /// @since 1.0.0
    /// @version 1.0.0
    public interface IGameEvent<TArgs> : IGameEvent
        where TArgs : struct
    {
        /// <summary>
        /// Occurs when <see cref="Publish"/> is invoked.
        /// </summary>
        /// <remarks>
        /// This callback is executed synchronously on the calling thread.
        /// </remarks>
        event EventCallback<TArgs> OnPublished;

        /// <summary>
        /// Occurs when <see cref="PublishAsync"/> is invoked.
        /// </summary>
        /// <remarks>
        /// This callback is executed asynchronously on a background thread or worker.
        /// </remarks>
        event AsyncEventCallback<TArgs> OnPublishedAsync;

        /// <summary>
        /// Publishes this event asynchronously.
        /// </summary>
        /// <param name="eventArgs">The payload to provide to all subscribers.</param>
        /// <param name="ct">An optional cancellation token.</param>
        /// <returns>A task that completes once all asynchronous subscribers finish.</returns>
        Task PublishAsync(TArgs eventArgs, CancellationToken ct = default);

        /// <summary>
        /// Publishes this event synchronously on the calling thread.
        /// </summary>
        /// <param name="eventArgs">The payload to provide to all subscribers.</param>
        void Publish(in TArgs eventArgs);

        /// <summary>
        /// Subscribes an asynchronous callback to this event.
        /// </summary>
        /// <param name="owner">The object that owns this subscription.</param>
        /// <param name="asyncCallback">The callback invoked when the event is published asynchronously.</param>
        /// <returns>A subscription handle that can be used to manage the subscription.</returns>
        ISubscriptionHandle SubscribeAsync(object owner, AsyncEventCallback<TArgs> asyncCallback);

        /// <summary>
        /// Subscribes a synchronous callback to this event.
        /// </summary>
        /// <param name="owner">The object that owns this subscription.</param>
        /// <param name="callback">The callback invoked when the event is published synchronously.</param>
        /// <returns>A subscription handle that can be used to manage the subscription.</returns>
        ISubscriptionHandle Subscribe(object owner, EventCallback<TArgs> callback);

        /// <summary>
        /// Unsubscribes a specific asynchronous callback for the specified owner.
        /// </summary>
        /// <param name="owner">The subscription owner.</param>
        /// <param name="asyncCallback">The asynchronous callback to remove.</param>
        void UnsubscribeAsync(object owner, AsyncEventCallback<TArgs> asyncCallback);

        /// <summary>
        /// Unsubscribes a specific synchronous callback for the specified owner.
        /// </summary>
        /// <param name="owner">The subscription owner.</param>
        /// <param name="callback">The callback to remove.</param>
        void Unsubscribe(object owner, EventCallback<TArgs> callback);

        /// <summary>
        /// Unsubscribes all callbacks associated with the specified owner.
        /// </summary>
        /// <param name="owner">The owner whose subscriptions should be removed.</param>
        void UnsubscribeAll(object owner);

#if NET7_OR_GREATER
        /// <summary>
        /// Adds a synchronous callback via operator syntax.
        /// </summary>
        /// <param name="other">The callback to add.</param>
        void operator +=(EventCallback<TArgs> other);

        /// <summary>
        /// Adds an asynchronous callback via operator syntax.
        /// </summary>
        /// <param name="other">The callback to add.</param>
        void operator +=(AsyncEventCallback<TArgs> other);

        /// <summary>
        /// Removes a synchronous callback via operator syntax.
        /// </summary>
        /// <param name="other">The callback to remove.</param>
        void operator -=(EventCallback<TArgs> other);

        /// <summary>
        /// Removes an asynchronous callback via operator syntax.
        /// </summary>
        /// <param name="other">The callback to remove.</param>
        void operator -=(AsyncEventCallback<TArgs> other);
#endif
    }
}
