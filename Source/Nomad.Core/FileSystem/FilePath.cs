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

using Nomad.Core.Compatibility;
using Nomad.Core.Util;
using System;
using System.Runtime.CompilerServices;

namespace Nomad.Core.FileSystem
{
    /// <summary>
    /// A wrapper class that holds a godot res:// or user:// path along with it's platform specific path.
    /// </summary>
    public record FilePath
    {
        /// <summary>
        /// The string path.
        /// </summary>
        private string _path { get; init; }

        /// <summary>
        /// The unique hash id of this FilePath.
        /// </summary>
        private readonly int _hashCode;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public FilePath(string filePath)
        {
            ExceptionCompat.ThrowIfNullOrEmpty(filePath);

            _path = filePath;
            _hashCode = (int)_path.HashFileName();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Exists() => System.IO.File.Exists(_path);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetExtension() => System.IO.Path.GetExtension(_path);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasExtension() => System.IO.Path.HasExtension(_path);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetDirectoryName() => System.IO.Path.GetDirectoryName(_path);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetFileName() => System.IO.Path.GetFileName(_path);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetFullPath() => System.IO.Path.GetFullPath(_path);

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => _hashCode;

        /// <summary>
        ///
        /// </summary>
        /// <param name="path"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator string(FilePath path) => path._path;
    }
}
