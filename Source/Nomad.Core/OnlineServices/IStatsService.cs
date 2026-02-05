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

using System.Threading.Tasks;

namespace Nomad.Core.OnlineServices
{
    /// <summary>
    /// 
    /// </summary>
    public interface IStatsService
    {
        /// <summary>
        /// 
        /// </summary>
        bool SupportsLeaderboards { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="statName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ValueTask SetStatInt(string statName, int value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="statName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ValueTask SetStatFloat(string statName, float value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="statName"></param>
        /// <returns></returns>
        ValueTask<int> GetStatInt(string statName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="statName"></param>
        /// <returns></returns>
        ValueTask<float> GetStatFloat(string statName);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        ValueTask<bool> StoreStats();
    }
}
