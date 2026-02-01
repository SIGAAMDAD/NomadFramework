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
using System.Collections.Generic;

namespace Nomad.Core.Abstractions
{
    public interface IReadOnlyRepository<TEntity, TId>
        where TEntity : IEntity<TId>
        where TId : IEquatable<TId>
    {
        TEntity? Find(TId id);
        IReadOnlyList<TEntity> GetAll();

        TResult? Project<TResult>(TId id, Func<TEntity, TResult> projector);
        IReadOnlyList<TResult> ProjectAll<TResult>(Func<TEntity, TResult> projector);
    }
}
