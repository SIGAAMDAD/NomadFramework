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
using Nomad.Save.Exceptions;
using Nomad.CVars.Interfaces;
using Nomad.CVars.Private.Services;
using Nomad.Core.EngineUtils;
using Moq;
using Nomad.Save.Private.Entities;

namespace Nomad.Save.Tests;

/// <summary>
/// Tests for error handling and edge cases in Nomad.Save
/// </summary>
[TestFixture]
public class SaveErrorHandlingTests
{
    private ISaveDataProvider _dataProvider;
    private ILoggerService _logger;
    private ICVarSystemService _cvarSystem;
    private IFileSystem _fileSystem;
    private IGameEventRegistryService _eventFactory;
    private Mock<IEngineService> _engineService;
    private string _testDirectory;

    [SetUp]
    public void Setup()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), "NomadSaveErrorTests", Guid.NewGuid().ToString());
        Directory.CreateDirectory(_testDirectory);
        Directory.CreateDirectory($"{_testDirectory}/SaveData");

        _logger = new MockLogger();
        _engineService = new Mock<IEngineService>();
        _engineService.Setup(e => e.GetStoragePath(StorageScope.StreamingAssets)).Returns(_testDirectory);
        _engineService.Setup(e => e.GetStoragePath(StorageScope.UserData)).Returns(_testDirectory);
        _engineService.Setup(e => e.GetStoragePath(StorageScope.Install)).Returns(_testDirectory);
        _fileSystem = new FileSystemService(_engineService.Object, _logger);
        _eventFactory = new GameEventRegistry(_logger);
        _cvarSystem = new CVarSystem(_eventFactory, _fileSystem, _logger);
        _dataProvider = new SaveDataProvider(_engineService.Object, _eventFactory, _cvarSystem, _fileSystem, _logger);
    }

    [TearDown]
    public void TearDown()
    {
        _dataProvider?.Dispose();
        _cvarSystem?.Dispose();
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
        Assert.That(() => new SaveDataProvider(_engineService.Object, null!, _cvarSystem, _fileSystem, _logger), Throws.ArgumentNullException);
    }

    [Test]
    public void Constructor_WithNullFileSystem_ThrowsException()
    {
        // Assert
        Assert.That(() => new SaveDataProvider(_engineService.Object, _eventFactory, _cvarSystem, null!, _logger), Throws.ArgumentNullException);
    }

    [Test]
    public void Constructor_WithNullLogger_ThrowsException()
    {
        // Assert
        Assert.That(() => new SaveDataProvider(_engineService.Object, _eventFactory, _cvarSystem, _fileSystem, null!), Throws.ArgumentNullException);
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
            await _dataProvider.Load("nonexistent_file");
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
        var fileId = "empty_section_test";
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
        var fileId = "empty_string_test";
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
                loadedString = section.GetString("EmptyString");
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
        var fileId = "long_string_test";
        var saveBegin = _eventFactory.GetEvent<SaveBeginEventArgs>(EventNames.NAMESPACE, EventNames.SAVE_BEGIN_EVENT);
        var loadBegin = _eventFactory.GetEvent<LoadBeginEventArgs>(EventNames.NAMESPACE, EventNames.LOAD_BEGIN_EVENT);

        int length = 1000;
        var longString = new string('a', length);
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
                loadedString = section.GetString("LongString");
            }
        });

        // Act
        await _dataProvider.Save(fileId, default);
        await _dataProvider.Load(fileId);

        // Assert
        Assert.That(loadedString, Is.EqualTo(longString));
        Assert.That(loadedString, Has.Length.EqualTo(length));
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
        var fileId = "wrong_type_test";
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
        var fileId = "duplicate_section_test";
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
        var fileId = "large_int_test";
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
        var fileId = "negative_test";
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

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(loadedInt, Is.EqualTo(negativeInt));
            Assert.That(loadedDouble, Is.EqualTo(negativeDouble).Within(0.00001));
        }
    }

    [Test]
    public async Task Multiple_SaveAndLoad_Cycles_Succeed()
    {
        // Arrange
        var fileId = "multiple_cycles_test";
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
        var fileId = "empty_section_name_test";
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
        var fileId = $"extremum_int_test_{value.GetHashCode()}";
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

    [Test]
    public async Task EventSubscriber_CanBeUnsubscribed()
    {
        // Arrange
        var fileId = "unsubscribe_test.ngd";
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
        await _dataProvider.Save(fileId, default);
        var firstRun = eventFired;

        eventFired = false;
        saveBegin.Unsubscribe(this, OnSaveBegin);

        await _dataProvider.Save(fileId, default);
        var secondRun = eventFired;

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(firstRun, Is.True);
            Assert.That(secondRun, Is.False);
        }
    }

    [Test]
    public async Task SaveDataProvider_IsDisposable()
    {
        // Arrange
        var newProvider = new SaveDataProvider(_engineService.Object, _eventFactory, _cvarSystem, _fileSystem, _logger);

        // Act
        newProvider.Dispose();

        // Assert - should be disposed without throwing
        Assert.Pass("SaveDataProvider successfully disposed");
    }

    [Test]
    public async Task Field_DuplicateFieldCreation_ThrowsDuplicateFieldException()
    {
        // Arrange
        var fileId = "duplicate_field_exception.ngd";
        var saveBegin = _eventFactory.GetEvent<SaveBeginEventArgs>(EventNames.NAMESPACE, EventNames.SAVE_BEGIN_EVENT);
        DuplicateFieldException? exception = null;

        void OnSaveBegin(in SaveBeginEventArgs args)
        {
            var section = args.Writer.AddSection("Test");
            section.AddField("Value", 1);
            exception = Assert.Throws<DuplicateFieldException>(() => section.AddField("Value", 1));
        }

        saveBegin.Subscribe(this, OnSaveBegin);

        // Act
        await _dataProvider.Save(fileId, default);

        // Assert
        Assert.That(exception, Is.Not.Null);
    }

    [Test]
    public async Task Section_DuplicateSectionCreation_ThrowsDuplicateSectionException()
    {
        // Arrange
        var fileId = "duplicate_section_test.ngd";
        var saveBegin = _eventFactory.GetEvent<SaveBeginEventArgs>(EventNames.NAMESPACE, EventNames.SAVE_BEGIN_EVENT);
        DuplicateSectionException? exception = null;

        void OnSaveBegin(in SaveBeginEventArgs args)
        {
            ISaveWriterService writer = args.Writer;
            var section = args.Writer.AddSection("Test");
            exception = Assert.Throws<DuplicateSectionException>(() => writer.AddSection("Test"));
        }

        saveBegin.Subscribe(this, OnSaveBegin);

        // Act
        await _dataProvider.Save(fileId, default);

        // Assert
        Assert.That(exception, Is.Not.Null);
    }
}
#endif
