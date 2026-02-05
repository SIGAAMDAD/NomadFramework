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
    /// Provides a generic interface for writing to a stream.
    /// </summary>
    public interface IWriteStream : IDataStream
    {
        /// <summary>
        /// Writes a sequence of bytes to the current stream.
        /// </summary>
        /// <param name="buffer">The buffer containing data to write.</param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin copying bytes to the stream.</param>
        /// <param name="count">The number of bytes to write.</param>
        void Write(byte[] buffer, int offset, int count);

        /// <summary>
        /// Writes a sequence of bytes to the current stream.
        /// </summary>
        /// <param name="buffer">The region of memory containing data to write.</param>
        void Write(ReadOnlySpan<byte> buffer);

        /// <summary>
        /// Asynchronously writes a sequence of bytes to the current stream.
        /// </summary>
        /// <param name="buffer">The buffer containing data to write.</param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin copying bytes to the stream.</param>
        /// <param name="count">The number of bytes to write.</param>
        /// <param name="ct">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        ValueTask WriteAsync(byte[] buffer, int offset, int count, CancellationToken ct = default);

        /// <summary>
        /// Asynchronously writes a sequence of bytes to the current stream.
        /// </summary>
        /// <param name="buffer">The region of memory containing data to write.</param>
        /// <param name="ct">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken ct = default);

        /// <summary>
        /// Writes the entire content of another stream to this stream.
        /// </summary>
        /// <param name="stream">The stream to copy from.</param>
        void WriteFromStream(IReadStream stream);

        /// <summary>
        /// Asynchronously writes the entire content of another stream to this stream.
        /// </summary>
        /// <param name="stream">The stream to copy from.</param>
        /// <param name="ct">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous copy operation.</returns>
        ValueTask WriteFromStreamAsync(IReadStream stream, CancellationToken ct = default);

        /// <summary>
        /// Writes a signed byte to the current stream and advances the position by 1 byte.
        /// </summary>
        /// <param name="value">The signed byte value to write.</param>
        void WriteSByte(sbyte value);

        /// <summary>
        /// Writes a 16-bit signed integer to the current stream and advances the position by 2 bytes.
        /// </summary>
        /// <param name="value">The 16-bit signed integer value to write.</param>
        void WriteShort(short value);

        /// <summary>
        /// Writes a 32-bit signed integer to the current stream and advances the position by 4 bytes.
        /// </summary>
        /// <param name="value">The 32-bit signed integer value to write.</param>
        void WriteInt(int value);

        /// <summary>
        /// Writes a 64-bit signed integer to the current stream and advances the position by 8 bytes.
        /// </summary>
        /// <param name="value">The 64-bit signed integer value to write.</param>
        void WriteLong(long value);

        /// <summary>
        /// Writes an unsigned byte to the current stream and advances the position by 1 byte.
        /// </summary>
        /// <param name="value">The unsigned byte value to write.</param>
        void WriteByte(byte value);

        /// <summary>
        /// Writes a 16-bit unsigned integer to the current stream and advances the position by 2 bytes.
        /// </summary>
        /// <param name="value">The 16-bit unsigned integer value to write.</param>
        void WriteUShort(ushort value);

        /// <summary>
        /// Writes a 32-bit unsigned integer to the current stream and advances the position by 4 bytes.
        /// </summary>
        /// <param name="value">The 32-bit unsigned integer value to write.</param>
        void WriteUInt(uint value);

        /// <summary>
        /// Writes a 64-bit unsigned integer to the current stream and advances the position by 8 bytes.
        /// </summary>
        /// <param name="value">The 64-bit unsigned integer value to write.</param>
        void WriteULong(ulong value);

        /// <summary>
        /// Writes an 8-bit signed integer to the current stream and advances the position by 1 byte.
        /// </summary>
        /// <param name="value">The 8-bit signed integer value to write.</param>
        void WriteInt8(sbyte value);

        /// <summary>
        /// Writes a 16-bit signed integer to the current stream and advances the position by 2 bytes.
        /// </summary>
        /// <param name="value">The 16-bit signed integer value to write.</param>
        void WriteInt16(short value);

        /// <summary>
        /// Writes a 32-bit signed integer to the current stream and advances the position by 4 bytes.
        /// </summary>
        /// <param name="value">The 32-bit signed integer value to write.</param>
        void WriteInt32(int value);

        /// <summary>
        /// Writes a 64-bit signed integer to the current stream and advances the position by 8 bytes.
        /// </summary>
        /// <param name="value">The 64-bit signed integer value to write.</param>
        void WriteInt64(long value);

        /// <summary>
        /// Writes an 8-bit unsigned integer to the current stream and advances the position by 1 byte.
        /// </summary>
        /// <param name="value">The 8-bit unsigned integer value to write.</param>
        void WriteUInt8(byte value);

        /// <summary>
        /// Writes a 16-bit unsigned integer to the current stream and advances the position by 2 bytes.
        /// </summary>
        /// <param name="value">The 16-bit unsigned integer value to write.</param>
        void WriteUInt16(ushort value);

        /// <summary>
        /// Writes a 32-bit unsigned integer to the current stream and advances the position by 4 bytes.
        /// </summary>
        /// <param name="value">The 32-bit unsigned integer value to write.</param>
        void WriteUInt32(uint value);

        /// <summary>
        /// Writes a 64-bit unsigned integer to the current stream and advances the position by 8 bytes.
        /// </summary>
        /// <param name="value">The 64-bit unsigned integer value to write.</param>
        void WriteUInt64(ulong value);

        /// <summary>
        /// Writes a 32-bit floating point value to the current stream and advances the position by 4 bytes.
        /// </summary>
        /// <param name="value">The 32-bit floating point value to write.</param>
        void WriteFloat(float value);

        /// <summary>
        /// Writes a 32-bit floating point value to the current stream and advances the position by 4 bytes.
        /// </summary>
        /// <param name="value">The 32-bit floating point value to write.</param>
        void WriteSingle(float value);

        /// <summary>
        /// Writes a 64-bit floating point value to the current stream and advances the position by 8 bytes.
        /// </summary>
        /// <param name="value">The 64-bit floating point value to write.</param>
        void WriteDouble(double value);

        /// <summary>
        /// Writes a 32-bit floating point value to the current stream and advances the position by 4 bytes.
        /// </summary>
        /// <param name="value">The 32-bit floating point value to write.</param>
        void WriteFloat32(float value);

        /// <summary>
        /// Writes a 64-bit floating point value to the current stream and advances the position by 8 bytes.
        /// </summary>
        /// <param name="value">The 64-bit floating point value to write.</param>
        void WriteFloat64(double value);

        /// <summary>
        /// Writes an 8-bit boolean value to the current stream and advances the position by 1 byte.
        /// </summary>
        /// <param name="value">The 8-bit boolean value to write.</param>
        void WriteBoolean(bool value);

        /// <summary>
        /// Writes a string to the current stream. The string format (length-prefixed, null-terminated, etc.)
        /// depends on the stream's implementation.
        /// </summary>
        /// <param name="value">The string value to write.</param>
        void WriteString(string value);
    }
}
