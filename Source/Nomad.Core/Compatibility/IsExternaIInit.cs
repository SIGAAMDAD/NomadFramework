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

#if USE_COMPATIBILITY_EXTENSIONS
namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// According to the principal lead developer on the C# Language Team, Jared Parsons, this is not included
    /// in any version of the framework older than .NET v5, hence it must be defined manually here when we
    /// compile for unity.
    /// </summary>
    /// <remarks>
    /// From stackoverflow post https://stackoverflow.com/questions/64749385/predefined-type-system-runtime-compilerservices-isexternalinit-is-not-defined
    /// </remarks>
    public static class IsExternalInit { }
}
#endif
