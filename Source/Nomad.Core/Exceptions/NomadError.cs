/*
===========================================================================
The Nomad MPL Source Code
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

namespace Nomad.Core.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    public class NomadError : Exception
    {
        /// <summary>
        /// The system where the exception was thrown.
        /// </summary>
        public string Module => _module;
        private readonly string _module;

        /// <summary>
        /// The time at which the exception was thrown.
        /// </summary>
        public DateTime TimeStap => _timeStamp;
        private readonly DateTime _timeStamp;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="module"></param>
        public NomadError(string message, string module)
            : base(message)
        {
            _module = module;
            _timeStamp = DateTime.Now;
        }
    }
}