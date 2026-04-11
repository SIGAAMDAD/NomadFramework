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
	public struct Axis1DBinding
	{
		public float Deadzone { get; set; }
		public float Sensitivity { get; set; }
		public float Scale { get; set; }
		public bool Invert { get; set; }

		public ResponseCurve ResponseCurve { get; set; }

		public InputDeviceSlot DeviceId { get; set; }
		public InputControlId ControlId { get; set; }

		public Axis1DBinding(
			InputDeviceSlot deviceId,
			InputControlId controlId,
			float deadzone = 0.0f,
			float sensitivity = 1.0f,
			float scale = 1.0f,
			bool invert = false,
			ResponseCurve responseCurve = ResponseCurve.Linear
		) {
			DeviceId = deviceId;
			ControlId = controlId;
			Deadzone = deadzone;
			Sensitivity = sensitivity;
			Scale = scale;
			Invert = invert;
			ResponseCurve = responseCurve;
		}
	}
}