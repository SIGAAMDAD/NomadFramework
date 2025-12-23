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
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Nomad.Core.Abstractions;

namespace Nomad.Core.Util
{
    /// <summary>
    /// Everything occupies the same memory space because it's meant to be like c++'s std::any
    /// </summary>
    public readonly record struct Any : IValueObject<Any>
    {
        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        private struct Union
        {
            [FieldOffset(0)] public bool Boolean;

            [FieldOffset(0)] public sbyte Int8;
            [FieldOffset(0)] public short Int16;
            [FieldOffset(0)] public int Int32;
            [FieldOffset(0)] public long Int64;

            [FieldOffset(0)] public byte UInt8;
            [FieldOffset(0)] public ushort UInt16;
            [FieldOffset(0)] public uint UInt32;
            [FieldOffset(0)] public ulong UInt64;

            [FieldOffset(0)] public float Float32;
            [FieldOffset(0)] public double Float64;
        }

        private static readonly IReadOnlyDictionary<Type, AnyType> _systemTypeToAnyType = new Dictionary<Type, AnyType>() {
            { typeof( bool ), AnyType.Boolean },
            { typeof( sbyte ), AnyType.Int8 },
            { typeof( short ), AnyType.Int16 },
            { typeof( int ), AnyType.Int32 },
            { typeof( long ), AnyType.Int64 },
            { typeof( byte ), AnyType.UInt8 },
            { typeof( ushort ), AnyType.UInt16 },
            { typeof( uint ), AnyType.UInt32 },
            { typeof( ulong ), AnyType.UInt64 },
            { typeof( float ), AnyType.Float32 },
            { typeof( double ), AnyType.Float64 }
        };
        private static readonly IReadOnlyDictionary<AnyType, Type> _anyTypeToSystemType = new Dictionary<AnyType, Type>() {
            { AnyType.Boolean, typeof( bool ) },
            { AnyType.Int8, typeof( sbyte ) },
            { AnyType.Int16, typeof( short ) },
            { AnyType.Int32, typeof( int ) },
            { AnyType.Int64, typeof( long ) },
            { AnyType.UInt8, typeof( byte ) },
            { AnyType.UInt16, typeof( ushort ) },
            { AnyType.UInt32, typeof( uint ) },
            { AnyType.UInt64, typeof( ulong ) },
            { AnyType.Float32, typeof( float ) },
            { AnyType.Float64, typeof( double ) }
        };

        private readonly Union _value;

        public Any(bool b) => _value = new Union { Boolean = b };
        public Any(sbyte i8) => _value = new Union { Int8 = i8 };
        public Any(short i16) => _value = new Union { Int16 = i16 };
        public Any(int i32) => _value = new Union { Int32 = i32 };
        public Any(long i64) => _value = new Union { Int64 = i64 };
        public Any(byte u8) => _value = new Union { UInt8 = u8 };
        public Any(ushort u16) => _value = new Union { UInt16 = u16 };
        public Any(uint u32) => _value = new Union { UInt32 = u32 };
        public Any(ulong u64) => _value = new Union { UInt64 = u64 };
        public Any(float f32) => _value = new Union { Float32 = f32 };
        public Any(double f64) => _value = new Union { Float64 = f64 };

        public static implicit operator bool(Any value) => value._value.Boolean;
        public static implicit operator sbyte(Any value) => value._value.Int8;
        public static implicit operator short(Any value) => value._value.Int16;
        public static implicit operator int(Any value) => value._value.Int32;
        public static implicit operator long(Any value) => value._value.Int64;
        public static implicit operator byte(Any value) => value._value.UInt8;
        public static implicit operator ushort(Any value) => value._value.UInt16;
        public static implicit operator uint(Any value) => value._value.UInt32;
        public static implicit operator ulong(Any value) => value._value.UInt64;
        public static implicit operator float(Any value) => value._value.Float32;
        public static implicit operator double(Any value) => value._value.Float64;


        /// <summary>
        /// Creates an Any object from the provided primitive value.
        /// </summary>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <returns>A new Any object</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Any From<T>(T value) where T : unmanaged => value switch
        {
            bool b => new Any(b),
            sbyte i8 => new Any(i8),
            short i16 => new Any(i16),
            int i32 => new Any(i32),
            long i64 => new Any(i64),
            byte u8 => new Any(u8),
            ushort u16 => new Any(u16),
            uint u32 => new Any(u32),
            ulong u64 => new Any(u64),
            float f32 => new Any(f32),
            double f64 => new Any(f64),
            _ => throw new InvalidCastException($"An Any object cannot hold a value of type {typeof(T)}")
        };

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T? GetValue<T>() => typeof(T) switch
        {
            Type t when t == typeof(bool) => (T)(object)_value.Boolean,
            Type t when t == typeof(sbyte) => (T)(object)_value.Int8,
            Type t when t == typeof(short) => (T)(object)_value.Int16,
            Type t when t == typeof(int) => (T)(object)_value.Int32,
            Type t when t == typeof(long) => (T)(object)_value.Int64,
            Type t when t == typeof(byte) => (T)(object)_value.UInt8,
            Type t when t == typeof(ushort) => (T)(object)_value.UInt16,
            Type t when t == typeof(uint) => (T)(object)_value.UInt32,
            Type t when t == typeof(ulong) => (T)(object)_value.UInt64,
            Type t when t == typeof(float) => (T)(object)_value.Float32,
            Type t when t == typeof(double) => (T)(object)_value.Float64,
            _ => throw new InvalidOperationException($"An any object cannot hold a value of type {typeof(T)}")
        };

        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="InvalidCastException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type GetAnyType(AnyType type) =>
            _anyTypeToSystemType.TryGetValue(type, out Type? value) ? value : throw new InvalidCastException($"Invalid field type {type}");

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="InvalidCastException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AnyType GetType<T>() =>
            _systemTypeToAnyType.TryGetValue(typeof(T), out AnyType type) ? type : throw new InvalidCastException($"Invalid field type {typeof(T)}");
    };
};
