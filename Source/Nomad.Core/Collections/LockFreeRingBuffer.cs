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

using System.Threading;

namespace Nomad.Core.Collections
{
    /// <summary>
    /// Fixed-size lock-free ring buffer (circular queue)
    /// Similar to boost::lockfree::spsc_queue
    /// </summary>

    public class LockFreeRingBuffer<T>
        where T : struct
    {
        public int Capacity => _capacity;

        public int Count
        {
            get
            {
                int head = Volatile.Read(ref _head);
                int tail = Volatile.Read(ref _tail);

                if (head >= tail)
                {
                    return head - tail;
                }
                else
                {
                    return _capacity - (tail - head);
                }
            }
        }

        public bool IsEmpty => _head == _tail;
        public bool IsFull => ((_head + 1) & _mask) == _tail;

        private readonly T[] _buffer;
        private volatile int _head; // Write index
        private volatile int _tail; // Read index
        private readonly int _capacity;
        private readonly int _mask; // For power-of-two sizes

        /// <summary>
        ///
        /// </summary>
        /// <param name="capacity"></param>
        public LockFreeRingBuffer(int capacity)
        {
            // Round up to next power of two for efficient wrapping
            capacity = RoundUpToPowerOfTwo(capacity);
            _buffer = new T[capacity];
            _capacity = capacity;
            _mask = capacity - 1;
            _head = 0;
            _tail = 0;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private static int RoundUpToPowerOfTwo(int x)
        {
            x--;
            x |= x >> 1;
            x |= x >> 2;
            x |= x >> 4;
            x |= x >> 8;
            x |= x >> 16;
            return x + 1;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool TryEnqueue(T item)
        {
            int currentHead = _head;
            int nextHead = (currentHead + 1) & _mask;

            // Check if full (tail hasn't caught up)
            if (nextHead == _tail)
                return false;

            _buffer[currentHead] = item;

            // Ensure item is written before head is updated
            Thread.MemoryBarrier();
            _head = nextHead;
            return true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool TryDequeue(out T item)
        {
            int currentTail = _tail;

            // Check if empty
            if (currentTail == _head)
            {
                item = default;
                return false;
            }

            item = _buffer[currentTail];
            int nextTail = (currentTail + 1) & _mask;

            // Ensure item is read before tail is updated
            Thread.MemoryBarrier();
            _tail = nextTail;
            return true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool TryPeek(out T item)
        {
            int currentTail = _tail;

            if (currentTail == _head)
            {
                item = default;
                return false;
            }

            item = _buffer[currentTail];
            return true;
        }
    }
}
