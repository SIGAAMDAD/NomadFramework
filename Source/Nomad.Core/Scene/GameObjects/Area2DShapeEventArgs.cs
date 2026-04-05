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

namespace Nomad.Core.Scene.GameObjects
{
    /// <summary>
    /// Describes a shape-level overlap event involving another area.
    /// </summary>
    public readonly struct Area2DShapeEventArgs
    {
        /// <summary>
        /// The overlapping area.
        /// </summary>
        public IArea2D Area => _area;

        /// <summary>
        /// The shape index on the other area.
        /// </summary>
        public long AreaShapeIndex => _areaShapeIndex;

        /// <summary>
        /// The local shape index on this area.
        /// </summary>
        public long LocalShapeIndex => _localShapeIndex;

        private readonly IArea2D _area;
        private readonly long _areaShapeIndex;
        private readonly long _localShapeIndex;

        /// <summary>
        /// Creates a new set of shape-level area overlap event arguments.
        /// </summary>
        public Area2DShapeEventArgs(IArea2D area, long areaShapeIndex, long localShapeIndex)
        {
            _area = area;
            _areaShapeIndex = areaShapeIndex;
            _localShapeIndex = localShapeIndex;
        }
    }
}
