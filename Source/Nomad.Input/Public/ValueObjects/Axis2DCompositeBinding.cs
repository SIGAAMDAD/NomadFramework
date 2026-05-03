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
    public struct Axis2DCompositeBinding
    {
        public float ScaleX { get; set; }
        public float ScaleY { get; set; }

        public bool Normalize { get; set; }

        public InputControlId Up { get; set; }
        public InputControlId Down { get; set; }
        public InputControlId Left { get; set; }
        public InputControlId Right { get; set; }

        public Axis2DCompositeBinding(
            InputControlId up,
            InputControlId down,
            InputControlId left,
            InputControlId right,
            bool normalize = true,
            float scaleX = 1.0f,
            float scaleY = 1.0f
        )
        {
            Up = up;
            Down = down;
            Left = left;
            Right = right;
            Normalize = normalize;
            ScaleX = scaleX;
            ScaleY = scaleY;
        }
    }
}
