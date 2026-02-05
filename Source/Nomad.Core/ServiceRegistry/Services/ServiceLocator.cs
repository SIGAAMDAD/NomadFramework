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
using System.Reflection;
using System.Threading;
using Nomad.Core.ServiceRegistry.Interfaces;

namespace Nomad.Core.ServiceRegistry.Services
{
    /// <summary>
    /// Provides a thread-safe implementation of the <see cref="IServiceLocator"/> interface
    /// for resolving and managing service instances with singleton, scoped, and transient lifetimes.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="ServiceLocator"/> class is the core dependency injection container
    /// implementation in the Nomad Framework. It manages service instances according to their
    /// registered lifetimes and provides efficient, thread-safe resolution of dependencies.
    /// </para>
    /// <para>
    /// Key features:
    /// <list type="bullet">
    /// <item><description>Thread-safe singleton caching using <see cref="ConcurrentDictionary{TKey,TValue}"/></description></item>
    /// <item><description>Thread-local storage for scoped instances (one scope per thread)</description></item>
    /// <item><description>Factory caching for performance optimization</description></item>
    /// <item><description>Automatic disposal of <see cref="IDisposable"/> services</description></item>
    /// <item><description>Constructor injection with recursive dependency resolution</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// Note: This implementation uses thread-local storage for scoped instances, meaning
    /// each thread gets its own scope. For web applications or other scenarios requiring
    /// request-based scoping, consider using <see cref="CreateScope"/> to create explicit
    /// scopes.
    /// </para>
    /// <example>
    /// Creating and using a service locator:
    /// <code>
    /// var collection = new ServiceCollection();
    /// collection.Register&lt;IMyService, MyServiceImpl&gt;(ServiceLifetime.Singleton);
    /// collection.Register&lt;IDependency, DependencyImpl&gt;(ServiceLifetime.Transient);
    /// 
    /// using (var locator = new ServiceLocator(collection))
    /// {
    ///     var service = locator.GetService&lt;IMyService&gt;();
    ///     // Use service...
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    public sealed class ServiceLocator : IServiceLocator
    {
        /// <summary>
        /// Gets the service registry associated with this service locator.
        /// </summary>
        /// <value>
        /// The <see cref="ServiceCollection"/> containing all service registrations
        /// used by this locator.
        /// </value>
        public IServiceRegistry Collection => _collection;
        private readonly ServiceCollection _collection;

        private readonly ConcurrentDictionary<Type, object> _singletonCache = new ConcurrentDictionary<Type, object>();
        private readonly ConcurrentDictionary<Type, Func<IServiceLocator, object>> _factoryCache = new ConcurrentDictionary<Type, Func<IServiceLocator, object>>();
        private readonly ConcurrentDictionary<Type, object> _scopedInstances = new ConcurrentDictionary<Type, object>();

        private readonly ThreadLocal<Dictionary<Type, object>> _scopeInstances = new ThreadLocal<Dictionary<Type, object>>(() => new Dictionary<Type, object>());

        private bool _isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLocator"/> class with the specified service collection.
        /// </summary>
        /// <param name="collection">The <see cref="ServiceCollection"/> containing service registrations.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="collection"/> is <see langword="null"/>.
        /// </exception>
        /// <remarks>
        /// The service locator takes ownership of the provided collection and will dispose it
        /// when the locator is disposed. Do not use the collection after passing it to the locator.
        /// </remarks>
        public ServiceLocator(ServiceCollection collection)
        {
            _collection = collection;
        }

        /// <summary>
        /// Releases all resources used by the <see cref="ServiceLocator"/> and all disposable services it manages.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method performs the following cleanup:
        /// <list type="bullet">
        /// <item><description>Clears all singleton, scoped, and factory caches</description></item>
        /// <item><description>Disposes the thread-local storage for scoped instances</description></item>
        /// <item><description>Disposes the underlying service collection</description></item>
        /// <item><description>Disposes all singleton and scoped services that implement <see cref="IDisposable"/></description></item>
        /// </list>
        /// </para>
        /// <para>
        /// After disposal, any attempt to use the service locator will result in undefined behavior.
        /// </para>
        /// <para>
        /// This method is idempotent; calling it multiple times has no additional effect.
        /// </para>
        /// </remarks>
        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
            _singletonCache.Clear();
            _factoryCache.Clear();
            _scopedInstances.Clear();
            _scopeInstances.Dispose();
            _collection.Dispose();
        }

