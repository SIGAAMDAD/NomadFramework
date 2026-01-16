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
using System.Runtime.InteropServices;

namespace NomadCore.Infrastructure.Collections
{
    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public unsafe struct NativeList<T> : IDisposable where T : unmanaged
    {
        public int Count => _length;
        private int _length = 0;

        public int Capacity => _capacity;
        private int _capacity = 0;

        private readonly int _growGranularity = 2;

        private T* _data;

        /// <summary>
        /// Creates a NativeList
        /// </summary>
        /// <param name="size"></param>
        /// <param name="granularity"></param>
        public NativeList(int size, int granularity = 2)
        {
            _data = (T*)NativeMemory.AlignedAlloc((nuint)(size * Marshal.SizeOf<T>()), 64);
            _length = size;
            _capacity = size;
            _growGranularity = granularity;
        }

        /// <summary>
        ///
        /// </summary>
        public readonly void Dispose()
        {
            if (_data != null)
            {
                NativeMemory.AlignedFree(_data);
            }
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="addCount"></param>
        private void CheckCapacity(int addCount)
        {
            if (_length + addCount >= _capacity)
            {
                _capacity *= _growGranularity;
                void* pBuffer = NativeMemory.AlignedAlloc((nuint)(_capacity * sizeof(T)), 64);
                if (_data != null)
                {
                    NativeMemory.Copy(_data, pBuffer, (nuint)(_length * sizeof(T)));
                    NativeMemory.AlignedFree(_data);
                }
                _data = (T*)pBuffer;
            }
        }
    }
}
