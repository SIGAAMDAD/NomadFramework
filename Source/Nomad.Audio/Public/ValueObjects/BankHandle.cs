/*
===========================================================================
The Nomad Framework
Copyright (C) 2025 Noah Van Til

This Source Code Form is subject to the terms of the Mozilla Public
License, v2. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.

This software is provided "as is", without warranty of any kind,
express or implied, including but not limited to the warranties
of merchantability, fitness for a particular purpose and noninfringement.
===========================================================================
*/

using System.Runtime.CompilerServices;
using Nomad.Core.Abstractions;

namespace Nomad.Audio.ValueObjects
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="value"></param>
    public readonly struct BankHandle(uint value) : IValueObject<BankHandle>
    {
        private readonly uint _value = value;

        public static readonly BankHandle Invalid = new(0);

        public bool IsValid => (_value & 0x00FFFFFF) != 0;

        /// <summary>
        ///
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(BankHandle other) => other._value == _value;

        public static implicit operator uint(BankHandle handle) => handle._value;
    }
}
