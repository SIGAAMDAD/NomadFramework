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

namespace Nomad.Core.FileSystem
{
    /// <summary>
    /// Represents a file stream.
    /// </summary>
    public interface IFileStream : IDataStream
    {
        /// <summary>
        /// The file path associated with this stream.
        /// </summary>
        string FilePath { get; }

        /// <summary>
        /// Indicates whether the file stream is currently open.
        /// </summary>
        bool IsOpen { get; }

        /// <summary>
        /// Closes the file stream.
        /// </summary>
        void Close();

        /// <summary>
        /// Opens the file stream.
        /// </summary>
        /// <param name="filepath">The path of the file to open.</param>
        /// <param name="openMode">The mode to open the file with.</param>
        /// <param name="accessMode">The access mode to open the file with.</param>
        bool Open(string filepath, System.IO.FileMode openMode, System.IO.FileAccess accessMode);
    }
}
