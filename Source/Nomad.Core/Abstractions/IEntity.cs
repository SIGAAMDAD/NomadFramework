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

namespace Nomad.Core.Abstractions
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    public interface IEntity<TId> : IEquatable<IEntity<TId>>
        where TId : IEquatable<TId>
    {
        /// <summary>
        /// 
        /// </summary>
        TId Id { get; }

        /// <summary>
        /// 
        /// </summary>
        DateTime CreatedAt { get; }

        /// <summary>
        /// 
        /// </summary>
        DateTime? ModifiedAt { get; }

        /// <summary>
        /// 
        /// </summary>
        int Version { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TSelf"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public interface IEntity<TSelf, TId> : IEquatable<TSelf>
        where TSelf : IEntity<TSelf, TId>
        where TId : IEquatable<TId>
    {
        /// <summary>
        /// 
        /// </summary>
        TId Id { get; }

        /// <summary>
        /// 
        /// </summary>
        DateTime CreatedAt { get; }
    }
}
