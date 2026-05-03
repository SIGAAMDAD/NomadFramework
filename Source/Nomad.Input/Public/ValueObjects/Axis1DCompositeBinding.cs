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
    public struct Axis1DCompositeBinding
    {
        public float Scale { get; set; }
        public bool Normalize { get; set; }

        public InputControlId Negative { get; set; }
        public InputControlId Positive { get; set; }

        public Axis1DCompositeBinding(
            InputControlId negative,
            InputControlId positive,
            float scale = 1.0f,
            bool normalize = true
        )
        {
            Negative = negative;
            Positive = positive;
            Scale = scale;
            Normalize = normalize;
        }
    }
}
