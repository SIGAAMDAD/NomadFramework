/*
===========================================================================
The Nomad Framework
Copyright (C) 2025 Noah Van Til

This Source Code Form is subject to the terms of the Mozilla Public
License, v2. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.

This software is provided "as is", without warranty of any kind,
express or implied, including but not limited to the warranties
of merchantability, fitness for a particular purpose and noninfringement.
===========================================================================
*/

using System;
using System.Collections.Generic;
using Godot;

namespace Nomad.Rendering.PostProcessing
{
    public class UniformRingBuffer
    {
        private readonly RenderingDevice _rd;
        private readonly List<Rid> _bufferRing = new List<Rid>();
        private readonly int _bufferSize = 0;
        private readonly int _framesInFlight = 2;
        private int _currentIndex = 0;

        public UniformRingBuffer(RenderingDevice rd, int bufferSize, int framesInFlight)
        {
            _rd = rd;
            _bufferSize = bufferSize;
            _framesInFlight = framesInFlight;

            for (int i = 0; i < _framesInFlight; i++)
            {
                _bufferRing.Add(CreateBuffer());
            }
        }

        public Rid Update(Span<byte> data)
        {
            Rid buffer;

            if (ShouldAllocateNewBuffer())
            {
                buffer = CreateBuffer();
                _bufferRing.Add(buffer);

                if (_bufferRing.Count > _framesInFlight * 2)
                {
                    Rid excess = _bufferRing[_bufferRing.Count - 1];
                    _rd.FreeRid(excess);
                    _bufferRing.RemoveAt(_bufferRing.Count - 1);
                }
            }
            else
            {
                buffer = _bufferRing[_currentIndex];
            }

            _rd.BufferUpdate(buffer, 0, (uint)data.Length, data);
            _currentIndex = (_currentIndex + 1) % _bufferRing.Count;

            return buffer;
        }

        private Rid CreateBuffer()
        {
            Rid buffer = _rd.StorageBufferCreate(
                (uint)_bufferSize,
                null
            );
            return buffer;
        }

        private Rid GetBufferForFrame(byte[] data)
        {
            if (_currentIndex >= _bufferRing.Count)
            {
                _bufferRing.Add(CreateBuffer());
            }

            Rid currentBuffer = _bufferRing[_currentIndex];

            _rd.BufferUpdate(currentBuffer, 0, (uint)data.Length, data);

            _currentIndex = (_currentIndex + 1) % _framesInFlight;

            return currentBuffer;
        }

        private bool ShouldAllocateNewBuffer()
        {
            return _bufferRing.Count < _framesInFlight;
        }
    }
}
