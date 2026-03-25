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

using System.Numerics;
using Nomad.Core.Util;
using Nomad.Input.ValueObjects;

namespace Nomad.Input.Private.ValueObjects {
	/// <summary>
	/// 
	/// </summary>
	internal readonly struct ResolvedAction {
		public InternString ActionId => _actionId;
		private readonly InternString _actionId;

		public InputValueType ValueType => _valueType;
		private readonly InputValueType _valueType;

		public InputActionPhase Phase => _phase;
		private readonly InputActionPhase _phase;

		public long TimeStamp => _timeStamp;
		private readonly long _timeStamp;

		public bool ButtonValue => _buttonValue;
		private readonly bool _buttonValue;

		public float FloatValue => _floatValue;
		private readonly float _floatValue;

		public Vector2 Vector2Value => _vector2Value;
		private readonly Vector2 _vector2Value;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="actionId"></param>
		/// <param name="valueType"></param>
		/// <param name="phase"></param>
		/// <param name="timeStamp"></param>
		/// <param name="buttonValue"></param>
		/// <param name="floatValue"></param>
		/// <param name="vector2Value"></param>
		public ResolvedAction( InternString actionId, InputValueType valueType, InputActionPhase phase, long timeStamp, bool buttonValue = default, float floatValue = default, Vector2 vector2Value = default ) {
			_actionId = actionId;
			_valueType = valueType;
			_phase = phase;
			_timeStamp = timeStamp;
			_buttonValue = buttonValue;
			_floatValue = floatValue;
			_vector2Value = vector2Value;
		}
	};
};