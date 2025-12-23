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
using System.Collections.Concurrent;

namespace Nomad.Core.Memory
{
    /// <summary>
    ///
    /// </summary>
    public class BasicObjectPool<T> : IObjectPool<T> where T : new()
    {
        public int AvailableCount => _availableObjects.Count;

        public int TotalCount => _currentSize;
        private int _currentSize = 0;

        public int ActiveObjectCount => _currentSize - _availableObjects.Count;

        private readonly ConcurrentBag<T> _availableObjects = new ConcurrentBag<T>();
        private readonly Func<T> _createObject;

        private readonly int _maxSize = 0;
        private bool _isDisposed = false;

        /// <summary>
        ///
        /// </summary>
        /// <param name="createObject"></param>
        /// <param name="initialSize"></param>
        /// <param name="maxSize"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public BasicObjectPool(Func<T> createObject, int initialSize = 32, int maxSize = int.MaxValue)
        {
            _availableObjects = new ConcurrentBag<T>();
            _createObject = createObject ?? throw new ArgumentNullException(nameof(createObject));
            _maxSize = maxSize;

            InitializePool(initialSize);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public T Rent()
        {
            ObjectDisposedException.ThrowIf(_isDisposed, this);

            if (_availableObjects.TryTake(out T? obj))
            {
                ArgumentNullException.ThrowIfNull(obj);
                return obj;
            }

            if (_currentSize < _maxSize)
            {
                _currentSize++;
                return _createObject.Invoke();
            }
            throw new InvalidOperationException("Object pool exhausted and maximum size reached");
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        public void Return(T obj)
        {
            if (_isDisposed)
            {
                if (obj is IDisposable disposable)
                {
                    disposable.Dispose();
                }
                return;
            }

            ArgumentNullException.ThrowIfNull(obj);

            if (_availableObjects.Count < _maxSize)
            {
                _availableObjects.Add(obj);
            }
            else
            {
                if (obj is IDisposable disposable)
                {
                    disposable.Dispose();
                }
                _currentSize--;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
            while (_availableObjects.TryTake(out T? obj))
            {
                ArgumentNullException.ThrowIfNull(obj);
                if (obj is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
            _currentSize = 0;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="initialSize"></param>
        private void InitializePool(int initialSize)
        {
            for (int i = 0; i < initialSize && _currentSize < _maxSize; i++)
            {
                _availableObjects.Add(_createObject.Invoke());
                _currentSize++;
            }
        }
    };
};
