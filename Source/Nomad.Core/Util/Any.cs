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
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Nomad.Core.Util
{
    /// <summary>
    /// A type-safe container that can hold any of the supported primitive types.
    /// Similar to C++'s std::any, it provides a unified way to store and retrieve
    /// different value types in a memory-efficient manner using a union structure.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="Any"/> struct occupies exactly 20 bytes of memory regardless
    /// of the stored type, using an explicit layout union to share the same memory space.
    /// </para>
    /// <para>
    /// This type is particularly useful in scenarios where type erasure is needed
    /// while maintaining type safety through explicit conversion or retrieval.
    /// </para>
    /// </remarks>
    public struct Any
    {
        [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 16)]
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

            [FieldOffset(8)] public string? String;
            [FieldOffset(8)] public object Reference;
        }

        private static readonly ImmutableDictionary<Type, AnyType> _systemTypeToAnyType = new Dictionary<Type, AnyType>() {
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
            { typeof( double ), AnyType.Float64 },
            { typeof( string ), AnyType.String },
            { typeof( object ), AnyType.Object }
        }.ToImmutableDictionary();

        private static readonly ImmutableDictionary<AnyType, Type> _anyTypeToSystemType = new Dictionary<AnyType, Type>() {
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
            { AnyType.Float64, typeof( double ) },
            { AnyType.String, typeof( string ) },
            { AnyType.Object, typeof( object ) }
        }.ToImmutableDictionary();

        private Union _value;
        private AnyType _type;

        /// <summary>
        /// Initializes a new instance of the <see cref="Any"/> struct with a boolean value.
        /// </summary>
        /// <param name="b">The boolean value to store.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Any(bool b)
        {
            _value = new Union { Boolean = b };
            _type = AnyType.Boolean;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Any"/> struct with a signed 8-bit integer.
        /// </summary>
        /// <param name="i8">The signed 8-bit integer value to store.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Any(sbyte i8)
        {
            _value = new Union { Int8 = i8 };
            _type = AnyType.Int8;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Any"/> struct with a signed 16-bit integer.
        /// </summary>
        /// <param name="i16">The signed 16-bit integer value to store.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Any(short i16)
        {
            _value = new Union { Int16 = i16 };
            _type = AnyType.Int16;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Any"/> struct with a signed 32-bit integer.
        /// </summary>
        /// <param name="i32">The signed 32-bit integer value to store.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Any(int i32)
        {
            _value = new Union { Int32 = i32 };
            _type = AnyType.Int32;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Any"/> struct with a signed 64-bit integer.
        /// </summary>
        /// <param name="i64">The signed 64-bit integer value to store.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Any(long i64)
        {
            _value = new Union { Int64 = i64 };
            _type = AnyType.Int64;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Any"/> struct with an unsigned 8-bit integer.
        /// </summary>
        /// <param name="u8">The unsigned 8-bit integer value to store.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Any(byte u8)
        {
            _value = new Union { UInt8 = u8 };
            _type = AnyType.UInt8;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Any"/> struct with an unsigned 16-bit integer.
        /// </summary>
        /// <param name="u16">The unsigned 16-bit integer value to store.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Any(ushort u16)
        {
            _value = new Union { UInt16 = u16 };
            _type = AnyType.UInt16;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Any"/> struct with an unsigned 32-bit integer.
        /// </summary>
        /// <param name="u32">The unsigned 32-bit integer value to store.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Any(uint u32)
        {
            _value = new Union { UInt32 = u32 };
            _type = AnyType.UInt32;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Any"/> struct with an unsigned 64-bit integer.
        /// </summary>
        /// <param name="u64">The unsigned 64-bit integer value to store.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Any(ulong u64)
        {
            _value = new Union { UInt64 = u64 };
            _type = AnyType.UInt64;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Any"/> struct with a 32-bit floating-point number.
        /// </summary>
        /// <param name="f32">The 32-bit floating-point value to store.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Any(float f32)
        {
            _value = new Union { Float32 = f32 };
            _type = AnyType.Float32;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Any"/> struct with a 64-bit floating-point number.
        /// </summary>
        /// <param name="f64">The 64-bit floating-point value to store.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Any(double f64)
        {
            _value = new Union { Float64 = f64 };
            _type = AnyType.Float64;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Any"/> struct with a string.
        /// </summary>
        /// <param name="str">The <see cref="string"/> value to store.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Any(string str)
        {
            _value = new Union { String = str };
            _type = AnyType.String;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Any"/> struct with an object.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> value to store.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Any(object obj)
        {
            _value = new Union { Reference = obj };
            _type = AnyType.Object;
        }

        /// <summary>
        /// Converts an <see cref="Any"/> instance to a boolean value.
        /// </summary>
        /// <param name="value">The <see cref="Any"/> instance to convert.</param>
        /// <returns>The stored boolean value.</returns>
        /// <remarks>
        /// This conversion assumes the <see cref="Any"/> instance was initialized with a boolean.
        /// Using this conversion on an <see cref="Any"/> instance storing a different type will
        /// reinterpret the underlying bytes as a boolean, which may produce unexpected results.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator bool(Any value)
        {
            value._type = AnyType.Boolean;
            return value._value.Boolean;
        }

        /// <summary>
        /// Converts an <see cref="Any"/> instance to a signed 8-bit integer.
        /// </summary>
        /// <param name="value">The <see cref="Any"/> instance to convert.</param>
        /// <returns>The stored signed 8-bit integer value.</returns>
        /// <remarks>
        /// This conversion assumes the <see cref="Any"/> instance was initialized with a signed 8-bit integer.
        /// Using this conversion on an <see cref="Any"/> instance storing a different type will
        /// reinterpret the underlying bytes as a signed 8-bit integer, which may produce unexpected results.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator sbyte(Any value)
        {
            value._type = AnyType.Int8;
            return value._value.Int8;
        }

        /// <summary>
        /// Converts an <see cref="Any"/> instance to a signed 16-bit integer.
        /// </summary>
        /// <param name="value">The <see cref="Any"/> instance to convert.</param>
        /// <returns>The stored signed 16-bit integer value.</returns>
        /// <remarks>
        /// This conversion assumes the <see cref="Any"/> instance was initialized with a signed 16-bit integer.
        /// Using this conversion on an <see cref="Any"/> instance storing a different type will
        /// reinterpret the underlying bytes as a signed 16-bit integer, which may produce unexpected results.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator short(Any value)
        {
            value._type = AnyType.Int16;
            return value._value.Int16;
        }

        /// <summary>
        /// Converts an <see cref="Any"/> instance to a signed 32-bit integer.
        /// </summary>
        /// <param name="value">The <see cref="Any"/> instance to convert.</param>
        /// <returns>The stored signed 32-bit integer value.</returns>
        /// <remarks>
        /// This conversion assumes the <see cref="Any"/> instance was initialized with a signed 32-bit integer.
        /// Using this conversion on an <see cref="Any"/> instance storing a different type will
        /// reinterpret the underlying bytes as a signed 32-bit integer, which may produce unexpected results.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator int(Any value)
        {
            value._type = AnyType.Int32;
            return value._value.Int32;
        }

        /// <summary>
        /// Converts an <see cref="Any"/> instance to a signed 64-bit integer.
        /// </summary>
        /// <param name="value">The <see cref="Any"/> instance to convert.</param>
        /// <returns>The stored signed 64-bit integer value.</returns>
        /// <remarks>
        /// This conversion assumes the <see cref="Any"/> instance was initialized with a signed 64-bit integer.
        /// Using this conversion on an <see cref="Any"/> instance storing a different type will
        /// reinterpret the underlying bytes as a signed 64-bit integer, which may produce unexpected results.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator long(Any value)
        {
            value._type = AnyType.Int64;
            return value._value.Int64;
        }

        /// <summary>
        /// Converts an <see cref="Any"/> instance to an unsigned 8-bit integer.
        /// </summary>
        /// <param name="value">The <see cref="Any"/> instance to convert.</param>
        /// <returns>The stored unsigned 8-bit integer value.</returns>
        /// <remarks>
        /// This conversion assumes the <see cref="Any"/> instance was initialized with an unsigned 8-bit integer.
        /// Using this conversion on an <see cref="Any"/> instance storing a different type will
        /// reinterpret the underlying bytes as an unsigned 8-bit integer, which may produce unexpected results.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator byte(Any value)
        {
            value._type = AnyType.UInt8;
            return value._value.UInt8;
        }

        /// <summary>
        /// Converts an <see cref="Any"/> instance to an unsigned 16-bit integer.
        /// </summary>
        /// <param name="value">The <see cref="Any"/> instance to convert.</param>
        /// <returns>The stored unsigned 16-bit integer value.</returns>
        /// <remarks>
        /// This conversion assumes the <see cref="Any"/> instance was initialized with an unsigned 16-bit integer.
        /// Using this conversion on an <see cref="Any"/> instance storing a different type will
        /// reinterpret the underlying bytes as an unsigned 16-bit integer, which may produce unexpected results.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ushort(Any value)
        {
            value._type = AnyType.UInt16;
            return value._value.UInt16;
        }

        /// <summary>
        /// Converts an <see cref="Any"/> instance to an unsigned 32-bit integer.
        /// </summary>
        /// <param name="value">The <see cref="Any"/> instance to convert.</param>
        /// <returns>The stored unsigned 32-bit integer value.</returns>
        /// <remarks>
        /// This conversion assumes the <see cref="Any"/> instance was initialized with an unsigned 32-bit integer.
        /// Using this conversion on an <see cref="Any"/> instance storing a different type will
        /// reinterpret the underlying bytes as an unsigned 32-bit integer, which may produce unexpected results.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator uint(Any value)
        {
            value._type = AnyType.UInt32;
            return value._value.UInt32;
        }

        /// <summary>
        /// Converts an <see cref="Any"/> instance to an unsigned 64-bit integer.
        /// </summary>
        /// <param name="value">The <see cref="Any"/> instance to convert.</param>
        /// <returns>The stored unsigned 64-bit integer value.</returns>
        /// <remarks>
        /// This conversion assumes the <see cref="Any"/> instance was initialized with an unsigned 64-bit integer.
        /// Using this conversion on an <see cref="Any"/> instance storing a different type will
        /// reinterpret the underlying bytes as an unsigned 64-bit integer, which may produce unexpected results.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ulong(Any value)
        {
            value._type = AnyType.UInt64;
            return value._value.UInt64;
        }

        /// <summary>
        /// Converts an <see cref="Any"/> instance to a 32-bit floating-point number.
        /// </summary>
        /// <param name="value">The <see cref="Any"/> instance to convert.</param>
        /// <returns>The stored 32-bit floating-point value.</returns>
        /// <remarks>
        /// This conversion assumes the <see cref="Any"/> instance was initialized with a 32-bit floating-point number.
        /// Using this conversion on an <see cref="Any"/> instance storing a different type will
        /// reinterpret the underlying bytes as a 32-bit floating-point number, which may produce unexpected results.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator float(Any value)
        {
            value._type = AnyType.Float32;
            return value._value.Float32;
        }

        /// <summary>
        /// Converts an <see cref="Any"/> instance to a 64-bit floating-point number.
        /// </summary>
        /// <param name="value">The <see cref="Any"/> instance to convert.</param>
        /// <returns>The stored 64-bit floating-point value.</returns>
        /// <remarks>
        /// This conversion assumes the <see cref="Any"/> instance was initialized with a 64-bit floating-point number.
        /// Using this conversion on an <see cref="Any"/> instance storing a different type will
        /// reinterpret the underlying bytes as a 64-bit floating-point number, which may produce unexpected results.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator double(Any value)
        {
            value._type = AnyType.Float64;
            return value._value.Float64;
        }

        /// <summary>
        /// Converts an <see cref="Any"/> instance to an string.
        /// </summary>
        /// <param name="value">The <see cref="Any"/> instance to convert.</param>
        /// <returns>The stored <see cref="string"/> value.</returns>
        /// <remarks>
        /// This conversion assumes the <see cref="Any"/> instance was initialized with an <see cref="string"/>.
        /// Using this conversion on an <see cref="Any"/> instance storing a different type will
        /// reinterpret the underlying bytes as an <see cref="string"/>, which may produce unexpected results.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator string(Any value)
        {
            value._type = AnyType.String;
            return value._value.String;
        }

        /// <summary>
        /// Creates an <see cref="Any"/> instance from a supported primitive value.
        /// </summary>
        /// <typeparam name="T">The type of the value to store. Must be one of the supported primitive types.</typeparam>
        /// <param name="value">The value to store in the <see cref="Any"/> instance.</param>
        /// <returns>A new <see cref="Any"/> instance containing the specified value.</returns>
        /// <exception cref="InvalidCastException">
        /// Thrown when <typeparamref name="T"/> is not one of the supported primitive types.
        /// </exception>
        /// <remarks>
        /// <para>
        /// Supported types are: <see cref="bool"/>, <see cref="sbyte"/>, <see cref="short"/>, <see cref="int"/>,
        /// <see cref="long"/>, <see cref="byte"/>, <see cref="ushort"/>, <see cref="uint"/>, <see cref="ulong"/>,
        /// <see cref="float"/>, <see cref="double"/>, and <see cref="InternString"/>.
        /// </para>
        /// <para>
        /// This method is aggressively inlined for performance.
        /// </para>
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Any From<T>(T value) => value switch
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
            null when typeof(T) == typeof(string) => new Any((string)(object)value),
            string str => new Any(str),
            object obj => new Any(obj),
            _ => throw new InvalidCastException($"An Any object cannot hold a value of type {typeof(T)}")
        };

        /// <summary>
        /// Retrieves the value stored in this <see cref="Any"/> instance as the specified type.
        /// </summary>
        /// <typeparam name="T">The type to retrieve the value as. Must be one of the supported primitive types.</typeparam>
        /// <returns>The stored value cast to <typeparamref name="T"/>.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when <typeparamref name="T"/> is not one of the supported primitive types.
        /// </exception>
        /// <remarks>
        /// <para>
        /// This method does not perform type checking beyond verifying that <typeparamref name="T"/>
        /// is a supported type. It is the caller's responsibility to ensure the <see cref="Any"/>
        /// instance was created with a value of the same type.
        /// </para>
        /// <para>
        /// This method is aggressively inlined for performance.
        /// </para>
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T GetPrimitiveValue<T>() where T : unmanaged
            => typeof(T) switch
            {
                Type t when t == typeof(bool) => Unsafe.As<bool, T>(ref Unsafe.AsRef(in _value.Boolean)),
                Type t when t == typeof(sbyte) => Unsafe.As<sbyte, T>(ref Unsafe.AsRef(in _value.Int8)),
                Type t when t == typeof(short) => Unsafe.As<short, T>(ref Unsafe.AsRef(in _value.Int16)),
                Type t when t == typeof(int) => Unsafe.As<int, T>(ref Unsafe.AsRef(in _value.Int32)),
                Type t when t == typeof(long) => Unsafe.As<long, T>(ref Unsafe.AsRef(in _value.Int64)),
                Type t when t == typeof(byte) => Unsafe.As<byte, T>(ref Unsafe.AsRef(in _value.UInt8)),
                Type t when t == typeof(ushort) => Unsafe.As<ushort, T>(ref Unsafe.AsRef(in _value.UInt16)),
                Type t when t == typeof(uint) => Unsafe.As<uint, T>(ref Unsafe.AsRef(in _value.UInt32)),
                Type t when t == typeof(ulong) => Unsafe.As<ulong, T>(ref Unsafe.AsRef(in _value.UInt64)),
                Type t when t == typeof(float) => Unsafe.As<float, T>(ref Unsafe.AsRef(in _value.Float32)),
                Type t when t == typeof(double) => Unsafe.As<double, T>(ref Unsafe.AsRef(in _value.Float64)),
                _ => throw new InvalidOperationException()
            };

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T? GetReferenceValue<T>() where T : class
            => typeof(T) switch
            {
                Type t when t == typeof(object) => Unsafe.As<object, T>(ref Unsafe.AsRef(in _value.Reference)),
                Type t when t == typeof(string) => Unsafe.As<string, T>(ref Unsafe.AsRef(in _value.String)),
                _ => throw new InvalidOperationException()
            };

        /// <summary>
        /// Gets the .NET <see cref="Type"/> corresponding to the specified <see cref="AnyType"/>.
        /// </summary>
        /// <param name="type">The <see cref="AnyType"/> value to convert.</param>
        /// <returns>The corresponding .NET <see cref="Type"/> for the specified <see cref="AnyType"/>.</returns>
        /// <exception cref="InvalidCastException">
        /// Thrown when <paramref name="type"/> is not a valid <see cref="AnyType"/> value.
        /// </exception>
        /// <remarks>
        /// This method is aggressively inlined for performance.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type GetAnyType(AnyType type) =>
            _anyTypeToSystemType.TryGetValue(type, out Type? value) ? value : throw new InvalidCastException();

        /// <summary>
        /// Gets the <see cref="AnyType"/> corresponding to the specified .NET type.
        /// </summary>
        /// <typeparam name="T">The .NET type to convert.</typeparam>
        /// <returns>The corresponding <see cref="AnyType"/> for the specified .NET type.</returns>
        /// <exception cref="InvalidCastException">
        /// Thrown when <typeparamref name="T"/> is not a supported type for <see cref="Any"/>.
        /// </exception>
        /// <remarks>
        /// This method is aggressively inlined for performance.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AnyType GetType<T>() =>
            _systemTypeToAnyType.TryGetValue(typeof(T), out AnyType type) ? type : throw new InvalidCastException();
    };
}