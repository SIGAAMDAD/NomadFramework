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
using Nomad.Core.CVars;
using Nomad.Core.EngineUtils;
using Nomad.Core.Exceptions;

namespace Nomad.EngineUtils
{
    /// <summary>
    ///
    /// </summary>
    internal sealed class GodotRenderService : IRenderingService
    {
        private readonly Rid _viewportRid;
        private readonly ICVarSystemService _cvarSystem;
        private readonly IWindowService _windowService;

        private RenderSettings _settings;

        public GodotRenderService(Rid viewportRid, IWindowService windowService, ICVarSystemService cvarSystem)
        {
            _viewportRid = viewportRid;
            _cvarSystem = cvarSystem;
            _windowService = windowService;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public RenderSettings GetRenderSettings()
        {
            return _settings;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="settings"></param>
        public void SetRenderSettings(RenderSettings settings)
        {
            _settings = settings;

            SetAntiAliasingMethod(settings.AntiAliasing);
            SetShadowQuality(settings.ShadowQuality);
            SetWindowData(settings.WindowMode, settings.Resolution);
            SetVSyncMode(settings.VSyncMode);

            ProjectSettings.SetSetting("display/window/stretch/scale", settings.AspectRatio.GetRatio());

            Engine.MaxFps = settings.MaxFrameRate;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="resolution"></param>
        private void SetWindowData(WindowMode mode, WindowResolution resolution)
        {
            var size = (WindowSize)resolution;
            _windowService.Width = size.Width;
            _windowService.Height = size.Height;
            _windowService.Mode = mode;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="mode"></param>
        private static void SetVSyncMode(VSyncMode mode)
        {
            var vsyncMode = DisplayServer.VSyncMode.Disabled;
            int swapChainImageCount = 2;
            switch (mode)
            {
                case VSyncMode.Adaptive:
                    vsyncMode = DisplayServer.VSyncMode.Adaptive;
                    break;
                case VSyncMode.Enabled:
                    vsyncMode = DisplayServer.VSyncMode.Enabled;
                    break;
                case VSyncMode.Disabled:
                    vsyncMode = DisplayServer.VSyncMode.Disabled;
                    break;
                case VSyncMode.TripleBuffered:
                    vsyncMode = DisplayServer.VSyncMode.Adaptive;
                    swapChainImageCount = 3;
                    break;
            }
            DisplayServer.WindowSetVsyncMode(vsyncMode);
            ProjectSettings.SetSetting("rendering/rendering_device/vsync/swap_chain_image_count", swapChainImageCount);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="quality"></param>
        private void SetShadowQuality(ShadowQuality quality)
        {
            int shadowAtlasSize = 0;
            var shadowFilter = Light2D.ShadowFilterEnum.None;
            float shadowFilterSmooth = 0.0f;
            switch (quality)
            {
                case ShadowQuality.Low:
                    shadowAtlasSize = 1 * 1024;
                    break;
                case ShadowQuality.Medium:
                    shadowAtlasSize = 2 * 1024;
                    shadowFilter = Light2D.ShadowFilterEnum.Pcf5;
                    shadowFilterSmooth = 0.010f;
                    break;
                case ShadowQuality.High:
                    shadowAtlasSize = 4 * 1024;
                    shadowFilter = Light2D.ShadowFilterEnum.Pcf13;
                    shadowFilterSmooth = 0.10f;
                    break;
                case ShadowQuality.VeryHigh:
                    shadowAtlasSize = 8 * 1024;
                    shadowFilter = Light2D.ShadowFilterEnum.Pcf13;
                    shadowFilterSmooth = 0.20f;
                    break;
            }

            // check for overrides
            if (_settings.EngineSpecific.TryGetValue("ShadowAtlasSize", out var atlasSize))
            {
                shadowAtlasSize = atlasSize;
            }
            if (_settings.EngineSpecific.TryGetValue("ShadowFilter", out var filterType))
            {
                shadowFilter = (Light2D.ShadowFilterEnum)(long)filterType;
            }
            if (_settings.EngineSpecific.TryGetValue("ShadowFilterSmooth", out var filterSmooth))
            {
                shadowFilterSmooth = filterSmooth;
            }

            RenderingServer.CanvasSetShadowTextureSize(shadowAtlasSize);

            var cvarAtlasSize = _cvarSystem.GetCVar<int>(Constants.CVars.SHADOW_ATLAS_SIZE) ?? throw new CVarMissing(Constants.CVars.SHADOW_ATLAS_SIZE);
            cvarAtlasSize.Value = shadowAtlasSize;

            var cvarShadowFilterType = _cvarSystem.GetCVar<int>(Constants.CVars.SHADOW_FILTER_TYPE) ?? throw new CVarMissing(Constants.CVars.SHADOW_FILTER_TYPE);
            cvarShadowFilterType.Value = (int)shadowFilter;

            var cvarShadowFilterSmooth = _cvarSystem.GetCVar<float>(Constants.CVars.SHADOW_FILTER_SMOOTH) ?? throw new CVarMissing(Constants.CVars.SHADOW_FILTER_SMOOTH);
            cvarShadowFilterSmooth.Value = shadowFilterSmooth;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="mode"></param>
        private void SetAntiAliasingMethod(AntiAliasingMode mode)
        {
            switch (mode)
            {
                case AntiAliasingMode.None:
                    RenderingServer.ViewportSetMsaa2D(_viewportRid, RenderingServer.ViewportMsaa.Disabled);
                    RenderingServer.ViewportSetScreenSpaceAA(_viewportRid, RenderingServer.ViewportScreenSpaceAA.Disabled);
                    RenderingServer.ViewportSetUseTaa(_viewportRid, false);
                    break;
                case AntiAliasingMode.FXAA:
                    RenderingServer.ViewportSetMsaa2D(_viewportRid, RenderingServer.ViewportMsaa.Disabled);
                    RenderingServer.ViewportSetScreenSpaceAA(_viewportRid, RenderingServer.ViewportScreenSpaceAA.Fxaa);
                    RenderingServer.ViewportSetUseTaa(_viewportRid, false);
                    break;
                case AntiAliasingMode.SMAA:
                    RenderingServer.ViewportSetMsaa2D(_viewportRid, RenderingServer.ViewportMsaa.Disabled);
                    RenderingServer.ViewportSetScreenSpaceAA(_viewportRid, RenderingServer.ViewportScreenSpaceAA.Smaa);
                    RenderingServer.ViewportSetUseTaa(_viewportRid, false);
                    break;
                case AntiAliasingMode.TAA:
                    RenderingServer.ViewportSetMsaa2D(_viewportRid, RenderingServer.ViewportMsaa.Disabled);
                    RenderingServer.ViewportSetScreenSpaceAA(_viewportRid, RenderingServer.ViewportScreenSpaceAA.Disabled);
                    RenderingServer.ViewportSetUseTaa(_viewportRid, true);
                    break;
                case AntiAliasingMode.MSAA_2x:
                    RenderingServer.ViewportSetMsaa2D(_viewportRid, RenderingServer.ViewportMsaa.Msaa2X);
                    RenderingServer.ViewportSetScreenSpaceAA(_viewportRid, RenderingServer.ViewportScreenSpaceAA.Disabled);
                    RenderingServer.ViewportSetUseTaa(_viewportRid, false);
                    break;
                case AntiAliasingMode.MSAA_4x:
                    RenderingServer.ViewportSetMsaa2D(_viewportRid, RenderingServer.ViewportMsaa.Msaa4X);
                    RenderingServer.ViewportSetScreenSpaceAA(_viewportRid, RenderingServer.ViewportScreenSpaceAA.Disabled);
                    RenderingServer.ViewportSetUseTaa(_viewportRid, false);
                    break;
                case AntiAliasingMode.MSAA_8x:
                    RenderingServer.ViewportSetMsaa2D(_viewportRid, RenderingServer.ViewportMsaa.Msaa8X);
                    RenderingServer.ViewportSetScreenSpaceAA(_viewportRid, RenderingServer.ViewportScreenSpaceAA.Disabled);
                    RenderingServer.ViewportSetUseTaa(_viewportRid, false);
                    break;
            }
        }
    }
}
