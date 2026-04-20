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
	/// Defines the available device slots for input devices.
	/// </summary>
	public enum InputDeviceSlot : byte
	{
		/// <summary>
		/// The primary keyboard device slot.
		/// </summary>
		Keyboard,

		/// <summary>
		/// The primary mouse device slot.
		/// </summary>
		Mouse,

		/// <summary>
		/// The first gamepad device slot.
		/// </summary>
		Gamepad0,

		/// <summary>
		/// The second gamepad device slot.
		/// </summary>
		Gamepad1,

		/// <summary>
		/// The third gamepad device slot.
		/// </summary>
		Gamepad2,

		/// <summary>
		/// The fourth gamepad device slot.
		/// </summary>
		Gamepad3,

		/// <summary>
		/// Sentinel value representing the total number of device slots.
		/// </summary>
		Count
	}
}