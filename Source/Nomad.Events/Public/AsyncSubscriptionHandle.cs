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
using System.Threading;
using Nomad.Core.Events;
using Nomad.Events.Private.SubscriptionSets;

namespace Nomad.Events
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class AsyncSubscriptionHandle<TArgs> : ISubscriptionHandle
        where TArgs : struct
    {
        private readonly object _owner;
        private readonly ISubscriptionSet<TArgs> _set;
        private readonly AsyncEventCallback<TArgs> _callback;

        private int _disposed = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="set"></param>
        /// <param name="callback"></param>
        internal AsyncSubscriptionHandle(object owner, ISubscriptionSet<TArgs> set, AsyncEventCallback<TArgs> callback)
        {
            _owner = owner;
            _set = set;
            _callback = callback;

            _set.AddSubscriptionAsync(_owner, _callback);
        }

        /// <summary>
        /// 
        /// </summary>
        ~AsyncSubscriptionHandle()
        {
            Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 1)
            {
                return;
            }
            _set.RemoveSubscriptionAsync(_owner, _callback);
        }
    }
}