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
using System.Buffers;

namespace Nomad.Core.Util
{
    /// <summary>
    /// A buffer ownership model. This handles the construction and destruction process
    /// of the provided <see cref="Buffer"/>.
    /// </summary>
    public interface IBufferHandle : IDisposable, IEquatable<IBufferHandle>
    {
        /// <summary>
        /// The length in bytes of the owned memory.
        /// </summary>
        long Length { get; }

        /// <summary>
        /// The memory we have ownership over.
        /// </summary>
        byte[] Buffer { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Span<byte> AsSpan();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        Span<byte> AsSpan(long start);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        Span<byte> AsSpan(long start, long length);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Memory<byte> AsMemory();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        Memory<byte> AsMemory(long start);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        Memory<byte> AsMemory(long start, long length);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        MemoryHandle Pin();

        /// <summary>
        /// 
        /// </summary>
        void Clear();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        void Clear(long start, long length);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        ReadOnlySpan<byte> GetSlice(long start, long length);

        /// <summary>
        /// Copies the <paramref name="source"/> buffer into the owned buffer starting at <paramref name="offset"/> to <paramref name="length"/>
        /// </summary>
        /// <param name="source">The source buffer to copy from</param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="dstOffset"></param>
        void CopyFrom(byte[] source, long offset, long length, long dstOffset = 0);

        /// <summary>
        /// Copies the <paramref name="source"/> buffer into the owned buffer starting at <paramref name="offset"/> to <paramref name="length"/>
        /// </summary>
        /// <param name="source">The source buffer to copy from</param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="dstOffset"></param>
        void CopyFrom(ReadOnlySpan<byte> source, long offset, long length, long dstOffset = 0);

        /// <summary>
        /// Copies the <paramref name="source"/> buffer into the owned buffer starting at <paramref name="offset"/> to <paramref name="length"/>
        /// </summary>
        /// <param name="source">The source buffer to copy from</param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="dstOffset"></param>
        void CopyFrom(IBufferHandle source, long offset, long length, long dstOffset = 0);

        /// <summary>
        /// Copies to the <paramref name="dest"/> buffer from the owned buffer starting at <paramref name="offset"/> to <paramref name="length"/>
        /// </summary>
        /// <param name="dest">The source buffer to copy from</param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="srcOffset"></param>
        void CopyTo(byte[] dest, long offset, long length, long srcOffset = 0);

        /// <summary>
        /// Copies to the <paramref name="dest"/> buffer from the owned buffer starting at <paramref name="offset"/> to <paramref name="length"/>
        /// </summary>
        /// <param name="dest">The source buffer to copy from</param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="srcOffset"></param>
        void CopyTo(Span<byte> dest, long offset, long length, long srcOffset = 0);

        /// <summary>
        /// Copies to the <paramref name="dest"/> buffer from the owned buffer starting at <paramref name="offset"/> to <paramref name="length"/>
        /// </summary>
        /// <param name="dest">The source buffer to copy from</param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="srcOffset"></param>
        void CopyTo(IBufferHandle dest, long offset, long length, long srcOffset = 0);
    }
}