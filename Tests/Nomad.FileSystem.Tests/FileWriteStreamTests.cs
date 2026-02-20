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

namespace Nomad.FileSystem.Tests
{
	[TestFixture]
	public class FileWriteStreamTests
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
			_filePath = Path.Combine(_tempDir, "writetest.bin");
		}

		[TearDown]
		public void TearDown()
		{
			_service.Dispose();
			if (Directory.Exists(_tempDir))
				Directory.Delete(_tempDir, true);
		}

		private IWriteStream OpenWriteStream(bool append = false)
		{
			var config = new WriteConfig(StreamType.File, append);
			return _service.OpenWrite(_filePath, config);
		}

		[Test]
		public void Write_WritesBytes()
		{
			byte[] data = { 1, 2, 3, 4, 5 };
			using (var stream = OpenWriteStream())
			{
				stream.Write(data, 0, data.Length);
			}

			var written = File.ReadAllBytes(_filePath);
			Assert.That(written, Is.EqualTo(data));
		}

		[Test]
		public void Write_SpanOverload_Writes()
		{
			Span<byte> data = stackalloc byte[] { 10, 20, 30 };
			using (var stream = OpenWriteStream())
			{
				stream.Write(data, 0, data.Length);
			}

			var written = File.ReadAllBytes(_filePath);
			Assert.That(written, Is.EqualTo(new byte[] { 10, 20, 30 }));
		}

		[Test]
		public async Task WriteAsync_WritesBytes()
		{
			byte[] data = { 100, 200, 255 };
			using (var stream = OpenWriteStream())
			{
				await stream.WriteAsync(data, 0, data.Length);
			}

			var written = File.ReadAllBytes(_filePath);
			Assert.That(written, Is.EqualTo(data));
		}

		[Test]
		public void AppendMode_AddsToEnd()
		{
			File.WriteAllBytes(_filePath, new byte[] { 1, 2, 3 });
			using (var stream = OpenWriteStream(append: true))
			{
				stream.Write(new byte[] { 4, 5, 6 }, 0, 3);
			}

			var written = File.ReadAllBytes(_filePath);
			Assert.That(written, Is.EqualTo(new byte[] { 1, 2, 3, 4, 5, 6 }));
		}

		[Test]
		public void WriteFromStream_CopiesData()
		{
			// Create a source file
			string sourcePath = Path.Combine(_tempDir, "source.bin");
			byte[] sourceData = [7, 8, 9, 10];
			{
				File.WriteAllBytes(sourcePath, sourceData);
			}
			{

				var readConfig = new ReadConfig(StreamType.File);
				using var sourceStream = _service.OpenRead(sourcePath, readConfig);

				using var destStream = OpenWriteStream();
				destStream.WriteFromStream(sourceStream);
			}
			{

				var written = File.ReadAllBytes(_filePath);
				Assert.That(written, Is.EqualTo(sourceData));
			}
		}

		[Test]
		public async Task WriteFromStreamAsync_CopiesData()
		{
			string sourcePath = Path.Combine(_tempDir, "source.bin");
			byte[] sourceData = { 11, 12, 13 };
			{
				File.WriteAllBytes(sourcePath, sourceData);
			}
			{
				var readConfig = new ReadConfig(StreamType.File);
				using var sourceStream = _service.OpenRead(sourcePath, readConfig);
				using var destStream = OpenWriteStream();
				await destStream.WriteFromStreamAsync(sourceStream);
			}
			{
				var written = File.ReadAllBytes(_filePath);
				Assert.That(written, Is.EqualTo(sourceData));
			}
		}

		[Test]
		public void WriteByte_WritesSingleByte()
		{
			{
				using var stream = OpenWriteStream();
				stream.WriteByte(0xAB);
			}
			{
				var written = File.ReadAllBytes(_filePath);
				Assert.That(written, Is.EqualTo(new byte[] { 0xAB }));
			}
		}

		[Test]
		public void WriteInt32_WritesLittleEndian()
		{
			{
				using var stream = OpenWriteStream();
				stream.WriteInt32(0x12345678);
			}
			{
				var written = File.ReadAllBytes(_filePath);
				// Little-endian: 0x78 0x56 0x34 0x12
				Assert.That(written, Is.EqualTo(new byte[] { 0x78, 0x56, 0x34, 0x12 }));
			}
		}

		[Test]
		public void WriteString_WritesLengthPrefixedString()
		{
			{
				using var stream = OpenWriteStream();
				stream.WriteString("Test");
			}
			{
				// Read back via BinaryReader
				using var br = new BinaryReader(File.OpenRead(_filePath));
				string read = br.ReadString();
				Assert.That(read, Is.EqualTo("Test"));
			}
		}

		[Test]
		public void Flush_EnsuresDataWritten()
		{
			{
				using var stream = OpenWriteStream();
				stream.Write(new byte[] { 1, 2, 3 }, 0, 3);
				stream.Flush();
			}
			{
				// Even without disposing, file should exist
				Assert.That(File.Exists(_filePath));
				var written = File.ReadAllBytes(_filePath);
				Assert.That(written, Is.EqualTo(new byte[] { 1, 2, 3 }));
			}
		}
	}
}
#endif