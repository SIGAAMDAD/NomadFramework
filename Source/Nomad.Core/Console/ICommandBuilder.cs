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

namespace Nomad.Core.Console
{
    /// <summary>
    ///
    /// </summary>
    public interface ICommandBuilder : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        int ArgumentCount { get; }

        /// <summary>
        /// 
        /// </summary>
        IGameEvent<TextEnteredEventArgs> TextEntered { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        string GetArgumentAt(int index);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        string[] GetArgs();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        void OnHistoryPrev(in HistoryPrevEventArgs args);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        void OnHistoryNext(in HistoryNextEventArgs args);
    }
}
