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

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System;

namespace Nomad.Core.Compatibility
{
    /// <summary>
    /// 
    /// </summary>
    public static class ExceptionCompat
    {
        /// <summary>
        /// (Compatibility Extension) Throws an <see cref="ArgumentNullException"/> if <paramref name="argument"/> is null.
        /// </summary>
        /// <param name="argument">The argument value to check.</param>
        /// <param name="paramName">The name of the parameter being checked.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfNull([NotNull] object? argument, string? paramName = null)
        {
#if USE_COMPATIBILITY_EXTENSIONS
            if (argument is null)
            {
                throw new ArgumentNullException(paramName);
            }
#else
            ArgumentNullException.ThrowIfNull(argument, paramName);
#endif
        }

        /// <summary>
        /// (Compatibility Extension) Throws an <see cref="ArgumentException"/> if <paramref name="argument"/> is null or empty.
        /// </summary>
        /// <param name="argument">The argument value to check.</param>
        /// <param name="paramName">The name of the parameter being checked.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfNullOrEmpty(string? argument, string? paramName = null)
        {
#if USE_COMPATIBILITY_EXTENSIONS
            if (string.IsNullOrEmpty(argument))
            {
                throw new ArgumentException("Argument cannot be null or empty.", paramName);
            }
#else
            ArgumentException.ThrowIfNullOrEmpty(argument, paramName);
#endif
        }

        /// <summary>
        /// (Compatibility Extension) Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="argument"/> is greater than or equal to <paramref name="value"/>.
        /// </summary>
        /// <param name="argument">The argument value to check.</param>
        /// <param name="value">The value to compare against.</param>
        /// <param name="paramName">The name of the parameter being checked.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfGreaterThanOrEqual(int argument, int value, string? paramName = null)
        {
#if USE_COMPATIBILITY_EXTENSIONS
            if (argument >= value)
            {
                throw new ArgumentOutOfRangeException(paramName, $"Argument must be greater than {value}.");
            }
#else
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(argument, value, paramName);
#endif
        }

        /// <summary>
        /// (Compatibility Extension) Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="argument"/> is less than or equal to <paramref name="value"/>.
        /// </summary>
        /// <param name="argument">The argument value to check.</param>
        /// <param name="value">The value to compare against.</param>
        /// <param name="paramName">The name of the parameter being checked.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfLessThanOrEqual(int argument, int value, string? paramName = null)
        {
#if USE_COMPATIBILITY_EXTENSIONS
            if (argument <= value)
            {
                throw new ArgumentOutOfRangeException(paramName, $"Argument must be less than {value}.");
            }
#else
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(argument, value, paramName);
#endif
        }

        /// <summary>
        /// (Compatibility Extension) Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="argument"/> is greater than <paramref name="value"/>.
        /// </summary>
        /// <param name="argument">The argument value to check.</param>
        /// <param name="value">The value to compare against.</param>
        /// <param name="paramName">The name of the parameter being checked.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfGreaterThan(int argument, int value, string? paramName = null)
        {
#if USE_COMPATIBILITY_EXTENSIONS
            if (argument > value)
            {
                throw new ArgumentOutOfRangeException(paramName, $"Argument must be greater than or equal to {value}.");
            }
#else
            ArgumentOutOfRangeException.ThrowIfGreaterThan(argument, value, paramName);
#endif
        }

        /// <summary>
        /// (Compatibility Extension) Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="argument"/> is less than <paramref name="value"/>.
        /// </summary>
        /// <param name="argument">The argument value to check.</param>
        /// <param name="value">The value to compare against.</param>
        /// <param name="paramName">The name of the parameter being checked.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfLessThan(int argument, int value, string? paramName = null)
        {
#if USE_COMPATIBILITY_EXTENSIONS
            if (argument < value)
            {
                throw new ArgumentOutOfRangeException(paramName, $"Argument must be less than or equal to {value}.");
            }
#else
            ArgumentOutOfRangeException.ThrowIfLessThan(argument, value, paramName);
#endif
        }

        /// <summary>
        /// (Compatibility Extension) Throws an <see cref="ObjectDisposedException"/> if <paramref name="isDisposed"/> is true.
        /// </summary>
        /// <param name="isDisposed">Whether the object is disposed.</param>
        /// <param name="obj">The object that is disposed.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfDisposed(bool isDisposed, object? obj)
        {
#if USE_COMPATIBILITY_EXTENSIONS
            if (isDisposed)
            {
                throw new ObjectDisposedException(obj?.GetType().FullName);
            }
#else
            ObjectDisposedException.ThrowIf(isDisposed, obj);
#endif
        }
    }
}
