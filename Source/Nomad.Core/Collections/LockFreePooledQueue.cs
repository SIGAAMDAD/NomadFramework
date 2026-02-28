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

using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Nomad.Core.Compatibility.Guards;

namespace Nomad.Core.Collections
{
    /// <summary>
    /// Lock-free queue with object pooling to reduce GC pressure.
    /// This is a C# adaptation of boost::lockfree::queue.
    /// </summary>
    /// <remarks>
    /// WARNING: Node reuse via pooling makes this implementation susceptible to the ABA problem.
    /// In practice, with .NET's memory model and typical usage, the risk is low, but it exists.
    /// For mission‑critical scenarios, consider using <see cref="System.Collections.Concurrent.ConcurrentQueue{T}"/>
    /// or a versioned approach (e.g., packing pointer+version into a long).
    /// </remarks>
    /// <typeparam name="T">Type of elements in the queue.</typeparam>
    public class LockFreePooledQueue<T> : IProducerConsumerCollection<T>
    {
        private class PooledNode
        {
            public T Value;
            public volatile PooledNode Next;  // Marked volatile for thread‑safe reads/writes
        }

        private class NodePool
        {
            private readonly ConcurrentStack<PooledNode> _pool = new();
            private readonly int _maxPoolSize;

            public NodePool(int maxPoolSize)
            {
                _maxPoolSize = maxPoolSize;
            }

            public PooledNode Get(T value)
            {
                if (_pool.TryPop(out var node))
                {
                    node.Value = value;
                    node.Next = null!;
                    return node;
                }
                return new PooledNode { Value = value, Next = null! };
            }

            public void Return(PooledNode node)
            {
                node.Value = default!; // Release reference
                node.Next = null!;
                if (_pool.Count < _maxPoolSize)
                {
                    _pool.Push(node);
                }
            }
        }

        private volatile PooledNode _head;
        private volatile PooledNode _tail;
        private readonly NodePool _pool;
        private int _count; // Volatile access via Interlocked

        /// <summary>
        /// Gets the number of elements contained in the queue.
        /// </summary>
        /// <remarks>This count is approximate and may not reflect concurrent operations.</remarks>
        public int Count => Interlocked.CompareExchange(ref _count, 0, 0);

        /// <summary>
        /// Gets a value indicating whether the queue is empty.
        /// </summary>
        public bool IsEmpty => _head.Next == null;

        /// <summary>
        /// Initializes a new instance of the <see cref="LockFreePooledQueue{T}"/> class.
        /// </summary>
        /// <param name="poolSize">Maximum number of nodes to keep in the pool.</param>
        public LockFreePooledQueue(int poolSize = 1000)
        {
            var dummy = new PooledNode();
            _head = dummy;
            _tail = dummy;
            _pool = new NodePool(poolSize);
        }

        /// <summary>
        /// Tries to add an item to the queue.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns>Always true (unbounded queue).</returns>
        public bool TryAdd(T item) => TryEnqueue(item);

        /// <summary>
        /// Tries to take an item from the queue.
        /// </summary>
        /// <param name="item">When this method returns, contains the item removed, if successful.</param>
        /// <returns>true if an item was removed; otherwise, false.</returns>
        public bool TryTake(out T item) => TryDequeue(out item);

        /// <summary>
        /// Tries to enqueue an item.
        /// </summary>
        public bool TryEnqueue(T item)
        {
            var node = _pool.Get(item);
            while (true)
            {
                var oldTail = _tail;
                var oldNext = oldTail.Next;

                if (oldTail != _tail) // Tail changed while we were reading
                {
                    continue;
                }

                if (oldNext == null)
                {
                    // Try to link the new node at the end
                    if (Interlocked.CompareExchange(ref oldTail.Next, node, null) == null)
                    {
                        // Success – try to advance the tail (non‑critical)
                        Interlocked.CompareExchange(ref _tail, node, oldTail);
                        Interlocked.Increment(ref _count);
                        return true;
                    }
                }
                else
                {
                    // Tail is lagging – help advance it
                    Interlocked.CompareExchange(ref _tail, oldNext, oldTail);
                }
            }
        }

        /// <summary>
        /// Tries to dequeue an item.
        /// </summary>
        public bool TryDequeue(out T item)
        {
            while (true)
            {
                var head = _head;
                var tail = _tail;
                var next = head.Next;

                if (head != _head) // Head changed
                {
                    continue;
                }

                if (head == tail)
                {
                    if (next == null)
                    {
                        item = default!;
                        return false;
                    }
                    // Tail is lagging – help advance it
                    Interlocked.CompareExchange(ref _tail, next, tail);
                }
                else
                {
                    item = next!.Value;
                    if (Interlocked.CompareExchange(ref _head, next, head) == head)
                    {
                        Interlocked.Decrement(ref _count);
                        _pool.Return(head); // Return old dummy node to pool
                        return true;
                    }
                }
            }
        }

        /// <summary>
        /// Removes all items from the queue.
        /// </summary>
        /// <remarks>
        /// This operation is NOT thread‑safe with concurrent enqueues/dequeues.
        /// It should only be used when no other threads are accessing the queue.
        /// </remarks>
        public void Clear()
        {
            // Drain the queue by dequeuing all items
            while (TryDequeue(out _))
            {
            }
        }

        /// <summary>
        /// Copies the queue elements to an array.
        /// </summary>
        public T[] ToArray()
        {
            var list = new List<T>();
            var current = _head.Next;
            while (current != null)
            {
                list.Add(current.Value);
                current = current.Next;
            }
            return list.ToArray();
        }

        /// <summary>
        /// Copies the queue elements to an array starting at the specified index.
        /// </summary>
        public void CopyTo(T[] array, int index)
        {
            ArgumentGuard.ThrowIfNull(array);
            if (index < 0 || index >= array.Length)
            {
                throw new System.ArgumentOutOfRangeException(nameof(index));
            }

            var current = _head.Next;
            while (current != null && index < array.Length)
            {
                array[index++] = current.Value;
                current = current.Next;
            }
        }

        /// <summary>
        /// Copies the queue elements to an array.
        /// </summary>
        public void CopyTo(System.Array array, int index)
        {
            ArgumentGuard.ThrowIfNull(array);

            var typedArray = array as T[];
            ArgumentGuard.ThrowIfNull(typedArray);

            CopyTo(typedArray, index);
        }

        /// <summary>
        /// Gets an enumerator for the queue.
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            var current = _head.Next;
            while (current != null)
            {
                yield return current.Value;
                current = current.Next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Synchronizes the queue (not supported, always returns false).
        /// </summary>
        public object SyncRoot => throw new System.NotSupportedException();

        /// <summary>
        /// Indicates whether access to the queue is synchronized (always false).
        /// </summary>
        public bool IsSynchronized => false;
    }
}
