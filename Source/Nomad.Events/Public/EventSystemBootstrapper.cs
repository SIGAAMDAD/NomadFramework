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

using Nomad.Core.Abstractions;
using Nomad.Core.Compatibility.Guards;
using Nomad.Core.Events;
using Nomad.Core.Logger;
using Nomad.Core.ServiceRegistry.Interfaces;

namespace Nomad.Events
{
    /// <summary>
    ///
    /// </summary>
    public sealed class EventBootstrapper : IBootstrapper
    {
        private IGameEventRegistryService _eventRegistry;

        /// <summary>
        ///
        /// </summary>
        /// <param name="registry"></param>
        /// <param name="locator"></param>
        public void Initialize(IServiceRegistry registry, IServiceLocator locator)
        {
            ArgumentGuard.ThrowIfNull(locator);
            ArgumentGuard.ThrowIfNull(registry);

            var logger = locator.GetService<ILoggerService>();

            _eventRegistry = registry.RegisterSingleton<IGameEventRegistryService>(new GameEventRegistry(logger));
        }

        /// <summary>
        /// 
        /// </summary>
        public void Shutdown()
        {
            _eventRegistry?.Dispose();
        }
    }
}
