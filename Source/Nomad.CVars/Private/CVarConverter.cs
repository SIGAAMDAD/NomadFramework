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
using Nomad.Core.Util;

namespace Nomad.CVars.Private {
	/*
	===================================================================================

	CVarConverter

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>
	/// <typeparam name="T"></typeparam>

	internal unsafe struct CVarConverter<T> {
		private readonly CVarType _type;
		private T _value;

		/*
		===============
		CVarConverter
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="pType"></param>
		/// <param name="pdefaultValuee"></param>
		public CVarConverter( CVarType type, T defaultValue ) {
			_type = type;
			_value = defaultValue;
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
		public readonly string? GetStringValue()
			=> _value is string strValue ? strValue : throw new InvalidCastException( $"CVar type mismatch: expected string but instead got '{_type}'" );

		/*
		===============
		GetUIntegerValue
		===============
		*/
		/// <summary>
		/// Retrieves the CVar's <see cref="Value"/> as a 32-bit integer.
		/// </summary>
		/// <returns>The <see cref="Value"/> in 32-bit integer format, 0 by default if int.TryParse failed</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public readonly uint GetUIntegerValue()
			=> _value is uint uintValue ? uintValue : throw new InvalidCastException( $"CVar type mismatch: expected unsigned integer but instead got '{_type}'" );

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
		public readonly int GetIntegerValue()
			=> _value is int intValue ? intValue : throw new InvalidCastException( $"CVar type mismatch: expected integer but instead got '{_type}'" );

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
		public readonly float GetDecimalValue()
			=> _value is float floatValue ? floatValue : throw new InvalidCastException( $"CVar type mismatch: expected float but instead got '{_type}'" );

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
		public readonly bool GetBooleanValue()
			=> _value is bool booleanValue ? booleanValue : throw new InvalidCastException( $"CVar type mismatch: expected boolean but instead got '{_type}'" );

		/*
		===============
		GetValue
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <returns></returns>
		/// <exception cref="NotImplementedException"></exception>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public readonly T1? GetValue<T1>()
			=> _value is T1 value ? value : throw new InvalidCastException();

		/*
		===============
		SetIntegerValue
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="value"></param>
		/// <exception cref="InvalidCastException"></exception>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void SetIntegerValue( int value )
			=> _value = _type == CVarType.Int ? Unsafe.As<int, T>(ref Unsafe.AsRef(value)) : throw new InvalidCastException( $"CVar type mismatch: expected integer but instead got '{_type}'" );

		/*
		===============
		SetUIntegerValue
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="value"></param>
		/// <exception cref="InvalidCastException"></exception>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void SetUIntegerValue( uint value )
			=> _value = _type == CVarType.UInt ? Unsafe.As<uint, T>(ref Unsafe.AsRef(value)) : throw new InvalidCastException( $"CVar type mismatch: expected unsigned integer but instead got '{_type}'" );

		/*
		===============
		SetBooleanValue
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="value"></param>
		/// <exception cref="InvalidCastException"></exception>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void SetBooleanValue( bool value )
			=> _value = _type == CVarType.Boolean ? Unsafe.As<bool, T>(ref Unsafe.AsRef(value)) : throw new InvalidCastException( $"CVar type mismatch: expected boolean but instead got '{_type}'" );

		/*
		===============
		SetDecimalValue
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="value"></param>
		/// <exception cref="InvalidCastException"></exception>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void SetDecimalValue( float value )
			=> _value = _type == CVarType.Decimal ? Unsafe.As<float, T>(ref Unsafe.AsRef(value)) : throw new InvalidCastException( $"CVar type mismatch: expected float but instead got '{_type}'" );

		/*
		===============
		SetStringValue
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="value"></param>
		/// <exception cref="InvalidCastException"></exception>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void SetStringValue( string value )
			=> _value = _type == CVarType.String ? Unsafe.As<string, T>(ref Unsafe.AsRef(value)) : throw new InvalidCastException( $"CVar type mismatch: expected string but instead got '{_type}'" );
		
		/*
		===============
		SetValue
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <param name="value"></param>
		/// <exception cref="InvalidCastException"></exception>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void SetValue<T1>( T1 value )
			=> _value = value is T thisValue ? thisValue : throw new InvalidCastException();

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
		public static bool TryConvertStringToType( string value, Type targetType, out object result ) {
			try {
				bool output;
				if ( targetType == typeof( sbyte ) || targetType.IsEnum ) {
					output = sbyte.TryParse( value, out sbyte data );
					result = data;
				} else if ( targetType == typeof( short ) || targetType.IsEnum ) {
					output = short.TryParse( value, out short data );
					result = data;
				} else if ( targetType == typeof( int ) || targetType.IsEnum ) {
					output = int.TryParse( value, out int data );
					result = data;
				} else if ( targetType == typeof( long ) || targetType.IsEnum ) {
					output = long.TryParse( value, out long data );
					result = data;
				} else if ( targetType == typeof( byte ) || targetType.IsEnum ) {
					output = byte.TryParse( value, out byte data );
					result = data;
				} else if ( targetType == typeof( ushort ) || targetType.IsEnum ) {
					output = ushort.TryParse( value, out ushort data );
					result = data;
				} else if ( targetType == typeof( uint ) || targetType.IsEnum ) {
					output = uint.TryParse( value, out uint data );
					result = data;
				} else if ( targetType == typeof( ulong ) || targetType.IsEnum ) {
					output = ulong.TryParse( value, out ulong data );
					result = data;
				} else if ( targetType == typeof( float ) ) {
					output = float.TryParse( value, out float data );
					result = data;
				} else if ( targetType == typeof( double ) ) {
					output = double.TryParse( value, out double data );
					result = data;
				} else if ( targetType == typeof( bool ) ) {
					output = bool.TryParse( value, out bool data );
					result = data;
				} else if ( targetType == typeof( string ) ) {
					output = true;
					result = value;
				} else {
					throw new NotSupportedException( $"CVars do not support type {targetType.Name}" );
				}
				return output;
			} catch {
				result = null;
				return false;
			}
		}
	};
};
