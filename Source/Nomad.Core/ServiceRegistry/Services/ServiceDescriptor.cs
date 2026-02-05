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

using System.Runtime.CompilerServices;
using System;
using Nomad.Core.ServiceRegistry.Interfaces;

namespace Nomad.Core.ServiceRegistry.Services
{
    /// <summary>
    ///
    /// </summary>

    public class ServiceDescriptor : IEquatable<ServiceDescriptor>
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
        public Func<IServiceLocator, object>? Factory { get; }

        /// <summary>
        /// 
        /// </summary>
        public readonly object? Instance;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="implementationType"></param>
        /// <param name="lifetime"></param>
        /// <param name="factory"></param>
        /// <param name="instance"></param>
        public ServiceDescriptor(Type serviceType, Type? implementationType, ServiceLifetime lifetime, Func<IServiceLocator, object>? factory, object? instance = null)
        {
            ServiceType = serviceType;
            ImplementationType = implementationType;
            Lifetime = lifetime;
            Factory = factory;
            Instance = instance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ServiceDescriptor CreateSingleton<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            return new ServiceDescriptor(typeof(TService), typeof(TImplementation), ServiceLifetime.Singleton, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ServiceDescriptor CreateSingleton<TService>(TService instance)
            where TService : class
        {
            return new ServiceDescriptor(typeof(TService), instance.GetType(), ServiceLifetime.Singleton, null, instance);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="factory"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ServiceDescriptor CreateSingleton<TService>(Func<IServiceLocator, TService> factory)
            where TService : class
        {
            return new ServiceDescriptor(typeof(TService), null, ServiceLifetime.Singleton, provider => factory(provider));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ServiceDescriptor CreateTransient<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            return new ServiceDescriptor(typeof(TService), typeof(TImplementation), ServiceLifetime.Transient, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="factory"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ServiceDescriptor CreateTransient<TService>(Func<IServiceLocator, TService> factory)
            where TService : class
        {
            return new ServiceDescriptor(typeof(TService), null, ServiceLifetime.Transient, provider => factory(provider));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ServiceDescriptor CreateScoped<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            return new ServiceDescriptor(typeof(TService), typeof(TImplementation), ServiceLifetime.Scoped, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>/
        public bool Equals(ServiceDescriptor? other)
        {
            return other is not null && other.ServiceType == ServiceType && other.ImplementationType == ImplementationType && other.Lifetime == Lifetime;
        }
    };
};
