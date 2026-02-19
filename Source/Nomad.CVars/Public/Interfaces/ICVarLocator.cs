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
using System.Collections.Generic;

namespace Nomad.CVars.Interfaces
{
    /// <summary>
    ///
    /// </summary>
    public interface ICVarLocator : IDisposable
    {
        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        ICVar<T>? GetCVar<T>(string name);

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        ICVar? GetCVar(string name);

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T">The value type of the CVar.</typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        bool CVarExists<T>(string name);

        /// <summary>
        /// Searches the CVar cache for a match of <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the CVar to search for</param>
        /// <returns><b>true</b> if CVar of <paramref name="name"/> exists, <b>false</b> if not.</returns>
        bool CVarExists(string name);

        /// <summary>
        /// Returns a list of all the currently registered CVars.
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<ICVar> GetCVars();
    }
}
