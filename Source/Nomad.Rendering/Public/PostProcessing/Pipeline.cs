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
using System.Runtime.InteropServices;
using Godot;

namespace Nomad.Rendering.PostProcessing
{
    /// <summary>
    ///
    /// </summary>
    public sealed class Pipeline
    {
        private readonly Rid _upscalePipeline;
        private readonly Rid _postProcessPipeline;

        private readonly Rid _lowResTexture;
        private readonly Rid _fullResTexture;
        private readonly Rid _postProcessTexture;
        private readonly Rid _filmGrainNoiseTexture;
        private readonly RDTextureView _textureView;

        //        private readonly Rid _uniformBuffer;
        private readonly ShaderStorageBuffer _uniformBuffer;
        private readonly ShaderStorageBuffer _upscaleBuffer;
        //        private readonly Rid _upscaleBuffer;

        // Uniform structures - must match GLSL std140 layout
        [StructLayout(LayoutKind.Sequential)]
        public struct PostProcessUniforms
        {
            public Vector2 InputSize;
            public Vector2 OutputSize;
            public float Brightness;
            public float Contrast;
            public float Saturation;
            public float VignetteIntensity;
            public float VignetteSoftness;
            public float FilmGrainIntensity;
            public float Time;
            public int ToneMappingMode; // 0: None, 1: Reinhard, 2: ACES, 3: Uncharted2
            public float Exposure;
            public float Gamma;
            public int _padding1;
            public int _padding2;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct UpscaleUniforms
        {
            public Vector2 InputSize;
            public Vector2 OutputSize;
            public float Sharpness;
            public int UpscaleMode;

            // here for the 16 byte alignment in glsl
            public float _padding1;
            public float _padding2;
        }

        public RenderingDevice Device => _device;
        private readonly RenderingDevice _device;

        private readonly Vector2 _windowSize;

        private readonly Rid _upscaleSet;
        private readonly Rid _postProcessSet;

        /// <summary>
        ///
        /// </summary>
        public Pipeline(SubViewport viewport)
        {
            Vector2I windowSize = DisplayServer.WindowGetSize();
            _windowSize = new Vector2(windowSize.X, windowSize.Y);
            uint width = (uint)windowSize.X;
            uint height = (uint)windowSize.Y;

            _device = RenderingServer.CreateLocalRenderingDevice();

            _textureView = new RDTextureView();
            _lowResTexture = CreateLowResTexture(width / 2, height / 2);
            _fullResTexture = CreateFullResTexture(width, height);
            _postProcessTexture = CreateFullResTexture(width, height);
            _filmGrainNoiseTexture = CreateNoiseTexture();

            Span<byte> uniformBuffer = stackalloc byte[Marshal.SizeOf<PostProcessUniforms>()];
            Span<byte> upscaleUniformBuffer = stackalloc byte[Marshal.SizeOf<UpscaleUniforms>()];

            _uniformBuffer = new ShaderStorageBuffer(_device, Marshal.SizeOf<PostProcessUniforms>());
            _upscaleBuffer = new ShaderStorageBuffer(_device, Marshal.SizeOf<UpscaleUniforms>());

            //            _uniformBuffer = _device.UniformBufferCreate((uint)uniformBuffer.Length, uniformBuffer);
            //            _upscaleBuffer = _device.UniformBufferCreate((uint)upscaleUniformBuffer.Length, upscaleUniformBuffer);

            _upscalePipeline = CreateShaderPipeline("res://Assets/Shaders/Upscale.glsl", out Rid upscaleShader);
            _postProcessPipeline = CreateShaderPipeline("res://Assets/Shaders/PostProcess.glsl", out Rid postProcessShader);

            Godot.Collections.Array<RDUniform> upscaleSets =
            [
                CreateUniformSet(RenderingDevice.UniformType.Image, 0, _fullResTexture),
                CreateUniformSet(RenderingDevice.UniformType.Image, 1, _postProcessTexture),
                CreateUniformSet(RenderingDevice.UniformType.UniformBuffer, 2, _uniformBuffer.BufferId),
                CreateUniformSet(RenderingDevice.UniformType.SamplerWithTexture, 3, _filmGrainNoiseTexture)
            ];
            _upscaleSet = _device.UniformSetCreate(upscaleSets, upscaleShader, 0);

//            Godot.Collections.Array<RDUniform> postProcessSets =
//            [
//                CreateUniformSet(RenderingDevice.UniformType.Image, 0, _fullResTexture),
//                CreateUniformSet(RenderingDevice.UniformType.Image, 1, _postProcessTexture),
//                CreateUniformSet(RenderingDevice.UniformType.UniformBuffer, 2, _uniformBuffer.BufferId),
//                CreateUniformSet(RenderingDevice.UniformType.SamplerWithTexture, 3, _filmGrainNoiseTexture)
//            ];
//            _postProcessSet = _device.UniformSetCreate(postProcessSets, postProcessShader, 0);
        }

