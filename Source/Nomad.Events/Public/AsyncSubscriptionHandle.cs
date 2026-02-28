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
        private readonly ISubscriptionSet<TArgs> _set;
        private readonly AsyncEventCallback<TArgs> _callback;

        /// <summary>
        /// <c>True</c> if the subscription is alive, <c>false</c> if not.
        /// </summary>
        public bool IsDisposed => _isDisposed;
        private bool _isDisposed = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="set"></param>
        /// <param name="callback"></param>
        internal AsyncSubscriptionHandle(ISubscriptionSet<TArgs> set, AsyncEventCallback<TArgs> callback)
        {
            _set = set;
            _callback = callback;

            _set.AddSubscriptionAsync(_callback);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (!_isDisposed)
            {
                _set.RemoveSubscriptionAsync(_callback);
            }
            GC.SuppressFinalize(this);
            _isDisposed = true;
        }
    }
}
