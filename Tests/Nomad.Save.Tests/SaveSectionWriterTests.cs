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
using Nomad.Save.Private.Services;
using Nomad.Save.Services;
using Nomad.Save.Exceptions;

namespace Nomad.Save.Tests;

/// <summary>
/// Tests specifically for SaveSectionWriter functionality
/// </summary>
[TestFixture]
public class SaveSectionWriterTests
{
    private ISaveDataProvider _dataProvider;
    private ILoggerService _logger;
    private IFileSystem _fileSystem;
    private IGameEventRegistryService _eventFactory;
    private string _testDirectory;

	 [SetUp]
    public void Setup()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), "NomadSaveSectionWriterTests", Guid.NewGuid().ToString());
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
	public async Task AddField_AsDuplicate_EnsureThrowsDuplicateFieldException()
	{
		// Arrange
		var fileId = Path.Combine(_testDirectory, "duplicate_fields_test.ngd");
		var saveBegin = _eventFactory.GetEvent<SaveBeginEventArgs>(EventNames.NAMESPACE, EventNames.SAVE_BEGIN_EVENT);
		DuplicateFieldException? exception = null;

		void OnSaveBegin(in SaveBeginEventArgs args)
		{
			var section = args.Writer.AddSection("TestSection");
			if (section != null)
			{
				section.AddField("ExactSameField", 0);
				exception = Assert.Throws<DuplicateFieldException>(
					() => section.AddField("ExactSameField", 0)
				);
			}
		}

		saveBegin.Subscribe(this, OnSaveBegin);

		// Act
		await _dataProvider.Save(fileId, default);

		// Assert
		Assert.That(exception, Is.Not.Null);
	}
}
#endif
