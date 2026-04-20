using System.Reflection;
using System.Numerics;
using NUnit.Framework;
using Nomad.Core.Input;
using Nomad.Events;
using Nomad.Input.Events;
using Nomad.Input.Private.Services;
using Nomad.Input.ValueObjects;
using Nomad.Core.ServiceRegistry.Services;
using Nomad.Core.Input.ValueObjects;

namespace Nomad.Input.Tests {
	[TestFixture]
	public class InputSystemTests {
		private static readonly FieldInfo InputModeField = typeof( InputSystem ).GetField( "_mode", BindingFlags.Instance | BindingFlags.NonPublic )!;

		private GameEventRegistry _eventRegistry;
		private MockLogger _logger;
		private ServiceCollection _registry;

		[SetUp]
		public void SetUp() {
			_eventRegistry = InputTestHelpers.CreateEventRegistry( out _logger );
			_registry = new ServiceCollection();
		}

		[TearDown]
		public void TearDown() {
			_eventRegistry.Dispose();
			_logger.Dispose();
			_registry.Dispose();
		}

		[Test]
		public void PushKeyboardEvent_DispatchesConfiguredButtonAction() {
			const string defaultsPath = "Assets/Config/Bindings/DefaultBinds.json";
			var fileSystem = new InputFileSystemFixture(
				(defaultsPath, """
				{
				  "Bindings": [
				    {
					  "Id": "Jump",
				      "Name": "Jump",
				      "ValueType": "Button",
				      "Scheme": "KeyboardAndMouse",
				      "Bindings": { "DeviceId": "Keyboard", "ControlId": "Space" }
				    }
				  ]
				}
				""" )
			);
			var cvarSystem = InputTestHelpers.CreateCVarSystem( _eventRegistry, defaultsPath );
			ButtonActionEventArgs? published = null;
			_eventRegistry.GetEvent<ButtonActionEventArgs>( $"Jump:{Constants.Events.BUTTON_ACTION}", Constants.Events.NAMESPACE )
				.Subscribe( ( in ButtonActionEventArgs args ) => published = args );

			using var inputSystem = new InputSystem( fileSystem.Object, cvarSystem, _logger, _eventRegistry, _registry );
			inputSystem.PushKeyboardEvent( new KeyboardEventArgs( KeyNum.Space, 100, true ) );

			using ( Assert.EnterMultipleScope() ) {
				Assert.That( published.HasValue, Is.True );
				Assert.That( published!.Value.Value, Is.True );
				Assert.That( published.Value.Phase, Is.EqualTo( InputActionPhase.Started ) );
			}
		}

		[Test]
		public void PushMouseMotionEvent_DispatchesConfiguredDeltaAction() {
			const string defaultsPath = "Assets/Config/Bindings/DefaultBinds.json";
			var fileSystem = new InputFileSystemFixture(
				(defaultsPath, """
				{
				  "Bindings": [
				    {
					  "Id": "Look",
				      "Name": "Look",
				      "ValueType": "Vector2",
				      "Scheme": "KeyboardAndMouse",
				      "Bindings": {
				        "Kind": "Delta2D",
				        "DeviceId": "Mouse",
				        "ControlId": "Delta",
				        "Sensitivity": 2.0,
				        "InvertY": true
				      }
				    }
				  ]
				}
				""" )
			);
			var cvarSystem = InputTestHelpers.CreateCVarSystem( _eventRegistry, defaultsPath );
			AxisActionEventArgs? published = null;
			_eventRegistry.GetEvent<AxisActionEventArgs>( $"Look:{Constants.Events.AXIS_ACTION}", Constants.Events.NAMESPACE )
				.Subscribe( ( in AxisActionEventArgs args ) => published = args );

			using var inputSystem = new InputSystem( fileSystem.Object, cvarSystem, _logger, _eventRegistry, _registry );
			inputSystem.PushMouseMotionEvent( new MouseMotionEventArgs( 200, 4, 3 ) );

			using ( Assert.EnterMultipleScope() ) {
				Assert.That( published.HasValue, Is.True );
				Assert.That( published!.Value.Value, Is.EqualTo( new Vector2( 8.0f, -6.0f ) ) );
			}
		}

