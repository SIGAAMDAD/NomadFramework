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

namespace Nomad.Input.Events
{
    /// <summary>
    /// 
    /// </summary>
    [Event(
        name: nameof(BindCollisionEventArgs),
        nameSpace: "Nomad.Input.Events"
    )]
    public readonly partial struct BindCollisionEventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public int OriginalBindId => _originalBindId;
        private readonly int _originalBindId;

        /// <summary>
        /// 
        /// </summary>
        public int IntrudingBindId => _intrudingBindId;
        private readonly int _intrudingBindId;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="originalBindId"></param>
        /// <param name="intrudingBindId"></param>
        public BindCollisionEventArgs(int originalBindId, int intrudingBindId)
        {
            _originalBindId = originalBindId;
            _intrudingBindId = intrudingBindId;
        }
    }
}