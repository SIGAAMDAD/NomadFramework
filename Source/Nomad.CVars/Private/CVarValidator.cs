/*
===========================================================================
The Nomad Framework
Copyright (C) 2025 Noah Van Til

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

	CVarValidator

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	internal readonly struct CVarValidator<T> {
		private readonly Func<T, bool>? _validator;

		/*
		===============
		CVarValidator
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="validator"></param>
		public CVarValidator( Func<T, bool>? validator ) {
			_validator = validator;
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
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public bool ValidateValue( T value ) {
			return _validator == null || _validator.Invoke( value );
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
		public bool IsValidName( string name ) {
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
		ValidateCVarType
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static bool ValidateCVarType() =>
			typeof( T ).GetCVarType() != CVarType.Count;

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
		private static bool IsValidNameCharacter( char c ) =>
			char.IsLetterOrDigit( c ) || c == '.' || c == '_';
	};
};
