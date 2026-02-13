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
using System.Runtime.CompilerServices;
using Nomad.Core.Events;
using Nomad.Core.Logger;
using Nomad.Core.Util;
using Nomad.Events.Private;

namespace Nomad.Events
{
    /// <summary>
    /// Handles event registration and lookup.
    /// </summary>
    public sealed class GameEventRegistry : IGameEventRegistryService
    {
        private readonly ConcurrentDictionary<EventKey, IGameEvent> _eventCache = new ConcurrentDictionary<EventKey, IGameEvent>();
        private readonly ILoggerService _logger;

        /// <summary>
        ///
        /// </summary>
        /// <param name="logger"></param>
        public GameEventRegistry(ILoggerService logger)
        {
            _logger = logger;
        }

        /// <summary>
        ///
        /// </summary>
        public void Dispose()
        {
            foreach (KeyValuePair<EventKey, IGameEvent> @event in _eventCache)
            {
                @event.Value.Dispose();
            }
            _eventCache.Clear();
        }

        /// <summary>
        /// Releases all events in the naming space of <paramref name="nameSpace"/>.
        /// </summary>
        /// <param name="nameSpace"></param>
        public void ClearEventsInNamespace(string nameSpace)
        {
            InternString nameSpaceCmp = new InternString(nameSpace);
            foreach (KeyValuePair<EventKey, IGameEvent> @event in _eventCache)
            {
                if (@event.Value.NameSpace == nameSpaceCmp)
                {
                    @event.Value.Dispose();
                    _eventCache.Remove(@event.Key, out _);
                }
            }
        }

        /// <summary>
        /// Registers a <see cref="IGameEvent"/> with argument parameters <typeparamref name="TArgs"/> and id <paramref name="name"/>.
        /// </summary>
        /// <typeparam name="TArgs">The type of <see cref="IEventArgs"/> used with the event.</typeparam>
        /// <param name="nameSpace"></param>
        /// <param name="name">Name of the event to register.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IGameEvent<TArgs> GetEvent<TArgs>(string nameSpace, string name, EventFlags flags = EventFlags.Default)
            where TArgs : struct
        {
            var key = new EventKey(
                name: new InternString(name),
                nameSpace: new InternString(nameSpace),
                argsType: typeof(TArgs)
            );

            if (_eventCache.TryGetValue(key, out IGameEvent? value))
            {
                if (value is IGameEvent<TArgs> typedEvent)
                {
                    return typedEvent;
                }
                throw new InvalidOperationException(
                    $"Event '{key.Name}' already registered with type {value.GetType().GenericTypeArguments[0].Name} cannot register with type {typeof(TArgs).Name}"
                );
            }

            value = new GameEvent<TArgs>(key.NameSpace, key.Name, _logger, flags);
            _eventCache.TryAdd(key, value);
            return (IGameEvent<TArgs>)value;
        }

        /// <summary>
        /// Attempts to retrieve an existing event by name and namespace.
        /// </summary>
        /// <typeparam name="TArgs">The struct type containing event data.</typeparam>
        /// <param name="name">The name of the event.</param>
        /// <param name="nameSpace">The namespace the event belongs to.</param>
        /// <param name="gameEvent">
        /// When this method returns, contains the event if found; otherwise <c>null</c>.
        /// </param>
        /// <returns><c>true</c> if the event exists; otherwise <c>false</c>.</returns>
        public bool TryGetEvent<TArgs>(string name, string nameSpace, out IGameEvent<TArgs>? gameEvent)
            where TArgs : struct
        {
            var key = new EventKey(
                name: new InternString(name),
                nameSpace: new InternString(nameSpace),
                argsType: typeof(TArgs)
            );
            if (_eventCache.TryGetValue(key, out IGameEvent? @event))
            {
                if (@event is IGameEvent<TArgs> typedEvent)
                {
                    gameEvent = typedEvent;
                    return true;
                }
            }
            gameEvent = null;
            return false;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TArgs"></typeparam>
        /// <param name="nameSpace"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryRemoveEvent<TArgs>(string nameSpace, string name)
            where TArgs : struct
        {
            var key = new EventKey(
                name: new InternString(name),
                nameSpace: new InternString(nameSpace),
                argsType: typeof(TArgs)
            );
            if (_eventCache.TryRemove(key, out IGameEvent? eventObj))
            {
                (eventObj as IDisposable)?.Dispose();
                return true;
            }
            return false;
        }

        /// <summary>
        ///
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void CleanupAllEvents()
        {
            foreach (var @event in _eventCache)
            {
                @event.Value.CleanupSubscriptions();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="nameSpace"></param>
        public void CleanupNamespace(string nameSpace)
        {
            foreach (var @event in _eventCache)
            {
                if (@event.Value.NameSpace == nameSpace)
                {
                    @event.Value.CleanupSubscriptions();
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        public void ClearAllEvents()
        {
            foreach (var @event in _eventCache)
            {
                @event.Value.Dispose();
            }
            _eventCache.Clear();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public IReadOnlyCollection<IGameEvent> GetAllEvents()
        {
            return (IReadOnlyCollection<IGameEvent>)_eventCache.Values;
        }
    };
};
