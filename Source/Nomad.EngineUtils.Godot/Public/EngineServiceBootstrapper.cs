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

using Godot;
using Nomad.Core.Abstractions;
using Nomad.Core.Compatibility.Guards;
using Nomad.Core.CVars;
using Nomad.Core.EngineUtils;
using Nomad.Core.Events;
using Nomad.Core.Logger;
using Nomad.Core.ServiceRegistry.Interfaces;

namespace Nomad.EngineUtils
{
    /// <summary>
    ///
    /// </summary>
    public sealed class EngineServiceBootstrapper : IBootstrapper
    {
        private IEngineService? _engineService;

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
            var cvarSystem = locator.GetService<ICVarSystemService>();
            var eventFactory = locator.GetService<IGameEventRegistryService>();
            _engineService = new GodotEngineService(
                (SceneTree)Engine.GetMainLoop(),
                null!, // FIXME
                logger,
                cvarSystem,
                eventFactory
            );

            registry.AddSingleton(_engineService);
        }

        /// <summary>
        ///
        /// </summary>
        public void Shutdown()
        {
            _engineService?.Dispose();
        }
    }
}
