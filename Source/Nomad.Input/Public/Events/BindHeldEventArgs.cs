/*
===========================================================================
The Nomad MPLv2 Source Code
Copyright (C) 2025-2026 Noah Van Til

This Source Code Form is subject to the terms of the Mozilla Public
License, v2. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.

This software is provided "as is", without warranty of any kind,
express or implied, including but not limited to the warranties
of merchantability, fitness for a particular purpose and noninfringement.
===========================================================================
*/

namespace Nomad.Input.Events
{
    /// <summary>
    /// 
    /// </summary>
    public readonly struct BindHeldEventArgs
    {
        /// <summary>
        /// The amount of time the action has been triggered for.
        /// </summary>
        public long TimeHeld => _deltaTime;
        private readonly long _deltaTime;
        
        /// <summary>
        /// 
        /// </summary>
        public float Value => _value;
        private readonly float _value;

        /// <summary>
        /// 
        /// </summary>
        public int BindId => _bindId;
        private readonly int _bindId;
    }
}