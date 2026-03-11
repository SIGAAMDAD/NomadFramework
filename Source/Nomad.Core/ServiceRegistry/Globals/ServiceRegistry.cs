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

using Nomad.Core.Compatibility.Guards;
using Nomad.Core.ServiceRegistry.Interfaces;
using Nomad.Core.ServiceRegistry.Services;

namespace Nomad.Core.ServiceRegistry.Globals
{
    /// <summary>
    ///
    /// </summary>
    public static class ServiceRegistry
    {
        /// <summary>
        ///
        /// </summary>
        public static IServiceRegistry Instance
        {
            get
            {
                _instance ??= new ServiceCollection();
                return _instance;
            }
        }
        private static IServiceRegistry? _instance;

        /// <summary>
        ///
        /// </summary>
        /// <param name="instance"></param>
        internal static void Initialize(IServiceRegistry instance)
        {
            ArgumentGuard.ThrowIfNull(instance);
            _instance = instance;
        }
    }
}
