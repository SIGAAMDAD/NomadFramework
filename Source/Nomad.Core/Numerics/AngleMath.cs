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
    /// Angle helpers with wrap-safe deltas and interpolation.
    /// </summary>
    public static class AngleMath
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToRadians(float degrees) => degrees * ScalarMath.Deg2RadF;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ToRadians(double degrees) => degrees * ScalarMath.Deg2RadD;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToDegrees(float radians) => radians * ScalarMath.Rad2DegF;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ToDegrees(double radians) => radians * ScalarMath.Rad2DegD;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float NormalizeDegrees(float degrees) => ScalarMath.Wrap(degrees, 0.0f, 360.0f);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double NormalizeDegrees(double degrees) => ScalarMath.Wrap(degrees, 0.0d, 360.0d);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float NormalizeDegreesSigned(float degrees) => ScalarMath.Wrap(degrees + 180.0f, 0.0f, 360.0f) - 180.0f;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double NormalizeDegreesSigned(double degrees) => ScalarMath.Wrap(degrees + 180.0d, 0.0d, 360.0d) - 180.0d;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float NormalizeRadians(float radians) => ScalarMath.Wrap(radians, 0.0f, ScalarMath.TauF);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double NormalizeRadians(double radians) => ScalarMath.Wrap(radians, 0.0d, ScalarMath.TauD);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float NormalizeRadiansSigned(float radians) => ScalarMath.Wrap(radians + MathF.PI, 0.0f, ScalarMath.TauF) - MathF.PI;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double NormalizeRadiansSigned(double radians) => ScalarMath.Wrap(radians + Math.PI, 0.0d, ScalarMath.TauD) - Math.PI;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DeltaDegrees(float fromDegrees, float toDegrees) => NormalizeDegreesSigned(toDegrees - fromDegrees);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double DeltaDegrees(double fromDegrees, double toDegrees) => NormalizeDegreesSigned(toDegrees - fromDegrees);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DeltaRadians(float fromRadians, float toRadians) => NormalizeRadiansSigned(toRadians - fromRadians);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double DeltaRadians(double fromRadians, double toRadians) => NormalizeRadiansSigned(toRadians - fromRadians);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float LerpDegrees(float fromDegrees, float toDegrees, float t) => fromDegrees + DeltaDegrees(fromDegrees, toDegrees) * ScalarMath.Clamp01(t);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double LerpDegrees(double fromDegrees, double toDegrees, double t) => fromDegrees + DeltaDegrees(fromDegrees, toDegrees) * ScalarMath.Clamp01(t);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float LerpRadians(float fromRadians, float toRadians, float t) => fromRadians + DeltaRadians(fromRadians, toRadians) * ScalarMath.Clamp01(t);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double LerpRadians(double fromRadians, double toRadians, double t) => fromRadians + DeltaRadians(fromRadians, toRadians) * ScalarMath.Clamp01(t);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MoveTowardsDegrees(float current, float target, float maxDelta)
        {
            float delta = DeltaDegrees(current, target);
            if (MathF.Abs(delta) <= maxDelta)
            {
                return target;
            }
            return current + ScalarMath.SignNonZero(delta) * maxDelta;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double MoveTowardsDegrees(double current, double target, double maxDelta)
        {
            double delta = DeltaDegrees(current, target);
            if (Math.Abs(delta) <= maxDelta)
            {
                return target;
            }
            return current + ScalarMath.SignNonZero(delta) * maxDelta;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MoveTowardsRadians(float current, float target, float maxDelta)
        {
            float delta = DeltaRadians(current, target);
            if (MathF.Abs(delta) <= maxDelta)
            {
                return target;
            }
            return current + ScalarMath.SignNonZero(delta) * maxDelta;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double MoveTowardsRadians(double current, double target, double maxDelta)
        {
            double delta = DeltaRadians(current, target);
            if (Math.Abs(delta) <= maxDelta)
            {
                return target;
            }
            return current + ScalarMath.SignNonZero(delta) * maxDelta;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float LookAtDegrees(float fromX, float fromY, float toX, float toY) => ToDegrees(MathF.Atan2(toY - fromY, toX - fromX));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double LookAtDegrees(double fromX, double fromY, double toX, double toY) => ToDegrees(Math.Atan2(toY - fromY, toX - fromX));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float LookAtRadians(float fromX, float fromY, float toX, float toY) => MathF.Atan2(toY - fromY, toX - fromX);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double LookAtRadians(double fromX, double fromY, double toX, double toY) => Math.Atan2(toY - fromY, toX - fromX);

        public static int GetQuadrantRadians(float angle)
        {
            angle = NormalizeRadians(angle);
            float halfPi = MathF.PI * 0.5f;
            float pi = MathF.PI;
            float threeHalfPi = MathF.PI * 1.5f;
            float twoPi = MathF.PI * 2.0f;

            if (angle > 0.0f && angle < halfPi)
            {
                return 1;
            }
            if (angle > halfPi && angle < pi)
            {
                return 2;
            }
            if (angle > pi && angle < threeHalfPi)
            {
                return 3;
            }
            if (angle > threeHalfPi && angle < twoPi)
            {
                return 4;
            }
            return 0; // on an axis
        }

        public static int GetQuadrantFromVector(float x, float y)
        {
            if (x > 0.0f && y > 0.0f)
            {
                return 1;
            }
            if (x < 0.0f && y > 0.0f)
            {
                return 2;
            }
            if (x < 0.0f && y < 0.0f)
            {
                return 3;
            }
            if (x > 0.0f && y < 0.0f)
            {
                return 4;
            }
            return 0; // on an axis
        }
    }
}
