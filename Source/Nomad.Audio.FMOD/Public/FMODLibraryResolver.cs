using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Nomad.Audio.Fmod
{
    internal static class FMODLibraryResolver
    {
        /// <summary>
        ///
        /// </summary>
        public static void Hook()
        {
            NativeLibrary.SetDllImportResolver(typeof(FMOD.Channel).Assembly, Resolve);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="libraryName"></param>
        /// <param name="assembly"></param>
        /// <param name="searchPath"></param>
        /// <returns></returns>
        private static IntPtr Resolve(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
        {
            string fileName =
                OperatingSystem.IsWindows() ? $"{libraryName}.dll" :
                OperatingSystem.IsLinux() ? $"{libraryName}.so" :
                OperatingSystem.IsMacOS() ? $"{libraryName}.dylib" :
                throw new PlatformNotSupportedException();

            string fullPath = Path.Combine(AppContext.BaseDirectory, fileName);
            if (File.Exists(fullPath))
            {
                return NativeLibrary.Load(fullPath);
            }

            fullPath = Path.Combine(Environment.CurrentDirectory, fileName);
            return File.Exists(fullPath) ? NativeLibrary.Load(fullPath) : IntPtr.Zero;
        }
    }
}
