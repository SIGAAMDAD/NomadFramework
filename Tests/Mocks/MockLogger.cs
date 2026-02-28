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

#if !UNITY_EDITOR
using System;
using Nomad.Core.CVars;
using Nomad.Core.Logger;

public class MockLogger : ILoggerService
{
    private bool _isDisposed = false;

    public void AddSink(ILoggerSink sink)
    {
    }

    public void Clear()
    {
        Console.Clear();
    }

    public ILoggerCategory CreateCategory(in string name, LogLevel level, bool enabled)
    {
        return null;
    }

    public void Dispose()
    {
        // DISPOSAL GOES HERE
        GC.SuppressFinalize(this);
        _isDisposed = true;
    }

    public void InitConfig(ICVarSystemService cvarSystem)
    {
        throw new System.NotImplementedException();
    }

    public void Print(in string message)
    {
        Console.WriteLine(message);
    }

    public void Print(in ILoggerCategory category, in string message)
    {
        Console.WriteLine(message);
    }

    public void PrintDebug(in string message)
    {
        Console.WriteLine($"DEBUG: {message}");
    }

    public void PrintDebug(in ILoggerCategory category, in string message)
    {
        Console.WriteLine($"DEBUG: {message}");
    }

    public void PrintError(in string message)
    {
        Console.WriteLine($"ERROR: {message}");
    }

    public void PrintError(in ILoggerCategory category, in string message)
    {
        Console.WriteLine($"ERROR: {message}");
    }

    public void PrintLine(in string message)
    {
        Console.WriteLine(message);
    }

    public void PrintLine(in ILoggerCategory category, in string message)
    {
        Console.WriteLine(message);
    }

    public void PrintWarning(in string message)
    {
        Console.WriteLine($"WARNING: {message}");
    }

    public void PrintWarning(in ILoggerCategory category, in string message)
    {
        Console.WriteLine($"WARNING: {message}");
    }
}
#endif
