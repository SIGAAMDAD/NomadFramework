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

namespace Nomad.Input.Private.ValueObjects {
	internal readonly struct ButtonLookupKey {
		/// <summary>
		/// 
		/// </summary>
		public InputDeviceSlot Device => _device;
		private readonly InputDeviceSlot _device;

		/// <summary>
		/// 
		/// </summary>
		public InputControlId Control => _control;
		private readonly InputControlId _control;

		/// <summary>
		/// 
		/// </summary>
		public bool Pressed => _pressed;
		private readonly bool _pressed;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="device"></param>
		/// <param name="control"></param>
		/// <param name="pressed"></param>
		public ButtonLookupKey( InputDeviceSlot device, InputControlId control, bool pressed ) {
			_device = device;
			_control = control;
			_pressed = pressed;
		}
	};
};