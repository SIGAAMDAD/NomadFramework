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
    /// Interpolation, easing, and curve helpers.
    /// </summary>
    public static class Interpolation
	{
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float InverseLerp(float a, float b, float value) {
            if (ScalarMath.NearlyEqual(a, b)) {
                return 0.0f;
            }
            return (value - a) / (b - a);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double InverseLerp(double a, double b, double value) {
            if (ScalarMath.NearlyEqual(a, b)) {
                return 0.0d;
            }
            return (value - a) / (b - a);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Remap(float inMin, float inMax, float outMin, float outMax, float value) {
            float t = InverseLerp(inMin, inMax, value);
            return LerpUnclamped(outMin, outMax, t);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Remap(double inMin, double inMax, double outMin, double outMax, double value) {
            double t = InverseLerp(inMin, inMax, value);
            return LerpUnclamped(outMin, outMax, t);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Lerp(float a, float b, float t) => a + (b - a) * ScalarMath.Clamp01(t);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Lerp(double a, double b, double t) => a + (b - a) * ScalarMath.Clamp01(t);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float LerpUnclamped(float a, float b, float t) => a + (b - a) * t;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double LerpUnclamped(double a, double b, double t) => a + (b - a) * t;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Damp(double current, double target, double smoothing, double deltaTime) {
            double t = 1.0d - Math.Exp(-smoothing * deltaTime);
            return LerpUnclamped(current, target, t);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CubicBezier(float p0, float p1, float p2, float p3, float t) {
            float u = 1.0f - t;
            float tt = t * t;
            float uu = u * u;
            return (uu * u * p0)
                 + (3.0f * uu * t * p1)
                 + (3.0f * u * tt * p2)
                 + (tt * t * p3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double CubicBezier(double p0, double p1, double p2, double p3, double t) {
            double u = 1.0d - t;
            double tt = t * t;
            double uu = u * u;
            return (uu * u * p0)
                 + (3.0d * uu * t * p1)
                 + (3.0d * u * tt * p2)
                 + (tt * t * p3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Hermite(float p0, float m0, float p1, float m1, float t) {
            float tt = t * t;
            float ttt = tt * t;

            return ((2.0f * ttt) - (3.0f * tt) + 1.0f) * p0
                 + (ttt - (2.0f * tt) + t) * m0
                 + ((-2.0f * ttt) + (3.0f * tt)) * p1
                 + (ttt - tt) * m1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Hermite(double p0, double m0, double p1, double m1, double t) {
            double tt = t * t;
            double ttt = tt * t;

            return ((2.0d * ttt) - (3.0d * tt) + 1.0d) * p0
                 + (ttt - (2.0d * tt) + t) * m0
                 + ((-2.0d * ttt) + (3.0d * tt)) * p1
                 + (ttt - tt) * m1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CatmullRom(float p0, float p1, float p2, float p3, float t) {
            float tt = t * t;
            float ttt = tt * t;
            return 0.5f * (
                (2.0f * p1) +
                (-p0 + p2) * t +
                (2.0f * p0 - 5.0f * p1 + 4.0f * p2 - p3) * tt +
                (-p0 + 3.0f * p1 - 3.0f * p2 + p3) * ttt);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double CatmullRom(double p0, double p1, double p2, double p3, double t) {
            double tt = t * t;
            double ttt = tt * t;
            return 0.5d * (
                (2.0d * p1) +
                (-p0 + p2) * t +
                (2.0d * p0 - 5.0d * p1 + 4.0d * p2 - p3) * tt +
                (-p0 + 3.0d * p1 - 3.0d * p2 + p3) * ttt);
        }
    }
}