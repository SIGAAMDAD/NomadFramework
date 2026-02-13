using System;
using System.Runtime.CompilerServices;

namespace Nomad.Core.Compatibility.Guards
{
    /// <summary>
    ///
    /// </summary>
    public static class MemoryGuard
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="ptr"></param>
        /// <param name="alignment"></param>
        /// <param name="paramName"></param>
        /// <exception cref="ArgumentException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfUnaligned(IntPtr ptr, int alignment, string? paramName = null)
        {
            if (((long)ptr & (alignment - 1)) != 0)
            {
                throw new ArgumentException($"Pointer is not aligned to {alignment} bytes.", paramName);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="ptr"></param>
        /// <param name="paramName"></param>
        /// <exception cref="ArgumentNullException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfNullPtr(IntPtr ptr, string? paramName = null)
        {
            if (ptr == IntPtr.Zero)
            {
                throw new ArgumentNullException(paramName, "Pointer cannot be null.");
            }
        }
    }
}
