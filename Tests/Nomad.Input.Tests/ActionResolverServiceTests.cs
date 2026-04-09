using System;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using Nomad.Core.Input;
using Nomad.Core.Input.ValueObjects;
using Nomad.Core.Util;
using Nomad.Input.Private.Repositories;
using Nomad.Input.Private.Services;
using Nomad.Input.Private.ValueObjects;
using Nomad.Input.ValueObjects;

namespace Nomad.Input.Tests {
	[TestFixture]
	public class ActionResolverServiceTests {
		[Test]
		public void ResolveMatches_ResolvesButtonBindingsFromPressedState() {
			var repository = InputTestHelpers.CompileToRepository(
				InputTestHelpers.Action(
					"Jump",
					InputValueType.Button,
					InputTestHelpers.Button( InputScheme.KeyboardAndMouse, InputDeviceSlot.Keyboard, InputControlId.Space )
				)
			);
			var state = new InputStateService();
			state.SetPressed( InputDeviceSlot.Keyboard, InputControlId.Space, true );
			var matcher = new BindingMatcherService( repository, state );
			var resolver = new ActionResolverService( repository, state );
			ReadOnlySpan<BindingMatch> matches = matcher.MatchKeyboard( new KeyboardEventArgs( KeyNum.Space, 25, true ), uint.MaxValue, InputScheme.KeyboardAndMouse ).Span;

			var actions = resolver.ResolveMatches( ref matches, 25 );

			using ( Assert.EnterMultipleScope() ) {
				Assert.That( actions, Has.Length.EqualTo( 1 ) );
				Assert.That( actions[0].ActionId.ToString(), Is.EqualTo( "Jump" ) );
				Assert.That( actions[0].Phase, Is.EqualTo( InputActionPhase.Started ) );
				Assert.That( actions[0].ButtonValue, Is.True );
			}
		}

		[Test]
		public void ResolveMatches_ButtonUsesStartedOnceUntilReleased() {
			var repository = InputTestHelpers.CompileToRepository(
				InputTestHelpers.Action(
					"Jump",
					InputValueType.Button,
					InputTestHelpers.Button( InputScheme.KeyboardAndMouse, InputDeviceSlot.Keyboard, InputControlId.Space )
				)
			);
			var state = new InputStateService();
			var matcher = new BindingMatcherService( repository, state );
			var resolver = new ActionResolverService( repository, state );

			state.SetPressed( InputDeviceSlot.Keyboard, InputControlId.Space, true );
			ReadOnlySpan<BindingMatch> pressMatches = matcher.MatchKeyboard( new KeyboardEventArgs( KeyNum.Space, 25, true ), uint.MaxValue, InputScheme.KeyboardAndMouse ).Span;
			var startedActions = resolver.ResolveMatches( ref pressMatches, 25 );

			state.SetPressed( InputDeviceSlot.Keyboard, InputControlId.Space, true );
			ReadOnlySpan<BindingMatch> repeatPressMatches = matcher.MatchKeyboard( new KeyboardEventArgs( KeyNum.Space, 35, true ), uint.MaxValue, InputScheme.KeyboardAndMouse ).Span;
			var performedActions = resolver.ResolveMatches( ref repeatPressMatches, 35 );

			state.SetPressed( InputDeviceSlot.Keyboard, InputControlId.Space, false );
			ReadOnlySpan<BindingMatch> releaseMatches = matcher.MatchKeyboard( new KeyboardEventArgs( KeyNum.Space, 45, false ), uint.MaxValue, InputScheme.KeyboardAndMouse ).Span;
			var canceledActions = resolver.ResolveMatches( ref releaseMatches, 45 );

			state.SetPressed( InputDeviceSlot.Keyboard, InputControlId.Space, true );
			ReadOnlySpan<BindingMatch> rePressMatches = matcher.MatchKeyboard( new KeyboardEventArgs( KeyNum.Space, 55, true ), uint.MaxValue, InputScheme.KeyboardAndMouse ).Span;
			var restartedActions = resolver.ResolveMatches( ref rePressMatches, 55 );

			using ( Assert.EnterMultipleScope() ) {
				Assert.That( startedActions, Has.Length.EqualTo( 1 ) );
				Assert.That( startedActions[0].Phase, Is.EqualTo( InputActionPhase.Started ) );
				Assert.That( performedActions, Has.Length.EqualTo( 1 ) );
				Assert.That( performedActions[0].Phase, Is.EqualTo( InputActionPhase.Performed ) );
				Assert.That( canceledActions, Has.Length.EqualTo( 1 ) );
				Assert.That( canceledActions[0].Phase, Is.EqualTo( InputActionPhase.Canceled ) );
				Assert.That( restartedActions, Has.Length.EqualTo( 1 ) );
				Assert.That( restartedActions[0].Phase, Is.EqualTo( InputActionPhase.Started ) );
			}
		}

