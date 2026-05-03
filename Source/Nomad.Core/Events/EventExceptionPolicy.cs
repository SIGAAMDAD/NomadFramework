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

namespace Nomad.Core.Events
{
    /// <summary>
    /// 
    /// </summary>
    public enum EventExceptionPolicy
    {
        /// <summary>
        /// First subscriber exception immediately aborts publishing.
        /// Best for command-style events where failure means the operation is invalid.
        /// </summary>
        Propagate,

        /// <summary>
        /// Run every subscriber, collect all failures, then throw one aggregate exception.
        /// Best default for editor/debug/framework-level correctness.
        /// </summary>
        AggregateAfterDispatch,

        /// <summary>
        /// Run every subscriber, log/report failures, never throw from Publish.
        /// Best for hot-path runtime events like input, UI notifications, telemetry, audio state changes.
        /// </summary>
        ReportAndContinue
    }
}
