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

using System.Runtime.CompilerServices;
using Nomad.Core.Compatibility.Guards;
using Nomad.Core.Exceptions;
using Nomad.Core.Logger;

namespace Nomad.Logger.Globals
{
    /// <summary>
    ///
    /// </summary>
    public static class Logging
    {
        /// <summary>
        ///
        /// </summary>
        public static ILoggerService Instance => _instance ?? throw new SubsystemNotInitializedException();
        private static ILoggerService? _instance;

        /// <summary>
        ///
        /// </summary>
        /// <param name="instance"></param>
        internal static void Initialize(ILoggerService instance)
        {
            ArgumentGuard.ThrowIfNull(instance);
            _instance = instance;
        }

        /// <summary>
        /// Prints a log message with a newline.
        /// </summary>
        /// <remarks>
        /// Prints a standard log message to all the available sinks.
        /// </remarks>
        /// <param name="message">The message to print with a newline.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PrintLine(string message)
        {
            Instance.PrintLine(message);
        }

        /// <summary>
        /// Prints a debug log message.
        /// </summary>
        /// <remarks>
        /// Prints a debug log message to all the available sinks.
        /// </remarks>
        /// <param name="message">The message to print.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PrintDebug(string message)
        {
            Instance.PrintDebug(message);
        }

        /// <summary>
        /// Prints a warning log message.
        /// </summary>
        /// <remarks>
        /// Prints a warning log message to all the available sinks.
        /// </remarks>
        /// <param name="message">The message to print.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PrintWarning(string message)
        {
            Instance.PrintWarning(message);
        }

        /// <summary>
        /// Prints an error log message.
        /// </summary>
        /// <remarks>
        /// Prints an error log message to all the available sinks.
        /// </remarks>
        /// <param name="message">The message to print.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PrintError(string message)
        {
            Instance.PrintError(message);
        }

        /// <summary>
        /// Clears all logged messages (in all categories, clears all sinks). This will reset all messages. This action in irreversible.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Clear()
        {
            Instance.Clear();
        }

        /// <summary>
        /// Adds a sink to the logger.
        /// </summary>
        /// <remarks>
        /// A sink is a dedicated output abstraction, that prints messages to IO outputs such as stdout, a logging file, or the in-game console. You can setup your own sink if you want to route log messages elswhere.
        /// </remarks>
        /// <param name="sink">The sink to add.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddSink(ILoggerSink sink)
        {
            Instance.AddSink(sink);
        }

        /// <summary>
        /// Creates a new logging category.
        /// </summary>
        /// <remarks>
        /// A logging category is a denotation of where a message is coming from for easier debugging and tracing. You can also filter categories by enabling or disabling them as well as by <see cref="LogLevel"/>.
        /// </remarks>
        /// <param name="name">The name of the category.</param>
        /// <param name="level">The log level of the category.</param>
        /// <param name="enabled">Whether the category is enabled. If <b>false</b>, any log messages sent with this category as a parameter will not send any messages to the sinks.</param>
        /// <returns>The created logging category.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ILoggerCategory CreateCategory(string name, LogLevel level, bool enabled)
        {
            return Instance.CreateCategory(name, level, enabled);
        }
    }
}
