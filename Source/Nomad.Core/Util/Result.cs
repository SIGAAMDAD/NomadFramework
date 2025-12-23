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
    public readonly record struct Result
    {
        public bool IsSuccess { get; init; } = true;
        public bool IsFailure => !IsSuccess;
        public IError? Error { get; init; } = null;

        private Result(IError error)
        {
            IsSuccess = false;
            Error = error;
        }

        public static Result Success()
        {
            return new Result();
        }
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
