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
    /// Represents an error that occurred during an operation.
    /// </summary>
    /// <remarks>
    /// Implementing types should provide a descriptive message and classify the error using <see cref="ErrorType"/>.
    /// This interface is used in conjunction with <see cref="Result"/> and <see cref="Result{T}"/> for error handling.
    /// </remarks>
    public interface IError
    {
        /// <summary>
        /// Gets a descriptive message explaining the error.
        /// </summary>
        string Message { get; }

        /// <summary>
        /// Gets the classification or type of the error.
        /// </summary>
        ErrorType Type { get; }
    }
}
