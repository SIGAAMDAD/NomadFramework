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

using System.Runtime.CompilerServices;
using Nomad.Core.Compatibility.Guards;
using Nomad.Core.EngineUtils;
using Nomad.Core.Events;
using Nomad.Core.Exceptions;

namespace Nomad.EngineUtils.Globals
{
    /// <summary>
    ///
    /// </summary>
    public static class WindowService
    {
        /// <summary>
        ///
        /// </summary>
        public static GodotWindowService Instance => _instance ?? throw new SubsystemNotInitializedException();
        private static GodotWindowService? _instance;

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
        public static IGameEvent<bool> FocusChanged => Instance.FocusChanged;

        /// <summary>
        ///
        /// </summary>
        public static IGameEvent<EmptyEventArgs> CloseRequested => Instance.CloseRequested;

        /// <summary>
        ///
        /// </summary>
        /// <param name="instance"></param>
        internal static void Initialize(GodotWindowService instance)
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
    }
}
