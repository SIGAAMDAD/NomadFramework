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
using Nomad.Core.ServiceRegistry.Interfaces;

namespace Nomad.EngineUtils
{
    /// <summary>
    ///
    /// </summary>
    public sealed class EngineBootstrapper : IBootstrapper
    {
        private IEngineService? _engineService;
        private IDisplayService? _displayService;

        /// <summary>
        /// Initializes the CVar system.
        /// </summary>
        /// <param name="registry"></param>
        /// <param name="locator"></param>
        public void Initialize(IServiceRegistry registry, IServiceLocator locator)
        {
            ArgumentGuard.ThrowIfNull(registry);
            ArgumentGuard.ThrowIfNull(locator);

            var sceneTree = (SceneTree)Engine.GetMainLoop();

            _engineService = new GodotEngineService(
                sceneTree,
                registry,
                locator
            );
            registry.AddSingleton(_engineService);

            _displayService = new GodotDisplayService(
                sceneTree,
                locator.GetService<IWindowService>(),
                locator.GetService<ICVarSystemService>()
            );
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
