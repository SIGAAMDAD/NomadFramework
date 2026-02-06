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
using Nomad.Core.EngineUtils;
using Nomad.Core.FileSystem;
using Nomad.Core.Logger;
using Nomad.Core.ServiceRegistry.Interfaces;
using Nomad.FileSystem.Private.Services;

namespace Nomad.FileSystem
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class FileSystemBootstrapper : IBootstrapper
    {
        private IFileSystem _fileSystem;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="registry"></param>
        /// <param name="locator"></param>
        public void Initialize(IServiceRegistry registry, IServiceLocator locator)
        {
            var engineService = locator.GetService<IEngineService>();
            var logger = locator.GetService<ILoggerService>();

            _fileSystem = registry.RegisterSingleton<IFileSystem>(new FileSystemService(engineService, logger));
        }

        /// <summary>
        /// 
        /// </summary>
        public void Shutdown()
        {
            _fileSystem.Dispose();
        }
    }
}