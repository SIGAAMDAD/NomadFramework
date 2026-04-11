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

using Nomad.Core.Util;
using Nomad.Core.Input;
using Nomad.Input.ValueObjects;

namespace Nomad.Input.Private.ValueObjects {
	internal readonly struct CompiledBinding {
		public InternString ActionName { get; }
		public InternString ActionId { get; }
		public InputValueType ValueType { get; }
		public InputBindingKind Kind { get; }
		public InputScheme Scheme { get; }
		public int Priority { get; }
		public bool ConsumesInput { get; }
		public uint ContextMask { get; }
		public ButtonBinding Button { get; }
		public Axis1DBinding Axis1D { get; }
		public Axis1DCompositeBinding Axis1DComposite { get; }
		public Axis2DBinding Axis2D { get; }
		public Axis2DCompositeBinding Axis2DComposite { get; }
		public Delta2DBinding Delta2D { get; }

		public CompiledBinding(
			InternString actionName,
			InternString actionId,
			InputValueType valueType,
			InputBindingKind kind,
			InputScheme scheme,
			int priority,
			bool consumesInput,
			uint contextMask,
			ButtonBinding button = default,
			Axis1DBinding axis1D = default,
			Axis1DCompositeBinding axis1DComposite = default,
			Axis2DBinding axis2D = default,
			Axis2DCompositeBinding axis2DComposite = default,
			Delta2DBinding delta2D = default
		) {
			ActionName = actionName;
			ActionId = actionId;
			ValueType = valueType;
			Kind = kind;
			Scheme = scheme;
			Priority = priority;
			ConsumesInput = consumesInput;
			ContextMask = contextMask;
			Button = button;
			Axis1D = axis1D;
			Axis1DComposite = axis1DComposite;
			Axis2D = axis2D;
			Axis2DComposite = axis2DComposite;
			Delta2D = delta2D;
		}
	};
};
