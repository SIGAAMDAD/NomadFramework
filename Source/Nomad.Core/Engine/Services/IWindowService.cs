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
using System.Collections.Generic;
using Nomad.Core.Engine.Windowing;
using Nomad.Core.Events;

namespace Nomad.Core.Engine.Services
{
    /// <summary>
    ///
    /// </summary>
    public interface IWindowService : IDisposable
    {
        /// <summary>
        ///
        /// </summary>
        string Title { get; set; }

        /// <summary>
        ///
        /// </summary>
        int Width { get; set; }

        /// <summary>
        ///
        /// </summary>
        int Height { get; set; }

        /// <summary>
        ///
        /// </summary>
        int ScreenIndex { get; set; }

        /// <summary>
        ///
        /// </summary>
        bool IsFocused { get; }

        /// <summary>
        ///
        /// </summary>
        float RefreshRate { get; }

        /// <summary>
        ///
        /// </summary>
        IReadOnlyList<Monitor> Monitors { get; }

        /// <summary>
        ///
        /// </summary>
        WindowMode Mode { get; set; }

        /// <summary>
        ///
        /// </summary>
        IGameEvent<WindowSizeChangedEventArgs> SizeChanged { get; }

        /// <summary>
        ///
        /// </summary>
        IGameEvent<bool> FocusChanged { get; }

        /// <summary>
        ///
        /// </summary>
        IGameEvent<EmptyEventArgs> CloseRequested { get; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="monitorIndex"></param>
        /// <returns></returns>
        IReadOnlyList<WindowResolution> GetSupportedResolutions(int monitorIndex);

        /// <summary>
        ///
        /// </summary>
        /// <param name="monitorIndex"></param>
        /// <param name="nativeSize"></param>
        void GetNativeResolutionForMonitor(int monitorIndex, out WindowSize nativeSize);

        /// <summary>
        ///
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        void GetScreenResolution(out int width, out int height);
    }
}
