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

namespace Nomad.Core.ServiceRegistry.Services
{
    /// <summary>
    /// Describes a service with its type, implementation, lifetime, and optional instance.
    /// </summary>
    public sealed record ServiceDescriptor
    {
        /// <summary>
        /// 
        /// </summary>
        public Type ServiceType { get; }

        /// <summary>
        /// 
        /// </summary>
        public Type? ImplementationType { get; }

        /// <summary>
        /// 
        /// </summary>
        public ServiceLifetime Lifetime { get; }

        /// <summary>
        /// 
        /// </summary>
        public object? Instance { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="implementationType"></param>
        /// <param name="lifetime"></param>
        /// <param name="instance"></param>
        private ServiceDescriptor(Type serviceType, Type? implementationType, ServiceLifetime lifetime, object? instance = null)
        {
            ServiceType = serviceType;
            ImplementationType = implementationType;
            Lifetime = lifetime;
            Instance = instance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ServiceDescriptor Singleton<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            return new ServiceDescriptor(typeof(TService), typeof(TImplementation), ServiceLifetime.Singleton);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ServiceDescriptor Singleton<TService>(TService instance)
            where TService : class
        {
            ArgumentGuard.ThrowIfNull(instance);
            return new ServiceDescriptor(typeof(TService), instance.GetType(), ServiceLifetime.Singleton, instance);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ServiceDescriptor Transient<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            return new ServiceDescriptor(typeof(TService), typeof(TImplementation), ServiceLifetime.Transient);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ServiceDescriptor Scoped<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            return new ServiceDescriptor(typeof(TService), typeof(TImplementation), ServiceLifetime.Scoped);
        }
    }
}
