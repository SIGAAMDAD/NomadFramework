using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Nomad.Core.ServiceRegistry.Interfaces;

namespace Nomad.Core.ServiceRegistry.Services
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class ServiceCollection : IServiceRegistry
    {
        private readonly ConcurrentDictionary<Type, ServiceDescriptor> _descriptors = new();
        private readonly ConcurrentBag<IDisposable> _singletonDisposables = new();
        private bool _isDisposed;

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }
            foreach (var d in _singletonDisposables)
            {
                d.Dispose();
            }

            _singletonDisposables.Clear();
            _descriptors.Clear();
            _isDisposed = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <param name="lifetime"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public IServiceRegistry Register<TService, TImplementation>(ServiceLifetime lifetime)
            where TService : class
            where TImplementation : class, TService
        {
            var descriptor = lifetime switch
            {
                ServiceLifetime.Singleton => ServiceDescriptor.Singleton<TService, TImplementation>(),
                ServiceLifetime.Transient => ServiceDescriptor.Transient<TService, TImplementation>(),
                ServiceLifetime.Scoped => ServiceDescriptor.Scoped<TService, TImplementation>(),
                _ => throw new ArgumentOutOfRangeException(nameof(lifetime))
            };
            _descriptors[typeof(TService)] = descriptor;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        public IServiceRegistry AddSingleton<TService>(TService instance)
            where TService : class
        {
            var descriptor = ServiceDescriptor.Singleton(instance);
            _descriptors[typeof(TService)] = descriptor;
            TrackSingletonDisposable(instance);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IServiceRegistry AddSingleton<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            return Register<TService, TImplementation>(ServiceLifetime.Singleton);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IServiceRegistry AddTransient<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            return Register<TService, TImplementation>(ServiceLifetime.Transient);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IServiceRegistry AddScoped<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            return Register<TService, TImplementation>(ServiceLifetime.Scoped);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsRegistered<TService>()
            where TService : class
        {
            return _descriptors.ContainsKey(typeof(TService));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<ServiceDescriptor> GetDescriptors()
            => _descriptors.Values;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="descriptor"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool TryGetDescriptor(Type serviceType, [NotNullWhen(true)] out ServiceDescriptor? descriptor)
            => _descriptors.TryGetValue(serviceType, out descriptor);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        internal void TrackSingletonDisposable(object instance)
        {
            if (instance is IDisposable d)
            {
                _singletonDisposables.Add(d);
            }
        }
    }
}
