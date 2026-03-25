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

namespace Nomad.Core.ServiceRegistry.Services
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class ServiceScope : IServiceScope, IServiceLocator
    {
        /// <summary>
        /// 
        /// </summary>
        public IServiceRegistry Collection => _root.Collection;
        private readonly ServiceLocator _root;

        public IServiceLocator ServiceLocator => this; // The scope itself is the locator.

        private readonly Dictionary<Type, object> _scopedInstances = new();
        private bool _isDisposed;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        public ServiceScope(ServiceLocator root)
        {
            ArgumentGuard.ThrowIfNull(root);
            _root = root;
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
            foreach (var instance in _scopedInstances.Values)
            {
                if (instance is IDisposable d)
                {
                    d.Dispose();
                }
            }
            _scopedInstances.Clear();
            _isDisposed = true;
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
        public bool TryGetService<TService>([NotNullWhen(true)] out TService? service)
            where TService : class
        {
            if (!_root._collection.TryGetDescriptor(typeof(TService), out var descriptor))
            {
                service = null;
                return false;
            }

            if (descriptor.Lifetime == ServiceLifetime.Scoped)
            {
                // Scoped: use our own cache
                if (_scopedInstances.TryGetValue(descriptor.ServiceType, out object? cached))
                {
                    service = (TService)cached;
                    return true;
                }

                var instance = CreateScopedInstance(descriptor);
                _scopedInstances[descriptor.ServiceType] = instance;
                service = (TService)instance;
                return true;
            }

            // Singleton/Transient: delegate to root locator
            return _root.TryGetService(out service);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        public IEnumerable<TService> GetServices<TService>()
            where TService : class
        {
            // Scoped services currently in cache
            foreach (var entry in _scopedInstances)
            {
                if (entry.Value is TService s)
                {
                    yield return s;
                }
            }

            // Also include singletons from root? Typically GetServices returns all instances of a type,
            // but here we mimic the original behavior (only singletons from root cache). Adjust if needed.
            foreach (var s in _root.GetServices<TService>())
            {
                yield return s;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <returns></returns>
        public TService CreateInstance<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            return _root.CreateInstance<TService, TImplementation>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public IServiceScope CreateScope()
            => throw new NotSupportedException("Cannot create a scope from another scope.");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="descriptor"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private object CreateScopedInstance(ServiceDescriptor descriptor)
        {
            if (descriptor.ImplementationType == null)
            {
                throw new InvalidOperationException($"No implementation type for {descriptor.ServiceType}");
            }

            // Use the root's factory cache to create the instance (avoids reflection).
            var factory = _root.CreateFactory(descriptor.ImplementationType); // we need to expose this or reuse internal
            var instance = factory(this);
            // Scoped instances are tracked for disposal by this scope.
            return instance;
        }
    }
}
