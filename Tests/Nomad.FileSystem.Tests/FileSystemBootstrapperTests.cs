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

using Nomad.Core.FileSystem;
using NUnit.Framework;
using Nomad.Core.ServiceRegistry.Interfaces;
using Nomad.Core.ServiceRegistry.Services;
using System;
using Nomad.Core.EngineUtils;
using Nomad.Core.Logger;

namespace Nomad.FileSystem.Tests
{
	[TestFixture]
	[Category("UnitTests")]
	public class FileSystemBootstrapperTests
	{
		private FileSystemBootstrapper _bootstrapper;
		private IServiceLocator _serviceLocator;
		private ServiceCollection _serviceRegistry;

		[SetUp]
		public void Setup()
		{
			_serviceRegistry = new ServiceCollection();
			_serviceLocator = new ServiceLocator(_serviceRegistry);

			_bootstrapper = new FileSystemBootstrapper();
		}

		[TearDown]
		public void TearDown()
		{
			_serviceLocator?.Dispose();
		}

		[Test]
		public void CreateBootstrapper_InitializeWithValidServices_DoesNotThrow()
		{
			_serviceRegistry.RegisterSingleton<IEngineService>(new MockEngineService());
			_serviceRegistry.RegisterSingleton<ILoggerService>(new MockLogger());

			Assert.DoesNotThrow(() => _bootstrapper.Initialize(_serviceRegistry, _serviceLocator));
		}

		[Test]
		public void CreateBootstrapper_InitializeWithoutExistingLoggerService_ThrowsInvalidOperationException()
		{
			_serviceRegistry.RegisterSingleton<IEngineService>(new MockEngineService());

			Assert.Throws<InvalidOperationException>(() => _bootstrapper.Initialize(_serviceRegistry, _serviceLocator));
		}

		[Test]
		public void CreateBootstrapper_InitializeWithoutExistingEngineService_ThrowsInvalidOperationException()
		{
			_serviceRegistry.RegisterSingleton<ILoggerService>(new MockLogger());

			Assert.Throws<InvalidOperationException>(() => _bootstrapper.Initialize(_serviceRegistry, _serviceLocator));
		}

		[Test]
		public void CreateBootstrapper_WithNullServiceRegistry_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => _bootstrapper.Initialize(null!, _serviceLocator));
		}

		[Test]
		public void CreateBootstrapper_WithNullServiceLocator_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => _bootstrapper.Initialize(_serviceRegistry, null!));
		}

		[Test]
		public void CreateBootstrapper_GetService_DoesNotThrow()
		{
			_serviceRegistry.RegisterSingleton<IEngineService>(new MockEngineService());
			_serviceRegistry.RegisterSingleton<ILoggerService>(new MockLogger());

			_bootstrapper.Initialize(_serviceRegistry, _serviceLocator);

			Assert.DoesNotThrow(() => _serviceLocator.GetService<IFileSystem>());
		}

		[Test]
		public void CreateBootstrapper_AndShutdownBootstrapper_DoesNotThrow()
		{
			// Arrange
			_serviceRegistry.RegisterSingleton<IEngineService>(new MockEngineService());
			_serviceRegistry.RegisterSingleton<ILoggerService>(new MockLogger());
			_bootstrapper.Initialize(_serviceRegistry, _serviceLocator);

			// Assert
			Assert.DoesNotThrow(() => _bootstrapper.Shutdown());
		}

		[Test]
		public void ShutdownBeforeInitialize_DoesNotThrow()
		{
			// Assert
			Assert.DoesNotThrow(() => _bootstrapper.Shutdown());
		}

		[Test]
		public void CreateBootstrapper_InitializeThenGetService_DoesNotThrow()
		{
			// Arrange
			_serviceRegistry.RegisterSingleton<IEngineService>(new MockEngineService());
			_serviceRegistry.RegisterSingleton<ILoggerService>(new MockLogger());

			// Act
			_bootstrapper.Initialize(_serviceRegistry, _serviceLocator);

			// Assert
			Assert.DoesNotThrow(() => _serviceLocator.GetService<IFileSystem>());
		}
	}
}