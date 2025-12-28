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

using Nomad.Core.Abstractions;

namespace Nomad.Audio.ValueObjects
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="value"></param>
    public readonly record struct ChannelGroupHandle(uint value) : IValueObject<ChannelGroupHandle>
    {
        private readonly uint _value = value;

        public static readonly ChannelGroupHandle Invalid = new(0);

        public bool IsValid => (_value & 0x00FFFFFF) != 0;

        public static implicit operator uint(ChannelGroupHandle handle) => handle._value;
    }
}
