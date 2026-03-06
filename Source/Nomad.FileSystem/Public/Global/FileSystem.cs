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
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Nomad.Core.Compatibility.Guards;
using Nomad.Core.Exceptions;
using Nomad.Core.FileSystem;
using Nomad.Core.FileSystem.Streams;
using Nomad.Core.Memory.Buffers;

namespace Nomad.FileSystem.Global
{
    /// <summary>
    /// 
    /// </summary>
    public static class FileSystem
    {
        private static IFileSystem Instance => _instance ?? throw new SubsystemNotInitializedException();
        private static IFileSystem? _instance;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        internal static void Initialize(IFileSystem instance)
        {
            ArgumentGuard.ThrowIfNull(instance);
            _instance = instance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="directory"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddSearchDirectory(string directory)
        {
            Instance.AddSearchDirectory(directory);
        }

        /// <summary>
        /// Gets the save directory path.
        /// </summary>
        /// <returns>The save directory path.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetSavePath()
        {
            return Instance.GetSavePath();
        }

        /// <summary>
        /// Gets the resource directory path.
        /// </summary>
        /// <returns>The resource directory path.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetResourcePath()
        {
            return Instance.GetResourcePath();
        }

        /// <summary>
        /// Gets the configuration directory path.
        /// </summary>
        /// <returns>The configuration directory path.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetConfigPath()
        {
            return Instance.GetConfigPath();
        }

        /// <summary>
        /// Gets the user data directory path.
        /// </summary>
        /// <returns>The user data directory path.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetUserDataPath()
        {
            return Instance.GetUserDataPath();
        }

        /// <summary>
        /// Checks if a file exists at the specified path.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns><c>true</c> if the file exists; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool FileExists(string path)
        {
            return Instance.FileExists(path);
        }

        /// <summary>
        /// Deletes the file at the specified path.
        /// </summary>
        /// <param name="path">The path of the file to delete.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DeleteFile(string path)
        {
            Instance.DeleteFile(path);
        }

        /// <summary>
        /// Moves a file from the source path to the destination path.
        /// </summary>
        /// <param name="sourcePath">The source path of the file to move.</param>
        /// <param name="destinationPath">The destination path of the file to move.</param>
        /// <param name="overwrite"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MoveFile(string sourcePath, string destinationPath, bool overwrite)
        {
            Instance.MoveFile(sourcePath, destinationPath, overwrite);
        }

        /// <summary>
        /// Copies a file from the source path to the destination path.
        /// </summary>
        /// <param name="sourcePath">The source path of the file to copy.</param>
        /// <param name="destinationPath">The destination path of the file to copy.</param>
        /// <param name="overwrite">Whether to overwrite an existing file at the destination path.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyFile(string sourcePath, string destinationPath, bool overwrite)
        {
            Instance.CopyFile(sourcePath, destinationPath, overwrite);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="destPath"></param>
        /// <param name="destBackupPath"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReplaceFile(string sourcePath, string destPath, string destBackupPath)
        {
            Instance.ReplaceFile(sourcePath, destPath, destBackupPath);
        }

        /// <summary>
        /// Gets the last write time of the file at the specified path.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        /// <returns>The last write time of the file.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTime GetLastWriteTime(string path)
        {
            return Instance.GetLastWriteTime(path);
        }

        /// <summary>
        /// Gets the size of the file at the specified path.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        /// <returns>The size of the file in bytes.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long GetFileSize(string path)
        {
            return Instance.GetFileSize(path);
        }

        /// <summary>
        /// Checks if a directory exists at the specified path.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns><c>true</c> if the directory exists; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool DirectoryExists(string path)
        {
            return Instance.DirectoryExists(path);
        }

        /// <summary>
        /// Creates a directory at the specified path.
        /// </summary>
        /// <param name="path">The path of the directory tso create.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CreateDirectory(string path)
        {
            Instance.CreateDirectory(path);
        }

        /// <summary>
        /// Deletes the directory at the specified path.
        /// </summary>
        /// <param name="path">The path of the directory to delete.</param>
        /// <param name="recursive">Whether to recursively delete subdirectories and files.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DeleteDirectory(string path, bool recursive)
        {
            Instance.DeleteDirectory(path, recursive);
        }

        /// <summary>
        /// Gets a list of directories at the specified path matching the search pattern.
        /// </summary>
        /// <param name="path">The path of the directory to search.</param>
        /// <returns>The list of matching directory paths.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IReadOnlyList<string> GetDirectories(string path)
        {
            return Instance.GetDirectories(path);
        }

        /// <summary>
        /// Gets a list of files at the specified path matching the search pattern.
        /// </summary>
        /// <param name="path">The path of the directory to search.</param>
        /// <param name="searchPattern">The search pattern to match against file names.</param>
        /// <param name="recursive">Whether to recursively search subdirectories.</param>
        /// <returns>The list of matching file paths.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IReadOnlyList<string> GetFiles(string path, string searchPattern, bool recursive)
        {
            return Instance.GetFiles(path, searchPattern, recursive);
        }

        /// <summary>
        /// Opens a read stream for the specified file path.
        /// </summary>
        /// <param name="config">How to create the stream and rules around how it should be handled.</param>
        /// <returns>The opened read stream.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IReadStream? OpenRead(IReadConfig config)
        {
            return Instance.OpenRead(config);
        }

        /// <summary>
        /// Opens a write stream for the specified file path.
        /// </summary>
        /// <param name="config">How to create the stream and rules around how it should be handled.</param>
        /// <returns>The opened write stream.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IWriteStream? OpenWrite(IWriteConfig config)
        {
            return Instance.OpenWrite(config);
        }

        /// <summary>
        /// Opens a read stream for the specified file path.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The opened read stream.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async ValueTask<IReadStream?> OpenReadAsync(IReadConfig config, CancellationToken ct = default)
        {
            return await Instance.OpenReadAsync(config, ct).ConfigureAwait(false);
        }

        /// <summary>
        /// Opens a write stream for the specified file path.
        /// </summary>
        /// <param name="config">How to create the stream and rules around how it should be handled.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The opened write stream.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async ValueTask<IWriteStream?> OpenWriteAsync(IWriteConfig config, CancellationToken ct = default)
        {
            return await Instance.OpenWriteAsync(config, ct).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates a memory stream.
        /// </summary>
        /// <param name="accessMode">The access mode to create the memory stream with.</param>
        /// <param name="type"></param>
        /// <param name="outputFile">Optional output file path for the memory stream.</param>
        /// <param name="length">The initial length of the memory stream.</param>
        /// <returns>The created data stream.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IDataStream CreateStream(System.IO.FileAccess accessMode, StreamType type, string outputFile = "", int length = 0)
        {
            return Instance.CreateStream(accessMode, type, outputFile, length);
        }

        /// <summary>
        /// Loads a file into memory.
        /// </summary>
        /// <param name="path">The file to open and load.</param>
        /// <returns>The data retrieved from the file.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IBufferHandle? LoadFile(string path)
        {
            return Instance.LoadFile(path);
        }

        /// <summary>
        /// Loads a file into memory asynchronously.
        /// </summary>
        /// <param name="path">The file to open and load.</param>
        /// <returns>The data retrieved from the file.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async ValueTask<IBufferHandle?> LoadFileAsync(string path)
        {
            return await Instance.LoadFileAsync(path).ConfigureAwait(false);
        }

        /// <summary>
        /// Writes a buffer to disk.
        /// </summary>
        /// <param name="path">The file to write to.</param>
        /// <param name="buffer">The data to write to disk.</param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteFile(string path, byte[] buffer, int offset, int length)
        {
            Instance.WriteFile(path, buffer, offset, length);
        }

        /// <summary>
        /// Writes a buffer asynchronously to disk.
        /// </summary>
        /// <param name="path">The file to write to.</param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="ct"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async ValueTask WriteFileAsync(string path, ReadOnlyMemory<byte> buffer, int offset, int length, CancellationToken ct = default)
        {
            await Instance.WriteFileAsync(path, buffer, offset, length, ct).ConfigureAwait(false);
        }
    }
}
