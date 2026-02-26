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
using System.Runtime.CompilerServices;
using Nomad.Core.Compatibility.Guards;

namespace Nomad.Core.Util
{
    /// <summary>
    /// The base buffer owner class.
    /// </summary>
    public abstract class BufferBase : IBufferHandle
    {
        /// <summary>
        /// The size of the owned buffer. This might not be the actual allocated size, but it is the maximum bytes we're allowed to work with.
        /// </summary>
        public long Length => length;
        protected readonly long length;

        /// <summary>
        /// The owned chunk of memory.
        /// </summary>
        public byte[] Buffer => buffer;
        protected readonly byte[] buffer;

        /// <summary>
        /// Whether the buffer owner has been killed.
        /// </summary>
        protected bool isDisposed = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="length"></param>
        protected BufferBase(byte[] buffer, long length)
        {
            this.buffer = buffer;
            this.length = length;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed)
            {
                return;
            }
            isDisposed = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<byte> AsSpan()
            => buffer.AsSpan();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<byte> AsSpan(long start)
            => buffer.AsSpan((int)start);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<byte> AsSpan(long start, long length)
            => buffer.AsSpan((int)start, (int)length);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Memory<byte> AsMemory()
            => buffer.AsMemory();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Memory<byte> AsMemory(long start)
            => buffer.AsMemory((int)start);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Memory<byte> AsMemory(long start, long length)
            => buffer.AsMemory((int)start, (int)length);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MemoryHandle Pin()
            => buffer.AsMemory().Pin();

        /// <summary>
        /// 
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
            => Array.Clear(buffer, 0, (int)length);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear(long start, long length)
            => Array.Clear(buffer, (int)start, (int)length);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IBufferHandle? other)
        {
            ArgumentGuard.ThrowIfNull(other);
            if (length != other.Length)
            {
                return false;
            }
            var span1 = new ReadOnlySpan<byte>(buffer, 0, (int)length);
            return span1.SequenceEqual(other.AsSpan(0, length));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<byte> GetSlice(long start, long length)
            => buffer.AsSpan().Slice((int)start, (int)length);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="dstOffset"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyFrom(byte[] source, long offset, long length, long dstOffset = 0)
            => MemCpy(source, offset, buffer, dstOffset, length);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="dstOffset"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyFrom(ReadOnlySpan<byte> source, long offset, long length, long dstOffset = 0)
            => MemCpy(source, offset, buffer, dstOffset, length);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="dstOffset"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyFrom(IBufferHandle source, long offset, long length, long dstOffset = 0)
        {
            ArgumentGuard.ThrowIfNull(source);
            MemCpy(source.AsSpan(), offset, buffer, dstOffset, length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dest"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="srcOffset"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(byte[] dest, long offset, long length, long srcOffset = 0)
            => MemCpy(buffer, srcOffset, dest, offset, length);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dest"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="srcOffset"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(Span<byte> dest, long offset, long length, long srcOffset = 0)
            => MemCpy(buffer, srcOffset, dest, offset, length);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dest"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="srcOffset"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(IBufferHandle dest, long offset, long length, long srcOffset = 0)
        {
            ArgumentGuard.ThrowIfNull(dest);
            MemCpy(buffer, srcOffset, dest.AsSpan(), offset, length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="src"></param>
        /// <param name="srcOffset"></param>
        /// <param name="dst"></param>
        /// <param name="dstOffset"></param>
        /// <param name="length"></param>
        private unsafe void MemCpy(ReadOnlySpan<byte> src, long srcOffset, Span<byte> dst, long dstOffset, long length)
        {
            long destSize = dst.Length - dstOffset;
            fixed (byte* srcPtr = &src[(int)srcOffset])
            fixed (byte* dstPtr = &dst[(int)dstOffset])
            {
                System.Buffer.MemoryCopy(srcPtr, dstPtr, destSize, length);
            }
        }
    }
}