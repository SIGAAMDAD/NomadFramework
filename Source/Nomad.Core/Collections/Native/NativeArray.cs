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

#if NET6_0_OR_GREATER
using System;
using System.Runtime.InteropServices;

namespace Nomad.Core.Collections.Native
{
    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public unsafe class NativeArray<T> : IDisposable where T : unmanaged
    {
        /// <summary>
        /// 
        /// </summary>
        public int Length => _length;
        private readonly int _length;

        private readonly T* _data;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public ref T this[int index]
        {
            get
            {
#if DEBUG
                if (index < 0 || index >= _length)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
#endif
                return ref _data[index];
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="length"></param>
        public NativeArray(int length)
        {
            _length = length;

            // assume a cacheline of 64
            _data = (T*)NativeMemory.AlignedAlloc((nuint)(_length * Marshal.SizeOf<T>()), 64);
        }

        /// <summary>
        ///
        /// </summary>
        public void Dispose()
        {
            if (_data != null)
            {
                NativeMemory.AlignedFree(_data);
            }
            GC.SuppressFinalize(this);
        }
    }
}
#endif
