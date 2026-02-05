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

namespace Nomad.Console.Interfaces
{
    /// <summary>
    ///
    /// </summary>
    public interface ICommandService : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        void RegisterCommand(ConsoleCommand command);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        bool CommandExists(string command);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        ConsoleCommand GetCommand(string command);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        bool TryGetCommand(string name, out ConsoleCommand command);
    }
}
