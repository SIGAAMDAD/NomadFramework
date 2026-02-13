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

namespace Nomad.Core.Memory
{
    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    public interface IObjectPool<TObject> : IDisposable
        where TObject : new()
    {
        /// <summary>
        /// The total amount of objects allocated.
        /// </summary>
        int TotalCount { get; }

        /// <summary>
        /// The total amount of objects currently in use.
        /// </summary>
        int ActiveObjectCount { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        TObject Rent();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        void Return(TObject value);
    }
}
