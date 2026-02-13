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
    /// <param name="Message"></param>
    /// <param name="Type"></param>
    public record Error(
        string Message,
        ErrorType Type = ErrorType.Validation
    ) : IError
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Error Create(string message, ErrorType type = ErrorType.Validation)
        {
            return new Error(message, type);
        }
    }
}
