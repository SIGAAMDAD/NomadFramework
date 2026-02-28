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
using System.Runtime.CompilerServices;
using Nomad.Core.Compatibility.Guards;

namespace Nomad.Core.Memory.Buffers
{
    /// <summary>
    /// 
    /// </summary>
    public static class BufferCopyExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dest">The buffer to copy into.</param>
        /// <param name="source"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="dstOffset"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyFrom(this IBufferHandle dest, IBufferHandle source, int offset, int length, int dstOffset = 0)
        {
            ArgumentGuard.ThrowIfNull(source);
            dest.CopyFrom(source.Span, offset, length, dstOffset);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dest">The buffer to copy into.</param>
        /// <param name="source"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="dstOffset"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyFrom(this IBufferHandle dest, byte[] source, int offset, int length, int dstOffset = 0)
        {
            ArgumentGuard.ThrowIfNull(source);
            dest.CopyFrom(source.AsSpan(), offset, length, dstOffset);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="srcOffset"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyTo(this IBufferHandle source, IBufferHandle dest, int offset, int length, int srcOffset = 0)
        {
            ArgumentGuard.ThrowIfNull(dest);
            source.CopyTo(dest.Span, offset, length, srcOffset);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="srcOffset"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyTo(this IBufferHandle source, byte[] dest, int offset, int length, int srcOffset = 0)
        {
            ArgumentGuard.ThrowIfNull(dest);
            source.CopyTo(dest.AsSpan(), offset, length, srcOffset);
        }
    }
}