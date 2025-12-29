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
using Nomad.Core.ServiceRegistry.Interfaces;

namespace Nomad.Core.Logger
{
    /// <summary>
    /// The generic logger interface.
    /// </summary>
    public interface ILoggerService : IDisposable
    {
        void Print(in string message);
        void PrintLine(in string message);
        void PrintDebug(in string message);
        void PrintWarning(in string message);
        void PrintError(in string message);

        void Print(in ILoggerCategory category, in string message);
        void PrintLine(in ILoggerCategory category, in string message);
        void PrintDebug(in ILoggerCategory category, in string message);
        void PrintWarning(in ILoggerCategory category, in string message);
        void PrintError(in ILoggerCategory category, in string message);

        void Clear();

        void InitConfig(IServiceLocator locator);
        void AddSink(ILoggerSink sink);

        ILoggerCategory CreateCategory(in string name, LogLevel level, bool enabled);
    }
}
