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

namespace Nomad.Core.CVars
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class CVarBinding<T> : IDisposable
    {
        private readonly IDisposable _subscription;

        public T Value => _value;
        private T _value;

        public event Action<T> Changed;

        public CVarBinding(ICVar<T> cvar)
        {
            _value = cvar.Value;

            _subscription = cvar.ValueChanged.Subscribe(OnValueChanged);
        }

        public void Dispose()
        {
            _subscription?.Dispose();
        }

        private void OnValueChanged(in CVarValueChangedEventArgs<T> args)
        {
            _value = args.NewValue;
            Changed?.Invoke(_value);
        }
    }
}
