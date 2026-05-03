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

namespace Nomad.Console
{
    public readonly struct ConsoleCommandInvocation
    {
        public string Name { get; }
        public string RawText { get; }
        public string[] Arguments { get; }
        public int CommandIndex { get; }

        public int ArgumentCount => Arguments.Length;

        public ConsoleCommandInvocation(string name, string rawText, string[] arguments, int commandIndex)
        {
            Name = name ?? string.Empty;
            RawText = rawText ?? string.Empty;
            Arguments = arguments ?? Array.Empty<string>();
            CommandIndex = commandIndex;
        }

        public string GetArgumentAt(int index)
        {
            return Arguments[index];
        }
    }
}
