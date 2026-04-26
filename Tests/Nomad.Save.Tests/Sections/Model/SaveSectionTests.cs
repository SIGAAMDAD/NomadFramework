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
using Nomad.Save.Private.Services;
using Nomad.Save.Services;
using Nomad.Core.CVars;
using Nomad.CVars.Private.Services;
using Moq;
using Nomad.Core.Engine.Services;

namespace Nomad.Save.Tests
{
    /// <summary>
    /// Tests for ISaveSectionWriter and ISaveSectionReader functionality
    /// </summary>
    [TestFixture]
    [Category("Nomad.Save")]
    [Category("Sections.Model")]
    [Category("Unit")]
    public class SaveSectionTests
    {
        private ISaveDataProvider _dataProvider;
        private ILoggerService _logger;
        private ICVarSystemService _cvarSystem;
        private IFileSystem _fileSystem;
        private IGameEventRegistryService _eventFactory;
        private string _testDirectory;

        private IGameEvent<SaveBeginEventArgs> _saveBegin;
        private IGameEvent<LoadBeginEventArgs> _loadBegin;

        [SetUp]
        public void Setup()
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), "NomadErrorHandlingTests", Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDirectory);
            Directory.CreateDirectory($"{_testDirectory}/SaveData");

            _logger = new MockLogger();
            var engineService = new Mock<IEngineService>();
            engineService.Setup(e => e.GetStoragePath(StorageScope.StreamingAssets)).Returns(_testDirectory);
            engineService.Setup(e => e.GetStoragePath(StorageScope.UserData)).Returns(_testDirectory);
            engineService.Setup(e => e.GetStoragePath(StorageScope.Install)).Returns(_testDirectory);
            _fileSystem = new FileSystemService(engineService.Object, _logger);
            _eventFactory = new GameEventRegistry(_logger);
            _cvarSystem = new CVarSystem(_eventFactory, _logger);
            _dataProvider = new SaveDataProvider(engineService.Object, _eventFactory, _cvarSystem, _fileSystem, _logger);

            _saveBegin = _eventFactory.GetEvent<SaveBeginEventArgs>(EventNames.SAVE_BEGIN_EVENT, EventNames.NAMESPACE);
            _loadBegin = _eventFactory.GetEvent<LoadBeginEventArgs>(EventNames.LOAD_BEGIN_EVENT, EventNames.NAMESPACE);
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
        public async Task AddSection_WithValidName_CreatesSection()
        {
            // Arrange
            string fileId = "section_test";
            bool sectionAdded = false;
            string sectionName = String.Empty;

            _saveBegin.Subscribe((in SaveBeginEventArgs args) =>
            {
                var section = args.Writer.AddSection("TestSection");
                sectionAdded = true;
                sectionName = section.Name;
                section.AddField("TestField", 42);
            });

            // Act
            await _dataProvider.Save(fileId, default);

            using (Assert.EnterMultipleScope())
            {
                // Assert
                Assert.That(sectionAdded, Is.True);
                Assert.That(sectionName, Is.EqualTo("TestSection"));
            }
        }

        [Test]
        public async Task AddField_WithVariousTypes_PreservesValues()
        {
            // Arrange
            string fileId = "field_types_test";

            float floatValue = 3.14159f;
            double doubleValue = 2.71828d;
            int intValue = 42;
            string stringValue = "test_string";
            bool boolValue = true;

            float loadedFloat = 0.0f;
            double loadedDouble = 0.0d;
            int loadedInt = 0;
            string loadedString = String.Empty;
            bool loadedBool = false;

            _saveBegin.Subscribe((in SaveBeginEventArgs args) =>
            {
                var section = args.Writer.AddSection("TypesSection");
                section.AddField("FloatField", floatValue);
                section.AddField("DoubleField", doubleValue);
                section.AddField("IntField", intValue);
                section.AddField("StringField", stringValue);
                section.AddField("BoolField", boolValue);
            });

            _loadBegin.Subscribe((in LoadBeginEventArgs args) =>
            {
                var section = args.Reader.FindSection("TypesSection");
                if (section != null)
                {
                    loadedFloat = section.GetField<float>("FloatField");
                    loadedDouble = section.GetField<double>("DoubleField");
                    loadedInt = section.GetField<int>("IntField");
                    loadedString = section.GetString("StringField");
                    loadedBool = section.GetField<bool>("BoolField");
                }
            });

            // Act
            await _dataProvider.Save(fileId, default);
            await _dataProvider.Load(fileId);

            using (Assert.EnterMultipleScope())
            {
                // Assert
                Assert.That(loadedFloat, Is.EqualTo(floatValue).Within(0.00001f));
                Assert.That(loadedDouble, Is.EqualTo(doubleValue).Within(0.00001f));
                Assert.That(loadedInt, Is.EqualTo(intValue));
                Assert.That(loadedString, Is.EqualTo(stringValue));
                Assert.That(loadedBool, Is.EqualTo(boolValue));
            }
        }

        [Test]
        public async Task FieldCount_ReturnsCorrectCount()
        {
            // Arrange
            string fileId = "field_count_test";
            int fieldCount = 0;

            void OnSaveBegin(in SaveBeginEventArgs args)
            {
                var section = args.Writer.AddSection("CountSection");
                section.AddField("Field1", 1);
                section.AddField("Field2", 2);
                section.AddField("Field3", 3);
                section.AddField("Field4", 4);
                section.AddField("Field5", 5);
                fieldCount = section.FieldCount;
            }

            _saveBegin.Subscribe(OnSaveBegin);

            // Act
            await _dataProvider.Save(fileId, default);

            // Assert
            Assert.That(fieldCount, Is.EqualTo(5));
        }

        [Test]
        public async Task SectionName_PropertyIsAccessible()
        {
            // Arrange
            string fileId = "section_name_test";
            string expectedSectionName = "MyCustomSection";
            string actualSectionName = String.Empty;

            _saveBegin.Subscribe((in SaveBeginEventArgs args) =>
            {
                var section = args.Writer.AddSection(expectedSectionName);
                actualSectionName = section.Name;
            });

            // Act
            await _dataProvider.Save(fileId, default);

            // Assert
            Assert.That(actualSectionName, Is.EqualTo(expectedSectionName));
        }

        [Test]
        public async Task GetField_WithNonExistentField_ReturnsDefault()
        {
            // Arrange
            string fileId = "nonexistent_field_test";
            int retrievedValue = int.MinValue;

            _saveBegin.Subscribe((in SaveBeginEventArgs args) =>
            {
                var section = args.Writer.AddSection("TestSection");
                section.AddField("ExistingField", 10);
            });

            _loadBegin.Subscribe((in LoadBeginEventArgs args) =>
            {
                var section = args.Reader.FindSection("TestSection");
                if (section != null)
                {
                    retrievedValue = section.GetField<int>("NonExistentField");
                }
            });

            // Act
            await _dataProvider.Save(fileId, default);
            await _dataProvider.Load(fileId);

            // Assert
            Assert.That(retrievedValue, Is.Default);
        }

        [Test]
        public async Task HasField_WithExistingField_ReturnsTrue()
        {
            // Arrange
            string fileId = "has_field_test";
            bool hasField = false;

            _saveBegin.Subscribe((in SaveBeginEventArgs args) =>
            {
                var section = args.Writer.AddSection("TestSection");
                section.AddField("KnownField", 100);
                hasField = section.HasField<int>("KnownField");
            });

            // Act
            await _dataProvider.Save(fileId, default);

            // Assert
            Assert.That(hasField, Is.True);
        }

        [Test]
        public async Task FindSection_WithValidSectionName_ReturnsSection()
        {
            // Arrange
            string fileId = "find_section_test";
            bool foundSection = false;

            _saveBegin.Subscribe((in SaveBeginEventArgs args) =>
            {
                var section = args.Writer.AddSection("SpecificSection");
                section.AddField("Value", 999);
            });

            _loadBegin.Subscribe((in LoadBeginEventArgs args) =>
            {
                var section = args.Reader.FindSection("SpecificSection");
                foundSection = section != null;
            });

            // Act
            await _dataProvider.Save(fileId, default);
            await _dataProvider.Load(fileId);

            // Assert
            Assert.That(foundSection, Is.True);
        }

        [Test]
        public async Task FindSection_WithInvalidSectionName_ReturnsNull()
        {
            // Arrange
            string fileId = "find_section_invalid_test";
            bool foundSection = true;

            _saveBegin.Subscribe((in SaveBeginEventArgs args) =>
            {
                var section = args.Writer.AddSection("ExistingSection");
                section.AddField("Value", 1);
            });

            _loadBegin.Subscribe((in LoadBeginEventArgs args) =>
            {
                var section = args.Reader.FindSection("NonExistentSection");
                foundSection = section != null;
            });

            // Act
            await _dataProvider.Save(fileId, default);
            await _dataProvider.Load(fileId);

            // Assert
            Assert.That(foundSection, Is.False);
        }

        [Test]
        public async Task AddField_WithIntegerTypes_PreservesValues()
        {
            // Arrange
            string fileId = "integer_types_test";
            var saveBegin = _eventFactory.GetEvent<SaveBeginEventArgs>(EventNames.SAVE_BEGIN_EVENT, EventNames.NAMESPACE);
            var loadBegin = _eventFactory.GetEvent<LoadBeginEventArgs>(EventNames.LOAD_BEGIN_EVENT, EventNames.NAMESPACE);

            var sbyte_val = (sbyte)-128;
            var short_val = (short)-32000;
            var int_val = -2000000;
            var long_val = -9000000000000000000L;
            var byte_val = (byte)200;
            var ushort_val = (ushort)60000;
            var uint_val = 3000000000U;
            var ulong_val = 18000000000000000000UL;

            var loaded_sbyte = (sbyte)0;
            var loaded_short = (short)0;
            var loaded_int = 0;
            var loaded_long = 0L;
            var loaded_byte = (byte)0;
            var loaded_ushort = (ushort)0;
            var loaded_uint = 0U;
            var loaded_ulong = 0UL;

            _saveBegin.Subscribe((in SaveBeginEventArgs args) =>
            {
                var section = args.Writer.AddSection("IntegerTypes");
                section.AddField("SByteField", sbyte_val);
                section.AddField("ShortField", short_val);
                section.AddField("IntField", int_val);
                section.AddField("LongField", long_val);
                section.AddField("ByteField", byte_val);
                section.AddField("UShortField", ushort_val);
                section.AddField("UIntField", uint_val);
                section.AddField("ULongField", ulong_val);
            });

            _loadBegin.Subscribe((in LoadBeginEventArgs args) =>
            {
                var section = args.Reader.FindSection("IntegerTypes");
                if (section != null)
                {
                    loaded_sbyte = section.GetField<sbyte>("SByteField");
                    loaded_short = section.GetField<short>("ShortField");
                    loaded_int = section.GetField<int>("IntField");
                    loaded_long = section.GetField<long>("LongField");
                    loaded_byte = section.GetField<byte>("ByteField");
                    loaded_ushort = section.GetField<ushort>("UShortField");
                    loaded_uint = section.GetField<uint>("UIntField");
                    loaded_ulong = section.GetField<ulong>("ULongField");
                }
            });

            // Act
            await _dataProvider.Save(fileId, default);
            await _dataProvider.Load(fileId);

            using (Assert.EnterMultipleScope())
            {
                // Assert
                Assert.That(loaded_sbyte, Is.EqualTo(sbyte_val));
                Assert.That(loaded_short, Is.EqualTo(short_val));
                Assert.That(loaded_int, Is.EqualTo(int_val));
                Assert.That(loaded_long, Is.EqualTo(long_val));
                Assert.That(loaded_byte, Is.EqualTo(byte_val));
                Assert.That(loaded_ushort, Is.EqualTo(ushort_val));
                Assert.That(loaded_uint, Is.EqualTo(uint_val));
                Assert.That(loaded_ulong, Is.EqualTo(ulong_val));
            }
        }

        [Test]
        public async Task AddField_WithFloatingPointTypes_PreservesValues()
        {
            // Arrange
            string fileId = "float_types_test";
            var saveBegin = _eventFactory.GetEvent<SaveBeginEventArgs>(EventNames.SAVE_BEGIN_EVENT, EventNames.NAMESPACE);
            var loadBegin = _eventFactory.GetEvent<LoadBeginEventArgs>(EventNames.LOAD_BEGIN_EVENT, EventNames.NAMESPACE);

            var float_val = 3.14159f;
            var double_val = 2.71828;

            var loaded_float = 0f;
            var loaded_double = 0d;

            _saveBegin.Subscribe((in SaveBeginEventArgs args) =>
            {
                var section = args.Writer.AddSection("FloatTypes");
                section.AddField("FloatField", float_val);
                section.AddField("DoubleField", double_val);
            });

            _loadBegin.Subscribe((in LoadBeginEventArgs args) =>
            {
                var section = args.Reader.FindSection("FloatTypes");
                if (section != null)
                {
                    loaded_float = section.GetField<float>("FloatField");
                    loaded_double = section.GetField<double>("DoubleField");
                }
            });

            // Act
            await _dataProvider.Save(fileId, default);
            await _dataProvider.Load(fileId);

            using (Assert.EnterMultipleScope())
            {
                // Assert
                Assert.That(loaded_float, Is.EqualTo(float_val).Within(0.00001f));
                Assert.That(loaded_double, Is.EqualTo(double_val).Within(0.00001));
            }
        }

        [Test]
        public async Task SectionReader_FieldCount_ReturnsCorrectCount()
        {
            // Arrange
            string fileId = "reader_field_count_test";
            var saveBegin = _eventFactory.GetEvent<SaveBeginEventArgs>(EventNames.SAVE_BEGIN_EVENT, EventNames.NAMESPACE);
            var loadBegin = _eventFactory.GetEvent<LoadBeginEventArgs>(EventNames.LOAD_BEGIN_EVENT, EventNames.NAMESPACE);

            var readerFieldCount = 0;

            _saveBegin.Subscribe((in SaveBeginEventArgs args) =>
            {
                var section = args.Writer.AddSection("CountTest");
                section.AddField("F1", 1);
                section.AddField("F2", 2);
                section.AddField("F3", 3);
            });

            Console.WriteLine("Testing save files...");

            _loadBegin.Subscribe((in LoadBeginEventArgs args) =>
            {
                var section = args.Reader.FindSection("CountTest");
                if (section != null)
                {
                    readerFieldCount = section.FieldCount;
                }
            });

            // Act
            await _dataProvider.Save(fileId, default);
            await _dataProvider.Load(fileId);

            // Assert
            Assert.That(readerFieldCount, Is.EqualTo(3));
        }

        [Test]
        public async Task Multiple_Sections_IndependentStorage()
        {
            // Arrange
            string fileId = "multiple_sections_test";
            int playerValue = 0;
            int enemyValue = 0;
            int worldValue = 0;

            _saveBegin.Subscribe((in SaveBeginEventArgs args) =>
            {
                var playerSection = args.Writer.AddSection("Player");
                playerSection.AddField("Health", 100);
                playerSection.AddField("Mana", 50);

                var enemySection = args.Writer.AddSection("Enemy");
                enemySection.AddField("Health", 80);

                var worldSection = args.Writer.AddSection("World");
                worldSection.AddField("Level", 5);
            });

            _loadBegin.Subscribe((in LoadBeginEventArgs args) =>
            {
                var playerSection = args.Reader.FindSection("Player");
                var enemySection = args.Reader.FindSection("Enemy");
                var worldSection = args.Reader.FindSection("World");

                if (playerSection != null)
                {
                    playerValue = playerSection.GetField<int>("Health");
                }
                if (enemySection != null)
                {
                    enemyValue = enemySection.GetField<int>("Health");
                }
                if (worldSection != null)
                {
                    worldValue = worldSection.GetField<int>("Level");
                }
            });

            // Act
            await _dataProvider.Save(fileId, default);
            await _dataProvider.Load(fileId);

            using (Assert.EnterMultipleScope())
            {
                // Assert
                Assert.That(playerValue, Is.EqualTo(100));
                Assert.That(enemyValue, Is.EqualTo(80));
                Assert.That(worldValue, Is.EqualTo(5));
            }
        }

        [Test]
        [TestCase("SimpleSection")]
        [TestCase("Section_With_Underscores")]
        [TestCase("Section123")]
        public async Task AddSection_WithVariousNames_Succeeds(string sectionName)
        {
            // Arrange
            string fileId = "various_names_test";
            string createdSectionName = String.Empty;

            _saveBegin.Subscribe((in SaveBeginEventArgs args) =>
            {
                var section = args.Writer.AddSection(sectionName);
                createdSectionName = section.Name;
                section.AddField("TestField", 1);
            });

            // Act
            await _dataProvider.Save(fileId, default);

            // Assert
            Assert.That(createdSectionName, Is.EqualTo(sectionName));
        }
    }
}
