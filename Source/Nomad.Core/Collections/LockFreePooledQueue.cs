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

using System.Collections.Concurrent;
using System.Threading;

namespace Nomad.Core.Collections
{
    /// <summary>
    /// High-performance lock-free queue using object pooling
    /// Reduces GC pressure for high-throughput scenarios
    /// </summary>
    public class LockFreePooledQueue<T>
    {
        private class PooledNode
        {
            public T Value;
            public PooledNode Next;
            public int Version; // For ABA protection

            public void Reset(T value)
            {
                Value = value;
                Next = null;
                Version++;
            }
        }
        private class NodePool
        {
            private readonly ConcurrentStack<PooledNode> _pool = new();
            private int _createdCount;
            private readonly int _maxPoolSize;

            public NodePool(int maxPoolSize = 1000)
            {
                _maxPoolSize = maxPoolSize;
            }

            public PooledNode Get(T value)
            {
                if (_pool.TryPop(out var node))
                {
                    node.Reset(value);
                    return node;
                }

                Interlocked.Increment(ref _createdCount);
                return new PooledNode { Value = value, Version = 0 };
            }

            public void Return(PooledNode node)
            {
                if (_pool.Count < _maxPoolSize)
                {
                    node.Value = default; // Release reference
                    _pool.Push(node);
                }
            }
        }

        private volatile PooledNode _head;
        private volatile PooledNode _tail;
        private readonly NodePool _pool;

        /// <summary>
        ///
        /// </summary>
        /// <param name="poolSize"></param>
        public LockFreePooledQueue(int poolSize = 1000)
        {
            var dummy = new PooledNode();
            _head = dummy;
            _tail = dummy;
            _pool = new NodePool(poolSize);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool TryEnqueue(in T item)
        {
            var node = _pool.Get(item);

            PooledNode oldTail, oldNext;

            while (true)
            {
                oldTail = _tail;
                oldNext = oldTail.Next;

                if (oldTail == _tail)
                {
                    if (oldNext == null)
                    {
                        if (Interlocked.CompareExchange(ref oldTail.Next, node, null) == null)
                        {
                            Interlocked.CompareExchange(ref _tail, node, oldTail);
                            return true;
                        }
                    }
                    else
                    {
                        Interlocked.CompareExchange(ref _tail, oldNext, oldTail);
                    }
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool TryDequeue(out T item)
        {
            while (true)
            {
                var head = _head;
                var tail = _tail;
                var next = head.Next;

                if (head == _head)
                {
                    if (head == tail)
                    {
                        if (next == null)
                        {
                            item = default;
                            return false;
                        }

                        Interlocked.CompareExchange(ref _tail, next, tail);
                    }
                    else
                    {
                        item = next.Value;

                        if (Interlocked.CompareExchange(ref _head, next, head) == head)
                        {
                            // Return node to pool
                            _pool.Return(head);
                            return true;
                        }
                    }
                }
            }
        }
    };
};
