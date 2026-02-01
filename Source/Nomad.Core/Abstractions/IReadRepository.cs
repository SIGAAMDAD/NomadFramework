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
    public interface IReadRepository<TEntity, TId>
        where TEntity : IDisposable
        where TId : IEquatable<TId>
    {
        bool TryGetById(TId id, out TEntity? entity);
        TEntity? GetById(TId id);
        IEnumerable<TEntity> GetByIds(ReadOnlySpan<TId> ids);
    }
}
