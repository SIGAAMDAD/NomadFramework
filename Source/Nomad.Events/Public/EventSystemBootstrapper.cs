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
using Nomad.Core.Compatibility;
using Nomad.Core.Events;
using Nomad.Core.Logger;
using Nomad.Core.ServiceRegistry.Interfaces;

namespace Nomad.Events
{
    /// <summary>
    /// 
    /// </summary>
    public static class EventSystemBootstrapper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="locator"></param>
        /// <param name="registry"></param>
        public static void Initialize(IServiceLocator locator, IServiceRegistry registry)
        {
            ExceptionCompat.ThrowIfNull(locator);
            ExceptionCompat.ThrowIfNull(registry);

            ILoggerService logger = locator.GetService<ILoggerService>();

            registry.RegisterSingleton<IGameEventRegistryService>(new GameEventRegistry(logger));
        }
    };
};