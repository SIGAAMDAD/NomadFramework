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

using Nomad.Core.Abstractions;
using Nomad.Core.CVars;
using Nomad.Core.EngineUtils;
using Nomad.Core.Events;
using Nomad.Core.FileSystem;
using Nomad.Core.Logger;
using Nomad.Core.OnlineServices;
using Nomad.Core.ServiceRegistry.Interfaces;
using Nomad.OnlineServices.Steam.Private;

namespace Nomad.OnlineServices.Steam
{
    public sealed class SteamBootstrapper : IBootstrapper
    {
        private IOnlinePlatformService? _service;

        /// <summary>
        ///
        /// </summary>
        /// <param name="registry"></param>
        /// <param name="locator"></param>
        public void Initialize(IServiceRegistry registry, IServiceLocator locator)
        {
            _service = new SteamService(
                locator.GetService<ILoggerService>(),
                locator.GetService<IFileSystem>(),
                locator.GetService<IEngineService>(),
                locator.GetService<IGameEventRegistryService>(),
                locator.GetService<ICVarSystemService>()
            );
            registry.AddSingleton(_service);
        }

        /// <summary>
        ///
        /// </summary>
        public void Shutdown()
        {
            _service?.Dispose();
        }
    }
}
