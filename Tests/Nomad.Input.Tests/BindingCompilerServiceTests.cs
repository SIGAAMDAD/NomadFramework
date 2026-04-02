using System.Collections.Immutable;
using NUnit.Framework;
using Nomad.Core.Input;
using Nomad.Input.Private.Repositories;
using Nomad.Input.Private.Services;
using Nomad.Input.Private.ValueObjects;
using Nomad.Input.ValueObjects;

namespace Nomad.Input.Tests {
	[TestFixture]
	public class BindingCompilerServiceTests {
		[Test]
		public void Compile_IndexesButtonBindingsForBothPressedStates() {
			var actions = new[] {
				InputTestHelpers.Action(
					"Jump",
					InputValueType.Button,
					InputTestHelpers.Button( InputScheme.KeyboardAndMouse, InputDeviceSlot.Keyboard, InputControlId.Space )
				)
			};

			var graph = BindingCompilerService.Compile( actions.ToImmutableArray() );

			using ( Assert.EnterMultipleScope() ) {
				Assert.That( graph.ButtonIndex[new ButtonLookupKey( InputDeviceSlot.Keyboard, InputControlId.Space, true )], Has.Length.EqualTo( 1 ) );
				Assert.That( graph.ButtonIndex[new ButtonLookupKey( InputDeviceSlot.Keyboard, InputControlId.Space, false )], Has.Length.EqualTo( 1 ) );
			}
		}

		[Test]
		public void Compile_IndexesAxisAndDeltaBindingsIntoSeparateLookups() {
			var actions = new[] {
				InputTestHelpers.Action(
					"Aim",
					InputValueType.Vector2,
					InputTestHelpers.Axis2D( InputScheme.Gamepad, InputDeviceSlot.Gamepad0, InputControlId.LeftStick )
				),
				InputTestHelpers.Action(
					"Look",
					InputValueType.Vector2,
					InputTestHelpers.Delta2D( InputScheme.KeyboardAndMouse, InputDeviceSlot.Mouse, InputControlId.Delta )
				)
			};

			var graph = BindingCompilerService.Compile( actions.ToImmutableArray() );

			using ( Assert.EnterMultipleScope() ) {
				Assert.That( graph.AxisIndex[new AxisLookupKey( InputDeviceSlot.Gamepad0, InputControlId.LeftStick )], Has.Length.EqualTo( 1 ) );
				Assert.That( graph.DeltaIndex[new AxisLookupKey( InputDeviceSlot.Mouse, InputControlId.Delta )], Has.Length.EqualTo( 1 ) );
			}
		}

		[Test]
		public void Compile_PlacesCompositeBindingsIntoCompositeCollections() {
			var actions = new[] {
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
			};

			var graph = BindingCompilerService.Compile( actions.ToImmutableArray() );

			using ( Assert.EnterMultipleScope() ) {
				Assert.That( graph.Composite1D, Has.Length.EqualTo( 1 ) );
				Assert.That( graph.Composite2D, Has.Length.EqualTo( 1 ) );
			}
		}

		[Test]
		public void CompileIntoRepository_ReplacesTheCurrentCompiledGraph() {
			var repository = new CompiledBindingRepository();
			var compiler = new BindingCompilerService( repository );
			var actions = new[] {
				InputTestHelpers.Action(
					"Jump",
					InputValueType.Button,
					InputTestHelpers.Button( InputScheme.KeyboardAndMouse, InputDeviceSlot.Keyboard, InputControlId.Space )
				)
			};

			compiler.CompileIntoRepository( actions.ToImmutableArray() );

			Assert.That( repository.GetButtonCandidates( new ButtonLookupKey( InputDeviceSlot.Keyboard, InputControlId.Space, true ) ).Length, Is.EqualTo( 1 ) );
		}
	}
}
