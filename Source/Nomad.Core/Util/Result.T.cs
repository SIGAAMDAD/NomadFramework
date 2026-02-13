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
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public record Result<T>
    {
        /// <summary>
        /// 
        /// </summary>
        public bool IsSuccess { get; init; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsFailure => !IsSuccess || Value == null;

        /// <summary>
        /// 
        /// </summary>
        public T? Value { get; init; }

        /// <summary>
        /// 
        /// </summary>
        public IError? Error { get; init; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        protected Result(T value)
        {
            IsSuccess = true;
            Value = value;
            Error = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="error"></param>
        protected Result(IError error)
        {
            IsSuccess = false;
            Value = default;
            Error = error;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Result<T> Success(T value)
        {
            return new Result<T>(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
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
