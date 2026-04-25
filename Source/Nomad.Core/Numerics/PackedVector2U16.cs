/*
===========================================================================
The Nomad MPLv2 Source Code
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
using System.Numerics;

namespace Nomad.Core.Numerics
{
    /// <summary>
    /// A vector2 structure that stores two signed normalized floats in a single 16-bit unsigned integer.
    /// Each component is quantized to 8 bits using signed normalization, allowing compact storage of normalized vector data.
    /// </summary>
    /// <remarks>
    /// This structure is useful for reducing memory footprint when storing large quantities of normalized vectors,
    /// such as normal vectors or direction vectors that don't require full floating-point precision.
    /// </remarks>
    public struct PackedVector2U16
    {
        /// <summary>Gets the X component as a byte.</summary>
        public readonly byte X => (byte)(Packed & 0xFF);
        /// <summary>Gets the Y component as a byte.</summary>
        public readonly byte Y => (byte)((Packed >> 8) & 0xFF);

        /// <summary>Gets or sets the packed 16-bit representation of both components.</summary>
        public ushort Packed { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PackedVector2U16"/> struct from normalized float components.
        /// </summary>
        /// <param name="x">The X component as a normalized float (typically between -1 and 1).</param>
        /// <param name="y">The Y component as a normalized float (typically between -1 and 1).</param>
        public PackedVector2U16(float x, float y)
        {
            byte packedX = QuantizeSignedNormalized8(x);
            byte packedY = QuantizeSignedNormalized8(y);

            Packed = (ushort)(packedX | (packedY << 8));
        }

        public static implicit operator Vector2(PackedVector2U16 vector)
        {
            return new Vector2(
                DequantizeSignedNormalized8(vector.X),
                DequantizeSignedNormalized8(vector.Y)
            );
        }

        private static byte QuantizeSignedNormalized8(float value)
        {
            value = Math.Clamp(value, -1.0f, 1.0f);

            // [-1, 1] -> [0, 1] -> [0, 255]
            float normalized = (value * 0.5f) + 0.5f;
            int quantized = (int)MathF.Round(normalized * byte.MaxValue);

            return (byte)Math.Clamp(quantized, 0, byte.MaxValue);
        }

        private static float DequantizeSignedNormalized8(byte value)
        {
            // [0, 255] -> [0, 1] -> [-1, 1]
            float normalized = value / byte.MaxValue;
            return (normalized * 2.0f) - 1.0f;
        }
    }
}