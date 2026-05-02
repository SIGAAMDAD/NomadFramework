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
using System.Runtime.CompilerServices;
using Nomad.Core.Compatibility.Guards;
using Nomad.Core.Events;
using Nomad.Core.Exceptions;
using Nomad.Core.Engine.Services;
using Nomad.Core.Engine.Windowing;

namespace Nomad.Core.Engine.Globals
{
    /// <summary>
    ///
    /// </summary>
    public static class WindowService
    {
        /// <summary>
        ///
        /// </summary>
        public static IWindowService Instance => _instance ?? throw new SubsystemNotInitializedException();
        private static IWindowService? _instance;

        /// <summary>
        ///
        /// </summary>
        public static int Width
        {
            get => Instance.Width;
            set => Instance.Width = value;
        }

        /// <summary>
        ///
        /// </summary>
        public static int Height
        {
            get => Instance.Height;
            set => Instance.Height = value;
        }

        /// <summary>
        ///
        /// </summary>
        public static int ScreenIndex
        {
            get => Instance.ScreenIndex;
            set => Instance.ScreenIndex = value;
        }

        /// <summary>
        ///
        /// </summary>
        public static IGameEvent<WindowSizeChangedEventArgs> SizeChanged => Instance.SizeChanged;

        /// <summary>
        ///
        /// </summary>
        public static IGameEvent<WindowFocusChangedEventArgs> FocusChanged => Instance.FocusChanged;

        /// <summary>
        ///
        /// </summary>
        public static IGameEvent<WindowCloseRequestedEventArgs> CloseRequested => Instance.CloseRequested;

        /// <summary>
        ///
        /// </summary>
        /// <param name="instance"></param>
        internal static void Initialize(IWindowService instance)
        {
            ArgumentGuard.ThrowIfNull(instance);
            _instance = instance;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetScreenResolution(out int width, out int height)
        {
            Instance.GetScreenResolution(out width, out height);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="monitorIndex"></param>
        /// <param name="nativeSize"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetNativeResolutionForMonitor(int monitorIndex, out WindowSize nativeSize)
        {
            Instance.GetNativeResolutionForMonitor(monitorIndex, out nativeSize);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="monitorIndex"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IReadOnlyList<WindowResolution> GetSupportedResolutions(int monitorIndex)
        {
            return Instance.GetSupportedResolutions(monitorIndex);
        }
    }
}
