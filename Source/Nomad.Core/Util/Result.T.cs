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
    /// Represents the result of an operation that either returns a value on success or fails with an error.
    /// </summary>
    /// <typeparam name="T">The type of the value returned on success.</typeparam>
    /// <remarks>
    /// This generic record extends the error handling pattern to include a success value.
    /// Use <see cref="Success"/> to create a successful result with a value and <see cref="Failure"/> to create a failed result with an <see cref="IError"/>.
    /// For operations that don't return a value, use <see cref="Result"/> instead.
    /// </remarks>
    public record Result<T>
    {
        /// <summary>
        /// Gets a value indicating whether the operation succeeded.
        /// </summary>
        public bool IsSuccess { get; init; }

        /// <summary>
        /// Gets a value indicating whether the operation failed. Considers failure if the operation did not succeed or the value is null.
        /// </summary>
        public bool IsFailure => !IsSuccess || Value == null;

        /// <summary>
        /// Gets the value returned by the operation on success.
        /// </summary>
        public T? Value { get; init; }

        /// <summary>
        /// Gets the error associated with this result, if the operation failed.
        /// </summary>
        public IError? Error { get; init; }

        /// <summary>
        /// Initializes a new successful result with the specified value.
        /// </summary>
        /// <param name="value">The value returned by the successful operation.</param>
        protected Result(T value)
        {
            IsSuccess = true;
            Value = value;
            Error = null;
        }

        /// <summary>
        /// Initializes a new failed result with the specified error.
        /// </summary>
        /// <param name="error">The error that caused the failure.</param>
        protected Result(IError error)
        {
            IsSuccess = false;
            Value = default;
            Error = error;
        }

        /// <summary>
        /// Creates a successful result with the specified value.
        /// </summary>
        /// <param name="value">The value to return on success.</param>
        /// <returns>A <see cref="Result{T}"/> indicating success with the specified value.</returns>
        public static Result<T> Success(T value)
        {
            return new Result<T>(value);
        }

        /// <summary>
        /// Creates a failed result with the specified error.
        /// </summary>
        /// <param name="error">The error that caused the failure.</param>
        /// <returns>A <see cref="Result{T}"/> indicating failure.</returns>
        public static Result<T> Failure(IError error)
        {
            return new Result<T>(error);
        }

        /// <summary>
        /// Deconstructs the result into its components for pattern matching.
        /// </summary>
        /// <param name="isSuccess">Whether the operation succeeded.</param>
        /// <param name="value">The value returned on success.</param>
        /// <param name="error">The error, if the operation failed.</param>
        public void Deconstruct(out bool isSuccess, out T? value, out IError? error)
        {
            isSuccess = IsSuccess;
            value = Value;
            error = Error;
        }
    }
}
