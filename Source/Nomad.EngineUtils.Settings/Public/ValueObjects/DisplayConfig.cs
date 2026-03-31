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

using Nomad.Core.Engine.Rendering;
using Nomad.Core.Engine.Windowing;
using Nomad.EngineUtils.Settings.Interfaces;

namespace Nomad.EngineUtils.Settings.ValueObjects
{
    /// <summary>
    ///
    /// </summary>
    public record DisplayConfig : IDisplayConfig
    {
        /// <summary>
        /// Which monitor we are using.
        /// </summary>
        public int MonitorIndex { get; set; }

        /// <summary>
        /// The maximum amount of frames that the engine is allowed to draw per second.
        /// </summary>
        /// <remarks>
        /// Set to 0 to allow uncapped framerates.
        /// </remarks>
        public int MaximumFrameRate { get; set; }

        /// <summary>
        ///
        /// </summary>
        public float Brightness { get; set; }

        /// <summary>
        ///
        /// </summary>
        public float ResolutionScale { get; set; }

        /// <summary>
        ///
        /// </summary>
        public VSyncMode VSyncMode { get; set; }

        /// <summary>
        ///
        /// </summary>
        public WindowMode WindowMode { get; set; }

        /// <summary>
        ///
        /// </summary>
        public WindowResolution Resolution { get; set; }

        /// <summary>
        ///
        /// </summary>
        public AntiAliasingMode AntiAliasing { get; set; }

        /// <summary>
        ///
        /// </summary>
        public AspectRatio AspectRatio { get; set; }

        /// <summary>
        ///
        /// </summary>
        public bool HDREnabled { get; set; }
    }
}
