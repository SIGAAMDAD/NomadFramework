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

namespace Nomad.Core.Logger
{
    /// <summary>
    /// Represents a logging category.
    /// </summary>
    /// <remarks>
    /// Categories show which part of a module/which system the log message is coming from to make tracing messages much easier instead of guesswork.
    /// </remarks>
    public interface ILoggerCategory : IDisposable
    {
        /// <summary>
        /// The name of the category.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The log level of the category.
        /// </summary>
        LogLevel Level { get; }

        /// <summary>
        /// Whether the category is enabled. <b>False</b> means all messages sent with this category as a parameter will simply be ignored.
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// Queues a log message into the category.
        /// </summary>
        /// <param name="message">The message to queue.</param>
        void QueueMessage(in string message);

        /// <summary>
        /// Adds a sink to the category.
        /// </summary>
        /// <remarks>
        /// By default, all sinks are added to a category, however, if you wish to control where a category can actually output to, utilize this method here to do so.
        /// </remarks>
        /// <param name="sink">The sink to add.</param>
        void AddSink(in ILoggerSink sink);

        /// <summary>
        /// Removes a sink from the category.
        /// </summary>
        /// <remarks>
        /// By default, all sinks are added to a category, however, if you wish to control where a category can actually output to, utilize this method here to do so.
        /// </remarks>
        /// <param name="sink">The sink to remove.</param>
        void RemoveSink(in ILoggerSink sink);
    }
}
