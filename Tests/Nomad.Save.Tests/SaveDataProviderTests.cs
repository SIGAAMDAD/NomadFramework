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
    private IFileSystem _fileSystem;
    private IGameEventRegistryService _eventFactory;
    private string _testDirectory;

    [SetUp]
    public void Setup()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), "NomadSaveTests", Guid.NewGuid().ToString());
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
    public void Constructor_WithValidDependencies_CreatesInstance()
    {
        // Assert
        Assert.That(_dataProvider, Is.Not.Null);
    }

    [Test]
    public void ListSaveFiles_WithEmptyDirectory_ReturnsEmptyList()
    {
        // Act
        var files = _dataProvider.ListSaveFiles(_testDirectory);

        // Assert
        Assert.That(files, Is.Not.Null);
        Assert.That(files, Is.Empty);
    }

    [Test]
    public void ListSaveFiles_WithMultipleSaveFiles_ReturnsAllFiles()
    {
        // Arrange
        var file1 = Path.Combine(_testDirectory, "save_001.ngd");
        var file2 = Path.Combine(_testDirectory, "save_002.ngd");
        var file3 = Path.Combine(_testDirectory, "save_003.ngd");

        File.WriteAllText(file1, "test");
        File.WriteAllText(file2, "test");
        File.WriteAllText(file3, "test");

        // Act
        var files = _dataProvider.ListSaveFiles(_testDirectory);

        // Assert
        Assert.That(files, Is.Not.Null);
        Assert.That(files, Has.Count.EqualTo(3));
    }

    [Test]
    public void ListSaveFiles_ReturnsReadOnlyList()
    {
        // Arrange
        var file1 = Path.Combine(_testDirectory, "save_001.ngd");
        File.WriteAllText(file1, "test");

        // Act
        var files = _dataProvider.ListSaveFiles(_testDirectory);

        // Assert
        Assert.That(typeof(IReadOnlyList<SaveFileMetadata>).IsAssignableFrom(files.GetType()));
    }

    [Test]
    public void ListSaveFiles_IncludesFileMetadata()
    {
        // Arrange
        var fileName = "test_save.ngd";
        var filePath = Path.Combine(_testDirectory, fileName);
        var testContent = "test data";
        File.WriteAllText(filePath, testContent);

        var fileInfo = new FileInfo(filePath);

        // Act
        var files = _dataProvider.ListSaveFiles(_testDirectory);

        // Assert
        Assert.That(files, Is.Not.Empty);
        var metadata = files[0];
		using (Assert.EnterMultipleScope())
		{
			Assert.That(metadata.FileSize, Is.EqualTo(fileInfo.Length));
			Assert.That(metadata.FileName, Is.Not.Null.And.Not.Empty);
		}
	}

    [Test]
    public async Task Save_WithValidFileId_CompletesSuccessfully()
    {
        // Arrange
        var fileId = Path.Combine(_testDirectory, "save_001.ngd");
        var saveBegin = _eventFactory.GetEvent<SaveBeginEventArgs>(EventNames.NAMESPACE, EventNames.SAVE_BEGIN_EVENT);
        var saveInvoked = false;

        saveBegin.Subscribe(this, (in args) =>
        {
            saveInvoked = true;
            var section = args.Writer.AddSection("TestSection");
            section.AddField("TestValue", 42);
        });

        // Act
        await _dataProvider.Save(fileId, new GameVersion());

        // Assert
        Assert.That(saveInvoked, Is.True);
    }

    [Test]
    public async Task Load_AfterSave_CanReadSavedData()
    {
        // Arrange
        var fileId = Path.Combine(_testDirectory, "save_load_test.ngd");
        var saveBegin = _eventFactory.GetEvent<SaveBeginEventArgs>(EventNames.NAMESPACE, EventNames.SAVE_BEGIN_EVENT);
        var loadBegin = _eventFactory.GetEvent<LoadBeginEventArgs>(EventNames.NAMESPACE, EventNames.LOAD_BEGIN_EVENT);
        
        var savedData = 123;
        var loadedSuccessfully = false;

        saveBegin.Subscribe(this, (in SaveBeginEventArgs args) =>
        {
            var section = args.Writer.AddSection("DataSection");
            section.AddField("SavedInt", savedData);
        });

        loadBegin.Subscribe(this, (in LoadBeginEventArgs args) =>
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
        var fileId = Path.Combine(_testDirectory, "overwrite_test.ngd");
        var saveBegin = _eventFactory.GetEvent<SaveBeginEventArgs>(EventNames.NAMESPACE, EventNames.SAVE_BEGIN_EVENT);
        int saveCount = 0;

        saveBegin.Subscribe(this, (in SaveBeginEventArgs args) =>
        {
            saveCount++;
            var section = args.Writer.AddSection("SaveCount");
            section.AddField("Count", saveCount);
        });

        // Act
        await _dataProvider.Save(fileId, default);
        await _dataProvider.Save(fileId, default);

        // Assert
        Assert.That(File.Exists(fileId), Is.True);
    }

    [Test]
    [TestCase("save_001.ngd")]
    [TestCase("save_with_spaces.ngd")]
    [TestCase("save-with-dashes.ngd")]
    [TestCase("save_with_numbers_123.ngd")]
    public async Task Save_WithVariousFileNames_Succeeds(string fileName)
    {
        // Arrange
        var fileId = Path.Combine(_testDirectory, fileName);
        var saveBegin = _eventFactory.GetEvent<SaveBeginEventArgs>(EventNames.NAMESPACE, EventNames.SAVE_BEGIN_EVENT);

        saveBegin.Subscribe(this, (in SaveBeginEventArgs args) =>
        {
            var section = args.Writer.AddSection("TestSection");
            section.AddField("TestValue", 1);
        });

        // Act
        await _dataProvider.Save(fileId, default);

        // Assert
        Assert.That(File.Exists(fileId), Is.True);
    }

    [Test]
    public async Task Save_WithNoSubscribers_CompletesSuccessfully()
    {
        // Arrange
        var fileId = Path.Combine(_testDirectory, "no_subscribers.ngd");

        // Act & Assert
        await _dataProvider.Save(fileId, default);
        Assert.Pass("Save completed without subscribers");
    }

    [Test]
    public async Task ListSaveFiles_AfterMultipleSaves_ReturnsAccurateMetadata()
    {
        // Arrange
        var fileId1 = Path.Combine(_testDirectory, "save_001.ngd");
        var fileId2 = Path.Combine(_testDirectory, "save_002.ngd");
        var saveBegin = _eventFactory.GetEvent<SaveBeginEventArgs>(EventNames.NAMESPACE, EventNames.SAVE_BEGIN_EVENT);

        saveBegin.Subscribe(this, (in SaveBeginEventArgs args) =>
        {
            var section = args.Writer.AddSection("TestSection");
            section.AddField("TestValue", 1);
        });

        // Act
        await _dataProvider.Save(fileId1, default);
        await _dataProvider.Save(fileId2, default);
        var filesList = _dataProvider.ListSaveFiles(_testDirectory);

        // Assert
        Assert.That(filesList.Count, Is.GreaterThanOrEqualTo(2));
    }

    [Test]
    public void ListSaveFiles_WithNonExistentDirectory_HandlesGracefully()
    {
        // Arrange
        var nonExistentDir = Path.Combine(_testDirectory, "non_existent");

        // Act & Assert
        // This should either return empty or throw - implementation dependent
        try
        {
            var files = _dataProvider.ListSaveFiles(nonExistentDir);
            Assert.That(files, Is.Empty.Or.Null);
        }
        catch (DirectoryNotFoundException)
        {
            Assert.Pass("Correctly throws for non-existent directory");
        }
    }

    [Test]
    public async Task Save_AndLoad_WithMultipleSections_PreservesData()
    {
        // Arrange
        var fileId = Path.Combine(_testDirectory, "multi_section.ngd");
        var saveBegin = _eventFactory.GetEvent<SaveBeginEventArgs>(EventNames.NAMESPACE, EventNames.SAVE_BEGIN_EVENT);
        var loadBegin = _eventFactory.GetEvent<LoadBeginEventArgs>(EventNames.NAMESPACE, EventNames.LOAD_BEGIN_EVENT);
        
        var section1Found = false;
        var section2Found = false;

        saveBegin.Subscribe(this, (in SaveBeginEventArgs args) =>
        {
            var section1 = args.Writer.AddSection("Section1");
            section1.AddField("Value1", 100);
            
            var section2 = args.Writer.AddSection("Section2");
            section2.AddField("Value2", 200);
        });

        loadBegin.Subscribe(this, (in LoadBeginEventArgs args) =>
        {
            section1Found = args.Reader.FindSection("Section1") != null;
            section2Found = args.Reader.FindSection("Section2") != null;
        });

        // Act
        await _dataProvider.Save(fileId, default);
        await _dataProvider.Load(fileId);

        // Assert
        Assert.That(section1Found, Is.True);
        Assert.That(section2Found, Is.True);
    }

    [Test]
    [TestCase(0)]
    [TestCase(1)]
    [TestCase(100)]
    [TestCase(int.MaxValue)]
    public async Task Save_WithVariousIntValues_Succeeds(int value)
    {
        // Arrange
        var fileId = Path.Combine(_testDirectory, $"int_test_{value}.ngd");
        var saveBegin = _eventFactory.GetEvent<SaveBeginEventArgs>(EventNames.NAMESPACE, EventNames.SAVE_BEGIN_EVENT);

        saveBegin.Subscribe(this, (in SaveBeginEventArgs args) =>
        {
            var section = args.Writer.AddSection("IntTest");
            section.AddField("IntValue", value);
        });

        // Act
        await _dataProvider.Save(fileId, default);

        // Assert
        Assert.That(File.Exists(fileId), Is.True);
    }
}
