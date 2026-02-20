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
using System.Collections.Generic;

namespace Nomad.Core.Compatibility.Guards
{
    /// <summary>
    ///
    /// </summary>
    public static class RangeGuard
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="other"></param>
        /// <param name="paramName"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfLessThanOrEqual<T>(T value, T other, string? paramName = null)
            where T : IComparable<T>
        {
#if NETSTANDARD2_1 || !NET6_0_OR_GREATER
            if (value.CompareTo(other) <= 0)
            {
                throw new ArgumentOutOfRangeException(paramName);
            }
#else
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(value, other, paramName);
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="other"></param>
        /// <param name="paramName"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfGreaterThanOrEqual<T>(T value, T other, string? paramName = null)
            where T : IComparable<T>
        {
#if NETSTANDARD2_1 || !NET6_0_OR_GREATER
            if (value.CompareTo(other) >= 0)
            {
                throw new ArgumentOutOfRangeException(paramName);
            }
#else
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(value, other, paramName);
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="other"></param>
        /// <param name="paramName"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfLessThan<T>(T value, T other, string? paramName)
            where T : IComparable<T>
        {
#if NETSTANDARD2_1 || !NET6_0_OR_GREATER
            if (value.CompareTo(other) < 0)
            {
                throw new ArgumentOutOfRangeException(paramName);
            }
#else
            ArgumentOutOfRangeException.ThrowIfLessThan(value, other, paramName);
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="other"></param>
        /// <param name="paramName"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfGreaterThan<T>(T value, T other, string? paramName)
            where T : IComparable<T>
        {
#if NETSTANDARD2_1 || !NET6_0_OR_GREATER
            if (value.CompareTo(other) > 0)
            {
                throw new ArgumentOutOfRangeException(paramName);
            }
#else
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value, other, paramName);
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="other"></param>
        /// <param name="paramName"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfEqual<T>(T value, T other, string? paramName)
            where T : IEquatable<T>
        {
#if NETSTANDARD2_1 || !NET6_0_OR_GREATER
            if (value.Equals(other))
            {
                throw new ArgumentOutOfRangeException(paramName);
            }
#else
            ArgumentOutOfRangeException.ThrowIfEqual(value, other, paramName);
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="other"></param>
        /// <param name="paramName"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfNotEqual<T>(T value, T other, string? paramName)
            where T : IEquatable<T>
        {
#if NETSTANDARD2_1 || !NET6_0_OR_GREATER
            if (!value.Equals(other))
            {
                throw new ArgumentOutOfRangeException(paramName);
            }
#else
            ArgumentOutOfRangeException.ThrowIfNotEqual(value, other, paramName);
#endif
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <param name="paramName"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfNegative(int value, string? paramName = null)
        {
#if NETSTANDARD2_1 || !NET6_0_OR_GREATER
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(paramName, "Value cannot be negative.");
            }
#else
            ArgumentOutOfRangeException.ThrowIfNegative(value, paramName);
#endif
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <param name="paramName"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfNegative(float value, string? paramName = null)
        {
#if NETSTANDARD2_1 || !NET6_0_OR_GREATER
            if (value < 0.0f)
            {
                throw new ArgumentOutOfRangeException(paramName, "Value cannot be negative.");
            }
#else
            ArgumentOutOfRangeException.ThrowIfNegative(value, paramName);
#endif
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <param name="paramName"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfZero(int value, string? paramName = null)
        {
#if NETSTANDARD2_1 || !NET6_0_OR_GREATER
            if (value == 0)
            {
                throw new ArgumentOutOfRangeException(paramName, "Value cannot be zero.");
            }
#else
            ArgumentOutOfRangeException.ThrowIfZero(value, paramName);
#endif
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <param name="paramName"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfZero(float value, string? paramName = null)
        {
#if NETSTANDARD2_1 || !NET6_0_OR_GREATER
            if (value == 0.0f)
            {
                throw new ArgumentOutOfRangeException(paramName, "Value cannot be zero.");
            }
#else
            ArgumentOutOfRangeException.ThrowIfZero(value, paramName);
#endif
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <param name="paramName"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfNegativeOrZero(int value, string? paramName = null)
        {
#if NETSTANDARD2_1 || !NET6_0_OR_GREATER
            if (value < 0 || value == 0)
            {
                throw new ArgumentOutOfRangeException(paramName, "Value must be greater than zero.");
            }
#else
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value, paramName);
#endif
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <param name="paramName"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfNegativeOrZero(float value, string? paramName = null)
        {
#if NETSTANDARD2_1 || !NET6_0_OR_GREATER
            if (value < 0.0f || value == 0.0f)
            {
                throw new ArgumentOutOfRangeException(paramName, "Value must be greater than zero.");
            }
#else
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value, paramName);
#endif
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="paramName"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfOutOfRange(int value, int min, int max, string? paramName = null)
        {
            if (value < min || value > max)
            {
                throw new ArgumentOutOfRangeException(paramName, $"Value must be between {min} and {max}.");
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <param name="paramName"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotPowerOfTwo(int value, string? paramName = null)
        {
            if (value <= 0 || (value & (value - 1)) != 0)
            {
                throw new ArgumentOutOfRangeException(paramName, "Value must be a power of two.");
            }
        }
    }
}
