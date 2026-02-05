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
    public readonly struct Result
    {
        public bool IsSuccess { get; init; }
        public IError? Error { get; init; }
        public bool IsFailure => !IsSuccess;

        private Result(IError error)
        {
            IsSuccess = false;
            Error = error;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Result Success()
        {
            return new Result();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public static Result Failure(IError error)
        {
            return new Result(error);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="isSuccess"></param>
        /// <param name="error"></param>
        public void Deconstruct(out bool isSuccess, out IError? error)
        {
            isSuccess = IsSuccess;
            error = Error;
        }
    };
};
