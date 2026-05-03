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

using Nomad.Console.Events;
using Nomad.Core.Events;

namespace Nomad.Console
{
    public interface IConsoleInputRouter
    {
        [Event("Nomad.Console.Events")]
        IGameEvent<PageDownEventArgs> PageDown { get; }

        [Event("Nomad.Console.Events")]
        IGameEvent<PageUpEventArgs> PageUp { get; }

        [Event("Nomad.Console.Events")]
        IGameEvent<ConsoleToggleRequestedEventArgs> ConsoleToggleRequested { get; }

        [Event("Nomad.Console.Events")]
        [EventPayload("CurrentText", typeof(string))]
        IGameEvent<HistoryPrevRequestedEventArgs> HistoryPrevRequested { get; }

        [Event("Nomad.Console.Events")]
        [EventPayload("CurrentText", typeof(string))]
        IGameEvent<HistoryNextRequestedEventArgs> HistoryNextRequested { get; }
    }
}
