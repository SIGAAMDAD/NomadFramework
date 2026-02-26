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
using Nomad.Core.Compatibility.Guards;
using Nomad.Core.Events;

namespace Nomad.Events
{
    /// <summary>
    /// An automated disposable subscription.
    /// </summary>
    public sealed class DisposableSubscription<TArgs> : IDisposable
        where TArgs : struct
    {
        private IGameEvent<TArgs>? _event;
        private EventCallback<TArgs>? _callback;

        private bool _isDisposed = false;

        /// <summary>
        ///
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="callback"></param>
        public DisposableSubscription(IGameEvent<TArgs> handler, EventCallback<TArgs> callback)
        {
            ArgumentGuard.ThrowIfNull(handler);

            handler.Subscribe(this, callback);
            _event = handler;
            _callback = callback;
        }

        /*
        ===============
        Dispose
        ===============
        */
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (!_isDisposed)
            {
                _event?.Unsubscribe(this, _callback);
                _event = null;
                _callback = null;
            }
            GC.SuppressFinalize(this);
            _isDisposed = true;
        }
    }
}
