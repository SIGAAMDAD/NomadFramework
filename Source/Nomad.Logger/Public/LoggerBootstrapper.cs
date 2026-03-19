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

using Nomad.Core.Logger;
using Nomad.Core.ServiceRegistry.Interfaces;
using Nomad.Core.Abstractions;
using Nomad.Logger.Private.Services;
using Nomad.Core.Compatibility.Guards;
using Nomad.Core.Util.Attributes;
using System.Reflection;

namespace Nomad.Logger
{
    /// <summary>
    ///
    /// </summary>
    public sealed class LoggerBootstrapper : IBootstrapper
    {
        private ILoggerService? _logger;

        /// <summary>
        ///
        /// </summary>
        /// <param name="serviceRegistry"></param>
        /// <param name="locator"></param>
        public void Initialize(IServiceRegistry serviceRegistry, IServiceLocator locator)
        {
            ArgumentGuard.ThrowIfNull(serviceRegistry);
            ArgumentGuard.ThrowIfNull(locator);

            _logger = new LoggerService();
            serviceRegistry.AddSingleton(_logger);

            var attribute = Assembly.GetAssembly(typeof(LoggerBootstrapper)).GetCustomAttribute<NomadModule>();
            _logger.PrintLine($"Initialized {attribute.Name}\n\tBuildId = {attribute.BuildId}\n\tCompileTime = {attribute.CompileTime}\n\tVersion = {attribute.VersionMajor}.{attribute.VersionMinor}.{attribute.VersionPatch}");
        }

        /// <summary>
        /// 
        /// </summary>
        public void Shutdown()
        {
            _logger?.Dispose();
        }
    }
}
