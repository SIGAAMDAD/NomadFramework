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
using System.Runtime.CompilerServices;

namespace Nomad.Core.Numerics
{
    /// <summary>
    /// Miscellaneous scalar helpers that complement <see cref="Math"/> and <see cref="MathF"/>.
    /// </summary>
    public static class ScalarMath
    {
        public const float EpsilonF = 1e-6f;
        public const double EpsilonD = 1e-12d;

        public const float TauF = MathF.PI * 2.0f;
        public const double TauD = Math.PI * 2.0d;

        public const float Deg2RadF = MathF.PI / 180.0f;
        public const float Rad2DegF = 180.0f / MathF.PI;
        public const double Deg2RadD = Math.PI / 180.0d;
        public const double Rad2DegD = 180.0d / Math.PI;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp01(float value) => Math.Clamp(value, 0.0f, 1.0f);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Clamp01(double value) => Math.Clamp(value, 0.0d, 1.0d);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Saturate(float value) => Clamp01(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Saturate(double value) => Clamp01(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsZero(float value, float epsilon = EpsilonF) => MathF.Abs(value) <= epsilon;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsZero(double value, double epsilon = EpsilonD) => Math.Abs(value) <= epsilon;

        public static bool NearlyEqual(float a, float b, float epsilon = EpsilonF)
        {
            float diff = MathF.Abs(a - b);
            if (diff <= epsilon)
            {
                return true;
            }
            return diff <= MathF.Max(MathF.Abs(a), MathF.Abs(b)) * epsilon;
        }

        public static bool NearlyEqual(double a, double b, double epsilon = EpsilonD)
        {
            double diff = Math.Abs(a - b);
            if (diff <= epsilon)
            {
                return true;
            }
            return diff <= Math.Max(Math.Abs(a), Math.Abs(b)) * epsilon;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Frac(float value) => value - MathF.Floor(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Frac(double value) => value - Math.Floor(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SignNonZero(int value) => value < 0 ? -1 : 1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SignNonZero(float value) => value < 0.0f ? -1.0f : 1.0f;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SignNonZero(double value) => value < 0.0d ? -1.0d : 1.0d;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Wrap(int value, int minInclusive, int maxExclusive)
        {
            int range = maxExclusive - minInclusive;
            if (range <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxExclusive));
            }

            int wrapped = (value - minInclusive) % range;
            if (wrapped < 0)
            {
                wrapped += range;
            }

            return wrapped + minInclusive;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Wrap(float value, float minInclusive, float maxExclusive)
        {
            float range = maxExclusive - minInclusive;
            if (range <= 0.0f)
            {
                throw new ArgumentOutOfRangeException(nameof(maxExclusive));
            }
            return value - range * MathF.Floor((value - minInclusive) / range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Wrap(double value, double minInclusive, double maxExclusive)
        {
            double range = maxExclusive - minInclusive;
            if (range <= 0.0d)
            {
                throw new ArgumentOutOfRangeException(nameof(maxExclusive));
            }
            return value - range * Math.Floor((value - minInclusive) / range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Repeat(float value, float length)
        {
            if (length <= 0.0f)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }
            return value - length * MathF.Floor(value / length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Repeat(double value, double length)
        {
            if (length <= 0.0d)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }
            return value - length * Math.Floor(value / length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float PingPong(float value, float length)
        {
            float repeated = Repeat(value, length * 2.0f);
            return length - MathF.Abs(repeated - length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double PingPong(double value, double length)
        {
            double repeated = Repeat(value, length * 2.0d);
            return length - Math.Abs(repeated - length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Step(float edge, float value) => value < edge ? 0.0f : 1.0f;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Step(double edge, double value) => value < edge ? 0.0d : 1.0d;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Snap(float value, float increment)
        {
            if (increment <= 0.0f)
            {
                return value;
            }
            return MathF.Round(value / increment) * increment;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Snap(double value, double increment)
        {
            if (increment <= 0.0d)
            {
                return value;
            }

            return Math.Round(value / increment) * increment;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CeilDiv(int numerator, int denominator)
        {
            if (denominator == 0)
            {
                throw new DivideByZeroException();
            }
            return (numerator + denominator - 1) / denominator;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint CeilDiv(uint numerator, uint denominator)
        {
            if (denominator == 0u)
            {
                throw new DivideByZeroException();
            }
            return (numerator + denominator - 1u) / denominator;
        }
    }
}
