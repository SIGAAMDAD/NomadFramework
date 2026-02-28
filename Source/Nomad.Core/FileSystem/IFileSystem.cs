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
using System.Threading;
using System.Threading.Tasks;
using Nomad.Core.FileSystem.Streams;
using Nomad.Core.Memory.Buffers;

namespace Nomad.Core.FileSystem
{
    /// <summary>
    /// Represents a virtual file system.
    /// </summary>
    public interface IFileSystem : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="directory"></param>
        void AddSearchDirectory(string directory);

        /// <summary>
        /// Gets the save directory path.
        /// </summary>
        /// <returns>The save directory path.</returns>
        string GetSavePath();

        /// <summary>
        /// Gets the resource directory path.
        /// </summary>
        /// <returns>The resource directory path.</returns>
        string GetResourcePath();

        /// <summary>
        /// Gets the configuration directory path.
        /// </summary>
        /// <returns>The configuration directory path.</returns>
        string GetConfigPath();

        /// <summary>
        /// Gets the user data directory path.
        /// </summary>
        /// <returns>The user data directory path.</returns>
        string GetUserDataPath();

        /// <summary>
        /// Checks if a file exists at the specified path.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns><c>true</c> if the file exists; otherwise, <c>false</c>.</returns>
        bool FileExists(string path);

        /// <summary>
        /// Deletes the file at the specified path.
        /// </summary>
        /// <param name="path">The path of the file to delete.</param>
        void DeleteFile(string path);

        /// <summary>
        /// Moves a file from the source path to the destination path.
        /// </summary>
        /// <param name="sourcePath">The source path of the file to move.</param>
        /// <param name="destinationPath">The destination path of the file to move.</param>
        /// <param name="overwrite"></param>
        void MoveFile(string sourcePath, string destinationPath, bool overwrite);

        /// <summary>
        /// Copies a file from the source path to the destination path.
        /// </summary>
        /// <param name="sourcePath">The source path of the file to copy.</param>
        /// <param name="destinationPath">The destination path of the file to copy.</param>
        /// <param name="overwrite">Whether to overwrite an existing file at the destination path.</param>
        void CopyFile(string sourcePath, string destinationPath, bool overwrite);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="destPath"></param>
        /// <param name="destBackupPath"></param>
        void ReplaceFile(string sourcePath, string destPath, string destBackupPath);

        /// <summary>
        /// Gets the last write time of the file at the specified path.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        /// <returns>The last write time of the file.</returns>
        DateTime GetLastWriteTime(string path);

        /// <summary>
        /// Gets the size of the file at the specified path.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        /// <returns>The size of the file in bytes.</returns>
        long GetFileSize(string path);

        /// <summary>
        /// Checks if a directory exists at the specified path.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns><c>true</c> if the directory exists; otherwise, <c>false</c>.</returns>
        bool DirectoryExists(string path);

        /// <summary>
        /// Creates a directory at the specified path.
        /// </summary>
        /// <param name="path">The path of the directory tso create.</param>
        void CreateDirectory(string path);

        /// <summary>
        /// Deletes the directory at the specified path.
        /// </summary>
        /// <param name="path">The path of the directory to delete.</param>
        /// <param name="recursive">Whether to recursively delete subdirectories and files.</param>
        void DeleteDirectory(string path, bool recursive);

        /// <summary>
        /// Gets a list of directories at the specified path matching the search pattern.
        /// </summary>
        /// <param name="path">The path of the directory to search.</param>
        /// <returns>The list of matching directory paths.</returns>
        IReadOnlyList<string> GetDirectories(string path);

        /// <summary>
        /// Gets a list of files at the specified path matching the search pattern.
        /// </summary>
        /// <param name="path">The path of the directory to search.</param>
        /// <param name="searchPattern">The search pattern to match against file names.</param>
        /// <param name="recursive">Whether to recursively search subdirectories.</param>
        /// <returns>The list of matching file paths.</returns>
        IReadOnlyList<string> GetFiles(string path, string searchPattern, bool recursive);

        /// <summary>
        /// Opens a read stream for the specified file path.
        /// </summary>
        /// <param name="config">How to create the stream and rules around how it should be handled.</param>
        /// <returns>The opened read stream.</returns>
        IReadStream? OpenRead(IReadConfig config);

        /// <summary>
        /// Opens a write stream for the specified file path.
        /// </summary>
        /// <param name="config">How to create the stream and rules around how it should be handled.</param>
        /// <returns>The opened write stream.</returns>
        IWriteStream? OpenWrite(IWriteConfig config);

        /// <summary>
        /// Opens a read stream for the specified file path.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The opened read stream.</returns>
        ValueTask<IReadStream?> OpenReadAsync(IReadConfig config, CancellationToken ct = default);

        /// <summary>
        /// Opens a write stream for the specified file path.
        /// </summary>
        /// <param name="config">How to create the stream and rules around how it should be handled.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The opened write stream.</returns>
        ValueTask<IWriteStream?> OpenWriteAsync(IWriteConfig config, CancellationToken ct = default);

        /// <summary>
        /// Creates a memory stream.
        /// </summary>
        /// <param name="accessMode">The access mode to create the memory stream with.</param>
        /// <param name="type"></param>
        /// <param name="outputFile">Optional output file path for the memory stream.</param>
        /// <param name="length">The initial length of the memory stream.</param>
        /// <returns>The created data stream.</returns>
        IDataStream CreateStream(System.IO.FileAccess accessMode, StreamType type, string outputFile = "", int length = 0);

        /// <summary>
        /// Loads a file into memory.
        /// </summary>
        /// <param name="path">The file to open and load.</param>
        /// <returns>The data retrieved from the file.</returns>
        IBufferHandle? LoadFile(string path);

        /// <summary>
        /// Loads a file into memory asynchronously.
        /// </summary>
        /// <param name="path">The file to open and load.</param>
        /// <param name="ct"></param>
        /// <returns>The data retrieved from the file.</returns>
        ValueTask<IBufferHandle?> LoadFileAsync(string path, CancellationToken ct = default);

        /// <summary>
        /// Writes a buffer to disk.
        /// </summary>
        /// <param name="path">The file to write to.</param>
        /// <param name="buffer">The data to write to disk.</param>
        /// <param name="offset">The position in <paramref name="buffer"/> to start writing from.</param>
        /// <param name="length">The number of bytes to be written.</param>
        void WriteFile(string path, in ReadOnlySpan<byte> buffer, int offset, int length);

        /// <summary>
        /// Writes a buffer asynchronously to disk.
        /// </summary>
        /// <param name="path">The file to write to.</param>
        /// <param name="buffer">The data to write to disk.</param>
        /// <param name="offset">The position in <paramref name="buffer"/> to start writing from.</param>
        /// <param name="length">The number of bytes to be written.</param>
        /// <param name="ct">The cancellation token.</param>
        ValueTask WriteFileAsync(string path, ReadOnlyMemory<byte> buffer, int offset, int length, CancellationToken ct = default);
    }
}