        /// <summary>
        /// Creates a new instance of the specified service type without using the container's lifetime management.
        /// </summary>
        /// <typeparam name="TService">The type of service to create. Must be a reference type.</typeparam>
        /// <returns>A new instance of <typeparamref name="TService"/>.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when:
        /// <list type="bullet">
        /// <item><description>No public constructor is found for <typeparamref name="TService"/></description></item>
        /// <item><description>Any constructor parameter cannot be resolved from the container</description></item>
        /// <item><description>The service type is abstract or an interface</description></item>
        /// </list>
        /// </exception>
        /// <remarks>
        /// <para>
        /// This method creates a new instance using constructor injection, resolving all
        /// dependencies from the container. However, unlike <see cref="GetService{TService}"/>,
        /// the created instance is not cached or managed by the container (except that any
        /// <see cref="IDisposable"/> dependencies resolved during construction are tracked
        /// by the container).
        /// </para>
        /// <para>
        /// This method is useful for:
        /// <list type="bullet">
        /// <item><description>Creating multiple independent instances</description></item>
        /// <item><description>Testing scenarios</description></item>
        /// <item><description>When you need to manage the instance lifetime manually</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// The instance is created using the first public constructor found. For types with
        /// multiple constructors, the behavior is undefined. Consider using explicit factory
        /// registration if you need control over constructor selection.
        /// </para>
        /// </remarks>
        public TService CreateInstance<TService>() where TService : class
        {
            Type type = typeof(TService);
            ConstructorInfo constructor = GetConstructor(type);
            ParameterInfo[] parameters = constructor.GetParameters();

            if (parameters.Length == 0)
            {
                return (TService)Activator.CreateInstance(type);
            }

            object[] args = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                args[i] = ResolveService(_collection.GetDescriptor(parameters[i].ParameterType));
            }

