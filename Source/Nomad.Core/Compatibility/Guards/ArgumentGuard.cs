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
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Nomad.Core.Compatibility.Guards
{
    /// <summary>
    ///
    /// </summary>
    public static class ArgumentGuard
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <param name="paramName"></param>
        /// <exception cref="ArgumentNullException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfNull([NotNull] object? value, string? paramName = null)
        {
#if USE_COMPATIBILITY_EXTENSIONS || UNITY_EDITOR
            if (value is null)
            {
                throw new ArgumentNullException(paramName);
            }
#else
            ArgumentNullException.ThrowIfNull(value, paramName);
#endif
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <param name="paramName"></param>
        /// <exception cref="ArgumentException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfNullOrEmpty(string? value, string? paramName = null)
        {
#if USE_COMPATIBILITY_EXTENSIONS || UNITY_EDITOR
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Value cannot be null or empty.", paramName);
            }
#else
            ArgumentException.ThrowIfNullOrEmpty(value, paramName);
#endif
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <param name="paramName"></param>
        /// <exception cref="ArgumentException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfNullOrWhiteSpace(string? value, string? paramName = null)
        {
#if USE_COMPATIBILITY_EXTENSIONS || UNITY_EDITOR
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Value cannot be null, empty, or whitespace.", paramName);
            }
#else
            ArgumentException.ThrowIfNullOrWhiteSpace(value, paramName);
#endif
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="paramName"></param>
        /// <exception cref="ArgumentException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfDefault<T>(T value, string? paramName = null) where T : struct
        {
            if (value.Equals(default(T)))
            {
                throw new ArgumentException("Value cannot be the default value.", paramName);
            }
        }
    }
}
