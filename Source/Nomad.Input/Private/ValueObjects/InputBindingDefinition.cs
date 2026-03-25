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

using Nomad.Core.Input;

namespace Nomad.Input.Private.ValueObjects {
	internal sealed class InputBindingDefinition {
		public InputScheme Scheme { get; set; }
		public InputBindingKind Kind { get; set; }
		
		public ButtonBinding Button { get; set; } = default;
		public Axis1DBinding Axis1D { get; set; } = default;
		public Axis1DCompositeBinding Axis1DComposite { get; set; } = default;
		public Axis2DBinding Axis2D { get; set; } = default;
		public Axis2DCompositeBinding Axis2DComposite { get; set; } = default;
		public Delta2DBinding Delta2D { get; set; } = default;
	};
};