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
using Nomad.Core.Events;
using Nomad.Input.ValueObjects;

namespace Nomad.Input.Events
{
	/// <summary>
	/// 
	/// </summary>
	[Event(
		name: nameof(AxisActionEventArgs),
		nameSpace: "Nomad.Input.Events"
	)]
	public readonly partial struct AxisActionEventArgs
	{
		public InternString ActionId => _actionId;
		private readonly InternString _actionId;

		public InputActionPhase Phase => _phase;
		private readonly InputActionPhase _phase;

		public Vector2 Value => _value;
		private readonly Vector2 _value;

		public long TimeStamp => _timeStamp;
		private readonly long _timeStamp;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="actionId"></param>
		/// <param name="phase"></param>
		/// <param name="value"></param>
		/// <param name="timeStamp"></param>
		public AxisActionEventArgs(InternString actionId, InputActionPhase phase, Vector2 value, long timeStamp)
		{
			_actionId = actionId;
			_phase = phase;
			_value = value;
			_timeStamp = timeStamp;
		}
	}
}