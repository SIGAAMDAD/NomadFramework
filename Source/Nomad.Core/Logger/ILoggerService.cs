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
using Nomad.Core.CVars;

namespace Nomad.Core.Logger
{
    /// <summary>
    /// The generic logger interface. Controls sink and category management.
    /// </summary>
    public interface ILoggerService : IDisposable
    {
        /// <summary>
        /// Prints a log message.
        /// </summary>
        /// <remarks>
        /// Prints a standard log message to all the available sinks.
        /// </remarks>
        /// <param name="message">The message to print.</param>
        void Print(in string message);

        /// <summary>
        /// Prints a log message with a newline.
        /// </summary>
        /// <remarks>
        /// Prints a standard log message to all the available sinks.
        /// </remarks>
        /// <param name="message">The message to print with a newline.</param>
        void PrintLine(in string message);

        /// <summary>
        /// Prints a debug log message.
        /// </summary>
        /// <remarks>
        /// Prints a debug log message to all the available sinks.
        /// </remarks>
        /// <param name="message">The message to print.</param>
        void PrintDebug(in string message);

        /// <summary>
        /// Prints a warning log message.
        /// </summary>
        /// <remarks>
        /// Prints a warning log message to all the available sinks.
        /// </remarks>
        /// <param name="message">The message to print.</param>
        void PrintWarning(in string message);

        /// <summary>
        /// Prints an error log message.
        /// </summary>
        /// <remarks>
        /// Prints an error log message to all the available sinks.
        /// </remarks>
        /// <param name="message">The message to print.</param>
        void PrintError(in string message);

        /// <summary>
        /// Prints a log message to a specific category.
        /// </summary>
        /// <remarks>
        /// Prints a standard log message to all the available sinks with a marker of the category in the actual message.
        /// </remarks>
        /// <param name="category">The category to print the message to.</param>
        /// <param name="message">The message to print.</param>
        void Print(in ILoggerCategory category, in string message);

        /// <summary>
        /// Prints a log message with a newline to a specific category.
        /// </summary>
        /// <remarks>
        /// Prints a standard log message to all the available sinks with a marker of the category in the actual message.
        /// </remarks>
        /// <param name="category">The category to print the message to.</param>
        /// <param name="message">The message to print with a newline.</param>
        void PrintLine(in ILoggerCategory category, in string message);

        /// <summary>
        /// Prints a debug log message to a specific category.
        /// </summary>
        /// <remarks>
        /// Prints a debug log message to all the available sinks with a marker of the category in the actual message.
        /// </remarks>
        /// <param name="category">The category to print the message to.</param>
        /// <param name="message">The message to print.</param>
        void PrintDebug(in ILoggerCategory category, in string message);

        /// <summary>
        /// Prints a warning log message to a specific category.
        /// </summary>
        /// <remarks>
        /// Prints a warning log message to all the available sinks with a marker of the category in the actual message.
        /// </remarks>
        /// <param name="category">The category to print the message to.</param>
        /// <param name="message">The message to print.</param>
        void PrintWarning(in ILoggerCategory category, in string message);

        /// <summary>
        /// Prints an error log message to a specific category.
        /// </summary>
        /// <remarks>
        /// Prints an error log message to all the available sinks with a marker of the category in the actual message.
        /// </remarks>
        /// <param name="category">The category to print the message to.</param>
        /// <param name="message">The message to print.</param>
        void PrintError(in ILoggerCategory category, in string message);

        /// <summary>
        /// Clears all logged messages (in all categories, clears all sinks). This will reset all messages. This action in irreversible.
        /// </summary>
        void Clear();

        /// <summary>
        /// Initializes the logger configuration.
        /// </summary>
        /// <param name="cvarSystem">The CVar system.</param>
        void InitConfig(ICVarSystemService cvarSystem);

        /// <summary>
        /// Adds a sink to the logger.
        /// </summary>
        /// <remarks>
        /// A sink is a dedicated output abstraction, that prints messages to IO outputs such as stdout, a logging file, or the in-game console. You can setup your own sink if you want to route log messages elswhere.
        /// </remarks>
        /// <param name="sink">The sink to add.</param>
        void AddSink(ILoggerSink sink);

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
        ILoggerCategory CreateCategory(in string name, LogLevel level, bool enabled);
    }
}
