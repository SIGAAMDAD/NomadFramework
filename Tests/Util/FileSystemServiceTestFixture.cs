using System;
using System.IO;
using Moq;
using Nomad.Core.Engine.Services;
using Nomad.Core.Logger;
using Nomad.FileSystem.Private.Services;

namespace Nomad.FileSystem.Tests
{
    internal sealed class FileSystemServiceTestFixture : IDisposable
    {
        public Mock<IEngineService> EngineMock { get; }
        public Mock<ILoggerService> LoggerMock { get; }
        public Mock<ILoggerCategory> CategoryMock { get; }
        public FileSystemService Service { get; }
        public string TempDir { get; }

        public FileSystemServiceTestFixture()
        {
            EngineMock = new Mock<IEngineService>();
            LoggerMock = new Mock<ILoggerService>();
            CategoryMock = new Mock<ILoggerCategory>();

            TempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(TempDir);

            EngineMock.Setup(e => e.GetStoragePath(StorageScope.StreamingAssets)).Returns(TempDir);
            EngineMock.Setup(e => e.GetStoragePath(StorageScope.UserData)).Returns(TempDir);
            EngineMock.Setup(e => e.GetStoragePath(StorageScope.Install)).Returns(TempDir);
            LoggerMock.Setup(l => l.CreateCategory(It.IsAny<string>(), It.IsAny<LogLevel>(), It.IsAny<bool>()))
                      .Returns(CategoryMock.Object);

            Service = new FileSystemService(EngineMock.Object, LoggerMock.Object);
        }

        public string GetPath(string fileName)
        {
            return Path.Combine(TempDir, fileName);
        }

        public void Dispose()
        {
            Service.Dispose();
            if (Directory.Exists(TempDir))
            {
                Directory.Delete(TempDir, true);
            }
        }
    }
}