        public void ExecuteComputePipeline()
        {
            var postProcessUniform = new PostProcessUniforms
            {
                InputSize = _windowSize,
                OutputSize = _windowSize,
                Brightness = 1.2f,
                Contrast = 1.1f,
                Saturation = 1.0f,
                VignetteIntensity = 0.3f,
                VignetteSoftness = 0.5f,
                FilmGrainIntensity = 0.05f,
                Time = Time.GetTicksMsec() / 1000.0f,
                ToneMappingMode = 2,
                Exposure = 1.0f,
                Gamma = 2.2f
            };
            UpdateBuffer(_uniformBuffer, ref postProcessUniform);

            long computeList = _device.ComputeListBegin();

            _device.ComputeListBindComputePipeline(computeList, _upscalePipeline);
            _device.ComputeListBindUniformSet(computeList, _upscaleSet, 0);

            Vector2I fullRes = DisplayServer.WindowGetSize();
            uint groupCountX = (uint)Math.Ceiling(fullRes.X / 8.0f);
            uint groupCountY = (uint)Math.Ceiling(fullRes.Y / 8.0f);

            _device.ComputeListDispatch(computeList, groupCountX, groupCountY, 1);

            /*
            _device.ComputeListAddBarrier(computeList);
            _device.ComputeListBindComputePipeline(computeList, _postProcessPipeline);
            _device.ComputeListBindUniformSet(computeList, _postProcessSet, 0);
            _device.ComputeListDispatch(computeList, groupCountX, groupCountY, 1);
            */

            _device.ComputeListEnd();

            _device.Submit(); // submit the work queue
            _device.Sync(); // wait for gpu sync
        }

        private Rid CreateNoiseTexture()
        {
            int size = 256;
            Span<byte> noiseData = stackalloc byte[size * size * 4];

            var random = new Random();
            for (int i = 0; i < noiseData.Length; i += 4)
            {
                float value = (float)((random.NextDouble() * 0.4f) + (random.NextDouble() * 0.4f) +
                    (random.NextDouble() * 0.2f));

                byte noiseValue = (byte)(value * 255);
                noiseData[i] = noiseValue;
                noiseData[i + 1] = noiseValue;
                noiseData[i + 2] = noiseValue;
                noiseData[i + 3] = 255;
            }

            var noiseFormat = new RDTextureFormat
            {
                Width = (uint)size,
                Height = (uint)size,
                Format = RenderingDevice.DataFormat.R8G8B8A8Unorm,
                UsageBits = RenderingDevice.TextureUsageBits.SamplingBit,
                ArrayLayers = 1,
                Mipmaps = 1
            };

            var noiseView = new RDTextureView();
            return _device.TextureCreate(noiseFormat, noiseView);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <param name="binding"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private RDUniform CreateUniformSet(RenderingDevice.UniformType type, int binding, Rid id)
        {
            var uniformSet = new RDUniform
            {
                UniformType = type,
                Binding = binding,
            };
            uniformSet.AddId(id);
            return uniformSet;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buffer"></param>
        /// <param name="structure"></param>
        private unsafe void UpdateBuffer<T>(ShaderStorageBuffer buffer, ref T structure)
            where T : struct
        {
            uint bufferSize = (uint)Marshal.SizeOf<PostProcessUniforms>();
            Span<byte> bytes = stackalloc byte[(int)bufferSize];
            fixed (byte* ptr = bytes)
            {
                Marshal.StructureToPtr(structure, (IntPtr)ptr, false);
            }
            buffer.Update(bytes);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="shaderFile"></param>
        /// <returns></returns>
        private Rid CreateShaderPipeline(string shaderFile, out Rid shader)
        {
            RDShaderFile spirv = ResourceLoader.Load<RDShaderFile>(shaderFile);
            shader = _device.ShaderCreateFromSpirV(spirv.GetSpirV());
            return _device.ComputePipelineCreate(shader);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private Rid CreateLowResTexture(uint width, uint height)
        {
            var lowResFormat = new RDTextureFormat()
            {
                Width = width,
                Height = height,
                Format = RenderingDevice.DataFormat.R8G8B8A8Unorm,
                UsageBits =
                    RenderingDevice.TextureUsageBits.SamplingBit |
                    RenderingDevice.TextureUsageBits.ColorAttachmentBit |
                    RenderingDevice.TextureUsageBits.CanCopyToBit
            };
            return _device.TextureCreate(lowResFormat, _textureView);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private Rid CreateFullResTexture(uint width, uint height)
        {
            var fullResFormat = new RDTextureFormat()
            {
                Width = width,
                Height = height,
                Format = RenderingDevice.DataFormat.R16G16B16A16Sfloat,
                UsageBits = RenderingDevice.TextureUsageBits.StorageBit | RenderingDevice.TextureUsageBits.SamplingBit,
                ArrayLayers = 1,
                Mipmaps = 1
            };
            return _device.TextureCreate(fullResFormat, _textureView);
        }
    }
}
