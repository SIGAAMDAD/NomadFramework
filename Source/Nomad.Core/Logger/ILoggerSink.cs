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
    /// Represents a logging sink.
    /// </summary>
    public interface ILoggerSink : IDisposable
    {
        /// <summary>
        /// Adds a log message. Queues it into the sink for output upon flush.
        /// </summary>
        /// <param name="message">The message to add to the sink's buffer.</param>
        void Print(in string message);

        /// <summary>
        /// Clears the sink.
        /// </summary>
        /// <remarks>
        /// This (should) remove all existing messages/char data from the sink.
        /// </remarks>
        void Clear();

        /// <summary>
        /// Flushes the sink.
        /// </summary>
        /// <remarks>
        /// Will automatically dump all the currently buffered messages held in the sink to its output.
        /// </remarks>
        void Flush();
    }
}
