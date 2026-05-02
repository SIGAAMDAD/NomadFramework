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
using Nomad.Console.Events;

namespace Nomad.Core.Engine.Services
{
    public interface IConsoleObject : IDisposable
    {
        bool IsOpen { get; }

        [Event("Nomad.Console.Events")]
        IGameEvent<ConsoleClosedEventArgs> ConsoleClosed { get; }

        [Event("Nomad.Console.Events")]
        IGameEvent<ConsoleOpenedEventArgs> ConsoleOpened { get; }

        void Show();
        void Hide();
        void Toggle();

        void Clear();
        void Print(string message);

        void ExecuteText(string text);
    }
}
