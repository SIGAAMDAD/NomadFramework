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
	/// Defines the phases of an input action's lifecycle.
	/// </summary>
	public enum InputActionPhase : byte
	{
		/// <summary>
		/// The action has just started (e.g., button pressed, axis moved from neutral).
		/// </summary>
		Started,

		/// <summary>
		/// The action is being performed (e.g., button held, axis at non-neutral position).
		/// </summary>
		Performed,

		/// <summary>
		/// The action has been canceled (e.g., button released, axis returned to neutral).
		/// </summary>
		Canceled,

		/// <summary>
		/// Sentinel value representing the total number of action phases.
		/// </summary>
		Count
	}
}