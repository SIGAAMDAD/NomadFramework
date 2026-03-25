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
using System.Runtime.CompilerServices;
using Nomad.Core.Compatibility.Guards;
using Nomad.Core.ServiceRegistry.Interfaces;
using Nomad.Core.ServiceRegistry.Services;
using System.Collections.Generic;

namespace Nomad.Core.ServiceRegistry.Globals
{
    /// <summary>
    ///
    /// </summary>
    public static class ServiceRegistry
    {
        /// <summary>
        ///
        /// </summary>
        public static IServiceRegistry Instance
        {
            get
            {
                _instance ??= new ServiceCollection();
                return _instance;
            }
        }
        private static IServiceRegistry? _instance;

        /// <summary>
        ///
        /// </summary>
        /// <param name="instance"></param>
        internal static void Initialize(IServiceRegistry instance)
        {
            _instance = instance ?? throw new ArgumentNullException(nameof(instance));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <param name="lifetime"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IServiceRegistry Register<TService, TImplementation>(ServiceLifetime lifetime)
            where TService : class
            where TImplementation : class, TService
        {
            return Instance.Register<TService, TImplementation>(lifetime);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IServiceRegistry AddSingleton<TService>(TService instance)
            where TService : class
        {
            return Instance.AddSingleton(instance);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IServiceRegistry AddSingleton<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            return Instance.AddSingleton<TService, TImplementation>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IServiceRegistry AddTransient<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            return Instance.AddTransient<TService, TImplementation>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IServiceScope AddScoped<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            return Instance.AddScoped<TService, TImplementation>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsRegistered<TService>()
            where TService : class
        {
            return Instance.IsRegistered<TService>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<ServiceDescriptor> GetDescriptors()
        {
            return Instance.GetDescriptors();
        }
    }
}
