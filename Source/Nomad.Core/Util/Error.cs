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
    /// Represents an internal error with a descriptive message and an error type classification.
    /// </summary>
    /// <remarks>
    /// This record implements the <see cref="IError"/> interface and provides a standard way to create and track errors
    /// within the Nomad framework. Use the <see cref="Create"/> factory method to instantiate errors.
    /// </remarks>
    /// <param name="Message">A descriptive message explaining the error.</param>
    /// <param name="Type">The classification of the error type.</param>
    public record InternalError(
        string Message,
        ErrorType Type = ErrorType.Validation
    ) : IError
    {
        /// <summary>
        /// Creates a new instance of <see cref="InternalError"/> with the specified message and type.
        /// </summary>
        /// <param name="message">A descriptive message explaining the error.</param>
        /// <param name="type">The classification of the error type. Defaults to <see cref="ErrorType.Validation"/>.</param>
        /// <returns>A new <see cref="InternalError"/> instance.</returns>
        public static InternalError Create(string message, ErrorType type = ErrorType.Validation)
        {
            return new InternalError(message, type);
        }
    }
}
