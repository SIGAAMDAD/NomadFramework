/*
===========================================================================
The Nomad Framework
Copyright (C) 2025 Noah Van Til

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
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public record Result<T>
    {
        public bool IsSuccess { get; init; }
        public bool IsFailure => !IsSuccess || Value == null;
        public T? Value { get; init; }
        public IError? Error { get; init; }

        protected Result(T value)
        {
            IsSuccess = true;
            Value = value;
            Error = null;
        }
        protected Result(IError error)
        {
            IsSuccess = false;
            Value = default;
            Error = error;
        }

        public static Result<T> Success(T value)
        {
            return new Result<T>(value);
        }
        public static Result<T> Failure(IError error)
        {
            return new Result<T>(error);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="isSuccess"></param>
        /// <param name="value"></param>
        /// <param name="error"></param>
        public void Deconstruct(out bool isSuccess, out T? value, out IError? error)
        {
            isSuccess = IsSuccess;
            value = Value;
            error = Error;
        }
    }
}
