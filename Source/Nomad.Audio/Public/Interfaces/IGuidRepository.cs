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

using System;
using Nomad.Audio.ValueObjects;

namespace Nomad.Audio.Interfaces
{
    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TEventId"></typeparam>
    /// <typeparam name="TBankId"></typeparam>
    public interface IGuidRepository<TEventId, TBankId> : IDisposable
    {
        void AddEventId(string path, TEventId guid);
        void AddBankId(string path, TBankId guid);

        EventId GetEventId(TEventId guid);
        BankId GetBankId(TBankId guid);

        TEventId GetEventGuid(EventId id);
        TBankId GetBankGuid(BankId id);
    }
}
