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

namespace Nomad.Save.Exceptions
{
    /// <summary>
    ///
    /// </summary>
    public sealed class SectionCorruptException : SaveFileCorruptException
    {
        /// <summary>
        /// 
        /// </summary>
        public string SectionName => _sectionName;
        private readonly string _sectionName;

        /// <summary>
        /// 
        /// </summary>
        public int SectionIndex => _sectionIndex;
        private readonly int _sectionIndex;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="sectionIndex"></param>
        /// <param name="fileOffset"></param>
        /// <param name="message"></param>
        public SectionCorruptException(string sectionName, int sectionIndex, int fileOffset, string message = "")
            : base(fileOffset, message)
        {
            _sectionName = sectionName;
            _sectionIndex = sectionIndex;
        }
    }
}
