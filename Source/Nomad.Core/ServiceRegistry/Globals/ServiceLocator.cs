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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Nomad.Core.Compatibility.Guards;
using Nomad.Core.ServiceRegistry.Interfaces;
using Nomad.Core.ServiceRegistry.Services;

namespace Nomad.Core.ServiceRegistry.Globals
{
    /// <summary>
    ///
    /// </summary>
    public static class ServiceLocator
    {
        /// <summary>
        ///
        /// </summary>
        public static IServiceLocator Instance
        {
            get
            {
                _instance ??= new Services.ServiceLocator(ServiceRegistry.Instance as ServiceCollection);
                return _instance;
            }
        }
        private static IServiceLocator? _instance;

        /// <summary>
        ///
        /// </summary>
        /// <param name="instance"></param>
        internal static void Initialize(IServiceLocator instance)
        {
            _instance = instance ?? throw new ArgumentNullException(nameof(instance));
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        public static TService GetService<TService>()
            where TService : class
        {
            return Instance.GetService<TService>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="service"></param>
        /// <returns></returns>
        public static bool TryGetService<TService>([NotNullWhen(true)] out TService? service)
            where TService : class
        {
            return Instance.TryGetService(out service);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        public static IEnumerable<TService> GetServices<TService>()
            where TService : class
        {
            return Instance.GetServices<TService>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <returns></returns>
        public static TService CreateInstance<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            return Instance.CreateInstance<TService, TImplementation>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public static IServiceScope CreateScope()
        {
            return Instance.CreateScope();
        }
    }
}
