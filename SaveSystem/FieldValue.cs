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
using System.Runtime.InteropServices;

namespace SaveSystem {
	/*
	===================================================================================
	
	FieldValue
	
	===================================================================================
	*/
	/// <summary>
	/// This is meant to be a more efficient way of storing a save field without the boxing side effects.
	/// Everything occupies the same memory space because it's meant to be like c++'s std::any
	/// </summary>

	[StructLayout( LayoutKind.Explicit, Pack = 1 )]
	public readonly struct FieldValue {
		[FieldOffset( 0 )] public readonly bool Boolean;

		[FieldOffset( 0 )] public readonly sbyte Int8;
		[FieldOffset( 0 )] public readonly short Int16;
		[FieldOffset( 0 )] public readonly int Int32;
		[FieldOffset( 0 )] public readonly long Int64;

		[FieldOffset( 0 )] public readonly byte UInt8;
		[FieldOffset( 0 )] public readonly ushort UInt16;
		[FieldOffset( 0 )] public readonly uint UInt32;
		[FieldOffset( 0 )] public readonly ulong UInt64;

		[FieldOffset( 0 )] public readonly float Float32;
		[FieldOffset( 0 )] public readonly double Float64;

		[FieldOffset( 0 )] public readonly Vector2 Vec2;
		[FieldOffset( 0 )] public readonly Vector2I Vec2i;

		[FieldOffset( 0 )] public readonly string String;

		internal FieldValue( bool boolean ) => Boolean = boolean;
		internal FieldValue( sbyte i8 ) => Int8 = i8;
		internal FieldValue( short i16 ) => Int16 = i16;
		internal FieldValue( int i32 ) => Int32 = i32;
		internal FieldValue( long i64 ) => Int64 = i64;
		internal FieldValue( byte u8 ) => UInt8 = u8;
		internal FieldValue( ushort u16 ) => UInt16 = u16;
		internal FieldValue( uint u32 ) => UInt32 = u32;
		internal FieldValue( ulong u64 ) => UInt64 = u64;
		internal FieldValue( float f32 ) => Float32 = f32;
		internal FieldValue( double f64 ) => Float64 = f64;
		internal FieldValue( Vector2 vec2 ) => Vec2 = vec2;
		internal FieldValue( Vector2I vec2i ) => Vec2i = vec2i;
		internal FieldValue( string str ) => String = str;

		/*
		===============
		GetFieldType
		===============
		*/
		internal static FieldType GetFieldType<T>() {
			Type type = typeof( T );
			if ( type == typeof( sbyte ) ) {
				return FieldType.Int8;
			} else if ( type == typeof( short ) ) {
				return FieldType.Int16;
			} else if ( type == typeof( int ) ) {
				return FieldType.Int32;
			} else if ( type == typeof( long ) ) {
				return FieldType.Int64;
			} else if ( type == typeof( byte ) ) {
				return FieldType.UInt8;
			} else if ( type == typeof( ushort ) ) {
				return FieldType.UInt16;
			} else if ( type == typeof( uint ) ) {
				return FieldType.UInt32;
			} else if ( type == typeof( ulong ) ) {
				return FieldType.UInt64;
			} else if ( type == typeof( string ) ) {
				return FieldType.String;
			} else if ( type == typeof( bool ) ) {
				return FieldType.Boolean;
			} else if ( type == typeof( Vector2 ) ) {
				return FieldType.Vector2;
			} else if ( type == typeof( Vector2I ) ) {
				return FieldType.Vector2I;
			}
			throw new InvalidCastException( $"Invalid field type {type}" );
		}
	};
};