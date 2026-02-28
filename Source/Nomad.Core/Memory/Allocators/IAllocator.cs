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

using Nomad.Core.Memory.Buffers;

namespace Nomad.Core.Memory
{
    /// <summary>
    /// The generic allocator interface.
    /// </summary>
    public interface IAllocator
    {
        /// <summary>
        /// Allocates <paramref name="count"/> objects of type <typeparamref name="T"/> in an array.
        /// </summary>
        /// <typeparam name="T">The type of object to create.</typeparam>
        /// <param name="count">The number of objects to create.</param>
        /// <returns>The newly allocated array.</returns>
        IBufferHandle? AllocateArray<T>(int count);

        /// <summary>
        /// Releases the memory back to the system.
        /// </summary>
        /// <param name="memory">The memory handle to release.</param>
        void Release(IBufferHandle memory);
    }
}
