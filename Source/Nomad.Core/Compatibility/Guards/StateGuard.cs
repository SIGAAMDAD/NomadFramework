using System;
using System.Runtime.CompilerServices;

namespace Nomad.Core.Compatibility.Guards
{
    /// <summary>
    ///
    /// </summary>
    public static class StateGuard
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="isDisposed"></param>
        /// <param name="obj"></param>
        /// <exception cref="ObjectDisposedException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Disposed(bool isDisposed, object? obj)
        {
#if USE_COMPATIBILITY_EXTENSIONS
            if (isDisposed)
            {
                throw new ObjectDisposedException(obj?.GetType().FullName);
            }
#else
            ObjectDisposedException.ThrowIf(isDisposed, obj);
#endif
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="message"></param>
        /// <exception cref="InvalidOperationException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void True(bool condition, string? message = null)
        {
            if (condition)
            {
                throw new InvalidOperationException(message ?? "Condition must be false.");
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="message"></param>
        /// <exception cref="InvalidOperationException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void False(bool condition, string? message = null)
        {
            if (!condition)
            {
                throw new InvalidOperationException(message ?? "Condition must be true.");
            }
        }
    }
}
