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

namespace Nomad.Input.Private.ValueObjects {
	/*
	===================================================================================
	
	BindKey
	
	===================================================================================
	*/
	/// <summary>
	/// Represents an input binding's unique identifier.
	/// </summary>
	
	internal readonly struct BindKey : IEquatable<BindKey> {
		/// <summary>
		/// The bind's name. Used for generating the hashkey.
		/// </summary>
		public readonly InternString Name { get; }

		/// <summary>
		/// The bind's required input sequence. Used for generating the hashkey.
		/// </summary>
		public readonly InputEventData InputData { get; }

		private readonly int _hashCode;

		/*
		===============
		BindKey
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="inputData"></param>
		public BindKey( InternString name, InputEventData inputData ) {
			Name = name;
			InputData = inputData;
			_hashCode = HashCode.Combine(
				name.GetHashCode()
			);
		}

		/*
		===============
		Equals
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public bool Equals( BindKey other )
			=> _hashCode == other._hashCode;
		
		/*
		===============
		GetHashCode
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public override int GetHashCode()
			=> _hashCode;

		/*
		===============
		Equals
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public override bool Equals( object? obj )
			=> obj is BindKey key && Equals( key );
	};
};