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
using System.Runtime.CompilerServices;
using Nomad.Core.Collections;
using Nomad.Core.Events;

namespace Nomad.Events.Private
{
    /// <summary>
    /// 
    /// </summary>
    /// TODO: Finish this up w/ various priorities, sorting, and potentially multithreaded extensions
    public sealed class EventQueue
    {
        /// <summary>
        /// The number of currently queued events.
        /// </summary>
        public int Count
        {
            get
            {
                int count = 0;
                for (int i = 0; i < _queues.Length; i++)
                {
                    count += _queues[i].Count;
                }
                return count;
            }
        }

        private readonly LockFreePooledQueue<QueuedEvent>[] _queues;

        /// <summary>
        /// Creates an EventQueue object.
        /// </summary>
        public EventQueue()
        {
            _queues = new LockFreePooledQueue<QueuedEvent>[(int)EventPriority.Count];
            for (var priority = EventPriority.Critical; priority >= EventPriority.VeryLow; priority--)
            {
                _queues[(int)priority] = new LockFreePooledQueue<QueuedEvent>();
            }
        }

        /// <summary>
        /// Enqueues an event to be published later.
        /// </summary>
        /// <typeparam name="TArgs">The event argument type (must be a struct).</typeparam>
        /// <param name="gameEvent">The event to publish</param>
        /// <param name="args"></param>
        /// <param name="priority"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Enqueue<TArgs>(IGameEvent<TArgs> gameEvent, TArgs args, EventPriority priority = EventPriority.Normal)
            where TArgs : struct
        {
            _queues[(int)priority].TryEnqueue(new QueuedEvent<TArgs>(gameEvent, args));
        }

        /// <summary>
        /// 
        /// </summary>
        public void ProcessAll()
        {
            for (var priority = EventPriority.Critical; priority >= EventPriority.VeryLow; priority--)
            {
                var queue = _queues[(int)priority];
                while (queue.TryDequeue(out var gameEvent))
                {
                    gameEvent.Process();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool ProcessNext()
        {
            for (var priority = EventPriority.Critical; priority >= EventPriority.VeryLow; priority--)
            {
                var queue = _queues[(int)priority];
                if (queue.TryDequeue(out var queuedEvent))
                {
                    queuedEvent.Process();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            for (var priority = EventPriority.Critical; priority >= EventPriority.VeryLow; priority--)
            {
                _queues[(int)priority].Clear();
            }
        }
    }
}
