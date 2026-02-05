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
using Nomad.Core.ServiceRegistry.Services;

namespace Nomad.Core.ServiceRegistry.Interfaces
{
    /// <summary>
    /// Defines a contract for registering and managing service dependencies in the dependency injection container.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="IServiceRegistry"/> interface provides the foundational building blocks for configuring
    /// the dependency injection system in the Nomad Framework. It allows services to be registered with
    /// various lifetime scopes (singleton, scoped, or transient) and supports different registration patterns
    /// including type-based, factory-based, and instance-based registrations.
    /// </para>
    /// <para>
    /// This interface typically serves as the configuration API used during application startup or module
    /// initialization, while <see cref="IServiceLocator"/> serves as the runtime resolution API.
    /// </para>
    /// <para>
    /// Implementations should ensure thread safety for registration operations if the registry may be
    /// accessed concurrently during application initialization.
    /// </para>
    /// <example>
    /// The following example demonstrates typical service registration patterns:
    /// <code>
    /// // Register a service with its implementation and lifetime
    /// registry.Register&lt;IMyService, MyServiceImpl&gt;(ServiceLifetime.Scoped);
    /// 
    /// // Register a singleton instance
    /// registry.RegisterSingleton&lt;IMySingleton&gt;(new MySingleton());
    /// 
    /// // Register using a factory method
    /// registry.Register&lt;IFactoryService&gt;(locator => 
    ///     new FactoryService(locator.GetService&lt;IDependency&gt;()), 
    ///     ServiceLifetime.Transient);
    /// 
    /// // Check if a service is already registered
    /// if (!registry.IsRegistered&lt;IMyService&gt;())
    /// {
    ///     registry.Register&lt;IMyService, DefaultImpl&gt;(ServiceLifetime.Singleton);
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    public interface IServiceRegistry
    {
        /// <summary>
        /// Registers a service type with its implementation type and specified lifetime.
        /// </summary>
        /// <typeparam name="TService">The service type to register. Must be a reference type.</typeparam>
        /// <typeparam name="TImplementation">The implementation type. Must be a reference type and implement <typeparamref name="TService"/>.</typeparam>
        /// <param name="lifetime">The <see cref="ServiceLifetime"/> that determines how instances are created and cached.</param>
        /// <returns>The service registry instance for method chaining.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown when <typeparamref name="TImplementation"/> does not implement <typeparamref name="TService"/>,
        /// or when either type is not a concrete, instantiable class (for non-factory registrations).
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the service registry is read-only (typically after the first service resolution).
        /// </exception>
        /// <remarks>
        /// <para>
        /// This method registers a service using its implementation type. The container will use reflection
        /// to create instances of <typeparamref name="TImplementation"/> when needed, resolving its constructor
        /// dependencies from the container.
        /// </para>
        /// <para>
        /// The <paramref name="lifetime"/> parameter controls the instance management:
        /// <list type="bullet">
        /// <item><description><see cref="ServiceLifetime.Singleton"/>: A single instance is created and reused.</description></item>
        /// <item><description><see cref="ServiceLifetime.Scoped"/>: One instance per service scope.</description></item>
        /// <item><description><see cref="ServiceLifetime.Transient"/>: A new instance is created each time.</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// If <typeparamref name="TService"/> is already registered, this registration will typically
        /// replace the existing registration. The exact behavior may vary by implementation.
        /// </para>
        /// </remarks>
        TService Register<TService, TImplementation>(ServiceLifetime lifetime)
            where TService : class
            where TImplementation : class, TService;

        /// <summary>
        /// Registers a service using a factory function with the specified lifetime.
        /// </summary>
        /// <typeparam name="TService">The service type to register. Must be a reference type.</typeparam>
        /// <param name="factory">A factory function that creates instances of <typeparamref name="TService"/>.</param>
        /// <param name="lifetime">The <see cref="ServiceLifetime"/> that determines how instances are created and cached.</param>
        /// <returns>The service registry instance for method chaining.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="factory"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the service registry is read-only (typically after the first service resolution).
        /// </exception>
        /// <remarks>
        /// <para>
        /// This method allows custom instantiation logic for a service. The factory function receives
        /// an <see cref="IServiceLocator"/> that can be used to resolve dependencies, but care should
        /// be taken to avoid service locator anti-patterns within the factory.
        /// </para>
        /// <para>
        /// Factory registration is useful when:
        /// <list type="bullet">
        /// <item><description>The service requires complex initialization logic</description></item>
        /// <item><description>The service implementation depends on runtime values</description></item>
        /// <item><description>You need to wrap or decorate an existing service</description></item>
        /// <item><description>The service implements multiple interfaces and you need to choose one</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// The factory function will be called each time a new instance is needed according to the
        /// specified <paramref name="lifetime"/>.
        /// </para>
        /// </remarks>
        TService Register<TService>(Func<IServiceLocator, TService> factory, ServiceLifetime lifetime)
            where TService : class;

        /// <summary>
        /// Registers a pre-existing instance as a singleton service.
        /// </summary>
        /// <typeparam name="TService">The service type to register. Must be a reference type.</typeparam>
        /// <param name="instance">The instance to register as a singleton.</param>
        /// <returns>The service registry instance for method chaining.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="instance"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the service registry is read-only (typically after the first service resolution).
        /// </exception>
        /// <remarks>
        /// <para>
        /// This method registers an already-created instance as a singleton service. The instance
        /// will be returned for all subsequent requests for <typeparamref name="TService"/>.
        /// </para>
        /// <para>
        /// The container will not manage the lifecycle of the instance (it will not call Dispose
        /// on the instance even if it implements <see cref="IDisposable"/>). It is the caller's
        /// responsibility to manage the instance's lifetime.
        /// </para>
        /// <para>
        /// This method is useful for:
        /// <list type="bullet">
        /// <item><description>Registering configuration objects</description></item>
        /// <item><description>Registering services that require specific initialization parameters</description></item>
        /// <item><description>Integrating third-party objects into the DI container</description></item>
        /// <item><description>Registering services that implement multiple interfaces with specific instances</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        TService RegisterSingleton<TService>(TService instance)
            where TService : class;

        /// <summary>
        /// Registers a service type with its implementation type as a singleton.
        /// </summary>
        /// <typeparam name="TService">The service type to register. Must be a reference type.</typeparam>
        /// <typeparam name="TImplementation">The implementation type. Must be a reference type and implement <typeparamref name="TService"/>.</typeparam>
        /// <returns>The service registry instance for method chaining.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown when <typeparamref name="TImplementation"/> does not implement <typeparamref name="TService"/>,
        /// or when either type is not a concrete, instantiable class.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the service registry is read-only (typically after the first service resolution).
        /// </exception>
        /// <remarks>
        /// <para>
        /// This is a convenience method equivalent to calling <c>Register&lt;TService, TImplementation&gt;(ServiceLifetime.Singleton)</c>.
        /// It registers a singleton service where the container creates and manages a single instance
        /// of <typeparamref name="TImplementation"/>.
        /// </para>
        /// <para>
        /// Unlike <see cref="RegisterSingleton{TService}(TService)"/>, this method lets the container
        /// manage the instance lifecycle (including calling <see cref="IDisposable.Dispose"/> if the
        /// implementation implements it).
        /// </para>
        /// </remarks>
        TService RegisterSingleton<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService;

        /// <summary>
        /// Determines whether a service type is registered in the registry.
        /// </summary>
        /// <typeparam name="TService">The service type to check. Must be a reference type.</typeparam>
        /// <returns>
        /// <see langword="true"/> if <typeparamref name="TService"/> is registered; otherwise, <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method checks if there is at least one registration for the specified service type.
        /// It does not check for multiple registrations or the specific implementation details.
        /// </para>
        /// <para>
        /// This method is useful for conditional registration logic, where you might want to
        /// register a default implementation only if no other implementation has been registered.
        /// </para>
        /// <para>
        /// Note: Some service containers support multiple registrations for the same service type
        /// (for <see cref="IServiceLocator.GetServices{TService}"/>). This method returns <see langword="true"/>
        /// if there is at least one registration.
        /// </para>
        /// </remarks>
        bool IsRegistered<TService>()
            where TService : class;

        /// <summary>
        /// Gets all service descriptors registered in this registry.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="ServiceDescriptor"/> objects representing
        /// all service registrations.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method provides access to the raw service registration data. Each
        /// <see cref="ServiceDescriptor"/> contains information about a registered service,
        /// including its service type, implementation details, and lifetime.
        /// </para>
        /// <para>
        /// This method is primarily intended for:
        /// <list type="bullet">
        /// <item><description>Diagnostic and debugging purposes</description></item>
        /// <item><description>Implementing advanced container features</description></item>
        /// <item><description>Building tools that analyze or visualize the dependency graph</description></item>
        /// <item><description>Creating container snapshots for testing</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// The returned enumerable represents a snapshot of the registry at the time of calling.
        /// Subsequent modifications to the registry will not be reflected in the returned collection.
        /// </para>
        /// </remarks>
        IEnumerable<ServiceDescriptor> GetDescriptors();
    }
}