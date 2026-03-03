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
using System.Threading.Tasks;
using Nomad.Core.Memory.Buffers;

namespace Nomad.Core.OnlineServices
{
    /// <summary>
    /// Interface for cloud storage services.
    /// </summary>
    public interface ICloudStorageService : IDisposable
    {
        /// <summary>
        /// Indicates whether the service supports cloud storage.
        /// </summary>
        bool SupportsCloudStorage { get; }

        /// <summary>
        /// Writes a file to cloud storage.
        /// </summary>
        /// <param name="fileName">The name of the file to write.</param>
        /// <returns></returns>
        ValueTask WriteFile(string fileName);

        /// <summary>
        /// Reads a file from cloud storage.
        /// </summary>
        /// <param name="fileName">The name of the file to read.</param>
        /// <returns>The file data as a <see cref="IBufferHandle"/>.</returns>
        ValueTask<IBufferHandle?> ReadFile(string fileName);

        /// <summary>
        /// Checks if a file exists in cloud storage.
        /// </summary>
        /// <param name="fileName">The name of the file to check.</param>
        /// <returns>True if the file exists, false otherwise.</returns>
        ValueTask<bool> FileExists(string fileName);

        /// <summary>
        /// Synchronizes local files with cloud storage.
        /// </summary>
        /// <returns></returns>
        ValueTask Synchronize();

        /// <summary>
        /// Resolves a conflict between local and cloud data for a given file.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="localData"></param>
        /// <param name="cloudData"></param>
        /// <returns></returns>
        ValueTask ResolveConflict(string fileName, IBufferHandle localData, IBufferHandle cloudData);
    }
}
