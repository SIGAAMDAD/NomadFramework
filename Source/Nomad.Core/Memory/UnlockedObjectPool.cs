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
using System.Collections.Generic;

namespace Nomad.Core.Memory
{
    /// <summary>
    /// A version of the ObjectPool class that doesn't contain any thread synchronization. This is strictly meant for single-threaded optimization.
    /// </summary>
    public sealed class UnlockedObjectPool<T, TFactory> : IObjectPool<T>
        where T : class, IDisposable, new()
    {
        public int TotalCount => _currentSize;
        private int _currentSize = 0;

        public int ActiveObjectCount => _currentSize - _pool.Count;

        private readonly List<T> _pool = new List<T>();

        private readonly int _maxSize = int.MaxValue;
        private bool _isDisposed = false;

        public UnlockedObjectPool(int initialCapacity = 32, int maxCapacity = int.MaxValue)
        {
            _maxSize = maxCapacity;
            _pool = new List<T>(initialCapacity);
        }

        /// <summary>
        ///
        /// </summary>
        public void Dispose()
        {
            for (int i = 0; i < _currentSize; i++)
            {
                _pool[i].Dispose();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public T Rent()
        {
            if (_pool.Count > 0)
            {
                T value = _pool[_pool.Count - 1];
                _pool.RemoveAt(_pool.Count - 1);
                return value;
            }
            return new T();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        public void Return(T value)
        {
            _pool.Add(value);
        }
    };
};
