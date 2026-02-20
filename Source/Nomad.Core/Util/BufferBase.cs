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
        /// The size of the owned buffer. This might not be the actual allocated size, but it is the maximum bytes we're allowed to work with
        /// </summary>
        public int Length => _length;
        private int _length;

        /// <summary>
        /// The owned chunk of memory.
        /// </summary>
        public byte[] Buffer => _buffer;
        protected byte[] _buffer;

        /// <summary>
        /// Whether the buffer owner has been killed.
        /// </summary>
        protected bool _isDisposed = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="length"></param>
        protected BufferBase(byte[] buffer, int length)
        {
            _buffer = buffer;
            _length = length;
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }
            _isDisposed = true;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<byte> AsSpan()
            => _buffer.AsSpan();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<byte> AsSpan(int start)
            => _buffer.AsSpan(start);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<byte> AsSpan(int start, int length)
            => _buffer.AsSpan(start, length);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Memory<byte> AsMemory()
            => _buffer.AsMemory();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Memory<byte> AsMemory(int start)
            => _buffer.AsMemory(start);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Memory<byte> AsMemory(int start, int length)
            => _buffer.AsMemory(start, length);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public MemoryHandle Pin()
        {
            unsafe
            {
                fixed (void* ptr = _buffer)
                {
                    return new MemoryHandle(ptr);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
            => Array.Clear(_buffer, 0, _length);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear(int start, int length)
            => Array.Clear(_buffer, start, length);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IBufferHandle? other)
        {
            ArgumentGuard.ThrowIfNull(other);
            var span1 = new ReadOnlySpan<byte>(_buffer);
            return span1.SequenceEqual(other.AsSpan());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<byte> GetSlice(int start, int length)
            => _buffer.AsSpan().Slice(start, length);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="dstOffset"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyFrom(byte[] source, int offset, int length, int dstOffset = 0)
            => MemCpy(source, offset, _buffer, dstOffset, length);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="dstOffset"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyFrom(ReadOnlySpan<byte> source, int offset, int length, int dstOffset = 0)
            => MemCpy(source, offset, _buffer, dstOffset, length);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="dstOffset"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyFrom(IBufferHandle source, int offset, int length, int dstOffset = 0)
        {
            ArgumentGuard.ThrowIfNull(source);
            MemCpy(source.AsSpan(), offset, _buffer, dstOffset, length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dest"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="srcOffset"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(byte[] dest, int offset, int length, int srcOffset = 0)
            => MemCpy(_buffer, srcOffset, dest, offset, length);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dest"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="srcOffset"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(Span<byte> dest, int offset, int length, int srcOffset = 0)
            => MemCpy(_buffer, srcOffset, dest, offset, length);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dest"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="srcOffset"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(IBufferHandle dest, int offset, int length, int srcOffset = 0)
        {
            ArgumentGuard.ThrowIfNull(dest);
            MemCpy(_buffer, srcOffset, dest.AsSpan(), offset, length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="src"></param>
        /// <param name="srcOffset"></param>
        /// <param name="dst"></param>
        /// <param name="dstOffset"></param>
        /// <param name="length"></param>
        private unsafe void MemCpy(ReadOnlySpan<byte> src, int srcOffset, Span<byte> dst, int dstOffset, int length)
        {
            fixed (byte* srcPtr = &src[srcOffset])
            fixed (byte* dstPtr = &dst[dstOffset])
            {
                System.Buffer.MemoryCopy(srcPtr, dstPtr, dst.Length, length);
            }
        }
    }
}