		[Test]
		public void ResolveMatches_AppliesAxis1DProcessors() {
			var repository = InputTestHelpers.CompileToRepository(
				InputTestHelpers.Action(
					"Throttle",
					InputValueType.Float,
					InputTestHelpers.Axis1D( InputScheme.Gamepad, InputDeviceSlot.Gamepad0, InputControlId.LeftTrigger, deadzone: 0.2f, sensitivity: 2.0f, scale: 0.5f, invert: true )
				)
			);
			var state = new InputStateService();
			state.SetAxis1D( InputDeviceSlot.Gamepad0, InputControlId.LeftTrigger, 0.4f );
			var matches = repository.GetAxisCandidates( new AxisLookupKey( InputDeviceSlot.Gamepad0, InputControlId.LeftTrigger ) );
			ReadOnlySpan<BindingMatch> bindingMatches = new[] { new BindingMatch { Binding = matches[0], Score = 10 } };
			var resolver = new ActionResolverService( repository, state );

			var actions = resolver.ResolveMatches( ref bindingMatches, 50 );

			using ( Assert.EnterMultipleScope() ) {
				Assert.That( actions, Has.Length.EqualTo( 1 ) );
				Assert.That( actions[0].FloatValue, Is.EqualTo( -0.4f ).Within( 0.0001f ) );
				Assert.That( actions[0].Phase, Is.EqualTo( InputActionPhase.Started ) );
			}
		}

		[Test]
		public void ResolveMatches_Axis1DUsesStartedThenPerformedAcrossActiveFrames() {
			var repository = InputTestHelpers.CompileToRepository(
				InputTestHelpers.Action(
					"Throttle",
					InputValueType.Float,
					InputTestHelpers.Axis1D( InputScheme.Gamepad, InputDeviceSlot.Gamepad0, InputControlId.LeftTrigger )
				)
			);
			var state = new InputStateService();
			var matches = repository.GetAxisCandidates( new AxisLookupKey( InputDeviceSlot.Gamepad0, InputControlId.LeftTrigger ) );
			ReadOnlySpan<BindingMatch> bindingMatches = new[] { new BindingMatch { Binding = matches[0], Score = 10 } };
			var resolver = new ActionResolverService( repository, state );

			state.SetAxis1D( InputDeviceSlot.Gamepad0, InputControlId.LeftTrigger, 0.4f );
			var startedActions = resolver.ResolveMatches( ref bindingMatches, 50 );

			state.SetAxis1D( InputDeviceSlot.Gamepad0, InputControlId.LeftTrigger, 0.6f );
			var performedActions = resolver.ResolveMatches( ref bindingMatches, 60 );

			using ( Assert.EnterMultipleScope() ) {
				Assert.That( startedActions, Has.Length.EqualTo( 1 ) );
				Assert.That( startedActions[0].Phase, Is.EqualTo( InputActionPhase.Started ) );
				Assert.That( performedActions, Has.Length.EqualTo( 1 ) );
				Assert.That( performedActions[0].Phase, Is.EqualTo( InputActionPhase.Performed ) );
			}
		}

