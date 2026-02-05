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
using Nomad.Core.Console;
using Nomad.Core.Events;

namespace Nomad.Console.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICommandLineService : IDisposable
    {
        int ArgumentCount { get; }

        IGameEvent<TextEnteredEventArgs> TextEntered { get; }
        IGameEvent<CommandExecutedEventArgs> UnknownCommand { get; }
        IGameEvent<CommandExecutedEventArgs> CommandExecuted { get; }

        void ExecuteCommand(string text);
        string GetArgumentAt(int index);
        string[] GetArguments();
    };
};
