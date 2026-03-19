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

namespace Nomad.Core.UI
{
    /// <summary>
    ///
    /// </summary>
    public readonly struct Alignment
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly Alignment Center = new Alignment(HorizontalAlignment.Center, VerticalAlignment.Center);

        /// <summary>
        /// 
        /// </summary>
        public static readonly Alignment TopLeft = new Alignment(HorizontalAlignment.Start, VerticalAlignment.Start);

        /// <summary>
        /// 
        /// </summary>
        public static readonly Alignment TopRight = new Alignment(HorizontalAlignment.End, VerticalAlignment.Start);

        /// <summary>
        /// 
        /// </summary>
        public static readonly Alignment BottomLeft = new Alignment(HorizontalAlignment.Start, VerticalAlignment.End);

        /// <summary>
        /// 
        /// </summary>
        public static readonly Alignment BottomRight = new Alignment(HorizontalAlignment.End, VerticalAlignment.End);

        /// <summary>
        /// 
        /// </summary>
        public static readonly Alignment Fill = new Alignment(HorizontalAlignment.Stretch, VerticalAlignment.Stretch);

        /// <summary>
        ///
        /// </summary>
        public HorizontalAlignment Horizontal => _horizontal;
        private readonly HorizontalAlignment _horizontal;

        /// <summary>
        ///
        /// </summary>
        public VerticalAlignment Vertical => _vertical;
        private readonly VerticalAlignment _vertical;

        /// <summary>
        ///
        /// </summary>
        /// <param name="horizontal"></param>
        /// <param name="vertical"></param>
        public Alignment(HorizontalAlignment horizontal, VerticalAlignment vertical)
        {
            _horizontal = horizontal;
            _vertical = vertical;
        }
    }
}
