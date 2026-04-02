using System.Numerics;
using NUnit.Framework;
using Nomad.Core.Input;
using Nomad.Input.Private.Services;
using Nomad.Input.ValueObjects;

namespace Nomad.Input.Tests {
	[TestFixture]
	public class InputStateServiceTests {
		[Test]
		public void SetPressed_TracksPressedStatePerDeviceAndControl() {
			var state = new InputStateService();

			state.SetPressed( InputDeviceSlot.Keyboard, InputControlId.Space, true );

			Assert.That( state.IsPressed( InputDeviceSlot.Keyboard, InputControlId.Space ), Is.True );
		}

		[Test]
		public void TryGetAxis1D_ReturnsFalseForNonAxisControls() {
			var state = new InputStateService();

			bool success = state.TryGetAxis1D( InputDeviceSlot.Keyboard, InputControlId.Space, out float value );

			using ( Assert.EnterMultipleScope() ) {
				Assert.That( success, Is.False );
				Assert.That( value, Is.EqualTo( 0.0f ) );
			}
		}

		[Test]
		public void SetAxis1D_StoresTriggerValues() {
			var state = new InputStateService();

			state.SetAxis1D( InputDeviceSlot.Gamepad0, InputControlId.LeftTrigger, 0.75f );
			bool success = state.TryGetAxis1D( InputDeviceSlot.Gamepad0, InputControlId.LeftTrigger, out float value );

			using ( Assert.EnterMultipleScope() ) {
				Assert.That( success, Is.True );
				Assert.That( value, Is.EqualTo( 0.75f ) );
			}
		}

		[Test]
		public void SetAxis2D_StoresStickValues() {
			var state = new InputStateService();
			Vector2 expected = new( 0.25f, -0.75f );

			state.SetAxis2D( InputDeviceSlot.Gamepad0, InputControlId.LeftStick, expected );
			bool success = state.TryGetAxis2D( InputDeviceSlot.Gamepad0, InputControlId.LeftStick, out Vector2 value );

			using ( Assert.EnterMultipleScope() ) {
				Assert.That( success, Is.True );
				Assert.That( value, Is.EqualTo( expected ) );
			}
		}

		[Test]
		public void SetMousePositionAndAddMouseDelta_UpdateMouseState() {
			var state = new InputStateService();
			Vector2 position = new( 320.0f, 180.0f );
			Vector2 delta = new( 5.0f, -3.0f );

			state.SetMousePosition( position );
			state.AddMouseDelta( delta );

			using ( Assert.EnterMultipleScope() ) {
				Assert.That( state.MousePosition, Is.EqualTo( position ) );
				Assert.That( state.MouseDelta, Is.EqualTo( delta ) );
				Assert.That( state.TryGetAxis2D( InputDeviceSlot.Mouse, InputControlId.Position, out Vector2 storedPosition ), Is.True );
				Assert.That( storedPosition, Is.EqualTo( position ) );
				Assert.That( state.TryGetAxis2D( InputDeviceSlot.Mouse, InputControlId.Delta, out Vector2 storedDelta ), Is.True );
				Assert.That( storedDelta, Is.EqualTo( delta ) );
			}
		}
	}
}
