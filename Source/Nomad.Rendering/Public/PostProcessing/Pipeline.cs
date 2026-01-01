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

using Godot;

namespace Nomad.Rendering.PostProcessing
{
    /// <summary>
    ///
    /// </summary>
    public sealed class Pipeline
    {
        private readonly Rid _inputTextureRid;
        private readonly Rid _intermediateTextureRid;
        private readonly Rid _outputTextureRid;
        private readonly Rid[] _uniformSets;

        public RenderingDevice Device => _device;
        private readonly RenderingDevice _device;

        /// <summary>
        ///
        /// </summary>
        public Pipeline(uint width, uint height)
        {
            _device = RenderingServer.CreateLocalRenderingDevice();

            var inputFormat = new RDTextureFormat()
            {
                Width = width,
                Height = height,
                Format = RenderingDevice.DataFormat.R16G16B16A16Sfloat,
                UsageBits = (
                    RenderingDevice.TextureUsageBits.SamplingBit |
                    RenderingDevice.TextureUsageBits.ColorAttachmentBit |
                    RenderingDevice.TextureUsageBits.CanCopyToBit
                )
            };
            _inputTextureRid = _device.TextureCreate(inputFormat, new RDTextureView());

            var samplerState = new RDSamplerState();
            samplerState.MinFilter = RenderingDevice.SamplerFilter.Nearest;
            samplerState.MagFilter = RenderingDevice.SamplerFilter.Nearest;

            Rid inputSampler = _device.SamplerCreate(
                samplerState
            );

            _device.TextureCreate(  );
        }
    }
}
