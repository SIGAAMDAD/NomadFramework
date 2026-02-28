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

#if !UNITY_EDITOR
using System;
using Nomad.Core.EngineUtils;
using Nomad.Core.Events;
using Nomad.Core.FileSystem;
using Nomad.Core.Logger;
using Nomad.CVars.Private.Services;
using Nomad.Core.CVars;
using Nomad.Events;
using Nomad.FileSystem.Private.Services;
using NUnit.Framework;

namespace Nomad.CVars.Tests
{
    [TestFixture]
    public class CVarSystemServiceTests
    {
        private CVarSystem _cvarSystem;
        private IGameEventRegistryService _registry;
        private IEngineService _engineService;
        private IFileSystem _fileSystem;
        private ILoggerService _logger;

        [SetUp]
        public void Setup()
        {
            _logger = new MockLogger();
            _registry = new GameEventRegistry(_logger);
            _engineService = new MockEngineService();
            _fileSystem = new FileSystemService(_engineService, _logger);
            _cvarSystem = new CVarSystem(_registry, _fileSystem, _logger);
        }

        [TearDown]
        public void TearDown()
        {
            _logger?.Dispose();
            _registry?.Dispose();
            _fileSystem?.Dispose();
            _cvarSystem?.Dispose();
            _engineService?.Dispose();
        }

        #region CVar Registration Tests

        [Test]
        public void RegisterCVar_WithNullParameters_ThrowsArgumentNullException()
        {
            // Assert
            Assert.Throws<ArgumentNullException>(
                () => _cvarSystem.Register(new CVarCreateInfo<int>
                {
                    Name = null!,
                    DefaultValue = default,
                    Description = null!
                })
            );
        }

        [Test]
        public void RegisterCVar_WithEmptyParameters_ThrowsArgumentException()
        {
            // Assert
            Assert.Throws<ArgumentException>(
                () => _cvarSystem.Register(new CVarCreateInfo<int>
                {
                    Name = string.Empty,
                    DefaultValue = default,
                    Description = string.Empty
                })
            );
        }

        [Test]
        public void RegisterCVar_CalledTwiceWithSameTypeAndParameters_ReturnsSameInstance()
        {
            // Arrange
            var createInfo = new CVarCreateInfo<int>
            {
                Name = "TestCVar",
                DefaultValue = default,
                Description = "A test cvar."
            };

            // Act
            var cvar1 = _cvarSystem.Register(createInfo);
            var cvar2 = _cvarSystem.Register(createInfo);

            // Assert
            Assert.That(cvar1, Is.SameAs(cvar2));
        }

        [Test]
        public void RegisterCVar_CalledTwiceWithDifferentTypeSameParameters_ThrowsInvalidCast()
        {
            // Arrange
            var createInfo1 = new CVarCreateInfo<int>
            {
                Name = "TestCVar",
                DefaultValue = default,
                Description = "A test cvar."
            };
            var createInfo2 = new CVarCreateInfo<float>
            {
                Name = "TestCVar",
                DefaultValue = default,
                Description = "A test cvar."
            };

            // Act
            _cvarSystem.Register(createInfo1);

            // Assert
            Assert.Throws<InvalidCastException>(
                () => _cvarSystem.Register(createInfo2)
            );
        }

        [Test]
        [TestCase<bool>(false)]
        [TestCase<string>("")]
        [TestCase<uint>(0)]
        [TestCase<int>(0)]
        [TestCase<float>(0.0f)]
        public void RegisterCVar_CreatedWithExplicitType_CanBeFoundWithExplicitTypeSearch<T>(T value)
        {
            // Arrange
            var createInfo = new CVarCreateInfo<T>
            {
                Name = "TestCVar",
                DefaultValue = value,
                Description = "A test cvar."
            };

            // Act
            _cvarSystem.Register(createInfo);

            // Assert
            Assert.That(_cvarSystem.CVarExists<T>("TestCVar"), Is.True);
        }

        [Test]
        [TestCase<bool>(false)]
        [TestCase<uint>(0)]
        [TestCase<int>(0)]
        [TestCase<float>(0.0f)]
        [TestCase<string>("")]
        public void RegisterCVar_CreatedWithExplicitType_CanBeFoundWithNameOnly<T>(T value)
        {
            // Arrange
            var createInfo = new CVarCreateInfo<T>
            {
                Name = "TestCVar",
                DefaultValue = default!,
                Description = "A test cvar."
            };

            // Act
            _cvarSystem.Register(createInfo);

            // Assert
            Assert.That(_cvarSystem.CVarExists("TestCVar"), Is.True);
        }

        [Test]
        public void RegisterCVar_CalledTwiceWithDifferentTypes_ThrowsInvalidCastException()
        {
            // Arrange
            var createInfo1 = new CVarCreateInfo<int>
            {
                Name = "TestCVar",
                DefaultValue = default,
                Description = "A test cvar."
            };
            var createInfo2 = new CVarCreateInfo<float>
            {
                Name = "TestCVar",
                DefaultValue = default,
                Description = "A test cvar."
            };

            // Act
            _cvarSystem.Register(createInfo1);

            // Assert
            Assert.Throws<InvalidCastException>(
                () => _cvarSystem.Register(createInfo2)
            );
        }

        #endregion

        #region CVar Unregistration Tests

        [Test]
        public void UnregisterCVar_CalledWithNullValue_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(
                () => _cvarSystem.Unregister(null!)
            );
        }

        [Test]
        public void UnregisterCVar_CalledWithValueCVar_DoesntExist()
        {
            // Arrange
            var createInfo = new CVarCreateInfo<int>
            {
                Name = "TestCVar",
                DefaultValue = default,
                Description = "A test cvar."
            };

            // Act
            var cvar = _cvarSystem.Register(createInfo);
            _cvarSystem.Unregister(cvar);

            // Assert
            Assert.That(!_cvarSystem.CVarExists<int>(cvar.Name));
        }

        #endregion
    }
}
#endif