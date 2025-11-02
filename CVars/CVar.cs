/*
===========================================================================
The Nomad AGPL Source Code
Copyright (C) 2025 Noah Van Til

The Nomad Source Code is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published
by the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

The Nomad Source Code is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with The Nomad Source Code.  If not, see <http://www.gnu.org/licenses/>.

If you have questions concerning this license or the applicable additional
terms, you may contact me via email at nyvantil@gmail.com.
===========================================================================
*/

using EventSystem;
using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace CVars {
	/*
	===================================================================================

	CVar

	===================================================================================
	*/
	/// <summary>
	/// Literally just a Quake 3/UE5 style CVar but without the copyright issues.
	/// A configurable variable that can be set from the console and settings.ini
	/// </summary>

	public class CVar<T> : ICVar {
		public readonly struct CVarValueChangedEventData : IEventArgs {
			public readonly CVar<T> CVar;
			public readonly T Value;

			/*
			===============
			CVarValueChangedEventData
			===============
			*/
			public CVarValueChangedEventData( CVar<T> cvar, T value ) {
				CVar = cvar;
				Value = value;
			}
		};

		object ICVar.Value => Value ?? throw new InvalidOperationException( "CVar Value is null" );
		object ICVar.DefaultValue => Description ?? "";
		CVarType ICVar.Type => DetermineType( Value );

		/// <summary>
		/// Name of the CVar, should preferrably be prefixed with the system it's tied to, for instance: display.WindowMode
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// A description of what the CVar does or impacts. Can be empty but preferrably isn't
		/// </summary>
		public string Description { get; }

		/// <summary>
		/// The CVar's flags (permissions, abilities, capabilities)
		/// </summary>
		public CVarFlags Flags { get; }

		/// <summary>
		/// The default value of the CVar, what it resets to
		/// </summary>
		public T DefaultValue { get; }

		/// <summary>
		/// The current value of the CVar.
		/// </summary>
		public T Value {
			get => _value;
			set {
				if ( IsReadOnly ) {
					return;
				}
				if ( !ValidateValue( value ) ) {
					return;
				}

				if ( EqualityComparer<T>.Default.Equals( _value, value ) ) {
					return;
				}
				_value = value;
				ValueChanged.Publish( new CVarValueChangedEventData( this, _value ) );
			}
		}
		private T _value;

		/// <summary>
		/// Queries if the CVar is marked as "Read-Only". Meaning it cannot change from its <see cref="DefaultValue"/>.
		/// </summary>
		public bool IsReadOnly => ( Flags & CVarFlags.ReadOnly ) != 0;

		/// <summary>
		/// Queries if the CVar is marked as "User Created". Meaning it was created from a console command.
		/// </summary>
		public bool IsUserCreated => ( Flags & CVarFlags.UserCreated ) != 0;

		/// <summary>
		/// Queries if the CVar is marked as "Archive". Meaning it will be written to the settings configuration file.
		/// </summary>
		public bool IsSaved => ( Flags & CVarFlags.Archive ) != 0;

		/// <summary>
		/// 
		/// </summary>
		public bool IsHidden => ( Flags & CVarFlags.Hidden ) != 0;

		/// <summary>
		/// Validator function for custom validation
		/// </summary>
		private readonly Func<T, bool>? Validator;

		/// <summary>
		/// Event triggered the <see cref="Value"/> changes
		/// </summary>
		public readonly IGameEvent ValueChanged;

		/*
		===============
		CVar
		===============
		*/
		/// <summary>
		/// Constructs a cvar
		/// </summary>
		/// <param name="name">Name of the cvar</param>
		/// <param name="defaultValue">Initial value of the cvar</param>
		/// <param name="description">Description of the cvar, preferrably there, but optional</param>
		/// <param name="flags"></param>
		/// <param name="validator"></param>
		/// <exception cref="ArgumentException">Thrown if <paramref name="name"/> is invalid as a CVar name</exception>
		/// <exception cref="InvalidOperationException">Thrown if <paramref name="defaultValue"/> is an invalid CVar value type</exception>
		public CVar( string? name, T defaultValue, string? description = "", CVarFlags flags = CVarFlags.None, Func<T, bool>? validator = null ) {
			if ( !( typeof( T ) == typeof( int ) || typeof( T ) == typeof( bool ) || typeof( T ).IsEnum
				|| typeof( T ) == typeof( string ) || typeof( T ) == typeof( float ) ) )
			{
				throw new InvalidCastException( nameof( T ) );
			}

			// sanity
			ArgumentException.ThrowIfNullOrEmpty( name );

			if ( !IsValidName( name ) ) {
				throw new ArgumentException( $"CVar name {name} contains invalid characters" );
			}

			Name = name;
			Flags = flags;
			DefaultValue = defaultValue;

			_value = defaultValue;

			Validator = validator;
			Description = description ?? "";
		}

		/*
		===============
		~CVar
		===============
		*/
		/// <summary>
		/// Removes the CVar from the <see cref="CVarSystem"/> cache.
		/// </summary>
		~CVar() {
			CVarSystem.RemoveCVar( this );
		}

		/*
		===============
		Register
		===============
		*/
		public void Register() {
			CVarSystem.Register( this );
		}

		/*
		===============
		Reset
		===============
		*/
		/// <summary>
		/// Resets the cvar's <see cref="Value"/> to the <see cref="DefaultValue"/>
		/// </summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void Reset() {
			Value = DefaultValue;
		}

		/*
		===============
		SetFromStrings
		===============
		*/
		/// <summary>
		/// Sets the CVar's <see cref="Value"/>.
		/// </summary>
		/// <param name="value">The value to set the CVar's <see cref="Value"/> to.</param>
		/// <exception cref="ArgumentException"></exception>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void SetFromString( string? value ) {
			ArgumentNullException.ThrowIfNull( value );
			try {
				object? convertedValue = ConvertStringToType( value, typeof( T ) );
				Value = (T)convertedValue;
			} catch ( Exception ) {
				throw new ArgumentException( $"Failed to convert cvar value '{value}' to type {typeof( T ).Name}" );
			}
		}

		/*
		===============
		Set
		===============
		*/
		/// <summary>
		/// Sets the CVar's <see cref="Value"/> to a 32-bit floating-point value.
		/// </summary>
		/// <param name="value">The value to set the CVar's <see cref="Value"/> to.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void Set( float value ) {
			SetFromString( value.ToString() );
		}

		/*
		===============
		Set
		===============
		*/
		/// <summary>
		/// Sets the CVar's <see cref="Value"/> to a 32-bit integer value.
		/// </summary>
		/// <param name="value">The value to set the CVar's <see cref="Value"/> to.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void Set( int value ) {
			SetFromString( value.ToString() );
		}

		/*
		===============
		Set
		===============
		*/
		/// <summary>
		/// Sets the CVar's <see cref="Value"/> to an 8-bit boolean value.
		/// </summary>
		/// <param name="value">The value to set the CVar's <see cref="Value"/> to.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void Set( bool value ) {
			SetFromString( value.ToString() );
		}

		/*
		===============
		GetStringValue
		===============
		*/
		/// <summary>
		/// Retrieves the CVar's <see cref="Value"/> as a string.
		/// </summary>
		/// <returns>The <see cref="Value"/> in string format.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public string GetStringValue() {
			return Value?.ToString() ?? "";
		}

		/*
		===============
		GetIntegerValue
		===============
		*/
		/// <summary>
		/// Retrieves the CVar's <see cref="Value"/> as a 32-bit integer.
		/// </summary>
		/// <returns>The <see cref="Value"/> in 32-bit integer format, 0 by default if int.TryParse failed</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public int GetIntegerValue() {
			ArgumentNullException.ThrowIfNull( Value );
			return int.TryParse( Value.ToString(), out int result ) ? result : 0;
		}

		/*
		===============
		GetFloatValue
		===============
		*/
		/// <summary>
		/// Retrieves the CVar's <see cref="Value"/> as a 32-bit floating-point.
		/// </summary>
		/// <returns>The <see cref="Value"/> in 32-bit floating-point format, 0 by default if float.TryParse failed</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public float GetFloatValue() {
			ArgumentNullException.ThrowIfNull( Value );
			return float.TryParse( Value.ToString(), out float result ) ? result : 0.0f;
		}

		/*
		===============
		GetBooleanValue
		===============
		*/
		/// <summary>
		/// Retrieves the CVar's <see cref="Value"/> as an 8-bit boolean.
		/// </summary>
		/// <returns>The <see cref="Value"/> in 8-bit boolean format, false by default if bool.TryParse failed</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public bool GetBooleanValue() {
			ArgumentNullException.ThrowIfNull( Value );
			return bool.TryParse( Value.ToString(), out bool result ) ? result : false;
		}

		/*
		===============
		ValidateValue
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		private bool ValidateValue( T value ) {
			return Validator == null || Validator.Invoke( value );
		}

		/*
		===============
		ConvertStringToType
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <param name="targetType"></param>
		/// <returns></returns>
		/// <exception cref="NotSupportedException">Thrown if the provided <paramref name="targetType"/> isn't a valid CVar <see cref="Value"/> type.</exception>
		private static object ConvertStringToType( string? value, Type targetType ) {
			ArgumentNullException.ThrowIfNull( value );

			// FIXME: change these to TryParse

			if ( targetType.IsEnum ) {
				return Enum.Parse( targetType, value );
			}
			if ( targetType == typeof( int ) ) {
				return int.Parse( value );
			}
			if ( targetType == typeof( float ) ) {
				return float.Parse( value );
			}
			if ( targetType == typeof( bool ) ) {
				return bool.Parse( value );
			}
			if ( targetType == typeof( string ) ) {
				return value;
			}
			if ( targetType == typeof( StringName ) ) {
				return new StringName( value );
			}
			if ( targetType == typeof( NodePath ) ) {
				return new NodePath( value );
			}
			throw new NotSupportedException( $"CVars do not support type {targetType.Name}" );
		}

		/*
		===============
		IsValidNameCharacter
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="c"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private static bool IsValidNameCharacter( char c ) {
			return char.IsLetterOrDigit( c ) || c == '.' || c == '_';
		}

		/*
		===============
		IsValidName
		===============
		*/
		/// <summary>
		/// Ensures the provided <paramref name="name"/> string doesn't contain any invalid non alphanumeric characters
		/// </summary>
		/// <param name="name">The name to check</param>
		/// <returns>Returns true if the name is valid</returns>
		private static bool IsValidName( string? name ) {
			if ( string.IsNullOrEmpty( name ) ) {
				return false;
			}

			for ( int i = 0; i < name.Length; i++ ) {
				if ( !IsValidNameCharacter( name[ i ] ) ) {
					return false;
				}
			}
			return true;
		}

		/*
		===============
		DetermineType
		===============
		*/
		/// <summary>
		/// Determines the type of <see cref="Value"/>.
		/// </summary>
		/// <returns>Returns the CVar's value type.</returns>
		/// <exception cref="InvalidOperationException">Thrown if <see cref="Value"/> isn't a valid cvar value types</exception>
		private static CVarType DetermineType<B>( B? value ) {
			ArgumentNullException.ThrowIfNull( value );

			Type type = value.GetType();
			if ( type == typeof( int ) || type == typeof( long ) || type == typeof( short ) || type.IsEnum ) {
				return CVarType.Int;
			} else if ( type == typeof( float ) ) {
				return CVarType.Float;
			} else if ( type == typeof( string ) || type == typeof( StringName ) ) {
				return CVarType.String;
			} else if ( type == typeof( NodePath ) ) {
				return CVarType.NodePath;
			} else if ( type == typeof( bool ) ) {
				return CVarType.Boolean;
			}
			return CVarType.Count;
		}
	};
};