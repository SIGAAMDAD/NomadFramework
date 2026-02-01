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

namespace Nomad.Core.FileSystem
{
    /// <summary>
    /// Represents a file system.
    /// </summary>
    public interface IFileSystem : IDisposable
    {
        /// <summary>
        /// The root paths of the file system.
        /// </summary>
        IReadOnlyList<string> Paths { get; }

        /// <summary>
        /// Gets the save directory path.
        /// </summary>
        /// <returns></returns>
        string GetSaveDirectory();

        /// <summary>
        /// Gets the resource directory path.
        /// </summary>
        /// <returns></returns>
        string GetResourcePath();

        /// <summary>
        /// Gets the configuration directory path.
        /// </summary>
        /// <returns></returns>
        string GetConfigPath();

        /// <summary>
        /// Opens a file stream.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="openMode"></param>
        /// <param name="accessMode"></param>
        /// <returns></returns>
        IDataStream OpenFile(string path, System.IO.FileMode openMode, System.IO.FileAccess accessMode);

        /// <summary>
        /// Creates a memory stream.
        /// </summary>
        /// <param name="accessMode"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        IDataStream CreateMemoryStream(System.IO.FileAccess accessMode, int length = 0);
    }
}

