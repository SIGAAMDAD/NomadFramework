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

using System.Reflection;
using Nomad.Core.Abstractions;
using Nomad.Core.Compatibility.Guards;
using Nomad.Core.CVars;
using Nomad.Core.Events;
using Nomad.Core.Logger;
using Nomad.Core.ServiceRegistry.Interfaces;
using Nomad.Core.Util.Attributes;
using Nomad.CVars.Private.Services;
using Nomad.CVars.Global;

namespace Nomad.CVars
{
    /// <summary>
    ///
    /// </summary>
    public sealed class CVarBootstrapper : IBootstrapper
    {
        private ICVarSystemService? _cvarSystem;

        /// <summary>
        /// Initializes the CVar system.
        /// </summary>
        /// <param name="registry"></param>
        /// <param name="locator"></param>
        public void Initialize(IServiceRegistry registry, IServiceLocator locator)
        {
            ArgumentGuard.ThrowIfNull(registry);
            ArgumentGuard.ThrowIfNull(locator);

            var logger = locator.GetService<ILoggerService>();

            _cvarSystem = new Private.Services.CVarSystem(
                locator.GetService<IGameEventRegistryService>(),
                logger
            );
            registry.AddSingleton(_cvarSystem);

            var attribute = Assembly.GetAssembly(typeof(CVarBootstrapper)).GetCustomAttribute<NomadModule>();
            logger.PrintLine($"Initialized {attribute.Name}\n\tBuildId = {attribute.BuildId}\n\tCompileTime = {attribute.CompileTime}\n\tVersion = {attribute.VersionMajor}.{attribute.VersionMinor}.{attribute.VersionPatch}");

            Global.CVarSystem.Initialize(_cvarSystem);
        }

        /// <summary>
        ///
        /// </summary>
        public void Shutdown()
        {
            _cvarSystem?.Dispose();
        }
    }
}
