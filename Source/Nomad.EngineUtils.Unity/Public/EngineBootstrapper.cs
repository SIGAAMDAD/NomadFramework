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
using Nomad.Core.Engine.Services;
using Nomad.Core.Logger;
using Nomad.Core.ServiceRegistry.Interfaces;
using Nomad.Core.Util.Attributes;

namespace Nomad.EngineUtils
{
    /// <summary>
    ///
    /// </summary>
    public sealed class EngineBootstrapper : IBootstrapper
    {
        private IEngineService? _engineService;

        /// <summary>
        /// Initializes the Unity engine services.
        /// </summary>
        /// <param name="registry"></param>
        /// <param name="locator"></param>
        public void Initialize(IServiceRegistry registry, IServiceLocator locator)
        {
            ArgumentGuard.ThrowIfNull(registry);
            ArgumentGuard.ThrowIfNull(locator);

            ILoggerService logger = locator.GetService<ILoggerService>();

            _engineService = new UnityEngineService(registry, locator);
            registry.AddSingleton(_engineService);

            var assembly = Assembly.GetAssembly(typeof(EngineBootstrapper));
            NomadModule attribute = assembly?.GetCustomAttribute<NomadModule>();
            if (attribute != null)
            {
                logger.PrintLine(
                    "Initialized " + attribute.Name +
                    "\n\tBuildId = " + attribute.BuildId +
                    "\n\tCompileTime = " + attribute.CompileTime +
                    "\n\tVersion = " + attribute.VersionMajor + "." + attribute.VersionMinor + "." + attribute.VersionPatch
                );
            }
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
