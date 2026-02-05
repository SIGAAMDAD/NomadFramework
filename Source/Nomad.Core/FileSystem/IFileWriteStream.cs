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

using System.Threading.Tasks;
using System;

namespace Nomad.Core.FileSystem
{
    /// <summary>
    /// Represents a write-only file stream.
    /// </summary>
    public interface IFileWriteStream : IFileStream, IWriteStream
    {
        /// <summary>
        /// Writes a line to the file.
        /// </summary>
        /// <param name="line">The line to write.</param>
        void WriteLine(string line);

        /// <summary>
        /// Writes a line to the file.
        /// </summary>
        /// <param name="line">The line to write.</param>
        void WriteLine(ReadOnlySpan<char> line);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        ValueTask WriteLineAsync(string line);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        ValueTask WriteLineAsync(ReadOnlySpan<char> line);
    }
}

