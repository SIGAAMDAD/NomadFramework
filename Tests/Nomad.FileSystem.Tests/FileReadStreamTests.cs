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
using Nomad.FileSystem.Private.FileStream;

namespace Nomad.FileSystem.Tests
{
    [TestFixture]
    public class FileReadStreamTests
    {
        private FileSystemService _service;
        private string _tempDir;
        private string _filePath;
        private byte[] _testData;

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

            _testData = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            _filePath = Path.Combine(_tempDir, "readtest.bin");
            File.WriteAllBytes(_filePath, _testData);
        }

        [TearDown]
        public void TearDown()
        {
            _service.Dispose();
            if (Directory.Exists(_tempDir))
                Directory.Delete(_tempDir, true);
        }

        private IReadStream OpenReadStream()
        {
            var config = new ReadConfig(StreamType.File);
            return _service.OpenRead("readtest.bin", config);
        }

        [Test]
        public void Read_ReadsAllBytes()
        {
            using var stream = OpenReadStream();
            byte[] buffer = new byte[_testData.Length];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            Assert.That(bytesRead, Is.EqualTo(_testData.Length));
            Assert.That(buffer, Is.EqualTo(_testData));
        }

        [Test]
        public void Read_PartialRead_AdvancesPosition()
        {
            using var stream = OpenReadStream();
            byte[] buffer = new byte[4];
            int bytesRead = stream.Read(buffer, 0, 4);
            Assert.That(bytesRead, Is.EqualTo(4));
            Assert.That(buffer, Is.EqualTo(new byte[] { 0, 1, 2, 3 }));
            Assert.That(stream.Position, Is.EqualTo(4));
        }

        [Test]
        public void Read_BeyondEnd_ReturnsZero()
        {
            using var stream = OpenReadStream();
            stream.Position = _testData.Length;
            byte[] buffer = new byte[4];
            int bytesRead = stream.Read(buffer, 0, 4);
            Assert.That(bytesRead, Is.EqualTo(0));
        }

        [Test]
        public void Read_SpanOverload_Works()
        {
            using var stream = OpenReadStream();
            Span<byte> span = new byte[5];
            int bytesRead = stream.Read(span, 0, 5);
            Assert.That(bytesRead, Is.EqualTo(5));
            Assert.That(span.ToArray(), Is.EqualTo(new byte[] { 0, 1, 2, 3, 4 }));
        }

        [Test]
        public async Task ReadAsync_ReadsAllBytes()
        {
            using var stream = OpenReadStream();
            byte[] buffer = new byte[_testData.Length];
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            Assert.That(bytesRead, Is.EqualTo(_testData.Length));
            Assert.That(buffer, Is.EqualTo(_testData));
        }

        [Test]
        public void ReadToEnd_ReadsRemaining()
        {
            using var stream = OpenReadStream();
            stream.Position = 3;
            byte[] remaining = stream.ReadToEnd();
            Assert.That(remaining, Is.EqualTo(new byte[] { 3, 4, 5, 6, 7, 8, 9 }));
        }

        [Test]
        public void ToArray_ReadsEntireFileWithoutMovingPosition()
        {
            using var stream = OpenReadStream();
            stream.Position = 5;
            byte[] all = stream.ToArray();
            Assert.That(all, Is.EqualTo(_testData));
            Assert.That(stream.Position, Is.EqualTo(5));
        }

        [Test]
        public void Seek_SetsPosition()
        {
            using var stream = OpenReadStream();
            int newPos = stream.Seek(4, SeekOrigin.Begin);
            Assert.That(newPos, Is.EqualTo(4));
            Assert.That(stream.Position, Is.EqualTo(4));

            newPos = stream.Seek(2, SeekOrigin.Current);
            Assert.That(newPos, Is.EqualTo(6));

            newPos = stream.Seek(-2, SeekOrigin.End);
            Assert.That(newPos, Is.EqualTo(8));
        }

        [Test]
        public void ReadByte_ReturnsByte()
        {
            using var stream = OpenReadStream();
            byte b = stream.ReadByte();
            Assert.That(b, Is.EqualTo(0));
            Assert.That(stream.Position, Is.EqualTo(1));
        }

        [Test]
        public void ReadInt32_ReadsLittleEndian()
        {
            using var stream = OpenReadStream();
            int value = stream.ReadInt32();
            // Assuming little-endian: 0x03020100 = 50462976
            Assert.That(value, Is.EqualTo(50462976));
        }

        [Test]
        public void ReadString_ReadsLengthPrefixedString()
        {
            // Write a string via BinaryWriter to a separate file
            string testString = "Hello, world!";
            string stringFilePath = Path.Combine(_tempDir, "stringtest.bin");
            using (var bw = new BinaryWriter(File.OpenWrite(stringFilePath)))
            {
                bw.Write(testString);
            }

            var config = new ReadConfig(StreamType.File);
            using var stream = _service.OpenRead(stringFilePath, config) as FileReadStream;
            string read = stream.ReadString();
            Assert.That(read, Is.EqualTo(testString));
        }

        [Test]
        public void ReadBoolean_ReturnsBool()
        {
            // Prepare file with bool (1 byte)
            string boolFilePath = Path.Combine(_tempDir, "booltest.bin");
            File.WriteAllBytes(boolFilePath, new byte[] { 1 });
            var config = new ReadConfig(StreamType.File);
            using var stream = _service.OpenRead(boolFilePath, config) as FileReadStream;
            bool val = stream.ReadBoolean();
            Assert.That(val, Is.True);
        }
    }
}
#endif