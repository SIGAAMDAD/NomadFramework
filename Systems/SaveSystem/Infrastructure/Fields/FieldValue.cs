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

using NomadCore.Systems.SaveSystem.Enums;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NomadCore.Systems.SaveSystem.Infrastructure.Fields {
	/*
	===================================================================================
	
	FieldValue
	
	===================================================================================
	*/
	/// <summary>
	/// This is meant to be a more efficient way of storing a save field without the boxing side effects.
	/// Everything occupies the same memory space because it's meant to be like c++'s std::any
	/// </summary>

	internal readonly record struct FieldValue {
		[StructLayout( LayoutKind.Explicit )]
		private struct Union {
			[FieldOffset( 0 )] public bool Boolean;

			[FieldOffset( 0 )] public sbyte Int8;
			[FieldOffset( 0 )] public short Int16;
			[FieldOffset( 0 )] public int Int32;
			[FieldOffset( 0 )] public long Int64;

			[FieldOffset( 0 )] public byte UInt8;
			[FieldOffset( 0 )] public ushort UInt16;
			[FieldOffset( 0 )] public uint UInt32;
			[FieldOffset( 0 )] public ulong UInt64;

			[FieldOffset( 0 )] public float Float32;
			[FieldOffset( 0 )] public double Float64;

			[FieldOffset( 0 )] public string? String;
		};

		private static readonly IReadOnlyDictionary<Type, FieldType> SystemTypeToFieldType = new Dictionary<Type, FieldType>() {
			{ typeof( bool ), FieldType.Boolean },
			{ typeof( sbyte ), FieldType.Int8 },
			{ typeof( short ), FieldType.Int16 },
			{ typeof( int ), FieldType.Int32 },
			{ typeof( long ), FieldType.Int64 },
			{ typeof( byte ), FieldType.UInt8 },
			{ typeof( ushort ), FieldType.UInt16 },
			{ typeof( uint ), FieldType.UInt32 },
			{ typeof( ulong ), FieldType.UInt64 },
			{ typeof( string ), FieldType.String },
			{ typeof( float ), FieldType.Float },
			{ typeof( double ), FieldType.Double }
		};
		private static readonly IReadOnlyDictionary<FieldType, Type> FieldTypeToSystemType = new Dictionary<FieldType, Type>() {
			{ FieldType.Boolean, typeof( bool ) },
			{ FieldType.Int8, typeof( sbyte ) },
			{ FieldType.Int16, typeof( short ) },
			{ FieldType.Int32, typeof( int ) },
			{ FieldType.Int64, typeof( long ) },
			{ FieldType.UInt8, typeof( byte ) },
			{ FieldType.UInt16, typeof( ushort ) },
			{ FieldType.UInt32, typeof( uint ) },
			{ FieldType.UInt64, typeof( ulong ) },
			{ FieldType.String, typeof( string ) },
			{ FieldType.Float, typeof( float ) },
			{ FieldType.Double, typeof( double ) }
		};

		private readonly Union Value;

		public FieldValue( bool b ) => Value = new Union { Boolean = b };
		public FieldValue( sbyte i8 ) => Value = new Union { Int8 = i8 };
		public FieldValue( short i16 ) => Value = new Union { Int16 = i16 };
		public FieldValue( int i32 ) => Value = new Union { Int32 = i32 };
		public FieldValue( long i64 ) => Value = new Union { Int64 = i64 };
		public FieldValue( byte u8 ) => Value = new Union { UInt8 = u8 };
		public FieldValue( ushort u16 ) => Value = new Union { UInt16 = u16 };
		public FieldValue( uint u32 ) => Value = new Union { UInt32 = u32 };
		public FieldValue( ulong u64 ) => Value = new Union { UInt64 = u64 };
		public FieldValue( float f32 ) => Value = new Union { Float32 = f32 };
		public FieldValue( double f64 ) => Value = new Union { Float64 = f64 };
		public FieldValue( string? str ) => Value = new Union { String = str };

		/*
		===============
		GetValue
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public T? GetValue<T>() => typeof( T ) switch {
			Type t when t == typeof( bool ) => (T)(object)Value.Boolean,
			Type t when t == typeof( sbyte ) => (T)(object)Value.Int8,
			Type t when t == typeof( short ) => (T)(object)Value.Int16,
			Type t when t == typeof( int ) => (T)(object)Value.Int32,
			Type t when t == typeof( long ) => (T)(object)Value.Int64,
			Type t when t == typeof( byte ) => (T)(object)Value.UInt8,
			Type t when t == typeof( ushort ) => (T)(object)Value.UInt16,
			Type t when t == typeof( uint ) => (T)(object)Value.UInt32,
			Type t when t == typeof( ulong ) => (T)(object)Value.UInt64,
			Type t when t == typeof( string ) => (T?)(object?)Value.String,
			Type t when t == typeof( float ) => (T)(object)Value.Float32,
			Type t when t == typeof( double ) => (T)(object)Value.Float64,
			_ => throw new InvalidCastException( $"Field type of '{typeof( T )}' is not supported" )
		};

		/*
		===============
		From
		===============
		*/
		/// <summary>
		/// Creates an FieldValue object from the provided primitive value.
		/// </summary>
		/// <typeparam name="T">The type of the value</typeparam>
		/// <returns>A new FieldValue object</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static FieldValue From<T>( T value ) => value switch {
			bool b => new FieldValue( b ),
			sbyte i8 => new FieldValue( i8 ),
			short i16 => new FieldValue( i16 ),
			int i32 => new FieldValue( i32 ),
			long i64 => new FieldValue( i64 ),
			byte u8 => new FieldValue( u8 ),
			ushort u16 => new FieldValue( u16 ),
			uint u32 => new FieldValue( u32 ),
			ulong u64 => new FieldValue( u64 ),
			float f32 => new FieldValue( f32 ),
			double f64 => new FieldValue( f64 ),
			string str => new FieldValue( str ),
			_ => throw new InvalidCastException( $"A FieldValue object cannot hold a value of type {typeof( T )}" )
		};

		/*
		===============
		GetFieldType
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		/// <exception cref="InvalidCastException"></exception>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static Type GetFieldType( FieldType type ) =>
			FieldTypeToSystemType.TryGetValue( type, out Type value ) ? value : throw new InvalidCastException( $"Invalid field type {type}" );

		/*
		===============
		GetFieldType
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		/// <exception cref="InvalidCastException"></exception>
		public static FieldType GetFieldType<T>() =>
			SystemTypeToFieldType.TryGetValue( typeof( T ), out FieldType type ) ? type : throw new InvalidCastException( $"Invalid field type {typeof( T )}" );
	};
};