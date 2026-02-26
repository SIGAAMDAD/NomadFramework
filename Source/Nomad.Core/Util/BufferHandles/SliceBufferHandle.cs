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

namespace Nomad.Core.Util.BufferHandles
{
    /// <summary>
    /// 
    /// </summary>
    public readonly ref struct SliceBufferHandle
    {
        /// <summary>
        /// 
        /// </summary>
        public long Length => _length;

        /// <summary>
        /// 
        /// </summary>
        public ReadOnlySpan<byte> Buffer => GetSlice(0, _length);

        private readonly IBufferHandle _parent;
        private readonly long _offset;
        private readonly long _length;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        public SliceBufferHandle(IBufferHandle parent, long offset, long length)
        {
            _parent = parent;
            _offset = offset;
            _length = length;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IBufferHandle? other)
        {
            ArgumentGuard.ThrowIfNull(other);
            var span1 = AsSpan();
            return span1.SequenceEqual(other.AsSpan());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public ReadOnlySpan<byte> GetSlice(long start, long length)
            => _parent.GetSlice(start + _offset, length);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public MemoryHandle Pin()
        {
            return _parent.AsMemory(_offset, _length).Pin();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Memory<byte> AsMemory()
            => _parent.AsMemory().Slice((int)_offset, (int)_length);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Memory<byte> AsMemory(long start, long length = 0)
            => AsMemory().Slice((int)start, (int)length);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<byte> AsSpan()
            => _parent.AsSpan().Slice((int)_offset, (int)_length);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<byte> AsSpan(long start, long length = 0)
            => AsSpan().Slice((int)start, (int)length);

        /// <summary>
        /// 
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
            => _parent.Clear(_offset, _length);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear(long start, long length)
            => _parent.Clear(_offset + start, length);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="dstOffset"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyFrom(byte[] source, long offset, long length, long dstOffset = 0)
            => _parent.CopyFrom(source, offset, length, _offset + dstOffset);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="dstOffset"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyFrom(ReadOnlySpan<byte> source, long offset, long length, long dstOffset = 0)
            => _parent.CopyFrom(source, offset, length, _offset + dstOffset);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="dstOffset"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyFrom(IBufferHandle source, long offset, long length, long dstOffset = 0)
            => _parent.CopyFrom(source, offset, length, _offset + dstOffset);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dest"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="srcOffset"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(byte[] dest, long offset, long length, long srcOffset = 0)
            => _parent.CopyTo(dest, offset, length, _offset + srcOffset);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dest"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="srcOffset"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(Span<byte> dest, long offset, long length, long srcOffset = 0)
            => _parent.CopyTo(dest, offset, length, _offset + srcOffset);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dest"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="srcOffset"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(IBufferHandle dest, long offset, long length, long srcOffset = 0)
            => _parent.CopyTo(dest, offset, length, _offset + srcOffset);
    }
}