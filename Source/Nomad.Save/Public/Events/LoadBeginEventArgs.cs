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

using Nomad.Save.Interfaces;

namespace Nomad.Save.Events
{
    /// <summary>
    /// 
    /// </summary>
    public readonly struct LoadBeginEventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public readonly ISaveReaderService Reader;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        public LoadBeginEventArgs(ISaveReaderService reader)
        {
            Reader = reader;
        }
    }
}
