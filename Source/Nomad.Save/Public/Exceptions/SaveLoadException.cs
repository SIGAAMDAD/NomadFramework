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
    public sealed class SaveLoadException : NomadError
    {
        /// <summary>
        /// 
        /// </summary>
        public string FileId => _fileId;
        private readonly string _fileId;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="saveFileId"></param>
        public SaveLoadException(string saveFileId)
            : base($"")
        {
            _fileId = saveFileId;
        }
    }
}
