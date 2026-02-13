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

	internal unsafe readonly struct CVarConverter<T> {
		private readonly CVarType _type;
		private readonly T *_value;

		/*
		===============
		CVarConverter
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="pType"></param>
		/// <param name="pValue"></param>
		public CVarConverter( CVarType type, T *pValue ) {
			_type = type;
			_value = pValue;
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
		public string? GetStringValue()
			=> _type == CVarType.String ? *(string *)_value : null;

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
		public uint GetUIntegerValue()
			=> _type == CVarType.UInt ? *(uint *)_value : 0;

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
		public int GetIntegerValue()
			=> _type == CVarType.Int ? *(int *)_value : 0;

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
		public float GetDecimalValue()
			=> _type == CVarType.Decimal ? *(float *)_value : 0.0f;

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
		public bool GetBooleanValue()
			=> _type == CVarType.Boolean && *(bool *)_value;

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
		public T1 GetValue<T1>()
			=> *_value is T1 value ? value : default;

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
			=> *_value = _type == CVarType.Int ? (T)(object)value : throw new InvalidCastException( "Incompatible CVar cast!" );

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
			=> *_value = _type == CVarType.UInt ? (T)(object)value : throw new InvalidCastException( "Incompatible CVar cast!" );

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
			=> *_value = _type == CVarType.Boolean ? (T)(object)value : throw new InvalidCastException( "Incompatible CVar cast!" );

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
			=> *_value = _type == CVarType.Decimal ? (T)(object)value : throw new InvalidCastException( "Incompatible CVar cast!" );

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
			=> *_value = _type == CVarType.String ? (T)(object)value : throw new InvalidCastException( "Incompatible CVar cast!" );

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
