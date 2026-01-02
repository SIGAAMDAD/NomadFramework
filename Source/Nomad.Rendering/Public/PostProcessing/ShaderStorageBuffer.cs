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
using Godot;

namespace Nomad.Rendering.PostProcessing
{
    public class ShaderStorageBuffer : IDisposable
    {
        public Rid BufferId => _bufferId;
        private readonly Rid _bufferId;

        private readonly int _size;
        private readonly RenderingDevice _rd;

        public ShaderStorageBuffer(RenderingDevice rd, int size)
        {
            _rd = rd;
            _size = size;
            _bufferId = rd.UniformBufferCreate((uint)size, null);
        }

        public void Dispose()
        {
            if (_bufferId.IsValid)
            {
                _rd.FreeRid(_bufferId);
            }
            GC.SuppressFinalize(this);
        }

        public void Update(Span<byte> buffer)
        {
            _rd.BufferUpdate(_bufferId, 0, (uint)_size, buffer);
        }
    }
}
