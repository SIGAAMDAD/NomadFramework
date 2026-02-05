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

#if !USE_COMPATIBILITY_EXTENSIONS
namespace Nomad.Core.Collections.Native
{
    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public unsafe struct NativeStack<T>
        where T : unmanaged
    {
        public readonly int Capacity => _capacity;
        private readonly int _capacity;

        public readonly int Count => _length;
        private int _length;

        private readonly T* _data;

        public readonly T this[int index]
        {
            get => _data[index];
            set => _data[index] = value;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="buffer"></param>
        public NativeStack(Span<T> buffer)
        {
            fixed (T* ptr = buffer)
            {
                _data = ptr;
            }
            _capacity = buffer.Length;
            _length = 0;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        public void Push(T value)
        {
            _data[_length++] = value;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public T? Pop()
        {
            if (_length == 0)
            {
                return null;
            }

            T value = _data[_length];
            _length--;
            return value;
        }
    }
}
#endif
