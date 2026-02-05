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
    /// Base implementation of a logging sink.
    /// </summary>
    public abstract class SinkBase : ILoggerSink
    {
        /// <summary>
        /// Prints a log message.
        /// </summary>
        /// <param name="message">The message to print.</param>
        public abstract void Print(in string message);

        /// <summary>
        /// Clears the sink.
        /// </summary>
        public abstract void Clear();

        /// <summary>
        /// Flushes the sink.
        /// </summary>
        public abstract void Flush();

        /*
		===============
		Dispose
		===============
		*/
        /// <summary>
        /// Flushes the sink.
        /// </summary>
        public virtual void Dispose()
        {
            Flush();
            GC.SuppressFinalize(this);
        }
    }
}
