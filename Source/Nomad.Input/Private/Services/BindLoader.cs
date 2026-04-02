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

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json;
using Nomad.Core.FileSystem;
using Nomad.Core.FileSystem.Configs;
using Nomad.Core.Input;
using Nomad.Core.Input.ValueObjects;
using Nomad.Input.Private.Extensions;
using Nomad.Input.Private.ValueObjects;
using Nomad.Input.ValueObjects;
using Nomad.Core.Logger;

namespace Nomad.Input.Private.Services {
	/*
	===================================================================================
	
	BindLoader
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>

	internal sealed class BindLoader {
		private readonly IFileSystem _fileSystem;
		private readonly ILoggerService _logger;

		/*
		===============
		BindLoader
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="fileSystem"></param>
		public BindLoader( IFileSystem fileSystem, ILoggerService logger ) {
			_fileSystem = fileSystem;
			_logger = logger;
		}

		/*
		===============
		LoadBindDatabase
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="filePath"></param>
		/// <param name="binds"></param>
		/// <returns></returns>
		public bool LoadBindDatabase( string filePath, out ImmutableArray<InputActionDefinition> binds ) {
			using var fileBuffer = _fileSystem.LoadFile( filePath );
			if ( fileBuffer == null ) {
				binds = ImmutableArray<InputActionDefinition>.Empty;
				return false;
			}
			using var fileStream = fileBuffer.AsStream();

			using var document = JsonDocument.Parse(
				fileStream,
				new JsonDocumentOptions {
					CommentHandling = JsonCommentHandling.Skip,
					AllowTrailingCommas = true
				}
			);

			if ( !TryGetProperty( document.RootElement, "Bindings", out var bindingsElement ) ) {
				throw new Exception( $"Bindings file '{filePath}' is missing a 'Bindings' property." );
			}

			var actionIndices = new Dictionary<string, int>( StringComparer.Ordinal );
			var actions = new List<ActionBuilder>();

			switch ( bindingsElement.ValueKind ) {
				case JsonValueKind.Array:
					foreach ( var actionElement in bindingsElement.EnumerateArray() ) {
						ParseActionDefinition( actionElement, actionIndices, actions );
					}
					break;
				case JsonValueKind.Object:
					foreach ( var actionProperty in bindingsElement.EnumerateObject() ) {
						ParseActionDefinition( actionProperty.Value, actionIndices, actions, actionProperty.Name );
					}
					break;
				default:
					throw new Exception( $"Bindings file '{filePath}' has an invalid 'Bindings' payload. Expected an array or object." );
			}

			var builder = ImmutableArray.CreateBuilder<InputActionDefinition>( actions.Count );
			for ( int i = 0; i < actions.Count; i++ ) {
				builder.Add( actions[i].Build() );
			}
			binds = builder.ToImmutable();
			return true;
		}

		/*
		===============
		ParseActionDefinition
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="actionElement"></param>
		/// <param name="actionIndices"></param>
		/// <param name="actions"></param>
		/// <param name="fallbackName"></param>
		/// <exception cref="Exception"></exception>
		private static void ParseActionDefinition( JsonElement actionElement, Dictionary<string, int> actionIndices, List<ActionBuilder> actions, string? fallbackName = null ) {
			string name = GetRequiredString( actionElement, "Name", fallbackName );
			InputValueType valueType = ParseEnum<InputValueType>( GetRequiredString( actionElement, "ValueType" ), "ValueType" );
			InputScheme scheme = ParseEnum<InputScheme>( GetRequiredString( actionElement, "Scheme" ), "Scheme" );

			if ( !TryGetProperty( actionElement, "Bindings", out var bindingsElement ) && !TryGetProperty( actionElement, "Binding", out bindingsElement ) ) {
				throw new Exception( $"Binding action '{name}' is missing a binding payload." );
			}

			if ( !actionIndices.TryGetValue( name, out int actionIndex ) ) {
				actionIndex = actions.Count;
				actionIndices.Add( name, actionIndex );
				actions.Add( new ActionBuilder( name, valueType ) );
			} else if ( actions[actionIndex].ValueType != valueType ) {
				throw new Exception( $"Binding action '{name}' has conflicting value types." );
			}

			if ( bindingsElement.ValueKind == JsonValueKind.Array ) {
				foreach ( var bindingElement in bindingsElement.EnumerateArray() ) {
					actions[actionIndex].Bindings.Add( ParseBindingDefinition( bindingElement, scheme ) );
				}
				return;
			}

			if ( bindingsElement.ValueKind != JsonValueKind.Object ) {
				throw new Exception( $"Binding action '{name}' has an invalid binding payload." );
			}

			actions[actionIndex].Bindings.Add( ParseBindingDefinition( bindingsElement, scheme ) );
		}

		/*
		===============
		ParseBindingDefinition
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="bindingElement"></param>
		/// <param name="scheme"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		private static InputBindingDefinition ParseBindingDefinition( JsonElement bindingElement, InputScheme scheme ) {
			InputBindingKind kind = TryGetProperty( bindingElement, "Kind", out var kindElement )
				? ParseEnum<InputBindingKind>( GetRequiredString( kindElement, "Kind value" ), "Kind" )
				: InferBindingKind( bindingElement );

			var definition = new InputBindingDefinition {
				Scheme = scheme,
				Kind = kind
			};

			switch ( kind ) {
				case InputBindingKind.Button:
					definition.Button = ParseButtonBinding( bindingElement );
					break;
				case InputBindingKind.Axis1D:
					definition.Axis1D = ParseAxis1DBinding( bindingElement );
					break;
				case InputBindingKind.Axis1DComposite:
					definition.Axis1DComposite = ParseAxis1DCompositeBinding( bindingElement );
					break;
				case InputBindingKind.Axis2D:
					definition.Axis2D = ParseAxis2DBinding( bindingElement );
					break;
				case InputBindingKind.Axis2DComposite:
					definition.Axis2DComposite = ParseAxis2DCompositeBinding( bindingElement );
					break;
				case InputBindingKind.Delta2D:
					definition.Delta2D = ParseDelta2DBinding( bindingElement );
					break;
				default:
					throw new Exception( $"Unsupported binding kind '{kind}'." );
			}

			return definition;
		}

		/*
		===============
		InferBindingKind
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="bindingElement"></param>
		/// <returns></returns>
		private static InputBindingKind InferBindingKind( JsonElement bindingElement ) {
			string deviceId = GetRequiredString( bindingElement, "DeviceId" );

			if ( deviceId.Equals( Constants.MOUSE_MOTION_DEVICE_ID, StringComparison.OrdinalIgnoreCase ) ) {
				return InputBindingKind.Delta2D;
			}

			return InputBindingKind.Button;
		}

		/*
		===============
		ParseButtonBinding
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="bindingElement"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		private static ButtonBinding ParseButtonBinding( JsonElement bindingElement ) {
			InputDeviceSlot deviceId = ParseDeviceSlot( GetRequiredString( bindingElement, "DeviceId" ) );
			string controlName = GetRequiredString( bindingElement, "ControlId", GetOptionalString( bindingElement, "ButtonId" ) );

			var modifiers = ImmutableArray.CreateBuilder<InputControlId>();
			if ( TryGetProperty( bindingElement, "Modifiers", out var modifiersElement ) ) {
				if ( modifiersElement.ValueKind != JsonValueKind.Array ) {
					throw new Exception( "Binding modifiers must be an array." );
				}

				foreach ( var modifierElement in modifiersElement.EnumerateArray() ) {
					if ( modifierElement.ValueKind != JsonValueKind.String ) {
						throw new Exception( "Binding modifiers must be strings." );
					}
					modifiers.Add( ParseKeyboardControl( modifierElement.GetString()! ) );
				}
			}

			return new ButtonBinding {
				DeviceId = deviceId,
				ControlId = ParseButtonControl( deviceId, controlName ),
				Modifiers = modifiers.ToImmutable()
			};
		}

		/*
		===============
		ParseAxis1DBinding
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="bindingElement"></param>
		/// <returns></returns>
		private static Axis1DBinding ParseAxis1DBinding( JsonElement bindingElement ) {
			InputDeviceSlot deviceId = ParseDeviceSlot( GetRequiredString( bindingElement, "DeviceId" ) );
			return new Axis1DBinding {
				DeviceId = deviceId,
				ControlId = ParseAnalogControl( deviceId, GetRequiredString( bindingElement, "ControlId" ) ),
				Deadzone = GetOptionalSingle( bindingElement, "Deadzone", 0.0f ),
				Sensitivity = GetOptionalSingle( bindingElement, "Sensitivity", 1.0f ),
				Scale = GetOptionalSingle( bindingElement, "Scale", 1.0f ),
				Invert = GetOptionalBoolean( bindingElement, "Invert", false ),
				ResponseCurve = GetOptionalEnum( bindingElement, "ResponseCurve", ResponseCurve.Linear )
			};
		}
		
		/*
		===============
		ParseAxis1DCompositeBinding
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="bindingElement"></param>
		/// <returns></returns>
		private static Axis1DCompositeBinding ParseAxis1DCompositeBinding( JsonElement bindingElement ) {
			return new Axis1DCompositeBinding {
				Negative = ParseKeyboardControl( GetRequiredString( bindingElement, "Negative" ) ),
				Positive = ParseKeyboardControl( GetRequiredString( bindingElement, "Positive" ) ),
				Scale = GetOptionalSingle( bindingElement, "Scale", 1.0f ),
				Normalize = GetOptionalBoolean( bindingElement, "Normalize", true )
			};
		}

		/*
		===============
		ParseAxis2DBinding
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="bindingElement"></param>
		/// <returns></returns>
		private static Axis2DBinding ParseAxis2DBinding( JsonElement bindingElement ) {
			InputDeviceSlot deviceId = ParseDeviceSlot( GetRequiredString( bindingElement, "DeviceId" ) );
			return new Axis2DBinding {
				DeviceId = deviceId,
				ControlId = ParseAnalogControl( deviceId, GetRequiredString( bindingElement, "ControlId" ) ),
				Deadzone = GetOptionalSingle( bindingElement, "Deadzone", 0.0f ),
				Sensitivity = GetOptionalSingle( bindingElement, "Sensitivity", 1.0f ),
				ScaleX = GetOptionalSingle( bindingElement, "ScaleX", 1.0f ),
				ScaleY = GetOptionalSingle( bindingElement, "ScaleY", 1.0f ),
				InvertX = GetOptionalBoolean( bindingElement, "InvertX", false ),
				InvertY = GetOptionalBoolean( bindingElement, "InvertY", false ),
				ResponseCurve = GetOptionalEnum( bindingElement, "ResponseCurve", ResponseCurve.Linear )
			};
		}

		/*
		===============
		ParseAxis2DCompositeBinding
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="bindingElement"></param>
		/// <returns></returns>
		private static Axis2DCompositeBinding ParseAxis2DCompositeBinding( JsonElement bindingElement ) {
			return new Axis2DCompositeBinding {
				Up = ParseKeyboardControl( GetRequiredString( bindingElement, "Up" ) ),
				Down = ParseKeyboardControl( GetRequiredString( bindingElement, "Down" ) ),
				Left = ParseKeyboardControl( GetRequiredString( bindingElement, "Left" ) ),
				Right = ParseKeyboardControl( GetRequiredString( bindingElement, "Right" ) ),
				Normalize = GetOptionalBoolean( bindingElement, "Normalize", true ),
				ScaleX = GetOptionalSingle( bindingElement, "ScaleX", 1.0f ),
				ScaleY = GetOptionalSingle( bindingElement, "ScaleY", 1.0f )
			};
		}

		/*
		===============
		ParseDelta2DBinding
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="bindingElement"></param>
		/// <returns></returns>
		private static Delta2DBinding ParseDelta2DBinding( JsonElement bindingElement ) {
			InputDeviceSlot deviceId = ParseDeviceSlot( GetRequiredString( bindingElement, "DeviceId" ) );
			return new Delta2DBinding {
				DeviceId = deviceId,
				ControlId = ParseAnalogControl( deviceId, GetRequiredString( bindingElement, "ControlId", InputControlId.Delta.ToString() ) ),
				Sensitivity = GetOptionalSingle( bindingElement, "Sensitivity", 1.0f ),
				ScaleX = GetOptionalSingle( bindingElement, "ScaleX", 1.0f ),
				ScaleY = GetOptionalSingle( bindingElement, "ScaleY", 1.0f ),
				InvertX = GetOptionalBoolean( bindingElement, "InvertX", false ),
				InvertY = GetOptionalBoolean( bindingElement, "InvertY", false )
			};
		}

		private static InputDeviceSlot ParseDeviceSlot( string deviceId ) {
			if ( deviceId.Equals( Constants.KEYBOARD_DEVICE_ID, StringComparison.OrdinalIgnoreCase ) ) {
				return InputDeviceSlot.Keyboard;
			}

			if ( deviceId.Equals( Constants.MOUSE_BUTTON_DEVICE_ID, StringComparison.OrdinalIgnoreCase )
				|| deviceId.Equals( Constants.MOUSE_MOTION_DEVICE_ID, StringComparison.OrdinalIgnoreCase )
				|| deviceId.Equals( "Mouse", StringComparison.OrdinalIgnoreCase ) )
			{
				return InputDeviceSlot.Mouse;
			}

			if ( deviceId.Equals( Constants.GAMEPAD_DEVICE_ID, StringComparison.OrdinalIgnoreCase )
				|| deviceId.Equals( nameof( InputDeviceSlot.Gamepad0 ), StringComparison.OrdinalIgnoreCase ) )
			{
				return InputDeviceSlot.Gamepad0;
			}

			if ( deviceId.Equals( nameof( InputDeviceSlot.Gamepad1 ), StringComparison.OrdinalIgnoreCase ) ) {
				return InputDeviceSlot.Gamepad1;
			}

			if ( deviceId.Equals( nameof( InputDeviceSlot.Gamepad2 ), StringComparison.OrdinalIgnoreCase ) ) {
				return InputDeviceSlot.Gamepad2;
			}

			if ( deviceId.Equals( nameof( InputDeviceSlot.Gamepad3 ), StringComparison.OrdinalIgnoreCase ) ) {
				return InputDeviceSlot.Gamepad3;
			}

			throw new Exception( $"Invalid DeviceId '{deviceId}' in bindings file." );
		}

		private static InputControlId ParseButtonControl( InputDeviceSlot deviceId, string controlName ) {
			return deviceId switch {
				InputDeviceSlot.Keyboard => ParseKeyboardControl( controlName ),
				InputDeviceSlot.Mouse => ParseMouseButtonControl( controlName ),
				InputDeviceSlot.Gamepad0 or InputDeviceSlot.Gamepad1 or InputDeviceSlot.Gamepad2 or InputDeviceSlot.Gamepad3 => ParseGamepadButtonControl( controlName ),
				_ => throw new Exception( $"Unsupported button device '{deviceId}'." )
			};
		}

		private static InputControlId ParseAnalogControl( InputDeviceSlot deviceId, string controlName ) {
			return deviceId switch {
				InputDeviceSlot.Keyboard => ParseKeyboardControl( controlName ),
				InputDeviceSlot.Mouse => ParseInputControlId( controlName ),
				InputDeviceSlot.Gamepad0 or InputDeviceSlot.Gamepad1 or InputDeviceSlot.Gamepad2 or InputDeviceSlot.Gamepad3 => ParseInputControlId( controlName ),
				_ => throw new Exception( $"Unsupported analog device '{deviceId}'." )
			};
		}

		private static InputControlId ParseKeyboardControl( string controlName ) {
			if ( Enum.TryParse<KeyNum>( controlName, true, out var keyNum ) ) {
				return keyNum.ToControlId();
			}
			if ( Enum.TryParse<InputControlId>( controlName, true, out var controlId ) ) {
				return controlId;
			}
			throw new Exception( $"Invalid keyboard control '{controlName}' in bindings file." );
		}

		private static InputControlId ParseMouseButtonControl( string controlName ) {
			if ( Enum.TryParse<MouseButton>( controlName, true, out var mouseButton ) ) {
				return mouseButton.ToControlId();
			}
			if ( Enum.TryParse<InputControlId>( controlName, true, out var controlId ) ) {
				return controlId;
			}
			throw new Exception( $"Invalid mouse control '{controlName}' in bindings file." );
		}

		private static InputControlId ParseGamepadButtonControl( string controlName ) {
			if ( Enum.TryParse<GamepadButton>( controlName, true, out var gamepadButton ) ) {
				return gamepadButton.ToControlId();
			}
			if ( Enum.TryParse<InputControlId>( controlName, true, out var controlId ) ) {
				return controlId;
			}
			throw new Exception( $"Invalid gamepad control '{controlName}' in bindings file." );
		}

		private static InputControlId ParseInputControlId( string controlName ) {
			if ( Enum.TryParse<InputControlId>( controlName, true, out var controlId ) ) {
				return controlId;
			}
			throw new Exception( $"Invalid control '{controlName}' in bindings file." );
		}

		private static string GetRequiredString( JsonElement element, string propertyName, string? fallback = null ) {
			if ( element.ValueKind == JsonValueKind.String ) {
				return element.GetString() ?? throw new Exception( $"Encountered null string while reading '{propertyName}'." );
			}
			if ( TryGetProperty( element, propertyName, out var property ) && property.ValueKind == JsonValueKind.String ) {
				return property.GetString() ?? throw new Exception( $"Encountered null string while reading '{propertyName}'." );
			}
			if ( fallback != null ) {
				return fallback;
			}
			throw new Exception( $"Bindings file is missing string property '{propertyName}'." );
		}

		private static string? GetOptionalString( JsonElement element, string propertyName ) {
			if ( TryGetProperty( element, propertyName, out var property ) && property.ValueKind == JsonValueKind.String ) {
				return property.GetString();
			}
			return null;
		}

		private static float GetOptionalSingle( JsonElement element, string propertyName, float defaultValue ) {
			if ( !TryGetProperty( element, propertyName, out var property ) ) {
				return defaultValue;
			}
			if ( property.ValueKind == JsonValueKind.Number && property.TryGetSingle( out float value ) ) {
				return value;
			}
			throw new Exception( $"Binding property '{propertyName}' must be a number." );
		}

		private static bool GetOptionalBoolean( JsonElement element, string propertyName, bool defaultValue ) {
			if ( !TryGetProperty( element, propertyName, out var property ) ) {
				return defaultValue;
			}
			if ( property.ValueKind is JsonValueKind.True or JsonValueKind.False ) {
				return property.GetBoolean();
			}
			throw new Exception( $"Binding property '{propertyName}' must be a boolean." );
		}

		private static TEnum GetOptionalEnum<TEnum>( JsonElement element, string propertyName, TEnum defaultValue )
			where TEnum : struct, Enum
		{
			if ( !TryGetProperty( element, propertyName, out var property ) ) {
				return defaultValue;
			}
			if ( property.ValueKind != JsonValueKind.String ) {
				throw new Exception( $"Binding property '{propertyName}' must be a string." );
			}
			return ParseEnum<TEnum>( property.GetString()!, propertyName );
		}

		private static TEnum ParseEnum<TEnum>( string value, string propertyName )
			where TEnum : struct, Enum
		{
			if ( Enum.TryParse<TEnum>( value, true, out var result ) ) {
				return result;
			}
			throw new Exception( $"Invalid {propertyName} '{value}' in bindings file." );
		}

		private static bool TryGetProperty( JsonElement element, string propertyName, out JsonElement propertyValue ) {
			if ( element.ValueKind == JsonValueKind.Object ) {
				foreach ( var property in element.EnumerateObject() ) {
					if ( property.Name.Equals( propertyName, StringComparison.OrdinalIgnoreCase ) ) {
						propertyValue = property.Value;
						return true;
					}
				}
			}
			propertyValue = default;
			return false;
		}

		private sealed class ActionBuilder {
			public string Name { get; }
			public InputValueType ValueType { get; }
			public List<InputBindingDefinition> Bindings { get; } = new();

			public ActionBuilder( string name, InputValueType valueType ) {
				Name = name;
				ValueType = valueType;
			}

			public InputActionDefinition Build() {
				return new InputActionDefinition( Name, ValueType, Bindings.ToImmutableArray() );
			}
		}
	};
};