		[Test]
		public void ResolveMatches_Axis1DCanStartAgainAfterReturningToZero() {
			var repository = InputTestHelpers.CompileToRepository(
				InputTestHelpers.Action(
					"Throttle",
					InputValueType.Float,
					InputTestHelpers.Axis1D( InputScheme.Gamepad, InputDeviceSlot.Gamepad0, InputControlId.LeftTrigger )
				)
			);
			var state = new InputStateService();
			var matches = repository.GetAxisCandidates( new AxisLookupKey( InputDeviceSlot.Gamepad0, InputControlId.LeftTrigger ) );
			ReadOnlySpan<BindingMatch> bindingMatches = new[] { new BindingMatch { Binding = matches[0], Score = 10 } };
			var resolver = new ActionResolverService( repository, state );

			state.SetAxis1D( InputDeviceSlot.Gamepad0, InputControlId.LeftTrigger, 0.4f );
			var startedActions = resolver.ResolveMatches( ref bindingMatches, 50 );

			state.SetAxis1D( InputDeviceSlot.Gamepad0, InputControlId.LeftTrigger, 0.0f );
			var canceledActions = resolver.ResolveMatches( ref bindingMatches, 60 );

			state.SetAxis1D( InputDeviceSlot.Gamepad0, InputControlId.LeftTrigger, 0.3f );
			var restartedActions = resolver.ResolveMatches( ref bindingMatches, 70 );

			using ( Assert.EnterMultipleScope() ) {
				Assert.That( startedActions, Has.Length.EqualTo( 1 ) );
				Assert.That( startedActions[0].Phase, Is.EqualTo( InputActionPhase.Started ) );
				Assert.That( canceledActions, Has.Length.EqualTo( 1 ) );
				Assert.That( canceledActions[0].Phase, Is.EqualTo( InputActionPhase.Canceled ) );
				Assert.That( restartedActions, Has.Length.EqualTo( 1 ) );
				Assert.That( restartedActions[0].Phase, Is.EqualTo( InputActionPhase.Started ) );
			}
		}

		[Test]
		public void ResolveMatches_AppliesAxis2DProcessors() {
			var repository = InputTestHelpers.CompileToRepository(
				InputTestHelpers.Action(
					"LookStick",
					InputValueType.Vector2,
					InputTestHelpers.Axis2D( InputScheme.Gamepad, InputDeviceSlot.Gamepad0, InputControlId.RightStick, deadzone: 0.1f, sensitivity: 2.0f, scaleX: 2.0f, scaleY: 0.5f, invertX: true )
				)
			);
			var state = new InputStateService();
			state.SetAxis2D( InputDeviceSlot.Gamepad0, InputControlId.RightStick, new Vector2( 0.25f, -0.5f ) );
			var matches = repository.GetAxisCandidates( new AxisLookupKey( InputDeviceSlot.Gamepad0, InputControlId.RightStick ) );
			ReadOnlySpan<BindingMatch> bindingMatches = new[] { new BindingMatch { Binding = matches[0], Score = 10 } };
			var resolver = new ActionResolverService( repository, state );

			var actions = resolver.ResolveMatches( ref bindingMatches, 50 );

			using ( Assert.EnterMultipleScope() ) {
				Assert.That( actions, Has.Length.EqualTo( 1 ) );
				Assert.That( actions[0].Vector2Value.X, Is.EqualTo( -1.0f ).Within( 0.0001f ) );
				Assert.That( actions[0].Vector2Value.Y, Is.EqualTo( -0.5f ).Within( 0.0001f ) );
			}
		}

