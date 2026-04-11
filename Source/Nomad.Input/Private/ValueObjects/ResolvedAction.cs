using System.Numerics;
using System.Runtime.CompilerServices;
using Nomad.Core.Util;
using Nomad.Input.ValueObjects;

namespace Nomad.Input.Private.ValueObjects {
	internal readonly struct ResolvedAction {
		public readonly InternString ActionId;
		public readonly int ActionIndex;
		public readonly long TimeStamp;

		public readonly Vector2 Vector2Value;
		public readonly float FloatValue;

		public readonly InputValueType ValueType;
		public readonly InputActionPhase Phase;
		public readonly bool ButtonValue;

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public ResolvedAction(
			InternString actionId,
			int actionIndex,
			InputValueType valueType,
			InputActionPhase phase,
			long timeStamp,
			bool buttonValue = default,
			float floatValue = default,
			Vector2 vector2Value = default
		) {
			ActionId = actionId;
			ActionIndex = actionIndex;
			TimeStamp = timeStamp;
			Vector2Value = vector2Value;
			FloatValue = floatValue;
			ValueType = valueType;
			Phase = phase;
			ButtonValue = buttonValue;
		}
	}
}