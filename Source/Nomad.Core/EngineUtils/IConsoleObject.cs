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

namespace Nomad.Core.EngineUtils
{
    /// <summary>
    /// 
    /// </summary>
    public interface IConsoleObject : IDisposable
    {
        /// <summary>
        /// Adds a new string line to the console's buffer.
        /// </summary>
        /// <param name="message"></param>
        void PrintString(in string message);

        /// <summary>
        /// Clears the console of all messages.
        /// </summary>
        void Clear();

        /// <summary>
        /// Shows the console.
        /// </summary>
        void Show();

        /// <summary>
        /// Hides the console.
        /// </summary>
        void Hide();
    }
}