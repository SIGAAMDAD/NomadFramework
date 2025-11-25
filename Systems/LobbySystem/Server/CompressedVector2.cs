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


using Godot;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NomadCore.Systems.LobbySystem.Server {
	/*
	===================================================================================
	
	CompressedVector2
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>

	[StructLayout( LayoutKind.Explicit, Pack = 1, Size = sizeof( short ) )]
	public readonly struct CompressedVector2 {
		public enum Axis : sbyte {
			X,
			Y
		};

		public const float GRID_STEP = 16.0f;
		private const float INTERPOLATION = 1.0f / GRID_STEP;

		[FieldOffset( 0 )] public readonly sbyte X;
		[FieldOffset( 1 )] public readonly sbyte Y;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public readonly sbyte this[ int index ] {
			get {
				return index switch {
					0 => X,
					1 => Y,
					_ => throw new ArgumentOutOfRangeException( nameof( index ) ),
				};
			}
		}

		/*
		===============
		CompressedVector2
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public CompressedVector2( float x, float y ) {
			X = (sbyte)Mathf.Clamp( Mathf.Round( x / GRID_STEP ), -127, 127 );
			Y = (sbyte)Mathf.Clamp( Mathf.Round( y / GRID_STEP ), -127, 127 );
		}

		/*
		===============
		CompressedVector2
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="packed"></param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public CompressedVector2( short packed ) {
			X = (sbyte)( packed & 0xFF );
			Y = (sbyte)( ( packed >> 8 ) & 0xFF );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public CompressedVector2( in Vector2 vec2 ) {
			X = (sbyte)Mathf.Clamp( Mathf.Round( vec2.X / GRID_STEP ), -127, 127 );
			Y = (sbyte)Mathf.Clamp( Mathf.Round( vec2.Y / GRID_STEP ), -127, 127 );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static explicit operator short( CompressedVector2 value ) {
			return (short)( (byte)value.X | ( (byte)value.Y << 8 ) );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static explicit operator Vector2( CompressedVector2 value ) {
			return new Vector2( value.X * INTERPOLATION, value.Y * INTERPOLATION );
		}
	};
};