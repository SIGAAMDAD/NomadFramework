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
using System.Runtime.CompilerServices;
using Nomad.Core.Events;
using Nomad.Core;
using Nomad.Core.Compatibility;
using System.Collections.Generic;

namespace Nomad.CVars.Private {
	/*
	===================================================================================

	CVar

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	internal sealed class CVar<T> : ICVar<T> {
		/// <summary>
		/// The current value of the CVar.
		/// </summary>
		public T Value {
			get => _value;
			set => Set( value );
		}
		private T _value;

		/// <summary>
		/// The default value of the CVar, what it resets to.
		/// </summary>
		public T DefaultValue => _defaultValue;
		private readonly T _defaultValue;

		/// <summary>
		/// The CVar's value type.
		/// </summary>
		public CVarType Type => _metadata.Type;

		/// <summary>
		/// Name of the CVar, should preferrably be prefixed with the system it's tied to, for instance: display.WindowMode
		/// </summary>
		public string Name => _metadata.Name;

		/// <summary>
		/// A description of what the CVar does or impacts. Can be empty but preferrably shouldn't be.
		/// </summary>
		public string Description => _metadata.Description;

		/// <summary>
		/// The CVar's flags (permissions, abilities, capabilities).
		/// </summary>
		public CVarFlags Flags => _metadata.Flags;

		/// <summary>
		/// Queries if the CVar is marked as "Read-Only". Meaning it cannot change from its <see cref="DefaultValue"/>.
		/// </summary>
		public bool IsReadOnly => _metadata.IsReadOnly;

		/// <summary>
		/// Queries if the CVar is marked as "User Created". Meaning it was created from a console command.
		/// </summary>
		public bool IsUserCreated => _metadata.IsUserCreated;

		/// <summary>
		/// Queries if the CVar is marked as "Archive". Meaning it will be written to the settings configuration file.
		/// </summary>
		public bool IsSaved => _metadata.IsSaved;

		/// <summary>
		/// Queuries if the CVar is allowed to be shown in the console.
		/// </summary>
		public bool IsHidden => _metadata.IsHidden;

		private readonly CVarMetadata _metadata;
		private readonly CVarValidator<T> _validator;
		private readonly CVarConverter<T> _converter;

		/// <summary>
		/// Event triggered the <see cref="Value"/> changes.
		/// </summary>
		public IGameEvent<CVarValueChangedEventArgs<T>> ValueChanged => _valueChanged;
		private readonly IGameEvent<CVarValueChangedEventArgs<T>> _valueChanged;

		/*
		===============
		CVar
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="createInfo"></param>
		internal CVar( IGameEventRegistryService eventFactory, in CVarCreateInfo<T> createInfo ) {
			_validator = new CVarValidator<T>( createInfo.Validator );
			if ( !CVarValidator<T>.ValidateCVarType() ) {
				throw new InvalidCastException();
			}
			_metadata = new CVarMetadata(
				createInfo.Name,
				createInfo.Description,
				createInfo.Flags,
				typeof( T ).GetCVarType()
			);

			unsafe {
				fixed ( T *value = &_value ) {
					_converter = new CVarConverter<T>( Type, value );
				}
			}

			_value = createInfo.DefaultValue;
			_defaultValue = createInfo.DefaultValue;
			_valueChanged = eventFactory.GetEvent<CVarValueChangedEventArgs<T>>( Constants.Events.Console.NAMESPACE, $"{createInfo.Name}:{Constants.Events.CVars.CVAR_VALUE_CHANGED_EVENT}" );
		}

		/*
		===============
		Reset
		===============
		*/
		/// <summary>
		/// Resets the cvar's <see cref="Value"/> to the <see cref="DefaultValue"/>.
		/// </summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void Reset() {
			Value = _defaultValue;
		}

		/*
		===============
		SetFromString
		===============
		*/
		/// <summary>
		/// Sets the CVar's <see cref="Value"/>.
		/// </summary>
		/// <param name="value">The value to set the CVar's <see cref="Value"/> to.</param>
		/// <exception cref="ArgumentException"></exception>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void SetFromString( string value ) {
			ExceptionCompat.ThrowIfNull( value );
			if ( !CVarConverter<T>.TryConvertStringToType( value, typeof( T ), out object convertedValue ) ) {
				throw new ArgumentException( $"Failed to convert cvar value '{value}' to type {typeof( T ).Name}" );
			}
			Value = (T)convertedValue;
		}

		/*
		===============
		Set
		===============
		*/
		/// <summary>
		/// Sets the CVar's <see cref="Value"/> to the given value.
		/// </summary>
		/// <param name="value">The new value of the CVar.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void Set( T value ) {
			if ( IsReadOnly || !_validator.ValidateValue( value ) || EqualityComparer<T>.Default.Equals( _value, value ) ) {
				return;
			}
			T old = _value;
			_value = value;
			_valueChanged.Publish( new CVarValueChangedEventArgs<T>( old, value ) );
		}

		public float GetDecimalValue() => _converter.GetDecimalValue();
		public int GetIntegerValue() => _converter.GetIntegerValue();
		public uint GetUIntegerValue() => _converter.GetUIntegerValue();
		public string? GetStringValue() => _converter.GetStringValue();
		public bool GetBooleanValue() => _converter.GetBooleanValue();
		public T1 GetValue<T1>() => _converter.GetValue<T1>();

		public void SetDecimalValue( float value ) => _converter.SetDecimalValue( value );
		public void SetIntegerValue( int value ) => _converter.SetIntegerValue( value );
		public void SetUIntegerValue( uint value ) => _converter.SetUIntegerValue( value );
		public void SetBooleanValue( bool value ) => _converter.SetBooleanValue( value );
		public void SetStringValue( string value ) => _converter.SetStringValue( value );

		/*
		===============
		operator T
		===============
		*/
		/// <summary>
		/// An implicit conversion operator to smooth over abstraction when interacting with the CVar API.
		/// </summary>
		/// <param name="cvar"></param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static implicit operator T( CVar<T> cvar )
			=> cvar._value;
	};
};
