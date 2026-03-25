using System.Numerics;
using NUnit.Framework;
using Nomad.Core.Input;
using Nomad.Core.Input.ValueObjects;
using Nomad.Input.Private.Services;
using Nomad.Input.Private.ValueObjects;

namespace Nomad.Input.Tests {
	[TestFixture]
	public class BindingMatcherServiceTests {
		[Test]
		public void MatchKeyboard_ReturnsCandidateWhenContextAndSchemeMatch() {
			var repository = InputTestHelpers.CompileToRepository(
				InputTestHelpers.Action(
					"Jump",
					InputValueType.Button,
					InputTestHelpers.Button( InputScheme.KeyboardAndMouse, InputDeviceSlot.Keyboard, InputControlId.Space )
				)
			);
			var state = new InputStateService();
			var matcher = new BindingMatcherService( repository, state );

			var matches = matcher.MatchKeyboard( new KeyboardEventArgs( KeyNum.Space, 10, true ), uint.MaxValue, InputScheme.KeyboardAndMouse );

			Assert.That( matches.Span.Length, Is.EqualTo( 1 ) );
		}

		[Test]
		public void MatchKeyboard_RequiresAllModifiersToBePressed() {
			var repository = InputTestHelpers.CompileToRepository(
				InputTestHelpers.Action(
					"SprintJump",
					InputValueType.Button,
					InputTestHelpers.Button( InputScheme.KeyboardAndMouse, InputDeviceSlot.Keyboard, InputControlId.Space, InputControlId.Shift )
				)
			);
			var state = new InputStateService();
			var matcher = new BindingMatcherService( repository, state );

			var withoutModifier = matcher.MatchKeyboard( new KeyboardEventArgs( KeyNum.Space, 10, true ), uint.MaxValue, InputScheme.KeyboardAndMouse );
			state.SetPressed( InputDeviceSlot.Keyboard, InputControlId.Shift, true );
			var withModifier = matcher.MatchKeyboard( new KeyboardEventArgs( KeyNum.Space, 10, true ), uint.MaxValue, InputScheme.KeyboardAndMouse );

			using ( Assert.EnterMultipleScope() ) {
				Assert.That( withoutModifier.Span.Length, Is.EqualTo( 0 ) );
				Assert.That( withModifier.Span.Length, Is.EqualTo( 1 ) );
				Assert.That( withModifier.Span[0].Score, Is.EqualTo( 35 ) );
			}
		}

		[Test]
		public void MatchMouseButton_UsesMouseButtonControlConversion() {
			var repository = InputTestHelpers.CompileToRepository(
				InputTestHelpers.Action(
					"OpenContextMenu",
					InputValueType.Button,
					InputTestHelpers.Button( InputScheme.KeyboardAndMouse, InputDeviceSlot.Mouse, InputControlId.Right )
				)
			);
			var matcher = new BindingMatcherService( repository, new InputStateService() );

			var matches = matcher.MatchMouseButton( new MouseButtonEventArgs( MouseButton.Right, 10, true ), uint.MaxValue, InputScheme.KeyboardAndMouse );

			Assert.That( matches.Span.Length, Is.EqualTo( 1 ) );
		}

		[Test]
		public void MatchGamepadButton_UsesTheEventDeviceId() {
			var repository = InputTestHelpers.CompileToRepository(
				InputTestHelpers.Action(
					"Confirm",
					InputValueType.Button,
					InputTestHelpers.Button( InputScheme.Gamepad, InputDeviceSlot.Gamepad1, InputControlId.GamepadA )
				)
			);
			var matcher = new BindingMatcherService( repository, new InputStateService() );

			var matches = matcher.MatchGamepadButton( new GamepadButtonEventArgs( GamepadButton.A, 1, 10, true ), uint.MaxValue, InputScheme.Gamepad );

			Assert.That( matches.Span.Length, Is.EqualTo( 1 ) );
		}

		[Test]
		public void MatchGamepadAxis_FiltersByContextAndScheme() {
			var repository = InputTestHelpers.CompileToRepository(
				InputTestHelpers.Action(
					"LookStick",
					InputValueType.Vector2,
					InputTestHelpers.Axis2D( InputScheme.Gamepad, InputDeviceSlot.Gamepad0, InputControlId.RightStick )
				)
			);
			var matcher = new BindingMatcherService( repository, new InputStateService() );

			var wrongContext = matcher.MatchGamepadAxis( InputDeviceSlot.Gamepad0, InputControlId.RightStick, 0, InputScheme.Gamepad );
			var wrongScheme = matcher.MatchGamepadAxis( InputDeviceSlot.Gamepad0, InputControlId.RightStick, uint.MaxValue, InputScheme.KeyboardAndMouse );
			var correct = matcher.MatchGamepadAxis( InputDeviceSlot.Gamepad0, InputControlId.RightStick, uint.MaxValue, InputScheme.Gamepad );

			using ( Assert.EnterMultipleScope() ) {
				Assert.That( wrongContext.Span.Length, Is.EqualTo( 0 ) );
				Assert.That( wrongScheme.Span.Length, Is.EqualTo( 0 ) );
				Assert.That( correct.Span.Length, Is.EqualTo( 1 ) );
			}
		}

		[Test]
		public void MatchMouseDelta_ReturnsEmptyWhenThereIsNoMouseDelta() {
			var repository = InputTestHelpers.CompileToRepository(
				InputTestHelpers.Action(
					"Look",
					InputValueType.Vector2,
					InputTestHelpers.Delta2D( InputScheme.KeyboardAndMouse, InputDeviceSlot.Mouse, InputControlId.Delta )
				)
			);
			var state = new InputStateService();
			var matcher = new BindingMatcherService( repository, state );

			var noDelta = matcher.MatchMouseDelta( uint.MaxValue, InputScheme.KeyboardAndMouse );
			state.AddMouseDelta( new Vector2( 3.0f, -2.0f ) );
			var withDelta = matcher.MatchMouseDelta( uint.MaxValue, InputScheme.KeyboardAndMouse );

			using ( Assert.EnterMultipleScope() ) {
				Assert.That( noDelta.Span.Length, Is.EqualTo( 0 ) );
				Assert.That( withDelta.Span.Length, Is.EqualTo( 1 ) );
			}
		}
	}
}
