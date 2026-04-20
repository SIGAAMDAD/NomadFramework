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
	/// Defines the different kinds of input bindings that can be created.
	/// </summary>
	public enum InputBindingKind : byte
	{
		/// <summary>
		/// A binding for discrete button inputs (press/release).
		/// </summary>
		Button,

		/// <summary>
		/// A binding for 1D axis inputs (single float value).
		/// </summary>
		Axis1D,

		/// <summary>
		/// A composite binding that combines multiple inputs into a 1D axis.
		/// </summary>
		Axis1DComposite,

		/// <summary>
		/// A binding for 2D axis inputs (vector2 value).
		/// </summary>
		Axis2D,

		/// <summary>
		/// A composite binding that combines multiple inputs into a 2D axis.
		/// </summary>
		Axis2DComposite,

		/// <summary>
		/// A binding for 2D delta inputs (like mouse movement).
		/// </summary>
		Delta2D
	}
}