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

using System.Collections.Generic;
using Nomad.Core.Util;

namespace Nomad.Core.EngineUtils
{
    public record RenderSettings
    {
        public int MaxFrameRate { get; set; } = 60;
        public VSyncMode VSyncMode { get; set; } = VSyncMode.Disabled;
        public WindowMode WindowMode { get; set; } = WindowMode.ExclusiveFullscreen;
        public WindowResolution Resolution { get; set; }
        public AspectRatio AspectRatio { get; set; } = AspectRatio.Aspect_Automatic;

        public AntiAliasingMode AntiAliasing { get; set; } = AntiAliasingMode.None;

        public ShadowQuality ShadowQuality { get; set; } = ShadowQuality.Medium;

        public Dictionary<string, Any> EngineSpecific { get; } = new Dictionary<string, Any>();
    }
}
