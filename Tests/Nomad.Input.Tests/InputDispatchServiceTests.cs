using System.Numerics;
using NUnit.Framework;
using Nomad.Core.Util;
using Nomad.Events;
using Nomad.Input.Events;
using Nomad.Input.Private.Services;
using Nomad.Input.Private.ValueObjects;
using Nomad.Input.ValueObjects;

namespace Nomad.Input.Tests {
	[TestFixture]
	public class InputDispatchServiceTests {
		private GameEventRegistry _eventRegistry;
		private MockLogger _logger;

		[SetUp]
		public void SetUp() {
			_eventRegistry = InputTestHelpers.CreateEventRegistry( out _logger );
		}

		[TearDown]
		public void TearDown() {
			_eventRegistry.Dispose();
			_logger.Dispose();
		}

		[Test]
		public void Dispatch_PublishesButtonActionsToTheButtonEvent() {
			var dispatcher = new InputDispatchService( _eventRegistry );
			ButtonActionEventArgs? published = null;
			var gameEvent = _eventRegistry.GetEvent<ButtonActionEventArgs>( $"player.jump:{Constants.Events.BUTTON_ACTION}", Constants.Events.NAMESPACE );
			gameEvent.Subscribe( ( in ButtonActionEventArgs args ) => published = args );

			dispatcher.Dispatch( new ResolvedAction( new InternString( "player.jump" ), InputValueType.Button, InputActionPhase.Started, 10, buttonValue: true ) );

			using ( Assert.EnterMultipleScope() ) {
				Assert.That( published.HasValue, Is.True );
				Assert.That( published!.Value.ActionId.ToString(), Is.EqualTo( "player.jump" ) );
				Assert.That( published.Value.Value, Is.True );
			}
		}

		[Test]
		public void Dispatch_PublishesFloatActionsToTheFloatEvent() {
			var dispatcher = new InputDispatchService( _eventRegistry );
			FloatActionEventArgs? published = null;
			var gameEvent = _eventRegistry.GetEvent<FloatActionEventArgs>( $"Throttle:{Constants.Events.FLOAT_ACTION}", Constants.Events.NAMESPACE );
			gameEvent.Subscribe( ( in FloatActionEventArgs args ) => published = args );

			dispatcher.Dispatch( new ResolvedAction( new InternString( "Throttle" ), InputValueType.Float, InputActionPhase.Performed, 20, floatValue: 0.75f ) );

			using ( Assert.EnterMultipleScope() ) {
				Assert.That( published.HasValue, Is.True );
				Assert.That( published!.Value.Value, Is.EqualTo( 0.75f ).Within( 0.0001f ) );
			}
		}

		[Test]
		public void Dispatch_PublishesVector2ActionsToTheAxisEvent() {
			var dispatcher = new InputDispatchService( _eventRegistry );
			AxisActionEventArgs? published = null;
			var gameEvent = _eventRegistry.GetEvent<AxisActionEventArgs>( $"Look:{Constants.Events.AXIS_ACTION}", Constants.Events.NAMESPACE );
			gameEvent.Subscribe( ( in AxisActionEventArgs args ) => published = args );

			dispatcher.Dispatch( new ResolvedAction( new InternString( "Look" ), InputValueType.Vector2, InputActionPhase.Performed, 30, vector2Value: new Vector2( 4.0f, -2.0f ) ) );

			using ( Assert.EnterMultipleScope() ) {
				Assert.That( published.HasValue, Is.True );
				Assert.That( published!.Value.Value, Is.EqualTo( new Vector2( 4.0f, -2.0f ) ) );
			}
		}
	}
}
