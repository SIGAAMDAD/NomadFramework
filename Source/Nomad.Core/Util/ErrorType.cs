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

namespace Nomad.Core.Util
{
    /// <summary>
    /// Specifies the category or type of an error that occurred during operation.
    /// </summary>
    /// <remarks>
    /// Error types follow common HTTP status code conventions to categorize errors into logical groups.
    /// This classification helps determine how errors should be handled and what recovery strategies are appropriate.
    /// </remarks>
    public enum ErrorType : int
    {
        /// <summary>
        /// Validation error: Input data failed validation checks.
        /// </summary>
        Validation,

        /// <summary>
        /// Not found error: The requested resource could not be found.
        /// </summary>
        NotFound,

        /// <summary>
        /// Conflict error: The operation conflicts with existing state.
        /// </summary>
        Conflict,

        /// <summary>
        /// Unauthorized error: The operation requires authentication.
        /// </summary>
        Unauthorized,

        /// <summary>
        /// Forbidden error: The operation is not permitted for the current context.
        /// </summary>
        Forbidden,
        BusinessRule,

        /// <summary>
        /// 
        /// </summary>
        Infrastructure,

        /// <summary>
        /// 
        /// </summary>
        LoadFailed,

        /// <summary>
        /// 
        /// </summary>
        Count
    }
}
