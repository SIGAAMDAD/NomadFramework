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

namespace Nomad.Core.ServiceRegistry.Interfaces
{
    /// <summary>
    /// Provides a mechanism to locate and retrieve service instances from a service registry.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="IServiceLocator"/> interface defines the core functionality for
    /// dependency injection and service location within the Nomad Framework. It serves
    /// as the primary entry point for accessing registered services at runtime.
    /// </para>
    /// <para>
    /// Implementations of this interface typically work in conjunction with an
    /// <see cref="IServiceRegistry"/> to resolve service dependencies based on their
    /// registration configuration (singleton, scoped, or transient).
    /// </para>
    /// <para>
    /// This interface extends <see cref="IDisposable"/> to allow proper cleanup of
    /// resources, particularly for scoped service locators that may need to dispose
    /// of scoped services.
    /// </para>
    /// <example>
    /// The following example demonstrates typical usage:
    /// <code>
    /// // Get a service instance
    /// var service = serviceLocator.GetService&lt;IMyService&gt;();
    /// 
    /// // Try to get a service (safe for optional dependencies)
    /// if (serviceLocator.TryGetService&lt;IOptionalService&gt;(out var optionalService))
    /// {
    ///     optionalService.DoSomething();
    /// }
    /// 
    /// // Create a scope for scoped services
    /// using (var scope = serviceLocator.CreateScope())
    /// {
    ///     var scopedService = scope.GetService&lt;IScopedService&gt;();
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    public interface IServiceLocator : IDisposable
    {
        /// <summary>
        /// Gets the service registry associated with this service locator.
        /// </summary>
        /// <value>
        /// An <see cref="IServiceRegistry"/> instance that contains the service
        /// registrations and configuration used by this locator.
        /// </value>
        /// <remarks>
        /// This property provides access to the underlying registry for advanced
        /// scenarios such as dynamic registration or configuration inspection.
        /// Modifying the registry after service resolution has begun may result
        /// in undefined behavior.
        /// </remarks>
        IServiceRegistry Collection { get; }

        /// <summary>
        /// Gets an instance of the specified service type.
        /// </summary>
        /// <typeparam name="TService">The type of service to retrieve. Must be a reference type.</typeparam>
        /// <returns>An instance of type <typeparamref name="TService"/>.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when no service of type <typeparamref name="TService"/> is registered,
        /// or when the service cannot be resolved due to dependency resolution failures.
        /// </exception>
        /// <remarks>
        /// <para>
        /// This method resolves the service based on its registration in the service registry.
        /// The lifetime of the returned instance (singleton, scoped, or transient) depends
        /// on how the service was registered.
        /// </para>
        /// <para>
        /// For optional dependencies, consider using <see cref="TryGetService{TService}(out TService)"/>
        /// instead to avoid exceptions when a service is not available.
        /// </para>
        /// </remarks>
        TService GetService<TService>() where TService : class;

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
        /// This method provides a non-throwing alternative to <see cref="GetService{TService}"/> for
        /// optional dependencies. It returns <see langword="false"/> instead of throwing an exception
        /// when the service cannot be resolved.
        /// </para>
        /// <para>
        /// Note that this method may still throw exceptions for resolution failures other than
        /// "service not found", such as circular dependencies or constructor failures.
        /// </para>
        /// </remarks>
        bool TryGetService<TService>(out TService service) where TService : class;

        /// <summary>
        /// Gets all instances of the specified service type.
        /// </summary>
        /// <typeparam name="TService">The type of service to retrieve. Must be a reference type.</typeparam>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> containing all registered instances of type <typeparamref name="TService"/>.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method is useful when multiple implementations of a service interface are registered
        /// and you need to work with all of them (e.g., for plugin systems or event handlers).
        /// </para>
        /// <para>
        /// If no services of type <typeparamref name="TService"/> are registered, an empty
        /// enumerable is returned (not <see langword="null"/>).
        /// </para>
        /// <para>
        /// Each call to this method may return new instances for transient registrations,
        /// depending on the service lifetime configuration.
        /// </para>
        /// </remarks>
        IEnumerable<TService> GetServices<TService>() where TService : class;

        /// <summary>
        /// Creates a new service scope.
        /// </summary>
        /// <returns>A new <see cref="IServiceScope"/> instance.</returns>
        /// <remarks>
        /// <para>
        /// A service scope creates an isolated context for scoped services. Services registered
        /// as scoped are created once per scope and are disposed when the scope is disposed.
        /// </para>
        /// <para>
        /// It is the caller's responsibility to dispose of the returned scope when it is no
        /// longer needed. The recommended pattern is to use a <see langword="using"/> statement:
        /// <code>
        /// using (var scope = serviceLocator.CreateScope())
        /// {
        ///     var scopedService = scope.GetService&lt;IScopedService&gt;();
        ///     // Use scopedService...
        /// } // Scope and scoped services are disposed here
        /// </code>
        /// </para>
        /// <para>
        /// Scopes are particularly useful for web requests, unit of work patterns, or any
        /// scenario where you need isolated service instances with a controlled lifetime.
        /// </para>
        /// </remarks>
        IServiceScope CreateScope();

        /// <summary>
        /// Creates a new instance of the specified service type without using the service registry's caching.
        /// </summary>
        /// <typeparam name="TService">The type of service to create. Must be a reference type.</typeparam>
        /// <returns>A new instance of type <typeparamref name="TService"/>.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the service cannot be created due to missing dependencies or other resolution failures.
        /// </exception>
        /// <remarks>
        /// <para>
        /// This method bypasses any lifetime management (singleton/scoped caching) and always
        /// creates a new instance. It is useful for:
        /// <list type="bullet">
        /// <item><description>Creating multiple independent instances of a service</description></item>
        /// <item><description>Testing scenarios where you need fresh instances</description></item>
        /// <item><description>Situations where you need to manage the lifetime manually</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// Unlike <see cref="GetService{TService}"/>, this method does not return cached
        /// singleton or scoped instances. Each call creates a new instance.
        /// </para>
        /// <para>
        /// The created instance will have its dependencies resolved from the service locator,
        /// but the instance itself is not tracked for disposal by the locator (unless its
        /// dependencies implement <see cref="IDisposable"/> and are managed by the container).
        /// </para>
        /// </remarks>
        TService CreateInstance<TService>() where TService : class;
    }
}