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

namespace Nomad.Save.Tests;

/// <summary>
/// Tests specifically for SaveSectionReader functionality
/// </summary>
[TestFixture]
public class SaveSectionReaderTests
{
    private ISaveDataProvider _dataProvider;
    private ILoggerService _logger;
    private IFileSystem _fileSystem;
    private IGameEventRegistryService _eventFactory;
    private string _testDirectory;

    [SetUp]
    public void Setup()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), "NomadSaveSectionReaderTests", Guid.NewGuid().ToString());
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
    public async Task SaveSectionReader_Name_ReturnsCorrectSectionName()
    {
        // Arrange
        var fileId = Path.Combine(_testDirectory, "section_name_test.ngd");
        var saveBegin = _eventFactory.GetEvent<SaveBeginEventArgs>(EventNames.NAMESPACE, EventNames.SAVE_BEGIN_EVENT);
        var loadBegin = _eventFactory.GetEvent<LoadBeginEventArgs>(EventNames.NAMESPACE, EventNames.LOAD_BEGIN_EVENT);

        var expectedSectionName = "MyTestSection";
        var readSectionName = "";

        void OnSaveBegin(in SaveBeginEventArgs args)
        {
            var section = args.Writer.AddSection(expectedSectionName);
            section.AddField("TestField", 42);
        }

        saveBegin.Subscribe(this, OnSaveBegin);

        void OnLoadBegin(in LoadBeginEventArgs args)
        {
            var section = args.Reader.FindSection(expectedSectionName);
            if (section != null)
            {
                readSectionName = section.Name;
            }
        }

        loadBegin.Subscribe(this, OnLoadBegin);

        // Act
        await _dataProvider.Save(fileId, default);
        await _dataProvider.Load(fileId);

        // Assert
        Assert.That(readSectionName, Is.EqualTo(expectedSectionName));
    }

    [Test]
    public async Task SaveSectionReader_FieldCount_ReturnsCorrectFieldCount()
    {
        // Arrange
        var fileId = Path.Combine(_testDirectory, "field_count_test.ngd");
        var saveBegin = _eventFactory.GetEvent<SaveBeginEventArgs>(EventNames.NAMESPACE, EventNames.SAVE_BEGIN_EVENT);
        var loadBegin = _eventFactory.GetEvent<LoadBeginEventArgs>(EventNames.NAMESPACE, EventNames.LOAD_BEGIN_EVENT);

        var readFieldCount = 0;

        void OnSaveBegin(in SaveBeginEventArgs args)
        {
            var section = args.Writer.AddSection("FieldCountTest");
            section.AddField("Field1", 1);
            section.AddField("Field2", 2);
            section.AddField("Field3", 3);
            section.AddField("Field4", 4);
            section.AddField("Field5", 5);
        }

        saveBegin.Subscribe(this, OnSaveBegin);

        void OnLoadBegin(in LoadBeginEventArgs args)
        {
            var section = args.Reader.FindSection("FieldCountTest");
            if (section != null)
            {
                readFieldCount = section.FieldCount;
            }
        }

        loadBegin.Subscribe(this, OnLoadBegin);

        // Act
        await _dataProvider.Save(fileId, default);
        await _dataProvider.Load(fileId);

        // Assert
        Assert.That(readFieldCount, Is.EqualTo(5));
    }

    [Test]
    public async Task SaveSectionReader_GetField_WithIntType_ReturnsCorrectValue()
    {
        // Arrange
        var fileId = Path.Combine(_testDirectory, "get_int_field_test.ngd");
        var saveBegin = _eventFactory.GetEvent<SaveBeginEventArgs>(EventNames.NAMESPACE, EventNames.SAVE_BEGIN_EVENT);
        var loadBegin = _eventFactory.GetEvent<LoadBeginEventArgs>(EventNames.NAMESPACE, EventNames.LOAD_BEGIN_EVENT);

        int expectedValue = 12345;
        int retrievedValue = 0;

        void OnSaveBegin(in SaveBeginEventArgs args)
        {
            var section = args.Writer.AddSection("IntTest");
            section.AddField("IntValue", expectedValue);
        }

        saveBegin.Subscribe(this, OnSaveBegin);

        void OnLoadBegin(in LoadBeginEventArgs args)
        {
            var section = args.Reader.FindSection("IntTest");
            if (section != null)
            {
                retrievedValue = section.GetField<int>("IntValue");
            }
        }

        loadBegin.Subscribe(this, OnLoadBegin);

        // Act
        await _dataProvider.Save(fileId, default);
        await _dataProvider.Load(fileId);

        // Assert
        Assert.That(retrievedValue, Is.EqualTo(expectedValue));
    }

    [Test]
    public async Task SaveSectionReader_GetField_WithFloatType_ReturnsCorrectValue()
    {
        // Arrange
        var fileId = Path.Combine(_testDirectory, "get_float_field_test.ngd");
        var saveBegin = _eventFactory.GetEvent<SaveBeginEventArgs>(EventNames.NAMESPACE, EventNames.SAVE_BEGIN_EVENT);
        var loadBegin = _eventFactory.GetEvent<LoadBeginEventArgs>(EventNames.NAMESPACE, EventNames.LOAD_BEGIN_EVENT);

        float expectedValue = 3.14159f;
        float retrievedValue = 0f;

        void OnSaveBegin(in SaveBeginEventArgs args)
        {
            var section = args.Writer.AddSection("FloatTest");
            section.AddField("FloatValue", expectedValue);
        }

        saveBegin.Subscribe(this, OnSaveBegin);

        void OnLoadBegin(in LoadBeginEventArgs args)
        {
            var section = args.Reader.FindSection("FloatTest");
            if (section != null)
            {
                retrievedValue = section.GetField<float>("FloatValue");
            }
        }

        loadBegin.Subscribe(this, OnLoadBegin);

        // Act
        await _dataProvider.Save(fileId, default);
        await _dataProvider.Load(fileId);

        // Assert
        Assert.That(retrievedValue, Is.EqualTo(expectedValue).Within(0.00001f));
    }

    [Test]
    public async Task SaveSectionReader_GetField_WithDoubleType_ReturnsCorrectValue()
    {
        // Arrange
        var fileId = Path.Combine(_testDirectory, "get_double_field_test.ngd");
        var saveBegin = _eventFactory.GetEvent<SaveBeginEventArgs>(EventNames.NAMESPACE, EventNames.SAVE_BEGIN_EVENT);
        var loadBegin = _eventFactory.GetEvent<LoadBeginEventArgs>(EventNames.NAMESPACE, EventNames.LOAD_BEGIN_EVENT);

        double expectedValue = 2.71828;
        double retrievedValue = 0d;

        void OnSaveBegin(in SaveBeginEventArgs args)
        {
            var section = args.Writer.AddSection("DoubleTest");
            section.AddField("DoubleValue", expectedValue);
        }

        saveBegin.Subscribe(this, OnSaveBegin);

        void OnLoadBegin(in LoadBeginEventArgs args)
        {
            var section = args.Reader.FindSection("DoubleTest");
            if (section != null)
            {
                retrievedValue = section.GetField<double>("DoubleValue");
            }
        }

        loadBegin.Subscribe(this, OnLoadBegin);

        // Act
        await _dataProvider.Save(fileId, default);
        await _dataProvider.Load(fileId);

        // Assert
        Assert.That(retrievedValue, Is.EqualTo(expectedValue).Within(0.00001));
    }

    [Test]
    public async Task SaveSectionReader_GetField_WithStringType_ReturnsCorrectValue()
    {
        // Arrange
        var fileId = Path.Combine(_testDirectory, "get_string_field_test.ngd");
        var saveBegin = _eventFactory.GetEvent<SaveBeginEventArgs>(EventNames.NAMESPACE, EventNames.SAVE_BEGIN_EVENT);
        var loadBegin = _eventFactory.GetEvent<LoadBeginEventArgs>(EventNames.NAMESPACE, EventNames.LOAD_BEGIN_EVENT);

        string expectedValue = "Hello, Save System!";
        string retrievedValue = "";

        void OnSaveBegin(in SaveBeginEventArgs args)
        {
            var section = args.Writer.AddSection("StringTest");
            section.AddField("StringValue", expectedValue);
        }

        saveBegin.Subscribe(this, OnSaveBegin);

        void OnLoadBegin(in LoadBeginEventArgs args)
        {
            var section = args.Reader.FindSection("StringTest");
            if (section != null)
            {
                retrievedValue = section.GetField<string>("StringValue");
            }
        }

        loadBegin.Subscribe(this, OnLoadBegin);

        // Act
        await _dataProvider.Save(fileId, default);
        await _dataProvider.Load(fileId);

        // Assert
        Assert.That(retrievedValue, Is.EqualTo(expectedValue));
    }

    [Test]
    public async Task SaveSectionReader_GetField_WithBoolType_ReturnsCorrectValue()
    {
        // Arrange
        var fileId = Path.Combine(_testDirectory, "get_bool_field_test.ngd");
        var saveBegin = _eventFactory.GetEvent<SaveBeginEventArgs>(EventNames.NAMESPACE, EventNames.SAVE_BEGIN_EVENT);
        var loadBegin = _eventFactory.GetEvent<LoadBeginEventArgs>(EventNames.NAMESPACE, EventNames.LOAD_BEGIN_EVENT);

        bool expectedValue = true;
        bool retrievedValue = false;

        void OnSaveBegin(in SaveBeginEventArgs args)
        {
            var section = args.Writer.AddSection("BoolTest");
            section.AddField("BoolValue", expectedValue);
        }

        saveBegin.Subscribe(this, OnSaveBegin);

        void OnLoadBegin(in LoadBeginEventArgs args)
        {
            var section = args.Reader.FindSection("BoolTest");
            if (section != null)
            {
                retrievedValue = section.GetField<bool>("BoolValue");
            }
        }

        loadBegin.Subscribe(this, OnLoadBegin);

        // Act
        await _dataProvider.Save(fileId, default);
        await _dataProvider.Load(fileId);

        // Assert
        Assert.That(retrievedValue, Is.EqualTo(expectedValue));
    }

    [Test]
    public async Task SaveSectionReader_GetField_WithNonExistentField_ReturnsDefault()
    {
        // Arrange
        var fileId = Path.Combine(_testDirectory, "nonexistent_field_test.ngd");
        var saveBegin = _eventFactory.GetEvent<SaveBeginEventArgs>(EventNames.NAMESPACE, EventNames.SAVE_BEGIN_EVENT);
        var loadBegin = _eventFactory.GetEvent<LoadBeginEventArgs>(EventNames.NAMESPACE, EventNames.LOAD_BEGIN_EVENT);

        int retrievedValue = int.MinValue;

        void OnSaveBegin(in SaveBeginEventArgs args)
        {
            var section = args.Writer.AddSection("NoFieldTest");
            section.AddField("ExistingField", 100);
        }

        saveBegin.Subscribe(this, OnSaveBegin);

        void OnLoadBegin(in LoadBeginEventArgs args)
        {
            var section = args.Reader.FindSection("NoFieldTest");
            if (section != null)
            {
                retrievedValue = section.GetField<int>("NonExistentField");
            }
        }

        loadBegin.Subscribe(this, OnLoadBegin);

        // Act
        await _dataProvider.Save(fileId, default);
        await _dataProvider.Load(fileId);

        // Assert
        Assert.That(retrievedValue, Is.Default);
    }

    [Test]
    public async Task SaveSectionReader_GetField_WithWrongType_ThrowsInvalidCastException()
    {
        // Arrange
        var fileId = Path.Combine(_testDirectory, "wrong_type_test.ngd");
        var saveBegin = _eventFactory.GetEvent<SaveBeginEventArgs>(EventNames.NAMESPACE, EventNames.SAVE_BEGIN_EVENT);
        var loadBegin = _eventFactory.GetEvent<LoadBeginEventArgs>(EventNames.NAMESPACE, EventNames.LOAD_BEGIN_EVENT);

        var exceptionThrown = false;

        saveBegin.Subscribe(this, (in SaveBeginEventArgs args) =>
        {
            var section = args.Writer.AddSection("WrongTypeTest");
            section.AddField("IntField", 42);
        });

        loadBegin.Subscribe(this, (in LoadBeginEventArgs args) =>
        {
            var section = args.Reader.FindSection("WrongTypeTest");
            if (section != null)
            {
                try
                {
                    // Try to read int as string
                    var stringValue = section.GetField<string>("IntField");
                }
                catch (InvalidCastException)
                {
                    exceptionThrown = true;
                }
            }
        });

        // Act
        await _dataProvider.Save(fileId, default);
        await _dataProvider.Load(fileId);

        // Assert
        Assert.That(exceptionThrown, Is.True);
    }

    [Test]
    public async Task SaveSectionReader_Dispose_ClearsFields()
    {
        // Arrange
        var fileId = Path.Combine(_testDirectory, "dispose_test.ngd");
        var saveBegin = _eventFactory.GetEvent<SaveBeginEventArgs>(EventNames.NAMESPACE, EventNames.SAVE_BEGIN_EVENT);
        var loadBegin = _eventFactory.GetEvent<LoadBeginEventArgs>(EventNames.NAMESPACE, EventNames.LOAD_BEGIN_EVENT);

        ISaveSectionReader? sectionReader = null;

        saveBegin.Subscribe(this, (in SaveBeginEventArgs args) =>
        {
            var section = args.Writer.AddSection("DisposeTest");
            section.AddField("TestField", 99);
        });

        loadBegin.Subscribe(this, (in LoadBeginEventArgs args) =>
        {
            sectionReader = args.Reader.FindSection("DisposeTest");
        });

        // Act
        await _dataProvider.Save(fileId, default);
        await _dataProvider.Load(fileId);

        var fieldCountBeforeDispose = sectionReader?.FieldCount ?? 0;
        sectionReader?.Dispose();
        var fieldCountAfterDispose = sectionReader?.FieldCount ?? 0;

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(fieldCountBeforeDispose, Is.GreaterThan(0));
            Assert.That(fieldCountAfterDispose, Is.Zero);
        }
    }

    [Test]
    public async Task SaveSectionReader_GetField_WithAllIntegerTypes_ReturnsCorrectValues()
    {
        // Arrange
        var fileId = Path.Combine(_testDirectory, "all_int_types_test.ngd");
        var saveBegin = _eventFactory.GetEvent<SaveBeginEventArgs>(EventNames.NAMESPACE, EventNames.SAVE_BEGIN_EVENT);
        var loadBegin = _eventFactory.GetEvent<LoadBeginEventArgs>(EventNames.NAMESPACE, EventNames.LOAD_BEGIN_EVENT);

        var sbyte_val = (sbyte)-50;
        var short_val = (short)-1000;
        var int_val = -100000;
        var long_val = -9000000000000000L;
        var byte_val = (byte)200;
        var ushort_val = (ushort)50000;
        var uint_val = 3000000000U;
        var ulong_val = 15000000000000000UL;

        ISaveSectionReader? reader = null;

        saveBegin.Subscribe(this, (in SaveBeginEventArgs args) =>
        {
            var section = args.Writer.AddSection("IntTypes");
            section.AddField("sbyte", sbyte_val);
            section.AddField("short", short_val);
            section.AddField("int", int_val);
            section.AddField("long", long_val);
            section.AddField("byte", byte_val);
            section.AddField("ushort", ushort_val);
            section.AddField("uint", uint_val);
            section.AddField("ulong", ulong_val);
        });

        loadBegin.Subscribe(this, (in LoadBeginEventArgs args) =>
        {
            reader = args.Reader.FindSection("IntTypes");
        });

        // Act
        await _dataProvider.Save(fileId, default);
        await _dataProvider.Load(fileId);

        // Assert
        Assert.That(reader, Is.Not.Null);
        if (reader != null)
        {
			using (Assert.EnterMultipleScope())
			{
				Assert.That(reader.GetField<sbyte>("sbyte"), Is.EqualTo(sbyte_val));
				Assert.That(reader.GetField<short>("short"), Is.EqualTo(short_val));
				Assert.That(reader.GetField<int>("int"), Is.EqualTo(int_val));
				Assert.That(reader.GetField<long>("long"), Is.EqualTo(long_val));
				Assert.That(reader.GetField<byte>("byte"), Is.EqualTo(byte_val));
				Assert.That(reader.GetField<ushort>("ushort"), Is.EqualTo(ushort_val));
				Assert.That(reader.GetField<uint>("uint"), Is.EqualTo(uint_val));
				Assert.That(reader.GetField<ulong>("ulong"), Is.EqualTo(ulong_val));
			}
		}
    }

    [Test]
    public async Task SaveSectionReader_GetField_MultipleTimes_ReturnsSameValue()
    {
        // Arrange
        var fileId = Path.Combine(_testDirectory, "multiple_reads_test.ngd");
        var saveBegin = _eventFactory.GetEvent<SaveBeginEventArgs>(EventNames.NAMESPACE, EventNames.SAVE_BEGIN_EVENT);
        var loadBegin = _eventFactory.GetEvent<LoadBeginEventArgs>(EventNames.NAMESPACE, EventNames.LOAD_BEGIN_EVENT);

        ISaveSectionReader? sectionReader = null;
        int expectedValue = 777;

        saveBegin.Subscribe(this, (in SaveBeginEventArgs args) =>
        {
            var section = args.Writer.AddSection("MultiRead");
            section.AddField("Value", expectedValue);
        });

        loadBegin.Subscribe(this, (in LoadBeginEventArgs args) =>
        {
            sectionReader = args.Reader.FindSection("MultiRead");
        });

        // Act
        await _dataProvider.Save(fileId, default);
        await _dataProvider.Load(fileId);

        // Assert
        if (sectionReader != null)
        {
            var firstRead = sectionReader.GetField<int>("Value");
            var secondRead = sectionReader.GetField<int>("Value");
            var thirdRead = sectionReader.GetField<int>("Value");

			using (Assert.EnterMultipleScope())
			{
				Assert.That(firstRead, Is.EqualTo(expectedValue));
				Assert.That(secondRead, Is.EqualTo(expectedValue));
				Assert.That(thirdRead, Is.EqualTo(expectedValue));
			}
		}
    }

    [Test]
    public async Task SaveSectionReader_Name_IsNotNull()
    {
        // Arrange
        var fileId = Path.Combine(_testDirectory, "name_not_null_test.ngd");
        var saveBegin = _eventFactory.GetEvent<SaveBeginEventArgs>(EventNames.NAMESPACE, EventNames.SAVE_BEGIN_EVENT);
        var loadBegin = _eventFactory.GetEvent<LoadBeginEventArgs>(EventNames.NAMESPACE, EventNames.LOAD_BEGIN_EVENT);

        ISaveSectionReader? sectionReader = null;

        saveBegin.Subscribe(this, (in SaveBeginEventArgs args) =>
        {
            var section = args.Writer.AddSection("ValidSection");
            section.AddField("TestField", 1);
        });

        loadBegin.Subscribe(this, (in LoadBeginEventArgs args) =>
        {
            sectionReader = args.Reader.FindSection("ValidSection");
        });

        // Act
        await _dataProvider.Save(fileId, default);
        await _dataProvider.Load(fileId);

        // Assert
        Assert.That(sectionReader, Is.Not.Null);
        if (sectionReader != null)
        {
            Assert.That(sectionReader.Name, Is.Not.Null.And.Not.Empty);
        }
    }

    [Test]
    public async Task SaveSectionReader_FieldCount_IsNonNegative()
    {
        // Arrange
        var fileId = Path.Combine(_testDirectory, "field_count_nonneg_test.ngd");
        var saveBegin = _eventFactory.GetEvent<SaveBeginEventArgs>(EventNames.NAMESPACE, EventNames.SAVE_BEGIN_EVENT);
        var loadBegin = _eventFactory.GetEvent<LoadBeginEventArgs>(EventNames.NAMESPACE, EventNames.LOAD_BEGIN_EVENT);

        ISaveSectionReader? sectionReader = null;

        saveBegin.Subscribe(this, (in SaveBeginEventArgs args) =>
        {
            var section = args.Writer.AddSection("CountTest");
            section.AddField("Field1", 1);
            section.AddField("Field2", 2);
        });

        loadBegin.Subscribe(this, (in LoadBeginEventArgs args) =>
        {
            sectionReader = args.Reader.FindSection("CountTest");
        });

        // Act
        await _dataProvider.Save(fileId, default);
        await _dataProvider.Load(fileId);

        // Assert
        Assert.That(sectionReader, Is.Not.Null);
        if (sectionReader != null)
        {
            Assert.That(sectionReader.FieldCount, Is.GreaterThanOrEqualTo(0));
        }
    }

    [Test]
    public async Task SaveSectionReader_GetField_WithEmptyStringField_ReturnsEmptyString()
    {
        // Arrange
        var fileId = Path.Combine(_testDirectory, "empty_string_field_test.ngd");
        var saveBegin = _eventFactory.GetEvent<SaveBeginEventArgs>(EventNames.NAMESPACE, EventNames.SAVE_BEGIN_EVENT);
        var loadBegin = _eventFactory.GetEvent<LoadBeginEventArgs>(EventNames.NAMESPACE, EventNames.LOAD_BEGIN_EVENT);

        string retrievedValue = "unchanged";

        saveBegin.Subscribe(this, (in SaveBeginEventArgs args) =>
        {
            var section = args.Writer.AddSection("EmptyStringTest");
            section.AddField("EmptyString", "");
        });

        loadBegin.Subscribe(this, (in LoadBeginEventArgs args) =>
        {
            var section = args.Reader.FindSection("EmptyStringTest");
            if (section != null)
            {
                retrievedValue = section.GetField<string>("EmptyString");
            }
        });

        // Act
        await _dataProvider.Save(fileId, default);
        await _dataProvider.Load(fileId);

        // Assert
        Assert.That(retrievedValue, Is.EqualTo(""));
    }

    [Test]
    public async Task SaveSectionReader_GetField_WithLongString_ReturnsFullString()
    {
        // Arrange
        var fileId = Path.Combine(_testDirectory, "long_string_field_test.ngd");
        var saveBegin = _eventFactory.GetEvent<SaveBeginEventArgs>(EventNames.NAMESPACE, EventNames.SAVE_BEGIN_EVENT);
        var loadBegin = _eventFactory.GetEvent<LoadBeginEventArgs>(EventNames.NAMESPACE, EventNames.LOAD_BEGIN_EVENT);

        int length = 1000;
        string longString = new string('X', length);
        string retrievedValue = "";

        saveBegin.Subscribe(this, (in SaveBeginEventArgs args) =>
        {
            var section = args.Writer.AddSection("LongStringTest");
            section.AddField("LongString", longString);
        });

        loadBegin.Subscribe(this, (in LoadBeginEventArgs args) =>
        {
            var section = args.Reader.FindSection("LongStringTest");
            if (section != null)
            {
                retrievedValue = section.GetField<string>("LongString");
            }
        });

        // Act
        await _dataProvider.Save(fileId, default);
        await _dataProvider.Load(fileId);

        // Assert
        Assert.That(retrievedValue, Is.EqualTo(longString));
        Assert.That(retrievedValue, Has.Length.EqualTo(length));
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public async Task SaveSectionReader_GetField_WithBoolValues_ReturnsExactValue(bool boolValue)
    {
        // Arrange
        var fileId = Path.Combine(_testDirectory, $"bool_test_{boolValue}.ngd");
        var saveBegin = _eventFactory.GetEvent<SaveBeginEventArgs>(EventNames.NAMESPACE, EventNames.SAVE_BEGIN_EVENT);
        var loadBegin = _eventFactory.GetEvent<LoadBeginEventArgs>(EventNames.NAMESPACE, EventNames.LOAD_BEGIN_EVENT);

        bool retrievedValue = !boolValue; // Opposite to verify it changes

        saveBegin.Subscribe(this, (in SaveBeginEventArgs args) =>
        {
            var section = args.Writer.AddSection("BoolTest");
            section.AddField("BoolValue", boolValue);
        });

        loadBegin.Subscribe(this, (in LoadBeginEventArgs args) =>
        {
            var section = args.Reader.FindSection("BoolTest");
            if (section != null)
            {
                retrievedValue = section.GetField<bool>("BoolValue");
            }
        });

        // Act
        await _dataProvider.Save(fileId, default);
        await _dataProvider.Load(fileId);

        // Assert
        Assert.That(retrievedValue, Is.EqualTo(boolValue));
    }

    [Test]
    public async Task SaveSectionReader_GetField_WithNegativeValues_ReturnsCorrectValues()
    {
        // Arrange
        var fileId = Path.Combine(_testDirectory, "negative_values_test.ngd");
        var saveBegin = _eventFactory.GetEvent<SaveBeginEventArgs>(EventNames.NAMESPACE, EventNames.SAVE_BEGIN_EVENT);
        var loadBegin = _eventFactory.GetEvent<LoadBeginEventArgs>(EventNames.NAMESPACE, EventNames.LOAD_BEGIN_EVENT);

        int negInt = -999;
        double negDouble = -123.456;
        int retrievedInt = 0;
        double retrievedDouble = 0;

        saveBegin.Subscribe(this, (in SaveBeginEventArgs args) =>
        {
            var section = args.Writer.AddSection("NegativeTest");
            section.AddField("NegInt", negInt);
            section.AddField("NegDouble", negDouble);
        });

        loadBegin.Subscribe(this, (in LoadBeginEventArgs args) =>
        {
            var section = args.Reader.FindSection("NegativeTest");
            if (section != null)
            {
                retrievedInt = section.GetField<int>("NegInt");
                retrievedDouble = section.GetField<double>("NegDouble");
            }
        });

        // Act
        await _dataProvider.Save(fileId, default);
        await _dataProvider.Load(fileId);

		using (Assert.EnterMultipleScope())
		{
			// Assert
			Assert.That(retrievedInt, Is.EqualTo(negInt));
			Assert.That(retrievedDouble, Is.EqualTo(negDouble).Within(0.001));
		}
	}

    [Test]
    public async Task SaveSectionReader_Dispose_CanBeCalledMultipleTimes()
    {
        // Arrange
        var fileId = Path.Combine(_testDirectory, "dispose_multiple_test.ngd");
        var saveBegin = _eventFactory.GetEvent<SaveBeginEventArgs>(EventNames.NAMESPACE, EventNames.SAVE_BEGIN_EVENT);
        var loadBegin = _eventFactory.GetEvent<LoadBeginEventArgs>(EventNames.NAMESPACE, EventNames.LOAD_BEGIN_EVENT);

        ISaveSectionReader? sectionReader = null;

        void OnSaveBegin(in SaveBeginEventArgs args)
        {
            var section = args.Writer.AddSection("DisposeMultipleTest");
            section.AddField("TestField", 1);
        }

        saveBegin.Subscribe(this, OnSaveBegin);

        void OnLoadBegin(in LoadBeginEventArgs args)
        {
            sectionReader = args.Reader.FindSection("DisposeMultipleTest");
        }

        loadBegin.Subscribe(this, OnLoadBegin);

        // Act & Assert
        await _dataProvider.Save(fileId, default);
        await _dataProvider.Load(fileId);

        if (sectionReader != null)
        {
            sectionReader.Dispose();
            sectionReader.Dispose(); // Should not throw
            sectionReader.Dispose(); // Should not throw

            Assert.Pass("Multiple dispose calls handled");
        }
    }

    [Test]
    public async Task SaveSectionReader_Name_MatchesSectionWriterName()
    {
        // Arrange
        var fileId = Path.Combine(_testDirectory, "name_match_test.ngd");
        var saveBegin = _eventFactory.GetEvent<SaveBeginEventArgs>(EventNames.NAMESPACE, EventNames.SAVE_BEGIN_EVENT);
        var loadBegin = _eventFactory.GetEvent<LoadBeginEventArgs>(EventNames.NAMESPACE, EventNames.LOAD_BEGIN_EVENT);

        string writerSectionName = "NameMatchSection";
        string readerSectionName = "";

        void OnSaveBegin(in SaveBeginEventArgs args)
        {
            var section = args.Writer.AddSection(writerSectionName);
            Assert.That(section.Name, Is.EqualTo(writerSectionName));
            section.AddField("TestField", 1);
        }

        saveBegin.Subscribe(this, OnSaveBegin);

        void OnLoadBegin(in LoadBeginEventArgs args)
        {
            var section = args.Reader.FindSection(writerSectionName);
            if (section != null)
            {
                readerSectionName = section.Name;
            }
        }

        loadBegin.Subscribe(this, OnLoadBegin);

        // Act
        await _dataProvider.Save(fileId, default);
        await _dataProvider.Load(fileId);

        // Assert
        Assert.That(readerSectionName, Is.EqualTo(writerSectionName));
    }

    [Test]
    public async Task SaveSectionReader_FieldCount_MatchesSectionWriterFieldCount()
    {
        // Arrange
        var fileId = Path.Combine(_testDirectory, "field_count_match_test.ngd");
        var saveBegin = _eventFactory.GetEvent<SaveBeginEventArgs>(EventNames.NAMESPACE, EventNames.SAVE_BEGIN_EVENT);
        var loadBegin = _eventFactory.GetEvent<LoadBeginEventArgs>(EventNames.NAMESPACE, EventNames.LOAD_BEGIN_EVENT);

        int writerFieldCount = 0;
        int readerFieldCount = 0;

        void OnSaveBegin(in SaveBeginEventArgs args)
        {
            var section = args.Writer.AddSection("CountMatchTest");
            for (int i = 0; i < 10; i++)
            {
                section.AddField($"Field_{i}", i);
            }
            writerFieldCount = section.FieldCount;
        }

        saveBegin.Subscribe(this, OnSaveBegin);

        void OnLoadBegin(in LoadBeginEventArgs args)
        {
            var section = args.Reader.FindSection("CountMatchTest");
            if (section != null)
            {
                readerFieldCount = section.FieldCount;
            }
        }

        loadBegin.Subscribe(this, OnLoadBegin);

        // Act
        await _dataProvider.Save(fileId, default);
        await _dataProvider.Load(fileId);

        // Assert
        Assert.That(readerFieldCount, Is.EqualTo(writerFieldCount));
        Assert.That(readerFieldCount, Is.EqualTo(10));
    }
}
#endif
