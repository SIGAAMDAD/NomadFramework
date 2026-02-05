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
using Nomad.Core.Events;
using Nomad.Core.FileSystem;
using Nomad.Core.Logger;
using Nomad.Core.ServiceRegistry.Interfaces;
using Nomad.CVars.Private.Services;

namespace Nomad.CVars
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class CVarBootstrapper : IBootstrapper
    {
        private ICVarSystemService _cvarSystem;

        /// <summary>
        /// Initializes the CVar system.
        /// </summary>
        /// <param name="registry"></param>
        /// <param name="locator"></param>
        public void Initialize(IServiceRegistry registry, IServiceLocator locator)
        {
            _cvarSystem = registry.RegisterSingleton<ICVarSystemService>(
                new CVarSystem(
                    locator.GetService<IGameEventRegistryService>(),
                    locator.GetService<IFileSystem>(),
                    locator.GetService<ILoggerService>()
                )
            );
        }

        /// <summary>
        /// 
        /// </summary>
        public void Shutdown()
        {
            _cvarSystem.Dispose();
        }
    }
}