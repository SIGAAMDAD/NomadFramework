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
    public class MemoryFileReadStreamTests
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

            _testData = new byte[] { 10, 20, 30, 40, 50 };
            _filePath = Path.Combine(_tempDir, "memread.bin");
            File.WriteAllBytes(_filePath, _testData);
        }

        [TearDown]
        public void TearDown()
        {
            _service.Dispose();
            if (Directory.Exists(_tempDir))
                Directory.Delete(_tempDir, true);
        }

        private IReadStream OpenMemoryFileReadStream()
        {
            var config = new ReadConfig(StreamType.MemoryFile);
            return _service.OpenRead("memread.bin", config);
        }

        [Test]
        public void Read_ReadsAllBytes()
        {
            using var stream = OpenMemoryFileReadStream();
            Assert.That(stream, Is.Not.Null);
            
            byte[] buffer = new byte[_testData.Length];
            int read = stream.Read(buffer, 0, buffer.Length);
			using (Assert.EnterMultipleScope())
			{
				Assert.That(read, Is.EqualTo(_testData.Length));
				Assert.That(buffer, Is.EqualTo(_testData));
			}
		}

        [Test]
        public void Read_Partial_AdvancesPosition()
        {
            using var stream = OpenMemoryFileReadStream();
            byte[] buffer = new byte[2];
            int read = stream.Read(buffer, 0, 2);
			using (Assert.EnterMultipleScope())
			{
				Assert.That(read, Is.EqualTo(2));
				Assert.That(buffer, Is.EqualTo(new byte[] { 10, 20 }));
				Assert.That(stream.Position, Is.EqualTo(2));
			}
		}

        [Test]
        public void ReadToEnd_ReadsRemaining()
        {
            using var stream = OpenMemoryFileReadStream();
            stream.Position = 2;
            byte[] remaining = stream.ReadToEnd();
            Assert.That(remaining, Is.EqualTo(new byte[] { 30, 40, 50 }));
        }

        [Test]
        public void ToArray_ReturnsFullBuffer()
        {
            using var stream = OpenMemoryFileReadStream() as MemoryFileReadStream;
            byte[] full = stream.ToArray();
            Assert.That(full, Is.EqualTo(_testData));
        }

        [Test]
        public void Seek_Works()
        {
            using var stream = OpenMemoryFileReadStream();
            stream.Seek(3, SeekOrigin.Begin);
            Assert.That(stream.Position, Is.EqualTo(3));
            byte b = stream.ReadByte();
            Assert.That(b, Is.EqualTo(40));
        }

        [Test]
        public void ReadInt32_ReadsCorrectly()
        {
            // Overwrite file with known ints
            using (var bw = new BinaryWriter(File.OpenWrite(_filePath)))
            {
                bw.Write(123456);
            }
            using var stream = OpenMemoryFileReadStream() as MemoryFileReadStream;
            int val = stream.ReadInt32();
            Assert.That(val, Is.EqualTo(123456));
        }

        [Test]
        public void ReadString_ReadsCorrectly()
        {
            string test = "Hello Memory";
            using (var bw = new BinaryWriter(File.OpenWrite(_filePath)))
            {
                bw.Write(test);
            }
            using var stream = OpenMemoryFileReadStream() as MemoryFileReadStream;
            string read = stream.ReadString();
            Assert.That(read, Is.EqualTo(test));
        }

        [Test]
        public void Dispose_ReleasesBuffer()
        {
            var stream = OpenMemoryFileReadStream() as MemoryFileReadStream;
            var buffer = stream.ToArray(); // internal buffer reference
            stream.Dispose();
            // After dispose, buffer is returned to pool; we can't verify directly but no exception
            Assert.DoesNotThrow(() => stream.Dispose()); // double dispose safe
        }
    }
}
#endif