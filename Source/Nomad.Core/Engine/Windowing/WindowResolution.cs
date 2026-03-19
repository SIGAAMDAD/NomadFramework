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

using Nomad.Core.Util;
using System;
using System.Collections.Generic;

namespace Nomad.Core.Engine.Windowing
{
    /// <summary>
    ///
    /// </summary>
    public enum WindowResolution : byte
    {
        Res_640x480,
        Res_800x600,
        Res_1024x768,
        Res_1280x720,
        Res_1280x768,
        Res_1280x800,
        Res_1280x1024,
        Res_1360x768,
        Res_1366x768,
        Res_1440x900,
        Res_1536x864,
        Res_1600x900,
        Res_1600x1200,
        Res_1680x1050,
        Res_1920x1080,
        Res_1920x1200,
        Res_2048x1152,
        Res_2048x1536,
        Res_2560x1080,
        Res_2560x1440,
        Res_2560x1600,
        Res_3440x1440,
        Res_3840x2160,
        Res_Native,
        Count,
        Min = Res_640x480,
        Max = Res_3840x2160,
        Default = Res_1920x1080
    }

    /// <summary>
    ///
    /// </summary>
    public static class WindowResolutionExtensions
    {
        // Central data source: resolution, display string, width, height
        private static readonly (WindowResolution resolution, InternString display, int width, int height)[] _resolutionData = new[]
        {
            (WindowResolution.Res_640x480,   new InternString("640x480"),   640, 480),
            (WindowResolution.Res_800x600,   new InternString("800x600"),   800, 600),
            (WindowResolution.Res_1024x768,  new InternString("1024x768"),  1024, 768),
            (WindowResolution.Res_1280x720,  new InternString("1280x720"),  1280, 720),
            (WindowResolution.Res_1280x768,  new InternString("1280x768"),  1280, 768),
            (WindowResolution.Res_1280x800,  new InternString("1280x800"),  1280, 800),
            (WindowResolution.Res_1280x1024, new InternString("1280x1024"), 1280, 1024),
            (WindowResolution.Res_1360x768,  new InternString("1360x768"),  1360, 768),
            (WindowResolution.Res_1366x768,  new InternString("1366x768"),  1366, 768),
            (WindowResolution.Res_1440x900,  new InternString("1440x900"),  1440, 900),
            (WindowResolution.Res_1536x864,  new InternString("1536x864"),  1536, 864),
            (WindowResolution.Res_1600x900,  new InternString("1600x900"),  1600, 900),
            (WindowResolution.Res_1600x1200, new InternString("1600x1200"), 1600, 1200),
            (WindowResolution.Res_1680x1050, new InternString("1680x1050"), 1680, 1050),
            (WindowResolution.Res_1920x1080, new InternString("1920x1080"), 1920, 1080),
            (WindowResolution.Res_1920x1200, new InternString("1920x1200"), 1920, 1200),
            (WindowResolution.Res_2048x1152, new InternString("2048x1152"), 2048, 1152),
            (WindowResolution.Res_2048x1536, new InternString("2048x1536"), 2048, 1536),
            (WindowResolution.Res_2560x1080, new InternString("2560x1080"), 2560, 1080),
            (WindowResolution.Res_2560x1440, new InternString("2560x1440"), 2560, 1440),
            (WindowResolution.Res_2560x1600, new InternString("2560x1600"), 2560, 1600),
            (WindowResolution.Res_3440x1440, new InternString("3440x1440"), 3440, 1440),
            (WindowResolution.Res_3840x2160, new InternString("3840x2160"), 3840, 2160),
            (WindowResolution.Res_Native,    new InternString("Native Resolution"), 0, 0) // Native has no fixed size
        };

        private static readonly Dictionary<WindowResolution, InternString> _displayStringMap;
        private static readonly Dictionary<WindowResolution, WindowSize> _sizeMap;
        private static readonly Dictionary<InternString, WindowResolution> _parseMap;

        /// <summary>
        ///
        /// </summary>
        static WindowResolutionExtensions()
        {
            _displayStringMap = new Dictionary<WindowResolution, InternString>(_resolutionData.Length);
            _sizeMap = new Dictionary<WindowResolution, WindowSize>(_resolutionData.Length);
            _parseMap = new Dictionary<InternString, WindowResolution>(_resolutionData.Length);

            foreach (var data in _resolutionData)
            {
                // For Native resolution, there is no valid size; skip adding to _sizeMap
                if (data.resolution != WindowResolution.Res_Native)
                {
                    _sizeMap[data.resolution] = new WindowSize(data.width, data.height);
                }

                _displayStringMap[data.resolution] = data.display;
                _parseMap[data.display] = data.resolution;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="resolution"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static InternString ToDisplayString(this WindowResolution resolution)
        {
            if (_displayStringMap.TryGetValue(resolution, out var display))
            {
                return display;
            }
            throw new ArgumentOutOfRangeException(nameof(resolution), resolution, null);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="resolution"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static WindowSize GetSize(this WindowResolution resolution)
        {
            if (_sizeMap.TryGetValue(resolution, out var size))
            {
                return size;
            }
            throw new ArgumentOutOfRangeException(nameof(resolution), resolution, null);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="resolutionString"></param>
        /// <param name="resolution"></param>
        /// <returns></returns>
        public static bool TryParse(InternString resolutionString, out WindowResolution resolution)
        {
            if (resolutionString == InternString.Empty)
            {
                resolution = WindowResolution.Default;
                return false;
            }
            if (_parseMap.TryGetValue(resolutionString, out resolution))
            {
                return true;
            }
            resolution = WindowResolution.Default;
            return false;
        }
    }
}
