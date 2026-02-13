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
using System.Threading;

namespace Nomad.Core.Compatibility.Guards
{
    /// <summary>
    ///
    /// </summary>
    public static class ThreadGuard
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="message"></param>
        /// <exception cref="InvalidOperationException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfNotMainThread(string? message = null)
        {
            if (Environment.CurrentManagedThreadId != 1)
            {
                throw new InvalidOperationException(message ?? "Operation must be performed on the main thread.");
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="expectedThreadId"></param>
        /// <param name="message"></param>
        /// <exception cref="InvalidOperationException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfWrongThread(int expectedThreadId, string? message = null)
        {
            if (Environment.CurrentManagedThreadId != expectedThreadId)
            {
                throw new InvalidOperationException(message ?? "Operation executed on the wrong thread.");
            }
        }
    }
}
