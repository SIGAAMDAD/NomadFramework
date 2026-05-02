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

namespace Nomad.ResourceCache.Events
{
    /// <summary>
    ///
    /// </summary>
    public readonly partial struct ResourceLoadProgressEventArgs<TId>
    {
        /// <summary>
        /// The resource's internal cache id.
        /// </summary>
        public readonly TId Id;

        /// <summary>
        /// How much we've loaded, can be 0.
        /// </summary>
        public readonly float Progress;

        /// <summary>
        /// The current load status.
        /// </summary>
        public readonly ResourceLoadState State;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="progress"></param>
        /// <param name="state"></param>
        public ResourceLoadProgressEventArgs(TId id, float progress, ResourceLoadState state)
        {
            Id = id;
            Progress = progress;
            State = state;
        }
    }
}
