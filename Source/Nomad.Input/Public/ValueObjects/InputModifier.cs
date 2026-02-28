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

using Nomad.Core.Input.ValueObjects;

namespace Nomad.Input.ValueObjects
{
    /// <summary>
    /// 
    /// </summary>
    public readonly struct InputModifier
    {
        /// <summary>
        /// 
        /// </summary>
        public KeyNum ModifierKey => _modifierKey;
        private readonly KeyNum _modifierKey;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modifierKey"></param>
        public InputModifier(KeyNum modifierKey)
        {
            _modifierKey = modifierKey;
        }
    }
}