		[Test]
		public void ResolveMatches_AppliesMouseDeltaProcessors() {
			var repository = InputTestHelpers.CompileToRepository(
				InputTestHelpers.Action(
					"Look",
					InputValueType.Vector2,
					InputTestHelpers.Delta2D( InputScheme.KeyboardAndMouse, InputDeviceSlot.Mouse, InputControlId.Delta, sensitivity: 2.0f, scaleX: 0.5f, scaleY: 1.5f, invertY: true )
				)
			);
			var state = new InputStateService();
			state.AddMouseDelta( new Vector2( 4.0f, 2.0f ) );
			var matches = repository.GetDeltaCandidates( new AxisLookupKey( InputDeviceSlot.Mouse, InputControlId.Delta ) );
			ReadOnlySpan<BindingMatch> bindingMatches = new[] { new BindingMatch { Binding = matches[0], Score = 10 } };
			var resolver = new ActionResolverService( repository, state );

			var actions = resolver.ResolveMatches( ref bindingMatches, 75 );

			using ( Assert.EnterMultipleScope() ) {
				Assert.That( actions, Has.Length.EqualTo( 1 ) );
				Assert.That( actions[0].Vector2Value.X, Is.EqualTo( 4.0f ).Within( 0.0001f ) );
				Assert.That( actions[0].Vector2Value.Y, Is.EqualTo( -6.0f ).Within( 0.0001f ) );
			}
		}

		[Test]
		public void ResolveMatches_PrefersHigherScoreThenHigherMagnitudePerAction() {
			var state = new InputStateService();
			state.SetPressed( InputDeviceSlot.Keyboard, InputControlId.Space, true );
			var resolver = new ActionResolverService( new CompiledBindingRepository(), state );

			var higherScoreBinding = new CompiledBinding(
				actionId: new InternString( "Jump" ),
				valueType: InputValueType.Button,
				kind: InputBindingKind.Button,
				scheme: InputScheme.KeyboardAndMouse,
				priority: 0,
				consumesInput: false,
				contextMask: uint.MaxValue,
				button: new ButtonBinding { DeviceId = InputDeviceSlot.Keyboard, ControlId = InputControlId.Space }
			);
			var lowerScoreBinding = new CompiledBinding(
				actionId: new InternString( "Jump" ),
				valueType: InputValueType.Button,
				kind: InputBindingKind.Button,
				scheme: InputScheme.KeyboardAndMouse,
				priority: 0,
				consumesInput: false,
				contextMask: uint.MaxValue,
				button: new ButtonBinding { DeviceId = InputDeviceSlot.Keyboard, ControlId = InputControlId.Space }
			);
			ReadOnlySpan<BindingMatch> matches = new[] {
				new BindingMatch { Binding = lowerScoreBinding, Score = 10 },
				new BindingMatch { Binding = higherScoreBinding, Score = 100 }
			};

			var actions = resolver.ResolveMatches( ref matches, 90 );

			Assert.That( actions, Has.Length.EqualTo( 1 ) );
		}

		[Test]
		public void ResolveComposites_ResolvesCompositeBindingsFromPressedKeys() {
			var repository = InputTestHelpers.CompileToRepository(
				InputTestHelpers.Action(
					"Throttle",
					InputValueType.Float,
					InputTestHelpers.Composite1D( InputScheme.KeyboardAndMouse, InputControlId.S, InputControlId.W )
				),
				InputTestHelpers.Action(
					"Move",
					InputValueType.Vector2,
					InputTestHelpers.Composite2D( InputScheme.KeyboardAndMouse, InputControlId.W, InputControlId.S, InputControlId.A, InputControlId.D )
				)
			);
			var state = new InputStateService();
			state.SetPressed( InputDeviceSlot.Keyboard, InputControlId.W, true );
			state.SetPressed( InputDeviceSlot.Keyboard, InputControlId.D, true );
			var resolver = new ActionResolverService( repository, state );

			var actions = resolver.ResolveComposites( uint.MaxValue, InputScheme.KeyboardAndMouse, 120 );

			Assert.That( actions, Has.Length.EqualTo( 2 ) );
			Assert.That( actions.Any( action => action.ActionId.ToString() == "Throttle" && action.FloatValue > 0.0f ), Is.True );
			Assert.That( actions.Any( action => action.ActionId.ToString() == "Move" && action.Vector2Value != Vector2.Zero ), Is.True );
			Assert.That( actions.Any( action => action.ActionId.ToString() == "Throttle" && action.Phase == InputActionPhase.Started ), Is.True );
			Assert.That( actions.Any( action => action.ActionId.ToString() == "Move" && action.Phase == InputActionPhase.Started ), Is.True );
		}
	}
}
