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

namespace Nomad.Save.Exceptions
{
    /// <summary>
    ///
    /// </summary>
    /// @module Nomad.Save
    public sealed class FailedFieldLoadException : NomadError
    {
        public readonly string? FieldName;
        public readonly Exception Error;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="exception"></param>
        public FailedFieldLoadException(string fieldName, Exception exception)
            : base($"Failed to load save field {fieldName}, exception {exception}")
        {
            FieldName = fieldName;
            Error = exception;
        }
    }
}
