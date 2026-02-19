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
using System.Threading;
using System.Threading.Tasks;
using Nomad.Core.FileSystem;
using Nomad.FileSystem.Private.FileStream;
using Nomad.FileSystem.Private.Services;
using NUnit.Framework;

namespace Nomad.FileSystem.Tests
{
    [TestFixture]
    public class FileWriteStreamTests
    {
        private IFileSystem _fileSystem;
        private string _baseDir;

        [SetUp]
        public void Setup()
        {
            var engineService = new MockEngineService();
            var logger = new MockLogger();
            _fileSystem = new FileSystemService(engineService, logger);

            // Use a unique subdirectory under the current working directory so the
            // FileSystemService search helper (which uses the mock engine's storage path)
            // will find files created by the tests.
            _baseDir = Path.Combine(Directory.GetCurrentDirectory(), "NomadFileSystemTest_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_baseDir);
        }

        [TearDown]
        public void TearDown()
        {
            try
            {
                _fileSystem?.Dispose();
            }
            catch { }

            try
            {
                if (!string.IsNullOrEmpty(_baseDir) && Directory.Exists(_baseDir))
                {
                    Directory.Delete(_baseDir, true);
                }
            }
            catch { }
        }

		[Test]
		public void CreateWriteStream_EmptyFileName_ThrowsArgumentNullOrEmptyException()
		{
			// Arrange
			var exception = Assert.Throws<ArgumentException>(
				() => _fileSystem.OpenWrite(String.Empty, new WriteConfig(StreamType.File))
			);

			// Assert
			Assert.That(exception, Is.Not.Null);
		}

		[Test]
		public void FileWriteStream_CreateWithNullFileName_ThrowsArgumentNullOrEmptyException()
		{
			// Arrange
			var exception = Assert.Throws<ArgumentNullException>(
				() => _fileSystem.OpenWrite(null, new WriteConfig( StreamType.File))
			);

			// Assert
			Assert.That(exception, Is.Not.Null);
		}

		[Test]
		public void FileWriteStream_CreateWithFileStreamParams_CreatesFileWriteStream()
		{
			// Arrange
			using var stream = _fileSystem.OpenWrite($"{_baseDir}test_file.txt", new WriteConfig(StreamType.File));

			// Assert
			Assert.That(stream, Is.InstanceOf<FileWriteStream>());
		}

		[Test]
		public void FileWriteStream_CreateWithValidParams_CanReadIsFalseAndWriteIsTrue()
		{
			// Arrange
			using var stream = _fileSystem.OpenWrite($"{_baseDir}test_file.txt", new WriteConfig(StreamType.File));

			// Assert
			Assert.That(stream, Is.Not.Null);
			using (Assert.EnterMultipleScope())
			{
				Assert.That(stream.CanRead, Is.False);
				Assert.That(stream.CanWrite, Is.True);
			}
		}

		[Test]
		public void FileWriteStream_WriteNullBuffer_ThrowsArgumentNullException()
		{
			// Arrange
			using var stream = _fileSystem.OpenWrite($"{_baseDir}test_file.txt", new WriteConfig(StreamType.File));
			Assert.That(stream, Is.Not.Null);

			// Act
			var exception = Assert.Throws<ArgumentNullException>(
				() => stream.Write(null, 0, 1024)
			);

			// Assert
			Assert.That(exception, Is.InstanceOf<ArgumentNullException>());
		}

		[Test]
		public async Task FileWriteStream_WriteNullBufferAsync_ThrowsArgumentNullException()
		{
			// Arrange
			using var stream = _fileSystem.OpenWrite($"{_baseDir}test_file.txt", new WriteConfig(StreamType.File));
			Assert.That(stream, Is.Not.Null);

			// Act
			var exception = Assert.ThrowsAsync<ArgumentNullException>(
				async () => await stream.WriteAsync(null, 0, 1024)
			);

			// Assert
			Assert.That(exception, Is.InstanceOf<ArgumentNullException>());
		}
	}
}
#endif