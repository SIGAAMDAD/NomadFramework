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
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Nomad.Core.Events;
using Nomad.Core.FileSystem;
using Nomad.Core.Logger;
using Nomad.Core.CVars;
using Nomad.Events;
using Nomad.FileSystem.Private.Services;
using Nomad.Save.Data;
using Nomad.Save.Events;
using Nomad.Save.Private.Services;
using Nomad.Save.Services;
using Nomad.Save.ValueObjects;
using Nomad.CVars.Private.Services;
using Moq;
using Nomad.Core.EngineUtils;
using Nomad.Save.Private.ValueObjects;

namespace Nomad.Save.Tests;

/// <summary>
/// Tests for SaveDataProvider service
/// </summary>
[TestFixture]
public class SaveDataProviderTests
{
    private ISaveDataProvider _dataProvider;
    private ILoggerService _logger;
    private ICVarSystemService _cvarSystem;
    private IFileSystem _fileSystem;
    private IGameEventRegistryService _eventFactory;
    private string _testDirectory;
    private string _saveDirectory;

    private IGameEvent<SaveBeginEventArgs> _saveBegin;
    private IGameEvent<LoadBeginEventArgs> _loadBegin;

    [SetUp]
    public void Setup()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), "NomadSaveTests", Guid.NewGuid().ToString());
        Directory.CreateDirectory(_testDirectory);
        _saveDirectory = Path.Combine(_testDirectory, "SaveData");
        Directory.CreateDirectory(_saveDirectory);

        _logger = new MockLogger();
        var engineService = new Mock<IEngineService>();
        engineService.Setup(e => e.GetStoragePath(StorageScope.StreamingAssets)).Returns(_testDirectory);
        engineService.Setup(e => e.GetStoragePath(StorageScope.UserData)).Returns(_testDirectory);
        engineService.Setup(e => e.GetStoragePath(StorageScope.Install)).Returns(_testDirectory);

        _fileSystem = new FileSystemService(engineService.Object, _logger);
        _eventFactory = new GameEventRegistry(_logger);
        _cvarSystem = new CVarSystem(_eventFactory, _fileSystem, _logger);
        _dataProvider = new SaveDataProvider(engineService.Object, _eventFactory, _cvarSystem, _fileSystem, _logger);

        _saveBegin = _eventFactory.GetEvent<SaveBeginEventArgs>(EventNames.NAMESPACE, EventNames.SAVE_BEGIN_EVENT);
        _loadBegin = _eventFactory.GetEvent<LoadBeginEventArgs>(EventNames.NAMESPACE, EventNames.LOAD_BEGIN_EVENT);
    }

    [TearDown]
    public void TearDown()
    {
        _saveBegin?.Dispose();
        _loadBegin?.Dispose();
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
    public void Constructor_WithValidDependencies_CreatesInstance()
    {
        // Assert
        Assert.That(_dataProvider, Is.Not.Null);
    }

    [Test]
    public void ListSaveFiles_WithEmptyDirectory_ReturnsEmptyList()
    {
        // Act
        var files = _dataProvider.ListSaveFiles();

        // Assert
        Assert.That(files, Is.Not.Null);
        Assert.That(files, Is.Empty);
    }

    [Test]
    public async Task ListSaveFiles_WithMultipleSaveFiles_ReturnsAllFiles()
    {
        // Arrange
        string file1 = "save_001";
        string file2 = "save_002";
        string file3 = "save_003";

        // Act
        await _dataProvider.Save(file1, default);
        await _dataProvider.Save(file2, default);
        await _dataProvider.Save(file3, default);

        var files = _dataProvider.ListSaveFiles();

        // Assert
        Assert.That(files, Is.Not.Null);
        Assert.That(files, Has.Count.EqualTo(3));
    }

    [Test]
    public async Task ListSaveFiles_ReturnsReadOnlyList()
    {
        // Arrange
        string file1 = "save_001";
        await _dataProvider.Save(file1, default);

        // Act
        var files = _dataProvider.ListSaveFiles();

        // Assert
        Assert.That(typeof(IReadOnlyList<SaveFileMetadata>).IsAssignableFrom(files.GetType()));
    }

    [Test]
    public async Task ListSaveFiles_IncludesFileMetadata()
    {
        // Arrange
        string fileName = "test_save";
        var lastAccessTime = DateTime.Now;
        var creationTime = DateTime.Now;
        var fileMetadata = new SaveFileMetadata(
            SaveName: fileName,
            FileSize: 0,
            LastAccessYear: lastAccessTime.Year,
            LastAccessMonth: lastAccessTime.Month,
            LastAccessDay: lastAccessTime.Day,
            CreationYear: creationTime.Year,
            CreationMonth: creationTime.Month,
            CreationDay: creationTime.Day
        );

        // Act
        await _dataProvider.Save(fileName, default);
        SaveSlot.CalculateFileName(false, fileMetadata);
        var files = _dataProvider.ListSaveFiles();

        // Assert
        Assert.That(files, Is.Not.Empty);
        var metadata = files[0];
        Assert.That(metadata, Is.EqualTo(fileMetadata));
    }

    [Test]
    public async Task Save_WithValidFileId_CompletesSuccessfully()
    {
        // Arrange
        string fileId = "save_001";
        bool saveInvoked = false;

        _saveBegin.Subscribe((in SaveBeginEventArgs args) =>
        {
            saveInvoked = true;
            var section = args.Writer.AddSection("TestSection");
            section.AddField("TestValue", 42);
        });

        // Act
        await _dataProvider.Save(fileId, default);

        // Assert
        Assert.That(saveInvoked, Is.True);
    }

    [Test]
    public async Task Load_AfterSave_CanReadSavedData()
    {
        // Arrange
        string fileId = "save_load_test";

        int savedData = 123;
        bool loadedSuccessfully = false;

        _saveBegin.Subscribe((in SaveBeginEventArgs args) =>
        {
            var section = args.Writer.AddSection("DataSection");
            section.AddField("SavedInt", savedData);
        });

        _loadBegin.Subscribe((in LoadBeginEventArgs args) =>
        {
            var section = args.Reader.FindSection("DataSection");
            if (section != null)
            {
                var value = section.GetField<int>("SavedInt");
                loadedSuccessfully = value == savedData;
            }
        });

        // Act
        await _dataProvider.Save(fileId, default);
        await _dataProvider.Load(fileId);

        // Assert
        Assert.That(loadedSuccessfully, Is.True);
    }

    [Test]
    public void Dispose_ReleasesResources()
    {
        // Act
        _dataProvider.Dispose();

        // Assert - should not throw
        Assert.Pass("Dispose completed without throwing");
    }

    [Test]
    public async Task Save_MultipleTimesToSameFile_Overwrites()
    {
        // Arrange
        string fileId = "overwrite_test";
        int saveCount = 0;

        _saveBegin.Subscribe((in SaveBeginEventArgs args) =>
        {
            saveCount++;
            var section = args.Writer.AddSection("SaveCount");
            section.AddField("Count", saveCount);
        });

        // Act
        await _dataProvider.Save(fileId, default);
        await _dataProvider.Save(fileId, default);

        var saveFiles = _dataProvider.ListSaveFiles();

        // Assert
        Assert.That(saveFiles, Has.Count.GreaterThan(0));
    }

    [Test]
    public async Task Save_WithNoSubscribers_CompletesSuccessfully()
    {
        // Arrange
        string fileId = "no_subscribers";

        // Act & Assert
        await _dataProvider.Save(fileId, default);
        Assert.Pass("Save completed without subscribers");
    }

    [Test]
    public async Task ListSaveFiles_AfterMultipleSaves_ReturnsAccurateMetadata()
    {
        // Arrange
        string fileId1 = "save_001";
        string fileId2 = "save_002";

        _saveBegin.Subscribe((in SaveBeginEventArgs args) =>
        {
            var section = args.Writer.AddSection("TestSection");
            section.AddField("TestValue", 1);
        });

        // Act
        await _dataProvider.Save(fileId1, default);
        await _dataProvider.Save(fileId2, default);
        var filesList = _dataProvider.ListSaveFiles();

        // Assert
        Assert.That(filesList, Has.Count.GreaterThanOrEqualTo(2));
    }

    [Test]
    public async Task Save_AndLoad_WithMultipleSections_PreservesData()
    {
        // Arrange
        string fileId = "multi_section";
        bool section1Found = false;
        bool section2Found = false;

        _saveBegin.Subscribe((in SaveBeginEventArgs args) =>
        {
            var section1 = args.Writer.AddSection("Section1");
            section1.AddField("Value1", 100);

            var section2 = args.Writer.AddSection("Section2");
            section2.AddField("Value2", 200);
        });

        _loadBegin.Subscribe((in LoadBeginEventArgs args) =>
        {
            section1Found = args.Reader.FindSection("Section1") != null;
            section2Found = args.Reader.FindSection("Section2") != null;
        });

        // Act
        await _dataProvider.Save(fileId, default);
        await _dataProvider.Load(fileId);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(section1Found, Is.True);
            Assert.That(section2Found, Is.True);
        }
    }
}
#endif
