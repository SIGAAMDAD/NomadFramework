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

namespace Nomad.Input.Private.ValueObjects {
    internal readonly struct CompiledBinding {
        public InternString ActionId => _actionId;
        private readonly InternString _actionId;

        public InputValueType ValueType => _valueType;
        private readonly InputValueType _valueType;

        public InputBindingKind Kind => _kind;
        private readonly InputBindingKind _kind;

        public InputScheme Scheme => _scheme;
        private readonly InputScheme _scheme;

        public int Priority => _priority;
        private readonly int _priority;

        public bool ConsumesInput => _consumesInput;
        private readonly bool _consumesInput;

        public uint ContextMask => _contextMask;
        private readonly uint _contextMask;

        public ButtonBinding Button => _button;
        private readonly ButtonBinding _button;

        public Axis1DBinding Axis1D => _axis1D;
        private readonly Axis1DBinding _axis1D;

        public Axis1DCompositeBinding Axis1DComposite => _axis1DComposite;
        private readonly Axis1DCompositeBinding _axis1DComposite;

        public Axis2DBinding Axis2D => _axis2D;
        private readonly Axis2DBinding _axis2D;

        public Axis2DCompositeBinding Axis2DComposite => _axis2DComposite;
        private readonly Axis2DCompositeBinding _axis2DComposite;

        public Delta2DBinding Delta2D => _delta2D;
        private readonly Delta2DBinding _delta2D;

        public CompiledBinding(
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
            _actionId = actionId;
            _valueType = valueType;
            _kind = kind;
            _scheme = scheme;
            _priority = priority;
            _consumesInput = consumesInput;
            _contextMask = contextMask;
            _button = button;
            _axis1D = axis1D;
            _axis1DComposite = axis1DComposite;
            _axis2D = axis2D;
            _axis2DComposite = axis2DComposite;
            _delta2D = delta2D;
        }
    };
};
