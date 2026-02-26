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

using System;
using Nomad.Core.Exceptions;
using Nomad.Core.Util;

namespace Nomad.Save.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class InvalidFieldTypeException : NomadError
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="attemptedType"></param>
        public InvalidFieldTypeException(Type attemptedType)
            : base($"Cannot add a field of type '{attemptedType.Name}', it must be a valid AnyType")
        {
        }
    }
}