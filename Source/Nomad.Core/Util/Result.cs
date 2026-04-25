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
    /// Represents the result of an operation that either succeeds or fails with an error.
    /// </summary>
    /// <remarks>
    /// This record is used for error handling without exceptions. Use <see cref="Success"/> to create a successful result
    /// and <see cref="Failure"/> to create a failed result with an <see cref="IError"/>.
    /// For operations that return a value on success, use <see cref="Result{T}"/> instead.
    /// </remarks>
    public record Result
    {
        /// <summary>
        /// Gets a value indicating whether the operation succeeded.
        /// </summary>
        public bool IsSuccess { get; init; }

        /// <summary>
        /// Gets the error associated with this result, if the operation failed.
        /// </summary>
        public IError? Error { get; init; }

        /// <summary>
        /// Gets a value indicating whether the operation failed.
        /// </summary>
        public bool IsFailure => !IsSuccess;

        /// <summary>
        /// Initializes a new failed result with the specified error.
        /// </summary>
        /// <param name="error">The error that caused the failure.</param>
        private Result(IError error)
        {
            IsSuccess = false;
            Error = error;
        }

        /// <summary>
        /// Initializes a new successful result.
        /// </summary>
        private Result()
        {
            IsSuccess = true;
            Error = null;
        }

        /// <summary>
        /// Creates a successful result.
        /// </summary>
        /// <returns>A <see cref="Result"/> indicating success.</returns>
        public static Result Success()
        {
            return new Result();
        }

        /// <summary>
        /// Creates a failed result with the specified error.
        /// </summary>
        /// <param name="error">The error that caused the failure.</param>
        /// <returns>A <see cref="Result"/> indicating failure.</returns>
        public static Result Failure(IError error)
        {
            return new Result(error);
        }

        /// <summary>
        /// Deconstructs the result into its components for pattern matching.
        /// </summary>
        /// <param name="isSuccess">Whether the operation succeeded.</param>
        /// <param name="error">The error, if the operation failed.</param>
        public void Deconstruct(out bool isSuccess, out IError? error)
        {
            isSuccess = IsSuccess;
            error = Error;
        }
    }
}
