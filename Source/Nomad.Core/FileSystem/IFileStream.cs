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
        /// Indicates whether the file stream can be read from.
        /// </summary>
        bool CanRead { get; }

        /// <summary>
        /// Indicates whether the file stream can be written to.
        /// </summary>
        bool CanWrite { get; }

        /// <summary>
        /// Indicates whether the file stream is currently open.
        /// </summary>
        bool IsOpen { get; }

        /// <summary>
        /// The timestamp of which this file was last accessed.
        /// </summary>
        DateTime LastAccessTime { get; }

        /// <summary>
        /// The time of the file's initial creation.
        /// </summary>
        DateTime CreationTime { get; }

        /// <summary>
        /// Closes the file stream. Is automatically called when object disposal happens.
        /// </summary>
        void Close();
    }
}
