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
using System.Runtime.CompilerServices;
using Nomad.Core.UI;

namespace Nomad.EngineUtils
{
    /// <summary>
    ///
    /// </summary>
    public static class AlignmentExtensions
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static global::Godot.HorizontalAlignment ToGodot(this HorizontalAlignment from) => from switch
        {
            HorizontalAlignment.Center => global::Godot.HorizontalAlignment.Center,
            HorizontalAlignment.Start => global::Godot.HorizontalAlignment.Left,
            HorizontalAlignment.End => global::Godot.HorizontalAlignment.Right,
            HorizontalAlignment.Stretch => global::Godot.HorizontalAlignment.Fill,
            _ => throw new ArgumentOutOfRangeException(nameof(from))
        };

        /// <summary>
        ///
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static HorizontalAlignment ToNomad(this global::Godot.HorizontalAlignment from) => from switch
        {
            global::Godot.HorizontalAlignment.Center => HorizontalAlignment.Center,
            global::Godot.HorizontalAlignment.Left => HorizontalAlignment.Start,
            global::Godot.HorizontalAlignment.Right => HorizontalAlignment.End,
            global::Godot.HorizontalAlignment.Fill => HorizontalAlignment.Stretch,
            _ => throw new ArgumentOutOfRangeException(nameof(from))
        };

        /// <summary>
        ///
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static global::Godot.VerticalAlignment ToGodot(this VerticalAlignment from) => from switch
        {
            VerticalAlignment.Center => global::Godot.VerticalAlignment.Center,
            VerticalAlignment.Start => global::Godot.VerticalAlignment.Top,
            VerticalAlignment.End => global::Godot.VerticalAlignment.Bottom,
            VerticalAlignment.Stretch => global::Godot.VerticalAlignment.Fill,
            _ => throw new ArgumentOutOfRangeException(nameof(from))
        };

        /// <summary>
        ///
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VerticalAlignment ToNomad(this global::Godot.VerticalAlignment from) => from switch
        {
            global::Godot.VerticalAlignment.Center => VerticalAlignment.Center,
            global::Godot.VerticalAlignment.Top => VerticalAlignment.Start,
            global::Godot.VerticalAlignment.Bottom => VerticalAlignment.End,
            global::Godot.VerticalAlignment.Fill => VerticalAlignment.Stretch,
            _ => throw new ArgumentOutOfRangeException(nameof(from))
        };
    }
}
