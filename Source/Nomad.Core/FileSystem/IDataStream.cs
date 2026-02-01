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
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Nomad.Core.FileSystem
{
    /// <summary>
    /// Represents a stream for reading from and writing to a data source.
    /// This is the base interface for all chunk-oriented I/O operations.
    /// </summary>
    public interface IDataStream : IDisposable, IAsyncDisposable
    {
        /// <summary>
        /// Gets the length of the stream in bytes.
        /// </summary>
        int Length { get; }

        /// <summary>
        /// Gets or sets the current position within the stream.
        /// </summary>
        int Position { get; set; }

        /// <summary>
        /// Gets a value indicating whether the stream supports reading.
        /// </summary>
        bool CanRead { get; }

        /// <summary>
        /// Gets a value indicating whether the stream supports writing.
        /// </summary>
        bool CanWrite { get; }

        /// <summary>
        /// Gets a value indicating whether the stream supports seeking.
        /// </summary>
        bool CanSeek { get; }

        /// <summary>
        /// Clears all buffers for this stream and causes any buffered data to be written.
        /// </summary>
        void Flush();

        /// <summary>
        /// Asynchronously clears all buffers for this stream and causes any buffered data to be written.
        /// </summary>
        ValueTask FlushAsync(CancellationToken ct = default);

        /// <summary>
        /// Sets the position within the current stream.
        /// </summary>
        /// <param name="offset">A byte offset relative to the origin parameter.</param>
        /// <param name="origin">A value of type <see cref="SeekOrigin"/> indicating the reference point used to obtain the new position.</param>
        /// <returns>The new position within the current stream.</returns>
        int Seek(int offset, SeekOrigin origin);
    }
}

