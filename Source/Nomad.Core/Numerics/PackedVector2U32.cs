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
	/// A vector2 structure that stores two signed normalized floats in a single 32-bit unsigned integer.
	/// Each component is quantized to 16 bits using signed normalization, allowing compact storage of higher-precision normalized vector data.
	/// </summary>
	/// <remarks>
	/// This structure provides better precision than <see cref="PackedVector2U16"/> while still maintaining a compact 32-bit representation.
	/// It is useful for storing normal vectors or other directional data that require higher precision than 8-bit quantization.
	/// </remarks>
    public struct PackedVector2U32
    {
        /// <summary>Gets the X component as a ushort.</summary>
        public readonly ushort X => (ushort)(Packed & 0xFFFF);
        /// <summary>Gets the Y component as a ushort.</summary>
        public readonly ushort Y => (ushort)((Packed >> 16) & 0xFFFF);

        /// <summary>Gets or sets the packed 32-bit representation of both components.</summary>
        public uint Packed { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PackedVector2U32"/> struct from normalized float components.
        /// </summary>
        /// <param name="x">The X component as a normalized float (typically between -1 and 1).</param>
        /// <param name="y">The Y component as a normalized float (typically between -1 and 1).</param>
        public PackedVector2U32(float x, float y)
        {
            ushort packedX = QuantizeSignedNormalized16(x);
            ushort packedY = QuantizeSignedNormalized16(y);

            Packed = (uint)(packedX | ((uint)packedY << 16));
        }

		/// <summary>
		/// Implicitly converts a <see cref="PackedVector2U32"/> to a <see cref="Vector2"/>.
		/// </summary>
		/// <param name="vector">The packed vector to convert.</param>
		/// <returns>The unpacked vector2 with dequantized float components.</returns>
        public static implicit operator Vector2(PackedVector2U32 vector)
        {
            return new Vector2(
                DequantizeSignedNormalized16(vector.X),
                DequantizeSignedNormalized16(vector.Y)
            );
        }

        private static ushort QuantizeSignedNormalized16(float value)
        {
            value = Math.Clamp(value, -1.0f, 1.0f);

            // [-1, 1] -> [0, 1] -> [0, 65536]
            float normalized = (value * 0.5f) + 0.5f;
            int quantized = (int)MathF.Round(normalized * ushort.MaxValue);

            return (ushort)Math.Clamp(quantized, 0, ushort.MaxValue);
        }

        private static float DequantizeSignedNormalized16(ushort value)
        {
            // [0, 65536] -> [0, 1] -> [-1, 1]
            float normalized = value / ushort.MaxValue;
            return (normalized * 2.0f) - 1.0f;
        }
    }
}