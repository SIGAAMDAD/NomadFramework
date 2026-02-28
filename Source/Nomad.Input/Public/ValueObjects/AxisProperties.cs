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

namespace Nomad.Input.ValueObjects
{
    /// <summary>
    /// 
    /// </summary>
    public readonly struct AxisProperties
    {
        /// <summary>
        /// 
        /// </summary>
        public int AxisIndex => _axisIndex;
        private readonly int _axisIndex;

        /// <summary>
        /// 
        /// </summary>
        public bool Inverted => _inverted;
        private readonly bool _inverted;

        /// <summary>
        /// 
        /// </summary>
        public float DeadZone => _deadZone;
        private readonly float _deadZone;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="axisIndex"></param>
        /// <param name="inverted"></param>
        /// <param name="deadZone"></param>
        public AxisProperties(int axisIndex, bool inverted, float deadZone)
        {
            _axisIndex = axisIndex;
            _inverted = inverted;
            _deadZone = deadZone;
        }
    }
}
