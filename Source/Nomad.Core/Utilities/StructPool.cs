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
using Nomad.Core.Compatibility;

namespace Nomad.Core.Utilities
{
    /*
	===================================================================================

	StructPool

	===================================================================================
	*/
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// <param name="createObject"></param>
    /// <param name="initialSize"></param>
    /// <param name="maxSize"></param>

    public sealed class StructPool<T> : IDisposable where T : struct
    {
        public int AvailableCount => AvailableObjects.Count;

        public int TotalCount => _currentSize;
        private int _currentSize;

        public int ActiveObjectCount => _currentSize - AvailableObjects.Count;

        private readonly ConcurrentBag<T> AvailableObjects = new ConcurrentBag<T>();
        private readonly Func<T> CreateObject;

        private readonly int MaxSize;
        private bool IsDisposed;

        public StructPool(Func<T> createObject, int initialSize = 32, int maxSize = int.MaxValue)
        {
            AvailableObjects = new ConcurrentBag<T>();
            CreateObject = createObject ?? throw new ArgumentNullException(nameof(createObject));
            MaxSize = maxSize;

            InitializePool(initialSize);
        }

        /*
		===============
		Rent
		===============
		*/
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public T Rent()
        {
            ExceptionCompat.ThrowIfDisposed(IsDisposed, this);

            if (AvailableObjects.TryTake(out T obj))
            {
                return obj;
            }

            if (_currentSize < MaxSize)
            {
                _currentSize++;
                return CreateObject.Invoke();
            }
            throw new InvalidOperationException("Object pool exhausted and maximum size reached");
        }

        /*
		===============
		Return
		===============
		*/
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        public void Return(in T obj)
        {
            if (IsDisposed)
            {
                return;
            }

            if (AvailableObjects.Count < MaxSize)
            {
                AvailableObjects.Add(obj);
            }
            else
            {
                _currentSize--;
            }
        }

        /*
		===============
		Dispose
		===============
		*/
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            IsDisposed = true;
            AvailableObjects.Clear();
            _currentSize = 0;
        }

        /*
		===============
		InitializePool
		===============
		*/
        /// <summary>
        /// 
        /// </summary>
        /// <param name="initialSize"></param>
        private void InitializePool(int initialSize)
        {
            for (int i = 0; i < initialSize && _currentSize < MaxSize; i++)
            {
                AvailableObjects.Add(CreateObject.Invoke());
                _currentSize++;
            }
        }
    };
};