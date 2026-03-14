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
        public static Godot.HorizontalAlignment ToGodot(this HorizontalAlignment from) => from switch
        {
            HorizontalAlignment.Center => Godot.HorizontalAlignment.Center,
            HorizontalAlignment.Start => Godot.HorizontalAlignment.Left,
            HorizontalAlignment.End => Godot.HorizontalAlignment.Right,
            HorizontalAlignment.Stretch => Godot.HorizontalAlignment.Fill,
            _ => throw new ArgumentOutOfRangeException(nameof(from))
        };

        /// <summary>
        ///
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static HorizontalAlignment ToNomad(this Godot.HorizontalAlignment from) => from switch
        {
            Godot.HorizontalAlignment.Center => HorizontalAlignment.Center,
            Godot.HorizontalAlignment.Left => HorizontalAlignment.Start,
            Godot.HorizontalAlignment.Right => HorizontalAlignment.End,
            Godot.HorizontalAlignment.Fill => HorizontalAlignment.Stretch,
            _ => throw new ArgumentOutOfRangeException(nameof(from))
        };

        /// <summary>
        ///
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Godot.VerticalAlignment ToGodot(this VerticalAlignment from) => from switch
        {
            VerticalAlignment.Center => Godot.VerticalAlignment.Center,
            VerticalAlignment.Start => Godot.VerticalAlignment.Top,
            VerticalAlignment.End => Godot.VerticalAlignment.Bottom,
            VerticalAlignment.Stretch => Godot.VerticalAlignment.Fill,
            _ => throw new ArgumentOutOfRangeException(nameof(from))
        };

        /// <summary>
        ///
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VerticalAlignment ToNomad(this Godot.VerticalAlignment from) => from switch
        {
            Godot.VerticalAlignment.Center => VerticalAlignment.Center,
            Godot.VerticalAlignment.Top => VerticalAlignment.Start,
            Godot.VerticalAlignment.Bottom => VerticalAlignment.End,
            Godot.VerticalAlignment.Fill => VerticalAlignment.Stretch,
            _ => throw new ArgumentOutOfRangeException(nameof(from))
        };
    }
}
