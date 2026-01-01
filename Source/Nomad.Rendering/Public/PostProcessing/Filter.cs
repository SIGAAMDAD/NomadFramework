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
    /// <summary>
    ///
    /// </summary>
    public class Filter
    {
        private readonly Rid _shader;
        private readonly Rid _pipeline;

        private readonly RenderingDevice _device;

        /// <summary>
        ///
        /// </summary>
        /// <param name="device"></param>
        /// <param name="shaderPath"></param>
        public Filter(RenderingDevice device, string shaderPath)
        {
            ArgumentNullException.ThrowIfNull(device);
            ArgumentException.ThrowIfNullOrEmpty(shaderPath);

            _device = device;
            RDShaderFile shaderFile = ResourceLoader.Load<RDShaderFile>(shaderPath);

            _shader = device.ShaderCreateFromSpirV(shaderFile.GetSpirV());
            _pipeline = device.ComputePipelineCreate(_shader);
        }

        /// <summary>
        /// Releases the unmanaged godot resources.
        /// </summary>
        public void Dispose()
        {
            if (_shader.IsValid)
            {
                _device.FreeRid(_shader);
            }
            if (_pipeline.IsValid)
            {
                _device.FreeRid(_pipeline);
            }
        }
    }
}
