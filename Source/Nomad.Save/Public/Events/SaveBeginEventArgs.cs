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

using Nomad.Core.Events;
using Nomad.Save.Interfaces;
using Nomad.Save.Services;

namespace Nomad.Save.Events
{
    /// <summary>
    /// Event that triggers when <see cref="ISaveDataProvider.Save"/> is called.
    /// </summary>
    [Event(
        name: nameof(SaveBeginEventArgs),
        nameSpace: "Nomad.Save"
    )]
    public readonly partial struct SaveBeginEventArgs
    {
        /// <summary>
        /// The writer service to utilize.
        /// </summary>
        public ISaveWriterService Writer => _writer;
        private readonly ISaveWriterService _writer;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        public SaveBeginEventArgs(ISaveWriterService writer)
        {
            _writer = writer;
        }
    }
}
