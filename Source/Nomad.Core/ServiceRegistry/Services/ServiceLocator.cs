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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Nomad.Core.Compatibility.Guards;
using Nomad.Core.ServiceRegistry.Interfaces;

namespace Nomad.Core.ServiceRegistry.Services
{
    /// <summary>
    ///
    /// </summary>
    public sealed class ServiceLocator : IServiceLocator
    {
        /// <summary>
        ///
        /// </summary>
        public IServiceRegistry Collection => _collection;
        internal readonly ServiceCollection _collection;

        private readonly ConcurrentDictionary<Type, object> _singletonCache = new();
        private readonly ConcurrentDictionary<Type, Func<IServiceLocator, object>> _factoryCache = new();
        private bool _isDisposed;

        /// <summary>
        ///
        /// </summary>
        /// <param name="collection"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public ServiceLocator(ServiceCollection collection)
        {
            ArgumentGuard.ThrowIfNull(collection);
            _collection = collection;
        }

        /// <summary>
        ///
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
            _singletonCache.Clear();
            _factoryCache.Clear();
            _collection.Dispose();
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public TService GetService<TService>()
            where TService : class
        {
            if (TryGetService(out TService? service))
            {
                return service;
            }

            throw new InvalidOperationException($"Service '{typeof(TService)}' not registered.");
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="service"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public bool TryGetService<TService>([NotNullWhen(true)] out TService? service)
            where TService : class
        {
            if (!_collection.TryGetDescriptor(typeof(TService), out var descriptor))
            {
                service = null;
                return false;
            }

            if (descriptor.Lifetime == ServiceLifetime.Scoped)
            {
                throw new InvalidOperationException($"Cannot resolve scoped service '{typeof(TService)}' from root locator. Use CreateScope() to obtain a scope.");
            }

            service = (TService)ResolveCore(descriptor);
            return true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        public IEnumerable<TService> GetServices<TService>()
            where TService : class
        {
            foreach (var entry in _singletonCache)
            {
                if (entry.Value is TService service)
                {
                    yield return service;
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        public TService CreateInstance<TService>()
            where TService : class
        {
            var type = typeof(TService);
            var factory = _factoryCache.GetOrAdd(type, CreateFactory);
            return (TService)factory(this);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IServiceScope CreateScope()
            => new ServiceScope(this);

        /// <summary>
        ///
        /// </summary>
        /// <param name="descriptor"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private object ResolveCore(ServiceDescriptor descriptor)
        {
            return descriptor.Lifetime switch
            {
                ServiceLifetime.Singleton => GetOrCreateSingleton(descriptor),
                ServiceLifetime.Transient => CreateTransient(descriptor),
                _ => throw new InvalidOperationException($"Unexpected lifetime '{descriptor.Lifetime}'")
            };
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="descriptor"></param>
        /// <returns></returns>
        private object GetOrCreateSingleton(ServiceDescriptor descriptor)
        {
            // If an explicit instance was provided, use it.
            if (descriptor.Instance != null)
            {
                return descriptor.Instance;
            }
            return _singletonCache.GetOrAdd(descriptor.ServiceType, (_, d) => CreateServiceInstance(d), descriptor);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="descriptor"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private object CreateTransient(ServiceDescriptor descriptor)
            => CreateServiceInstance(descriptor);

        /// <summary>
        ///
        /// </summary>
        /// <param name="descriptor"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private object CreateServiceInstance(ServiceDescriptor descriptor)
        {
            if (descriptor.ImplementationType == null)
            {
                throw new InvalidOperationException($"No implementation type for {descriptor.ServiceType}");
            }

            var factory = _factoryCache.GetOrAdd(descriptor.ImplementationType, CreateFactory);
            var instance = factory(this);

            // Track only singletons (transients are not tracked).
            if (descriptor.Lifetime == ServiceLifetime.Singleton)
            {
                _collection.TrackSingletonDisposable(instance);
            }

            return instance;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        internal Func<IServiceLocator, object> CreateFactory(Type type)
        {
            var constructor = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance).FirstOrDefault()
                ?? throw new InvalidOperationException($"No public constructor for {type}");

            var parameters = constructor.GetParameters();
            if (parameters.Length == 0)
            {
                return _ => Activator.CreateInstance(type)!;
            }

            var paramExpr = Expression.Parameter(typeof(IServiceLocator), "locator");
            var args = new Expression[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                var paramType = parameters[i].ParameterType;
                // Emit: (TService)locator.GetService<TService>()  (using reflection-free call via expression)
                var getServiceMethod = typeof(IServiceLocator).GetMethod(nameof(GetService))!.MakeGenericMethod(paramType);
                args[i] = Expression.Call(paramExpr, getServiceMethod);
            }

            var newExpr = Expression.New(constructor, args);
            var lambda = Expression.Lambda<Func<IServiceLocator, object>>(newExpr, paramExpr);
            return lambda.Compile();
        }
    }
}
