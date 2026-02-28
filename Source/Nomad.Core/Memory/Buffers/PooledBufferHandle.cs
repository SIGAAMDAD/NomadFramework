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

using System.Buffers;

namespace Nomad.Core.Memory.Buffers
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class PooledBufferHandle : BufferBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="length"></param>
        public PooledBufferHandle(int length)
            : base(ArrayPool<byte>.Shared.Rent(length), length)
        {
        }

        /*
        ===============
        Dispose
        ===============
        */
        /// <summary>
        /// 
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (isDisposed)
            {
                return;
            }
            if (disposing)
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
            isDisposed = true;
            base.Dispose(disposing);
        }
    }
}
