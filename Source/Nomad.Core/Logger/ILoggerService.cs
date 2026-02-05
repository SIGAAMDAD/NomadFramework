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
using Nomad.Core.ServiceRegistry.Interfaces;

namespace Nomad.Core.Logger
{
    /// <summary>
    /// The generic logger interface.
    /// </summary>
    public interface ILoggerService : IDisposable
    {
        /// <summary>
        /// Prints a log message.
        /// </summary>
        /// <param name="message">The message to print.</param>
        void Print(in string message);

        /// <summary>
        /// Prints a log message with a newline.
        /// </summary>
        /// <param name="message">The message to print with a newline.</param>
        void PrintLine(in string message);

        /// <summary>
        /// Prints a debug log message.
        /// </summary>
        /// <param name="message">The message to print.</param>
        void PrintDebug(in string message);

        /// <summary>
        /// Prints a warning log message.
        /// </summary>
        /// <param name="message">The message to print.</param>
        void PrintWarning(in string message);

        /// <summary>
        /// Prints an error log message.
        /// </summary>
        /// <param name="message">The message to print.</param>
        void PrintError(in string message);

        /// <summary>
        /// Prints a log message to a specific category.
        /// </summary>
        /// <param name="category">The category to print the message to.</param>
        /// <param name="message">The message to print.</param>
        void Print(in ILoggerCategory category, in string message);

        /// <summary>
        /// Prints a log message with a newline to a specific category.
        /// </summary>
        /// <param name="category">The category to print the message to.</param>
        /// <param name="message">The message to print with a newline.</param>
        void PrintLine(in ILoggerCategory category, in string message);

        /// <summary>
        /// Prints a debug log message to a specific category.
        /// </summary>
        /// <param name="category">The category to print the message to.</param>
        /// <param name="message">The message to print.</param>
        void PrintDebug(in ILoggerCategory category, in string message);

        /// <summary>
        /// Prints a warning log message to a specific category.
        /// </summary>
        /// <param name="category">The category to print the message to.</param>
        /// <param name="message">The message to print.</param>
        void PrintWarning(in ILoggerCategory category, in string message);

        /// <summary>
        /// Prints an error log message to a specific category.
        /// </summary>
        /// <param name="category">The category to print the message to.</param>
        /// <param name="message">The message to print.</param>
        void PrintError(in ILoggerCategory category, in string message);

        /// <summary>
        /// Clears all logged messages (in all categories, clears all sinks).
        /// </summary>
        void Clear();

        /// <summary>
        /// Initializes the logger configuration.
        /// </summary>
        /// <param name="locator">The service locator to use for logger configuration.</param>
        void InitConfig(IServiceLocator locator);

        /// <summary>
        /// Adds a sink to the logger.
        /// </summary>
        /// <param name="sink">The sink to add.</param>
        void AddSink(ILoggerSink sink);

        /// <summary>
        /// Creates a new logging category.
        /// </summary>
        /// <param name="name">The name of the category.</param>
        /// <param name="level">The log level of the category.</param>
        /// <param name="enabled">Whether the category is enabled.</param>
        /// <returns>The created logging category.</returns>
        ILoggerCategory CreateCategory(in string name, LogLevel level, bool enabled);
    }
}