		[Test]
		public void PushGamepadAxisEvent_DispatchesConfiguredStickAction() {
			const string defaultsPath = "Assets/Config/Bindings/DefaultBinds.json";
			var fileSystem = new InputFileSystemFixture(
				(defaultsPath, """
				{
				  "Bindings": [
				    {
					  "Id": "Aim",
				      "Name": "Aim",
				      "ValueType": "Vector2",
				      "Scheme": "Gamepad",
				      "Bindings": {
				        "Kind": "Axis2D",
				        "DeviceId": "Gamepad",
				        "ControlId": "LeftStick"
				      }
				    }
				  ]
				}
				""" )
			);
			var cvarSystem = InputTestHelpers.CreateCVarSystem( _eventRegistry, defaultsPath );
			AxisActionEventArgs? published = null;
			_eventRegistry.GetEvent<AxisActionEventArgs>( $"Aim:{Constants.Events.AXIS_ACTION}", Constants.Events.NAMESPACE )
				.Subscribe( ( in AxisActionEventArgs args ) => published = args );

			using var inputSystem = new InputSystem( fileSystem.Object, cvarSystem, _logger, _eventRegistry, _registry );
			SetMode( inputSystem, InputScheme.Gamepad );
			inputSystem.PushGamepadAxisEvent( new GamepadAxisEventArgs( GamepadStick.Left, 300, 0, new Vector2( 0.25f, -0.5f ) ) );

			using ( Assert.EnterMultipleScope() ) {
				Assert.That( published.HasValue, Is.True );
				Assert.That( published!.Value.Value, Is.EqualTo( new Vector2( 0.25f, -0.5f ) ) );
			}
		}

		[Test]
		public void PushGamepadButtonEvent_DispatchesConfiguredGamepadButtonAction() {
			const string defaultsPath = "Assets/Config/Bindings/DefaultBinds.json";
			var fileSystem = new InputFileSystemFixture(
				(defaultsPath, """
				{
				  "Bindings": [
				    {
					  "Id": "Confirm",
				      "Name": "Confirm",
				      "ValueType": "Button",
				      "Scheme": "Gamepad",
				      "Bindings": { "DeviceId": "Gamepad1", "ControlId": "A" }
				    }
				  ]
				}
				""" )
			);
			var cvarSystem = InputTestHelpers.CreateCVarSystem( _eventRegistry, defaultsPath );
			ButtonActionEventArgs? published = null;
			_eventRegistry.GetEvent<ButtonActionEventArgs>( $"Confirm:{Constants.Events.BUTTON_ACTION}", Constants.Events.NAMESPACE )
				.Subscribe( ( in ButtonActionEventArgs args ) => published = args );

			using var inputSystem = new InputSystem( fileSystem.Object, cvarSystem, _logger, _eventRegistry, _registry );
			SetMode( inputSystem, InputScheme.Gamepad );
			inputSystem.PushGamepadButtonEvent( new GamepadButtonEventArgs( GamepadButton.A, 1, 400, true ) );

			using ( Assert.EnterMultipleScope() ) {
				Assert.That( published.HasValue, Is.True );
				Assert.That( published!.Value.Value, Is.True );
			}
		}

		[Test]
		public void PushKeyboardEvent_DispatchesCompositeMovementActions() {
			const string defaultsPath = "Assets/Config/Bindings/DefaultBinds.json";
			var fileSystem = new InputFileSystemFixture(
				(defaultsPath, """
				{
				  "Bindings": [
				    {
					  "Id": "Move",
				      "Name": "Move",
				      "ValueType": "Vector2",
				      "Scheme": "KeyboardAndMouse",
				      "Bindings": {
				        "Kind": "Axis2DComposite",
				        "Up": "W",
				        "Down": "S",
				        "Left": "A",
				        "Right": "D"
				      }
				    }
				  ]
				}
				""" )
			);
			var cvarSystem = InputTestHelpers.CreateCVarSystem( _eventRegistry, defaultsPath );
			AxisActionEventArgs? published = null;
			_eventRegistry.GetEvent<AxisActionEventArgs>( $"Move:{Constants.Events.AXIS_ACTION}", Constants.Events.NAMESPACE )
				.Subscribe( ( in AxisActionEventArgs args ) => published = args );

			using var inputSystem = new InputSystem( fileSystem.Object, cvarSystem, _logger, _eventRegistry, _registry );
			inputSystem.PushKeyboardEvent( new KeyboardEventArgs( KeyNum.W, 500, true ) );

			using ( Assert.EnterMultipleScope() ) {
				Assert.That( published.HasValue, Is.True );
				Assert.That( published!.Value.Value, Is.EqualTo( new Vector2( 0.0f, 1.0f ) ) );
			}
		}

		private static void SetMode( InputSystem inputSystem, InputScheme mode ) {
			InputModeField.SetValue( inputSystem, mode );
		}
	}
}
