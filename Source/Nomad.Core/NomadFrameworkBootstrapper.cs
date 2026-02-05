/*
===========================================================================
The Nomad MPL Source Code
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
using System.Collections.Generic;
using Nomad.Core.Abstractions;
using Nomad.Core.ServiceRegistry.Interfaces;

namespace Nomad.Core
{
    /// <summary>
    /// The main framework bootstrapper. If you want to activate a module, simply compile it, then add it via <see cref="AddBootstrapper"/>.
    /// </summary>
    public sealed class NomadFrameworkBootstrapper : IDisposable
    {
        private readonly IServiceRegistry _registry;
        private readonly IServiceLocator _locator;
        private readonly List<IBootstrapper> _bootstrappers;

        /// <summary>
        /// 
        /// </summary>
        public NomadFrameworkBootstrapper(IServiceRegistry registry, IServiceLocator locator)
        {
            _registry = registry;
            _locator = locator;
            _bootstrappers = new List<IBootstrapper>();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            for (int i = _bootstrappers.Count - 1; i >= 0; i--)
            {
                _bootstrappers[i].Shutdown();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public NomadFrameworkBootstrapper AddBootstrapper<T>()
            where T : IBootstrapper, new()
        {
            var bootstrapper = new T();
            _bootstrappers.Add(bootstrapper);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bootstrapper"></param>
        /// <returns></returns>
        public NomadFrameworkBootstrapper AddBootstrapper(IBootstrapper bootstrapper)
        {
            _bootstrappers.Add(bootstrapper);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Bootstrap()
        {
            for (int i = 0; i < _bootstrappers.Count; i++)
            {
                _bootstrappers[i].Initialize(_registry, _locator);
            }

            StartSystems();
        }

        /// <summary>
        /// 
        /// </summary>
        private void StartSystems()
        {
            var startables = _locator.GetServices<IStartable>();
            foreach (var startable in startables)
            {
                startable.Start();
            }
        }
    }
}