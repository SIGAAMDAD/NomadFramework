using System;
using System.Runtime.CompilerServices;

namespace Nomad.Core.Compatibility.Guards
{
    /// <summary>
    ///
    /// </summary>
    public static class RangeGuard
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <param name="paramName"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Negative(int value, string? paramName = null)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(paramName, "Value cannot be negative.");
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <param name="paramName"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Zero(int value, string? paramName = null)
        {
            if (value == 0)
            {
                throw new ArgumentOutOfRangeException(paramName, "Value cannot be zero.");
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <param name="paramName"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NegativeOrZero(int value, string? paramName = null)
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(paramName, "Value must be greater than zero.");
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="paramName"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void OutOfRange(int value, int min, int max, string? paramName = null)
        {
            if (value < min || value > max)
            {
                throw new ArgumentOutOfRangeException(paramName, $"Value must be between {min} and {max}.");
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <param name="paramName"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotPowerOfTwo(int value, string? paramName = null)
        {
            if (value <= 0 || (value & (value - 1)) != 0)
            {
                throw new ArgumentOutOfRangeException(paramName, "Value must be a power of two.");
            }
        }
    }
}
