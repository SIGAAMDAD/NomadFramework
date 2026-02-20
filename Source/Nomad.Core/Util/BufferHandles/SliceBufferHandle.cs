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
        public int Length => _length;

        /// <summary>
        /// 
        /// </summary>
        public ReadOnlySpan<byte> Buffer => GetSlice(0, _length);

        private readonly IBufferHandle _parent;
        private readonly int _offset;
        private readonly int _length;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        public SliceBufferHandle(IBufferHandle parent, int offset, int length)
        {
            _parent = parent;
            _offset = offset;
            _length = length;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
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
        public ReadOnlySpan<byte> GetSlice(int start, int length)
            => _parent.GetSlice(start + _offset, length);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public MemoryHandle Pin()
        {
            unsafe
            {
                fixed (void* ptr = _parent.GetSlice(_offset, _length))
                {
                    return new MemoryHandle(ptr);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Memory<byte> AsMemory()
            => _parent.AsMemory().Slice(_offset, _length);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Memory<byte> AsMemory(int start, int length = 0)
            => AsMemory().Slice(start, length);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<byte> AsSpan()
            => _parent.AsSpan().Slice(_offset, _length);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<byte> AsSpan(int start, int length = 0)
            => AsSpan().Slice(start, length);

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
            => _parent.Clear(_offset, _length);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        public void Clear(int start, int length)
            => _parent.Clear(_offset + start, length);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="dstOffset"></param>
        public void CopyFrom(byte[] source, int offset, int length, int dstOffset = 0)
            => _parent.CopyFrom(source, offset, length, _offset + dstOffset);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="dstOffset"></param>
        public void CopyFrom(ReadOnlySpan<byte> source, int offset, int length, int dstOffset = 0)
            => _parent.CopyFrom(source, offset, length, _offset + dstOffset);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="dstOffset"></param>
        public void CopyFrom(IBufferHandle source, int offset, int length, int dstOffset = 0)
            => _parent.CopyFrom(source, offset, length, _offset + dstOffset);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dest"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="srcOffset"></param>
        public void CopyTo(byte[] dest, int offset, int length, int srcOffset = 0)
            => _parent.CopyTo(dest, _offset, length, offset + srcOffset);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dest"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="srcOffset"></param>
        public void CopyTo(Span<byte> dest, int offset, int length, int srcOffset = 0)
            => _parent.CopyTo(dest, _offset, length, offset + srcOffset);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dest"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="srcOffset"></param>
        public void CopyTo(IBufferHandle dest, int offset, int length, int srcOffset = 0)
            => _parent.CopyTo(dest, _offset, length, offset + srcOffset);
    }
}