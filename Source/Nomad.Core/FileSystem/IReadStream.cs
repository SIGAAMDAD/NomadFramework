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
using System.Threading;
using System.Threading.Tasks;

namespace Nomad.Core.FileSystem
{
    /// <summary>
    /// Provides a generic interface for reading from a stream.
    /// </summary>
    public interface IReadStream : IDataStream
    {
        /// <summary>
        /// Reads a sequence of bytes from the current stream.
        /// </summary>
        /// <param name="buffer">The buffer to write the data into.</param>
        /// <param name="offset">The byte offset in buffer at which to begin writing data from the stream.</param>
        /// <param name="count">The maximum number of bytes to read.</param>
        /// <returns>The total number of bytes read into the buffer.</returns>
        int Read(byte[] buffer, int offset, int count);

        /// <summary>
        /// Reads a sequence of bytes from the current stream.
        /// </summary>
        /// <param name="buffer">The region of memory to write the data into.</param>
        /// <returns>The total number of bytes read into the buffer.</returns>
        int Read(Span<byte> buffer);

        /// <summary>
        /// Asynchronously reads a sequence of bytes from the current stream.
        /// </summary>
        /// <param name="buffer">The buffer to write the data into.</param>
        /// <param name="offset">The byte offset in buffer at which to begin writing data from the stream.</param>
        /// <param name="count">The maximum number of bytes to read.</param>
        /// <param name="ct">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        ValueTask<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken ct = default);

        /// <summary>
        /// Asynchronously reads a sequence of bytes from the current stream.
        /// </summary>
        /// <param name="buffer">The region of memory to write the data into.</param>
        /// <param name="ct">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken ct = default);

        /// <summary>
        /// Reads the entire stream content as a byte array.
        /// </summary>
        /// <returns>A byte array containing the entire stream content.</returns>
        byte[] ToArray();

        /// <summary>
        /// Reads the remaining content from the current position to the end of the stream.
        /// </summary>
        /// <returns>A byte array containing the remaining content from the current position to the end of the stream.</returns>
        byte[] ReadToEnd();

        /// <summary>
        /// Asynchronously reads the remaining content from the current position to the end of the stream.
        /// </summary>
        /// <param name="ct">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous read operation, containing a byte array with the remaining content.</returns>
        ValueTask<byte[]> ReadToEndAsync(CancellationToken ct = default);

        /// <summary>
        /// Reads a signed byte from the current stream and advances the position by 1 byte.
        /// </summary>
        /// <returns>The next signed byte from the stream.</returns>
        sbyte ReadSByte();

        /// <summary>
        /// Reads a 16-bit signed integer from the current stream and advances the position by 2 bytes.
        /// </summary>
        /// <returns>A 16-bit signed integer read from the stream.</returns>
        short ReadShort();

        /// <summary>
        /// Reads a 32-bit signed integer from the current stream and advances the position by 4 bytes.
        /// </summary>
        /// <returns>A 32-bit signed integer read from the stream.</returns>
        int ReadInt();

        /// <summary>
        /// Reads a 64-bit signed integer from the current stream and advances the position by 8 bytes.
        /// </summary>
        /// <returns>A 64-bit signed integer read from the stream.</returns>
        long ReadLong();

        /// <summary>
        /// Reads an unsigned byte from the current stream and advances the position by 1 byte.
        /// </summary>
        /// <returns>The next unsigned byte from the stream.</returns>
        byte ReadByte();

        /// <summary>
        /// Reads a 16-bit unsigned integer from the current stream and advances the position by 2 bytes.
        /// </summary>
        /// <returns>A 16-bit unsigned integer read from the stream.</returns>
        ushort ReadUShort();

        /// <summary>
        /// Reads a 32-bit unsigned integer from the current stream and advances the position by 4 bytes.
        /// </summary>
        /// <returns>A 32-bit unsigned integer read from the stream.</returns>
        uint ReadUInt();

        /// <summary>
        /// Reads a 64-bit unsigned integer from the current stream and advances the position by 8 bytes.
        /// </summary>
        /// <returns>A 64-bit unsigned integer read from the stream.</returns>
        ulong ReadULong();

        /// <summary>
        /// Reads an 8-bit signed integer from the current stream and advances the position by 1 byte.
        /// </summary>
        /// <returns>An 8-bit signed integer read from the stream.</returns>
        sbyte ReadInt8();

        /// <summary>
        /// Reads a 16-bit signed integer from the current stream and advances the position by 2 bytes.
        /// </summary>
        /// <returns>A 16-bit signed integer read from the stream.</returns>
        short ReadInt16();

        /// <summary>
        /// Reads a 32-bit signed integer from the current stream and advances the position by 4 bytes.
        /// </summary>
        /// <returns>A 32-bit signed integer read from the stream.</returns>
        int ReadInt32();

        /// <summary>
        /// Reads a 64-bit signed integer from the current stream and advances the position by 8 bytes.
        /// </summary>
        /// <returns>A 64-bit signed integer read from the stream.</returns>
        long ReadInt64();

        /// <summary>
        /// Reads an 8-bit unsigned integer from the current stream and advances the position by 1 byte.
        /// </summary>
        /// <returns>An 8-bit unsigned integer read from the stream.</returns>
        byte ReadUInt8();

        /// <summary>
        /// Reads a 16-bit unsigned integer from the current stream and advances the position by 2 bytes.
        /// </summary>
        /// <returns>A 16-bit unsigned integer read from the stream.</returns>
        ushort ReadUInt16();

        /// <summary>
        /// Reads a 32-bit unsigned integer from the current stream and advances the position by 4 bytes.
        /// </summary>
        /// <returns>A 32-bit unsigned integer read from the stream.</returns>
        uint ReadUInt32();

        /// <summary>
        /// Reads a 64-bit unsigned integer from the current stream and advances the position by 8 bytes.
        /// </summary>
        /// <returns>A 64-bit unsigned integer read from the stream.</returns>
        ulong ReadUInt64();

        /// <summary>
        /// Reads a 32-bit floating point value from the current stream and advances the position by 4 bytes.
        /// </summary>
        /// <returns>A 32-bit floating point value read from the stream.</returns>
        float ReadFloat();

        /// <summary>
        /// Reads a 32-bit floating point value from the current stream and advances the position by 4 bytes.
        /// </summary>
        /// <returns>A 32-bit floating point value read from the stream.</returns>
        float ReadSingle();

        /// <summary>
        /// Reads a 64-bit floating point value from the current stream and advances the position by 8 bytes.
        /// </summary>
        /// <returns>A 64-bit floating point value read from the stream.</returns>
        double ReadDouble();

        /// <summary>
        /// Reads a 32-bit floating point value from the current stream and advances the position by 4 bytes.
        /// </summary>
        /// <returns>A 32-bit floating point value read from the stream.</returns>
        float ReadFloat32();

        /// <summary>
        /// Reads a 64-bit floating point value from the current stream and advances the position by 8 bytes.
        /// </summary>
        /// <returns>A 64-bit floating point value read from the stream.</returns>
        double ReadFloat64();

        /// <summary>
        /// Reads an 8-bit boolean value from the current stream and advances the position by 1 byte.
        /// </summary>
        /// <returns>A boolean value read from the stream.</returns>
        bool ReadBoolean();

        /// <summary>
        /// Reads a string from the current stream. The string is assumed to be length-prefixed
        /// or otherwise formatted according to the stream's implementation.
        /// </summary>
        /// <returns>A string read from the stream.</returns>
        string ReadString();
    }
}
