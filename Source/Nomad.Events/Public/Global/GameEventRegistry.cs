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

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Nomad.Core.Events;
using Nomad.Core.Exceptions;
using Nomad.Core.Compatibility.Guards;

namespace Nomad.Events.Global
{
    /// <summary>
    /// 
    /// </summary>
    public static class GameEventRegistry
    {
        private static IGameEventRegistryService Instance => _instance ?? throw new SubsystemNotInitializedException();
        private static IGameEventRegistryService? _instance = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        internal static void Initialize(IGameEventRegistryService instance)
        {
            ArgumentGuard.ThrowIfNull(instance);
            _instance = instance;
        }

        /// <summary>
        /// Creates a new event with the specified name, namespace, and flags.
        /// </summary>
        /// <typeparam name="TArgs">The struct type containing event data.</typeparam>
        /// <param name="name">The name of the event.</param>
        /// <param name="nameSpace">The namespace the event belongs to.</param>
        /// <param name="flags">Optional flags that control event behavior.</param>
        /// <returns>A new <see cref="IGameEvent{TArgs}"/> instance.</returns>
        /// <exception cref="InvalidEventRegistrationException">Thrown if an event with the same name and namespace already exists.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IGameEvent<TArgs> GetEvent<TArgs>(string name, string nameSpace, EventFlags flags = EventFlags.Default)
            where TArgs : struct
        {
            return Instance.GetEvent<TArgs>(name, nameSpace, flags);
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetEvent<TArgs>(string name, string nameSpace, out IGameEvent<TArgs>? gameEvent)
            where TArgs : struct
        {
            return Instance.TryGetEvent(name, nameSpace, out gameEvent);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TArgs"></typeparam>
        /// <param name="nameSpace"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryRemoveEvent<TArgs>(string nameSpace, string name)
            where TArgs : struct
        {
            return Instance.TryRemoveEvent<TArgs>(nameSpace, name);
        }

        /// <summary>
        /// Removes and disposes all events within the specified namespace.
        /// </summary>
        /// <param name="nameSpace">The namespace to clear.</param>
        /// <remarks>
        /// This method is useful for unloading scenes, modules, or systems that
        /// register events under a shared namespace.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ClearEventsInNamespace(string nameSpace)
        {
            Instance.ClearEventsInNamespace(nameSpace);
        }

        /// <summary>
        /// Removes and disposes all registered events.
        /// </summary>
        /// <remarks>
        /// This method is typically used during shutdown or when resetting the
        /// event system entirely.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ClearAllEvents()
        {
            Instance.ClearAllEvents();
        }

        /// <summary>
        /// Gets a read‑only collection of all registered events.
        /// </summary>
        /// <remarks>
        /// This collection is intended for debugging, tooling, and inspection.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IReadOnlyCollection<IGameEvent> GetAllEvents()
        {
            return Instance.GetAllEvents();
        }
    }
}
