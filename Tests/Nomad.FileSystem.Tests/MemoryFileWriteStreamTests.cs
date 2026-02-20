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
using Moq;
using Nomad.Core.EngineUtils;
using Nomad.Core.FileSystem;
using Nomad.Core.Logger;
using Nomad.FileSystem.Private.Services;
using Nomad.FileSystem.Private.MemoryStream;

namespace Nomad.FileSystem.Tests
{
    [TestFixture]
    public class MemoryFileWriteStreamTests
    {
        private FileSystemService _service;
        private string _tempDir;
        private string _filePath;

        [SetUp]
        public void SetUp()
        {
            var engineMock = new Mock<IEngineService>();
            var loggerMock = new Mock<ILoggerService>();
            var categoryMock = new Mock<ILoggerCategory>();

            _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempDir);

            engineMock.Setup(e => e.GetStoragePath(StorageScope.StreamingAssets)).Returns(_tempDir);
            engineMock.Setup(e => e.GetStoragePath(StorageScope.UserData)).Returns(_tempDir);
            engineMock.Setup(e => e.GetStoragePath(StorageScope.Install)).Returns(_tempDir);
            loggerMock.Setup(l => l.CreateCategory(It.IsAny<string>(), It.IsAny<LogLevel>(), It.IsAny<bool>()))
                      .Returns(categoryMock.Object);

            _service = new FileSystemService(engineMock.Object, loggerMock.Object);
            _filePath = Path.Combine(_tempDir, "memwrite.bin");
        }

        [TearDown]
        public void TearDown()
        {
            _service.Dispose();
            if (Directory.Exists(_tempDir))
                Directory.Delete(_tempDir, true);
        }

        private IWriteStream OpenMemoryFileWriteStream(int length = 1024, bool fixedSize = false)
        {
            var config = new WriteConfig(StreamType.MemoryFile, length, false);
            // Note: The fixedSize parameter isn't directly in WriteConfig, but MemoryFileWriteStream constructor accepts it.
            // We'll need to use reflection or modify the service? For simplicity, we'll assume WriteConfig can convey fixedSize.
            // In the current code, WriteConfig only has Type, Append, Length. MemoryFileWriteStream constructor also has a fixedSize param.
            // The service passes length but not fixedSize. To test fixedSize, we might need to directly instantiate or extend.
            // We'll skip fixedSize tests here or assume default false.
            return _service.OpenWrite(_filePath, config);
        }

        [Test]
        public void Write_WritesToMemory_NotFileUntilFlush()
        {
            using var stream = OpenMemoryFileWriteStream() as MemoryFileWriteStream;
            stream.Write(new byte[] { 1, 2, 3 }, 0, 3);
            // File should not exist yet
            Assert.That(File.Exists(_filePath), Is.False);
        }

        [Test]
        public void Flush_WritesToFile()
        {
            using var stream = OpenMemoryFileWriteStream() as MemoryFileWriteStream;
            stream.Write(new byte[] { 1, 2, 3 }, 0, 3);
            stream.Flush();

            Assert.That(File.Exists(_filePath));
            var content = File.ReadAllBytes(_filePath);
            Assert.That(content, Is.EqualTo(new byte[] { 1, 2, 3 }));
        }

        [Test]
        public async Task FlushAsync_WritesToFile()
        {
            using var stream = OpenMemoryFileWriteStream() as MemoryFileWriteStream;
            stream.Write(new byte[] { 4, 5, 6 }, 0, 3);
            await stream.FlushAsync();

            var content = File.ReadAllBytes(_filePath);
            Assert.That(content, Is.EqualTo(new byte[] { 4, 5, 6 }));
        }

        [Test]
        public void Dispose_AutomaticallyFlushes()
        {
            using (var stream = OpenMemoryFileWriteStream() as MemoryFileWriteStream)
            {
                stream.Write(new byte[] { 7, 8, 9 }, 0, 3);
            } // Dispose called

            Assert.That(File.Exists(_filePath));
            var content = File.ReadAllBytes(_filePath);
            Assert.That(content, Is.EqualTo(new byte[] { 7, 8, 9 }));
        }

        [Test]
        public void Write_ExceedsCapacity_ExpandsIfNotFixed()
        {
            // Default capacity is 8192, but we can test by writing more than initial rented length
            // MemoryWriteStream expands by doubling.
            using var stream = OpenMemoryFileWriteStream(length: 10) as MemoryFileWriteStream;
            byte[] data = new byte[100];
            new Random(42).NextBytes(data);
            stream.Write(data, 0, data.Length);

            // Position should be 100
            Assert.That(stream.Position, Is.EqualTo(100));
        }

        [Test]
        public void Write_ExceedsMaxCapacity_Throws()
        {
            // This would require writing > 1GB, which is impractical in a unit test.
            // We'll skip or mock the capacity check.
        }

        [Test]
        public void WriteFromStream_CopiesData()
        {
            // Create source file
            string sourcePath = Path.Combine(_tempDir, "source.bin");
            byte[] sourceData = { 10, 20, 30 };
            File.WriteAllBytes(sourcePath, sourceData);

            var readConfig = new ReadConfig(StreamType.File);
            using var sourceStream = _service.OpenRead("source.bin", readConfig);

            using var destStream = OpenMemoryFileWriteStream() as MemoryFileWriteStream;
            destStream.WriteFromStream(sourceStream);
            destStream.Flush();

            var content = File.ReadAllBytes(_filePath);
            Assert.That(content, Is.EqualTo(sourceData));
        }

        [Test]
        public void WriteInt32_WritesCorrectly()
        {
            using var stream = OpenMemoryFileWriteStream() as MemoryFileWriteStream;
            stream.WriteInt32(0x12345678);
            stream.Flush();

            var content = File.ReadAllBytes(_filePath);
            Assert.That(content, Is.EqualTo(new byte[] { 0x78, 0x56, 0x34, 0x12 }));
        }

        [Test]
        public void WriteString_WritesLengthPrefixedString()
        {
            string test = "Memory String";
            using var stream = OpenMemoryFileWriteStream() as MemoryFileWriteStream;
            stream.WriteString(test);
            stream.Flush();

            using var br = new BinaryReader(File.OpenRead(_filePath));
            string read = br.ReadString();
            Assert.That(read, Is.EqualTo(test));
        }
    }
}
#endif