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

namespace Nomad.Core.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class StringNotInternedException : NomadException
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public StringNotInternedException(ulong id)
            : base($"String not interned with 64-bit unsigned value of '{id}'")
        {
        }
    }
}
