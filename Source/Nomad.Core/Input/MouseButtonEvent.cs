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

namespace Nomad.Core.Input
{
    /// <summary>
    /// 
    /// </summary>
    public readonly struct MouseButtonEvent
    {
        /// <summary>
        /// 
        /// </summary>
        public MouseButton Button => _button;
        private readonly MouseButton _button;

        /// <summary>
        /// 
        /// </summary>
        public bool Released => _released;
        private readonly bool _released;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="button"></param>
        /// <param name="released"></param>
        public MouseButtonEvent(MouseButton button, bool released)
        {
            _button = button;
            _released = released;
        }
    }
}