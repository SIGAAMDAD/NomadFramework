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
using System.Collections.Generic;

namespace Nomad.Core.Engine.Windowing
{
    /// <summary>
    /// Represents the size of a game window.
    /// </summary>
    public readonly struct WindowSize
    {
        /// <summary>
        /// Gets the width of the window.
        /// </summary>
        public int Width => _width;
        private readonly int _width;

        /// <summary>
        /// Gets the height of the window.
        /// </summary>
        public int Height => _height;
        private readonly int _height;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowSize"/> struct.
        /// </summary>
        /// <param name="width">The width of the window.</param>
        /// <param name="height">The height of the window.</param>
        public WindowSize(int width, int height)
        {
            _width = width;
            _height = height;
        }

        // Central data source: resolution, width, height (excluding Native)
        private static readonly (WindowResolution resolution, int width, int height)[] _resolutionData = new[]
        {
            (WindowResolution.Res_800x600,   800, 600),
            (WindowResolution.Res_1024x768,  1024, 768),
            (WindowResolution.Res_1280x720,  1280, 720),
            (WindowResolution.Res_1280x768,  1280, 768),
            (WindowResolution.Res_1280x800,  1280, 800),
            (WindowResolution.Res_1280x1024, 1280, 1024),
            (WindowResolution.Res_1360x768,  1360, 768),
            (WindowResolution.Res_1366x768,  1366, 768),
            (WindowResolution.Res_1440x900,  1440, 900),
            (WindowResolution.Res_1536x864,  1536, 864),
            (WindowResolution.Res_1600x900,  1600, 900),
            (WindowResolution.Res_1600x1200, 1600, 1200),
            (WindowResolution.Res_1680x1050, 1680, 1050),
            (WindowResolution.Res_1920x1080, 1920, 1080),
            (WindowResolution.Res_1920x1200, 1920, 1200),
            (WindowResolution.Res_2048x1152, 2048, 1152),
            (WindowResolution.Res_2048x1536, 2048, 1536),
            (WindowResolution.Res_2560x1080, 2560, 1080),
            (WindowResolution.Res_2560x1440, 2560, 1440),
            (WindowResolution.Res_2560x1600, 2560, 1600),
            (WindowResolution.Res_3440x1440, 3440, 1440),
            (WindowResolution.Res_3840x2160, 3840, 2160),
        };

        private static readonly Dictionary<WindowResolution, WindowSize> _resolutionToSizeMap;
        private static readonly Dictionary<(int width, int height), WindowResolution> _sizeToResolutionMap;

        /// <summary>
        ///
        /// </summary>
        static WindowSize()
        {
            _resolutionToSizeMap = new Dictionary<WindowResolution, WindowSize>(_resolutionData.Length);
            _sizeToResolutionMap = new Dictionary<(int, int), WindowResolution>(_resolutionData.Length);

            foreach (var data in _resolutionData)
            {
                var size = new WindowSize(data.width, data.height);
                _resolutionToSizeMap[data.resolution] = size;
                _sizeToResolutionMap[(data.width, data.height)] = data.resolution;
            }
        }

        /// <summary>
        /// Converts a <see cref="WindowResolution"/> to its corresponding <see cref="WindowSize"/>.
        /// </summary>
        /// <param name="value">The resolution to convert.</param>
        /// <returns>The window size matching the resolution.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the resolution is not a fixed size (e.g., Native).</exception>
        public static implicit operator WindowSize(WindowResolution value)
        {
            if (_resolutionToSizeMap.TryGetValue(value, out var size))
            {
                return size;
            }

            throw new ArgumentOutOfRangeException(nameof(value), value, "Resolution does not have a fixed size.");
        }

        /// <summary>
        /// Converts a <see cref="WindowSize"/> to the closest matching <see cref="WindowResolution"/>.
        /// </summary>
        /// <param name="size">The window size to convert.</param>
        /// <returns>The resolution that exactly matches the size, or <see cref="WindowResolution.Default"/> if no match.</returns>
        public static implicit operator WindowResolution(WindowSize size)
        {
            var key = (size.Width, size.Height);
            return _sizeToResolutionMap.TryGetValue(key, out var resolution) ? resolution : WindowResolution.Default;
        }

        public static bool operator <(WindowSize left, WindowSize right) => left.Width < right.Width && left.Height < right.Height;
        public static bool operator >(WindowSize left, WindowSize right) => left.Width > right.Width && left.Height > right.Height;
        public static bool operator <=(WindowSize left, WindowSize right) => left.Width <= right.Width && left.Height <= right.Height;
        public static bool operator >=(WindowSize left, WindowSize right) => left.Width >= right.Width && left.Height >= right.Height;
    }
}
