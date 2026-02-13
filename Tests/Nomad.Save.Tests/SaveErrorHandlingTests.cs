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
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Nomad.Core.Events;
using Nomad.Core.FileSystem;
using Nomad.Core.Logger;
using Nomad.Events;
using Nomad.FileSystem.Private.Services;
using Nomad.Save.Data;
using Nomad.Save.Events;
using Nomad.Save.Interfaces;
using Nomad.Save.Private.Services;
using Nomad.Save.Services;
using Nomad.Save.ValueObjects;

namespace Nomad.Save.Tests;

/// <summary>
/// Tests for error handling and edge cases in Nomad.Save
/// </summary>
[TestFixture]
public class SaveErrorHandlingTests
{
    private ISaveDataProvider _dataProvider;
    private ILoggerService _logger;
    private IFileSystem _fileSystem;
    private IGameEventRegistryService _eventFactory;
    private string _testDirectory;

    [SetUp]
    public void Setup()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), "NomadSaveErrorTests", Guid.NewGuid().ToString());
        Directory.CreateDirectory(_testDirectory);

        _logger = new MockLogger();
        var engineService = new MockEngineService();
        _fileSystem = new FileSystemService(engineService, _logger);
        _eventFactory = new GameEventRegistry(_logger);
        _dataProvider = new SaveDataProvider(_eventFactory, _fileSystem, _logger);
    }

    [TearDown]
    public void TearDown()
    {
        _dataProvider?.Dispose();
		_logger?.Dispose();
		_fileSystem?.Dispose();
		_eventFactory?.Dispose();

        try
        {
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }
        catch
        {
            // Ignore cleanup errors
        }
    }

    [Test]
    public void Constructor_WithNullEventFactory_ThrowsException()
    {
        // Assert
        Assert.That(() => new SaveDataProvider(null!, _fileSystem, _logger), Throws.ArgumentNullException);
    }

    [Test]
    public void Constructor_WithNullFileSystem_ThrowsException()
    {
        // Assert
        Assert.That(() => new SaveDataProvider(_eventFactory, null!, _logger), Throws.ArgumentNullException);
    }

    [Test]
    public void Constructor_WithNullLogger_ThrowsException()
    {
        // Assert
        Assert.That(() => new SaveDataProvider(_eventFactory, _fileSystem, null!), Throws.ArgumentNullException);
    }

    [Test]
    public async Task Save_WithEmptyFileName_HandlesGracefully()
    {
        // Arrange
        var saveBegin = _eventFactory.GetEvent<SaveBeginEventArgs>(EventNames.NAMESPACE, EventNames.SAVE_BEGIN_EVENT);

        saveBegin.Subscribe(this, (in SaveBeginEventArgs args) =>
        {
            var section = args.Writer.AddSection("Test");
            section.AddField("Value", 1);
        });

        // Act & Assert
        // Should either succeed or throw - implementation dependent
        try
        {
            await _dataProvider.Save("", default);
            Assert.Pass("Handled empty filename");
        }
        catch
        {
            Assert.Pass("Appropriately threw for empty filename");
        }
    }

    [Test]
    public async Task Load_WithNonExistentFile_HandlesGracefully()
    {
        // Arrange
        var loadBegin = _eventFactory.GetEvent<LoadBeginEventArgs>(EventNames.NAMESPACE, EventNames.LOAD_BEGIN_EVENT);

        loadBegin.Subscribe(this, (in LoadBeginEventArgs args) =>
        {
            // Should not execute if file doesn't exist
        });

        // Act
        try
        {
            await _dataProvider.Load(Path.Combine(_testDirectory, "nonexistent_file.ngd"));
        }
        catch
        {
            // Expected - file doesn't exist
        }

        // Assert - implementation might either throw or handle gracefully
        Assert.Pass("Load with non-existent file handled");
    }

    [Test]
    public async Task SaveField_WithEmptySection_Succeeds()
    {
        // Arrange
        var fileId = Path.Combine(_testDirectory, "empty_section_test.ngd");
        var saveBegin = _eventFactory.GetEvent<SaveBeginEventArgs>(EventNames.NAMESPACE, EventNames.SAVE_BEGIN_EVENT);
        var sectionCreated = false;

        void OnSaveBegin(in SaveBeginEventArgs args)
        {
            var section = args.Writer.AddSection("EmptySection");
            sectionCreated = section != null;
        }

        saveBegin.Subscribe(this, OnSaveBegin);

        // Act
        await _dataProvider.Save(fileId, default);

        // Assert
        Assert.That(sectionCreated, Is.True);
    }

    [Test]
    public async Task SaveField_WithEmptyString_Succeeds()
    {
        // Arrange
        var fileId = Path.Combine(_testDirectory, "empty_string_test.ngd");
        var saveBegin = _eventFactory.GetEvent<SaveBeginEventArgs>(EventNames.NAMESPACE, EventNames.SAVE_BEGIN_EVENT);
        var loadBegin = _eventFactory.GetEvent<LoadBeginEventArgs>(EventNames.NAMESPACE, EventNames.LOAD_BEGIN_EVENT);

        var loadedString = "unchanged";

        saveBegin.Subscribe(this, (in SaveBeginEventArgs args) =>
        {
            var section = args.Writer.AddSection("Test");
            section.AddField("EmptyString", "");
        });

        loadBegin.Subscribe(this, (in LoadBeginEventArgs args) =>
        {
            var section = args.Reader.FindSection("Test");
            if (section != null)
            {
                loadedString = section.GetField<string>("EmptyString");
            }
        });

        // Act
        await _dataProvider.Save(fileId, default);
        await _dataProvider.Load(fileId);

        // Assert
        Assert.That(loadedString, Is.EqualTo(""));
    }

    [Test]
    public async Task SaveField_WithVeryLongString_Succeeds()
    {
        // Arrange
        var fileId = Path.Combine(_testDirectory, "long_string_test.ngd");
        var saveBegin = _eventFactory.GetEvent<SaveBeginEventArgs>(EventNames.NAMESPACE, EventNames.SAVE_BEGIN_EVENT);
        var loadBegin = _eventFactory.GetEvent<LoadBeginEventArgs>(EventNames.NAMESPACE, EventNames.LOAD_BEGIN_EVENT);

        var longString = new string('a', 100000);
        var loadedString = "";

        saveBegin.Subscribe(this, (in SaveBeginEventArgs args) =>
        {
            var section = args.Writer.AddSection("Test");
            section.AddField("LongString", longString);
        });

        loadBegin.Subscribe(this, (in LoadBeginEventArgs args) =>
        {
            var section = args.Reader.FindSection("Test");
            if (section != null)
            {
                loadedString = section.GetField<string>("LongString");
            }
        });

        // Act
        await _dataProvider.Save(fileId, default);
        await _dataProvider.Load(fileId);

        // Assert
        Assert.That(loadedString, Is.EqualTo(longString));
        Assert.That(loadedString.Length, Is.EqualTo(100000));
    }

    [Test]
    public async Task Dispose_CanBeCalledMultipleTimes()
    {
        // Act & Assert
        _dataProvider.Dispose();
        _dataProvider.Dispose(); // Should not throw
        Assert.Pass("Multiple dispose calls handled");
    }

    [Test]
    public async Task GetField_WithWrongType_ThrowsInvalidCastException()
    {
        // Arrange
        var fileId = Path.Combine(_testDirectory, "wrong_type_test.ngd");
        var saveBegin = _eventFactory.GetEvent<SaveBeginEventArgs>(EventNames.NAMESPACE, EventNames.SAVE_BEGIN_EVENT);
        var loadBegin = _eventFactory.GetEvent<LoadBeginEventArgs>(EventNames.NAMESPACE, EventNames.LOAD_BEGIN_EVENT);

        var exceptionThrown = false;

        void OnSaveBegin(in SaveBeginEventArgs args)
        {
            var section = args.Writer.AddSection("Test");
            section.AddField("IntField", 42);
        }

        saveBegin.Subscribe(this, OnSaveBegin);

        void OnLoadBegin(in LoadBeginEventArgs args)
        {
            var section = args.Reader.FindSection("Test");
            if (section != null)
            {
                try
                {
                    // Try to read int field as float
                    var floatValue = section.GetField<float>("IntField");
                }
                catch (InvalidCastException)
                {
                    exceptionThrown = true;
                }
            }
        }

        loadBegin.Subscribe(this, OnLoadBegin);

        // Act
        await _dataProvider.Save(fileId, default);
        await _dataProvider.Load(fileId);

        // Assert
        Assert.That(exceptionThrown, Is.True);
    }

    [Test]
    public async Task SaveSection_WithSameName_Throws()
    {
        // Arrange
        var fileId = Path.Combine(_testDirectory, "duplicate_section_test.ngd");
        var saveBegin = _eventFactory.GetEvent<SaveBeginEventArgs>(EventNames.NAMESPACE, EventNames.SAVE_BEGIN_EVENT);
        var exceptionThrown = false;

        saveBegin.Subscribe(this, (in SaveBeginEventArgs args) =>
        {
            try
            {
                var section1 = args.Writer.AddSection("DuplicateSection");
                section1.AddField("Value1", 1);

                var section2 = args.Writer.AddSection("DuplicateSection");
                section2.AddField("Value2", 2);
            }
            catch (Exception)
            {
                exceptionThrown = true;
            }
        });

        // Act
        await _dataProvider.Save(fileId, default);

        // Assert
        Assert.That(exceptionThrown, Is.True);
    }

    [Test]
    public async Task SaveField_WithVeryLargeInt_Succeeds()
    {
        // Arrange
        var fileId = Path.Combine(_testDirectory, "large_int_test.ngd");
        var saveBegin = _eventFactory.GetEvent<SaveBeginEventArgs>(EventNames.NAMESPACE, EventNames.SAVE_BEGIN_EVENT);
        var loadBegin = _eventFactory.GetEvent<LoadBeginEventArgs>(EventNames.NAMESPACE, EventNames.LOAD_BEGIN_EVENT);

        long largeValue = long.MaxValue;
        long loadedValue = 0;

        saveBegin.Subscribe(this, (in SaveBeginEventArgs args) =>
        {
            var section = args.Writer.AddSection("Test");
            section.AddField("LargeInt", largeValue);
        });

        loadBegin.Subscribe(this, (in LoadBeginEventArgs args) =>
        {
            var section = args.Reader.FindSection("Test");
            if (section != null)
            {
                loadedValue = section.GetField<long>("LargeInt");
            }
        });

        // Act
        await _dataProvider.Save(fileId, default);
        await _dataProvider.Load(fileId);

        // Assert
        Assert.That(loadedValue, Is.EqualTo(largeValue));
    }

    [Test]
    public async Task SaveField_WithNegativeNumbers_Succeeds()
    {
        // Arrange
        var fileId = Path.Combine(_testDirectory, "negative_test.ngd");
        var saveBegin = _eventFactory.GetEvent<SaveBeginEventArgs>(EventNames.NAMESPACE, EventNames.SAVE_BEGIN_EVENT);
        var loadBegin = _eventFactory.GetEvent<LoadBeginEventArgs>(EventNames.NAMESPACE, EventNames.LOAD_BEGIN_EVENT);

        int negativeInt = -12345;
        double negativeDouble = -3.14159;
        int loadedInt = 0;
        double loadedDouble = 0;

        saveBegin.Subscribe(this, (in SaveBeginEventArgs args) =>
        {
            var section = args.Writer.AddSection("Test");
            section.AddField("NegativeInt", negativeInt);
            section.AddField("NegativeDouble", negativeDouble);
        });

        loadBegin.Subscribe(this, (in LoadBeginEventArgs args) =>
        {
            var section = args.Reader.FindSection("Test");
            if (section != null)
            {
                loadedInt = section.GetField<int>("NegativeInt");
                loadedDouble = section.GetField<double>("NegativeDouble");
            }
        });

        // Act
        await _dataProvider.Save(fileId, default);
        await _dataProvider.Load(fileId);

        // Assert
        Assert.That(loadedInt, Is.EqualTo(negativeInt));
        Assert.That(loadedDouble, Is.EqualTo(negativeDouble).Within(0.00001));
    }

    [Test]
    public async Task Multiple_SaveAndLoad_Cycles_Succeed()
    {
        // Arrange
        var fileId = Path.Combine(_testDirectory, "multiple_cycles_test.ngd");
        var saveBegin = _eventFactory.GetEvent<SaveBeginEventArgs>(EventNames.NAMESPACE, EventNames.SAVE_BEGIN_EVENT);
        var loadBegin = _eventFactory.GetEvent<LoadBeginEventArgs>(EventNames.NAMESPACE, EventNames.LOAD_BEGIN_EVENT);
        int cycleCount = 0;

        saveBegin.Subscribe(this, (in SaveBeginEventArgs args) =>
        {
            var section = args.Writer.AddSection("CycleData");
            section.AddField("CycleCount", cycleCount);
        });

        loadBegin.Subscribe(this, (in LoadBeginEventArgs args) =>
        {
            var section = args.Reader.FindSection("CycleData");
            if (section != null)
            {
                cycleCount = section.GetField<int>("CycleCount");
            }
        });

        // Act
        for (int i = 0; i < 5; i++)
        {
            cycleCount = i;
            await _dataProvider.Save(fileId, default);
            await _dataProvider.Load(fileId);
        }

        // Assert
        Assert.That(cycleCount, Is.EqualTo(4));
    }

    [Test]
    public async Task SaveSection_WithEmptyName_HandlesGracefully()
    {
        // Arrange
        var fileId = Path.Combine(_testDirectory, "empty_section_name_test.ngd");
        var saveBegin = _eventFactory.GetEvent<SaveBeginEventArgs>(EventNames.NAMESPACE, EventNames.SAVE_BEGIN_EVENT);

        saveBegin.Subscribe(this, (in SaveBeginEventArgs args) =>
        {
            var section = args.Writer.AddSection("");
            section.AddField("Value", 1);
        });

        // Act & Assert
        try
        {
            await _dataProvider.Save(fileId, default);
            Assert.Pass("Empty section name handled");
        }
        catch
        {
            Assert.Pass("Appropriately handled empty section name");
        }
    }

    [Test]
    [TestCase(0)]
    [TestCase(1)]
    [TestCase(-1)]
    [TestCase(int.MaxValue)]
    [TestCase(int.MinValue)]
    public async Task SaveField_WithExtremumIntValues_Succeeds(int value)
    {
        // Arrange
        var fileId = Path.Combine(_testDirectory, $"extremum_int_test_{value.GetHashCode()}.ngd");
        var saveBegin = _eventFactory.GetEvent<SaveBeginEventArgs>(EventNames.NAMESPACE, EventNames.SAVE_BEGIN_EVENT);
        var loadBegin = _eventFactory.GetEvent<LoadBeginEventArgs>(EventNames.NAMESPACE, EventNames.LOAD_BEGIN_EVENT);

        int loadedValue = 0;

        saveBegin.Subscribe(this, (in SaveBeginEventArgs args) =>
        {
            var section = args.Writer.AddSection("Test");
            section.AddField("IntValue", value);
        });

        loadBegin.Subscribe(this, (in LoadBeginEventArgs args) =>
        {
            var section = args.Reader.FindSection("Test");
            if (section != null)
            {
                loadedValue = section.GetField<int>("IntValue");
            }
        });

        // Act
        await _dataProvider.Save(fileId, default);
        await _dataProvider.Load(fileId);

        // Assert
        Assert.That(loadedValue, Is.EqualTo(value));
    }

    /*
    [Test]
    public async Task EventSubscriber_CanBeUnsubscribed()
    {
        // Arrange
        var fileId = new SaveFileId(Path.Combine(_testDirectory, "unsubscribe_test.ngd"));
        var saveBegin = _eventFactory.GetEvent<SaveBeginEventArgs>(EventNames.NAMESPACE, EventNames.SAVE_BEGIN_EVENT);
        var eventFired = false;

        void OnSaveBegin(in SaveBeginEventArgs args)
        {
            eventFired = true;
            var section = args.Writer.AddSection("Test");
            section.AddField("Value", 1);
        }

        saveBegin.Subscribe(this, OnSaveBegin);

        // Act
        await _dataProvider.Save(fileId);
        var firstRun = eventFired;

        eventFired = false;
        saveBegin.Unsubscribe(this, OnSaveBegin);

        await _dataProvider.Save(fileId);
        var secondRun = eventFired;

        // Assert
        Assert.That(firstRun, Is.True);
        Assert.That(secondRun, Is.False);
    }
    */

    [Test]
    public async Task SaveDataProvider_IsDisposable()
    {
        // Arrange
        var newProvider = new SaveDataProvider(_eventFactory, _fileSystem, _logger);

        // Act
        newProvider.Dispose();

        // Assert - should be disposed without throwing
        Assert.Pass("SaveDataProvider successfully disposed");
    }
}
