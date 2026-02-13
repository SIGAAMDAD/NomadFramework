using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Nomad.Core.Compatibility.Guards
{
    /// <summary>
    ///
    /// </summary>
    public static class ArgumentGuard
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <param name="paramName"></param>
        /// <exception cref="ArgumentNullException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Null([NotNull] object? value, string? paramName = null)
        {
#if USE_COMPATIBILITY_EXTENSIONS
            if (value is null)
            {
                throw new ArgumentNullException(paramName);
            }
#else
            ArgumentNullException.ThrowIfNull(value, paramName);
#endif
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <param name="paramName"></param>
        /// <exception cref="ArgumentException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NullOrEmpty(string? value, string? paramName = null)
        {
#if USE_COMPATIBILITY_EXTENSIONS
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Value cannot be null or empty.", paramName);
            }
#else
            ArgumentException.ThrowIfNullOrEmpty(value, paramName);
#endif
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <param name="paramName"></param>
        /// <exception cref="ArgumentException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NullOrWhiteSpace(string? value, string? paramName = null)
        {
#if USE_COMPATIBILITY_EXTENSIONS
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Value cannot be null, empty, or whitespace.", paramName);
            }
#else
            ArgumentException.ThrowIfNullOrWhiteSpace(value, paramName);
#endif
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="paramName"></param>
        /// <exception cref="ArgumentException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Default<T>(T value, string? paramName = null) where T : struct
        {
            if (value.Equals(default(T)))
            {
                throw new ArgumentException("Value cannot be the default value.", paramName);
            }
        }
    }
}
