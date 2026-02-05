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

namespace Nomad.Core.Console
{
    /// <summary>
    /// 
    /// </summary>
    public readonly struct CommandExecutedEventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public readonly ConsoleCommand Command;

        /// <summary>
        /// 
        /// </summary>
        public readonly int ArgumentCount;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="argumentCount"></param>
        public CommandExecutedEventArgs(ConsoleCommand command, int argumentCount)
        {
            Command = command;
            ArgumentCount = argumentCount;
        }
    }
}
