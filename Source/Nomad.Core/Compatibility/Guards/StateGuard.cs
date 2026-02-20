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

namespace Nomad.Core.Compatibility.Guards
{
    /// <summary>
    ///
    /// </summary>
    public static class StateGuard
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="isDisposed"></param>
        /// <param name="obj"></param>
        /// <exception cref="ObjectDisposedException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfDisposed(bool isDisposed, object obj)
        {
#if NETSTANDARD2_1 || !NET6_0_OR_GREATER
            if (isDisposed)
            {
                throw new ObjectDisposedException(obj?.GetType().FullName);
            }
#else
            ObjectDisposedException.ThrowIf(isDisposed, obj);
#endif
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="message"></param>
        /// <exception cref="InvalidOperationException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfTrue(bool condition, string? message = null)
        {
            if (condition)
            {
                throw new InvalidOperationException(message ?? "Condition must be false.");
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="message"></param>
        /// <exception cref="InvalidOperationException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfFalse(bool condition, string? message = null)
        {
            if (!condition)
            {
                throw new InvalidOperationException(message ?? "Condition must be true.");
            }
        }
    }
}
