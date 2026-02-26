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

using Nomad.Core.Exceptions;

namespace Nomad.Save.Exceptions
{
    /// <summary>
    ///
    /// </summary>
    public abstract class SaveFileCorruptException : NomadError
    {
        /// <summary>
        /// 
        /// </summary>
        public long FileOffset => _fileOffset;
        private readonly long _fileOffset;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileOffset"></param>
        /// <param name="message"></param>
        public SaveFileCorruptException(long fileOffset, string message)
            : base(message)
        {
            _fileOffset = fileOffset;
        }
    }
}
