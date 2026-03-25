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

using System.Collections.Generic;
using System.Numerics;
using Nomad.Core.Input;
using Nomad.Input.Private.ValueObjects;

namespace Nomad.Input.Private.Entities {
	internal sealed class InputState {
		public readonly bool[] PressedButtons = new bool[ (int)InputControlId.Count ];
		public readonly Dictionary<(InputDeviceType, int), float> Axis1D = new();
		public readonly Dictionary<(InputDeviceType, int), Vector2> Axis2D = new();
		public readonly Vector2 MouseDelta;
		public readonly Vector2 MousePosition;
	};
};