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

using Nomad.Core.CVars;
using Nomad.Core.Engine.Rendering;
using Nomad.Core.Engine.Windowing;

namespace Nomad.EngineUtils
{
    /// <summary>
    ///
    /// </summary>
    public static class DisplayCVars
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="cvarSystem"></param>
        public static void Register(ICVarSystemService cvarSystem)
        {
            cvarSystem.Register(
                new CVarCreateInfo<int>
                {
                    Name = Core.Constants.CVars.EngineUtils.Display.MONITOR,
                    DefaultValue = 0,
                    Description = "The game's current display.",
                    Flags = CVarFlags.Archive,
                    Validator = delegate(int value) { return value >= 0; }
                }
            );
            cvarSystem.Register(
                new CVarCreateInfo<WindowMode>
                {
                    Name = Core.Constants.CVars.EngineUtils.Display.WINDOW_MODE,
                    DefaultValue = WindowMode.ExclusiveFullscreen,
                    Description = "The game's window mode.",
                    Flags = CVarFlags.Archive,
                    Validator = delegate(WindowMode value) { return value >= WindowMode.Windowed && value < WindowMode.Count; }
                }
            );
            cvarSystem.Register(
                new CVarCreateInfo<WindowResolution>
                {
                    Name = Core.Constants.CVars.EngineUtils.Display.WINDOW_RESOLUTION,
                    DefaultValue = WindowResolution.Res_640x480,
                    Description = "Size of the game's display window.",
                    Flags = CVarFlags.Archive,
                    Validator = delegate(WindowResolution value) { return value >= WindowResolution.Res_640x480 && value < WindowResolution.Count; }
                }
            );
            cvarSystem.Register(
                new CVarCreateInfo<AspectRatio>
                {
                    Name = Core.Constants.CVars.EngineUtils.Display.ASPECT_RATIO,
                    DefaultValue = AspectRatio.Aspect_Automatic,
                    Description = "The display aspect ratio.",
                    Flags = CVarFlags.Archive,
                    Validator = delegate(AspectRatio value) { return value >= AspectRatio.Aspect_Automatic && value < AspectRatio.Count; }
                }
            );
            cvarSystem.Register(
                new CVarCreateInfo<VSyncMode>
                {
                    Name = Core.Constants.CVars.EngineUtils.Display.VSYNC_MODE,
                    DefaultValue = VSyncMode.Disabled,
                    Description = "Sets the engine's vertical sync policy.",
                    Flags = CVarFlags.Archive,
                    Validator = delegate(VSyncMode value) { return value >= VSyncMode.Disabled && value < VSyncMode.Count; }
                }
            );
            cvarSystem.Register(
                new CVarCreateInfo<int>
                {
                    Name = Core.Constants.CVars.EngineUtils.Display.MAX_FPS,
                    DefaultValue = 60,
                    Description = "Sets the maximum amount of gameplay loops per second, set to 0 for unlimited.",
                    Flags = CVarFlags.Archive,
                    Validator = delegate(int value) { return value >= 30 && value <= 1000; }
                }
            );
            cvarSystem.Register(
                new CVarCreateInfo<AntiAliasingMode>
                {
                    Name = Core.Constants.CVars.EngineUtils.Display.ANTI_ALIASING,
                    DefaultValue = AntiAliasingMode.None,
                    Description = "Sets the renderer's method for reducing aliasing in the final displayed image.",
                    Flags = CVarFlags.Archive,
                    Validator = delegate(AntiAliasingMode value) { return value >= AntiAliasingMode.None && value < AntiAliasingMode.Count; }
                }
            );
            cvarSystem.Register(
                new CVarCreateInfo<float>
                {
                    Name = Core.Constants.CVars.EngineUtils.Display.BRIGHTNESS,
                    DefaultValue = 90.0f,
                    Description = "Sets the brightness level of the game's rendered frame.",
                    Flags = CVarFlags.Archive
                }
            );
            cvarSystem.Register(
                new CVarCreateInfo<float>
                {
                    Name = Core.Constants.CVars.EngineUtils.Display.RESOLUTION_SCALE,
                    DefaultValue = 50.0f,
                    Description = "Sets rendering resolution for the game window.",
                    Flags = CVarFlags.Archive
                }
            );
        }
    }
}