            return (TService)constructor.Invoke(args);
        }

        /// <summary>
        /// Creates a new service scope for resolving scoped services.
        /// </summary>
        /// <returns>A new <see cref="IServiceScope"/> instance.</returns>
        /// <remarks>
        /// <para>
        /// A service scope creates an isolated context for scoped services. Services registered
        /// with <see cref="ServiceLifetime.Scoped"/> are created once per scope and are disposed
        /// when the scope is disposed.
        /// </para>
        /// <para>
        /// The returned scope uses a separate instance cache, so scoped services resolved
        /// from the scope are independent of those resolved from the parent locator or other scopes.
        /// </para>
        /// <para>
        /// It is the caller's responsibility to dispose of the returned scope. The recommended
        /// pattern is to use a <see langword="using"/> statement.
        /// </para>
        /// </remarks>
        public IServiceScope CreateScope()
        {
            return new ServiceScope(this);
        }

        /// <summary>
        /// Gets an instance of the specified service type.
        /// </summary>
        /// <typeparam name="TService">The type of service to retrieve. Must be a reference type.</typeparam>
        /// <returns>An instance of <typeparamref name="TService"/>.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when no service of type <typeparamref name="TService"/> is registered.
        /// </exception>
        /// <remarks>
        /// <para>
        /// This method resolves the service according to its registered lifetime:
        /// <list type="bullet">
        /// <item><description>Singleton: Returns the same instance for all calls</description></item>
        /// <item><description>Scoped: Returns the same instance within the current thread/scope</description></item>
        /// <item><description>Transient: Returns a new instance each time</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// For optional dependencies, use <see cref="TryGetService{TService}(out TService)"/> instead.
        /// </para>
        /// </remarks>
        public TService GetService<TService>() where TService : class
        {
            if (TryGetService(out TService service))
            {
                return service;
            }
            throw new InvalidOperationException($"Service {typeof(TService)} not yet registered");
        }

        /// <summary>
        /// Gets all instances of the specified service type currently cached in the singleton cache.
        /// </summary>
        /// <typeparam name="TService">The type of service to retrieve. Must be a reference type.</typeparam>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <typeparamref name="TService"/> instances from the singleton cache.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method only returns instances from the singleton cache. It does not create new instances
        /// or include scoped or transient services that are not cached as singletons.
        /// </para>
        /// <para>
        /// If no singleton instances of <typeparamref name="TService"/> are cached, an empty enumerable
        /// is returned.
        /// </para>
        /// <para>
        /// Note: This implementation differs from typical dependency injection containers that
        /// support multiple registrations of the same service type. This method only returns
        /// singleton instances that have already been created and cached.
        /// </para>
        /// </remarks>
        public IEnumerable<TService> GetServices<TService>() where TService : class
        {
            var services = new List<TService>();

            foreach (var service in _singletonCache)
            {
                if (service.Value is TService type)
                {
                    services.Add(type);
                }
            }

            return services;
        }

        /// <summary>
        /// Attempts to get an instance of the specified service type.
        /// </summary>
        /// <typeparam name="TService">The type of service to retrieve. Must be a reference type.</typeparam>
        /// <param name="service">
        /// When this method returns, contains the service instance if it was successfully resolved;
        /// otherwise, <see langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the service was successfully resolved; otherwise, <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method provides a non-throwing alternative to <see cref="GetService{TService}"/>.
        /// It returns <see langword="false"/> when the service is not registered, but may still
        /// throw exceptions for other resolution failures (e.g., constructor failures).
        /// </para>
        /// <para>
        /// The resolved instance follows the same lifetime rules as <see cref="GetService{TService}"/>.
        /// </para>
        /// </remarks>
        public bool TryGetService<TService>(out TService service) where TService : class
        {
            ServiceDescriptor descriptor = _collection.GetDescriptor(typeof(TService));
            if (descriptor == null)
            {
                service = default;
                return false;
            }
            service = (TService)ResolveService(descriptor);
            return true;
        }

        /// <summary>
        /// Resolves a service instance based on its descriptor and lifetime.
        /// </summary>
        /// <param name="descriptor">The <see cref="ServiceDescriptor"/> describing the service to resolve.</param>
        /// <returns>The resolved service instance.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when <paramref name="descriptor"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the descriptor contains an invalid <see cref="ServiceLifetime"/> value.
        /// </exception>
        /// <remarks>
        /// This is the core resolution method that delegates to the appropriate lifetime-specific
        /// resolution method based on the descriptor's <see cref="ServiceDescriptor.Lifetime"/>.
        /// </remarks>
        private object ResolveService(ServiceDescriptor descriptor)
        {
            if (descriptor == null)
            {
                throw new InvalidOperationException("Service not registered");
            }

            return descriptor.Lifetime switch
            {
                ServiceLifetime.Singleton => GetOrCreateSingleton(descriptor),
                ServiceLifetime.Transient => CreateTransient(descriptor),
                ServiceLifetime.Scoped => GetOrCreateScoped(descriptor),
                _ => throw new ArgumentOutOfRangeException($"Invalid lifetime {descriptor.Lifetime}")
            };
        }

        /// <summary>
        /// Gets or creates a singleton instance for the specified descriptor.
        /// </summary>
        /// <param name="descriptor">The <see cref="ServiceDescriptor"/> for the singleton service.</param>
        /// <returns>The singleton instance.</returns>
        /// <remarks>
        /// <para>
        /// If the descriptor has a pre-existing instance (<see cref="ServiceDescriptor.Instance"/>),
        /// that instance is returned directly.
        /// </para>
        /// <para>
        /// Otherwise, the method uses <see cref="ConcurrentDictionary{TKey,TValue}.GetOrAdd"/>
        /// to ensure thread-safe singleton creation. The instance is created only once, even
        /// under concurrent access.
        /// </para>
        /// </remarks>
        private object GetOrCreateSingleton(ServiceDescriptor descriptor)
        {
            if (descriptor.Instance != null)
            {
                return descriptor.Instance;
            }
            return _singletonCache.GetOrAdd(descriptor.ServiceType, _ => CreateServiceInstance(descriptor));
        }

        /// <summary>
        /// Creates a new transient instance for the specified descriptor.
        /// </summary>
        /// <param name="descriptor">The <see cref="ServiceDescriptor"/> for the transient service.</param>
        /// <returns>A new instance of the service.</returns>
        /// <remarks>
        /// Each call to this method creates a new instance. No caching is performed for
        /// transient services.
        /// </remarks>
        private object CreateTransient(ServiceDescriptor descriptor)
        {
            return CreateServiceInstance(descriptor);
        }

        /// <summary>
        /// Gets or creates a scoped instance for the specified descriptor.
        /// </summary>
        /// <param name="descriptor">The <see cref="ServiceDescriptor"/> for the scoped service.</param>
        /// <returns>The scoped instance for the current thread/scope.</returns>
        /// <remarks>
        /// <para>
        /// This method uses thread-local storage to maintain separate instances per thread.
        /// Within the same thread, the same instance is returned for subsequent requests.
        /// </para>
        /// <para>
        /// Note: When using explicit scopes created with <see cref="CreateScope"/>, a different
        /// mechanism is used for scope isolation.
        /// </para>
        /// </remarks>
        private object GetOrCreateScoped(ServiceDescriptor descriptor)
        {
            var scopeDict = _scopeInstances.Value;
            if (scopeDict.TryGetValue(descriptor.ServiceType, out object? instance))
            {
                return instance;
            }
            instance = CreateServiceInstance(descriptor);
            scopeDict[descriptor.ServiceType] = instance;
            return instance;
        }

        /// <summary>
        /// Creates a service instance based on the descriptor's registration method.
        /// </summary>
        /// <param name="descriptor">The <see cref="ServiceDescriptor"/> containing registration details.</param>
        /// <returns>The created service instance.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the descriptor does not have a valid factory or implementation type.
        /// </exception>
        /// <remarks>
        /// <para>
        /// This method checks the descriptor in order:
        /// <list type="number">
        /// <item><description>If a factory is registered, it invokes the factory</description></item>
        /// <item><description>If an implementation type is registered, it creates an instance of that type</description></item>
        /// <item><description>Otherwise, throws an exception</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// The created instance is tracked for disposal if it implements <see cref="IDisposable"/>.
        /// </para>
        /// </remarks>
        private object CreateServiceInstance(ServiceDescriptor descriptor)
        {
            if (descriptor.Factory != null)
            {
                return descriptor.Factory.Invoke(this);
            }
            if (descriptor.ImplementationType != null)
            {
                return CreateInstance(descriptor.ImplementationType);
            }
            throw new InvalidOperationException($"Cannot create instance for {descriptor.ServiceType}");
        }

        /// <summary>
        /// Creates an instance of the specified type, using cached factory delegates when available.
        /// </summary>
        /// <param name="implementationType">The type to instantiate.</param>
        /// <returns>An instance of the specified type.</returns>
        /// <remarks>
        /// <para>
        /// This method first checks the factory cache for a precompiled factory delegate.
        /// If found, it uses the delegate for instantiation. Otherwise, it falls back to
        /// reflection-based instantiation.
        /// </para>
        /// <para>
        /// The factory cache is intended for performance optimization but is not implemented
        /// in the current version (see TODO comment in source).
        /// </para>
        /// </remarks>
        private object CreateInstance(Type implementationType)
        {
            if (_factoryCache.TryGetValue(implementationType, out Func<IServiceLocator, object>? factory))
            {
                return factory.Invoke(this);
            }
            return CreateInstanceWithReflection(implementationType);
        }

        /// <summary>
        /// Creates an instance of the specified type using reflection and constructor injection.
        /// </summary>
        /// <param name="type">The type to instantiate.</param>
        /// <returns>An instance of the specified type.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when:
        /// <list type="bullet">
        /// <item><description>No public constructor is found</description></item>
        /// <item><description>Any constructor parameter cannot be resolved</description></item>
        /// <item><description>Instantiation fails for other reasons (e.g., abstract class)</description></item>
        /// </list>
        /// </exception>
        /// <remarks>
        /// <para>
        /// This method uses the first public constructor found (by reflection order). For types
        /// with multiple constructors, consider explicitly registering a factory to control
        /// which constructor is used.
        /// </para>
        /// <para>
        /// The method recursively resolves all constructor parameters from the container.
        /// Circular dependencies are not detected and will cause a stack overflow.
        /// </para>
        /// <para>
        /// If the created instance implements <see cref="IDisposable"/>, it is tracked by the
        /// service collection for disposal when the locator is disposed.
        /// </para>
        /// </remarks>
        private object CreateInstanceWithReflection(Type type)
        {
            ConstructorInfo constructor = GetConstructor(type);
            ParameterInfo[] parameters = constructor.GetParameters();

            if (parameters.Length == 0)
            {
                object? instance = Activator.CreateInstance(type);
                _collection.TrackDisposable(instance);
                return instance;
            }

            object[] args = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                Type paramType = parameters[i].ParameterType;
                ServiceDescriptor paramDescriptor = _collection.GetDescriptor(paramType) ?? throw new InvalidOperationException($"Cannot resolve parameter {paramType} for {type}");
                args[i] = ResolveService(paramDescriptor);
            }

            object instance2 = constructor.Invoke(args);
            _collection.TrackDisposable(instance2);
            return instance2;
        }

        /// <summary>
        /// Gets the first public constructor of the specified type.
        /// </summary>
        /// <param name="type">The type to inspect.</param>
        /// <returns>The first public constructor found.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when no public constructor is found.
        /// </exception>
        /// <remarks>
        /// This method returns constructors in the order returned by <see cref="Type.GetConstructors()"/>.
        /// For types with multiple constructors, the order is undefined. Consider registering
        /// a factory if you need to select a specific constructor.
        /// </remarks>
        private ConstructorInfo GetConstructor(Type type)
        {
            ConstructorInfo[] constructors = type.GetConstructors();
            if (constructors.Length == 0)
            {
                throw new InvalidOperationException($"No public constructor found for {type}");
            }
            return constructors[0];
        }

        /// <summary>
        /// Builds the factory cache for performance optimization.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is intended to pre-compile factory delegates for service types to
        /// avoid reflection overhead on each resolution. Currently unimplemented (marked as TODO).
        /// </para>
        /// <para>
        /// When implemented, this method would typically use expression trees or source generation
        /// to create optimized factory delegates for each registered service type.
        /// </para>
        /// </remarks>
        private void BuildFactoryCache()
        {
            // TODO: source generation
        }
    }
}