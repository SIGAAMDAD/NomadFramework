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

using Godot;
using Nomad.Core.Abstractions;
using System;
using System.Runtime.CompilerServices;

namespace Nomad.Core.Util
{
    /// <summary>
    /// A wrapper class that holds a godot res:// or user:// path along with it's platform specific path.
    /// </summary>
    public record FilePath : IValueObject<FilePath>
    {
        public string OSPath { get; init; }
        public string GodotPath { get; init; }
        public PathType Type { get; init; }

        private readonly int _hashCode;

        /// <summary>
        /// Creates a FilePath
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="type"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public FilePath(string filePath, PathType type)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath);

            switch (type)
            {
                case PathType.Native:
                    OSPath = filePath;
                    GodotPath = ProjectSettings.LocalizePath(OSPath);
                    _hashCode = GodotPath.GetHashCode();
                    break;
                case PathType.User:
                case PathType.Resource:
                    GodotPath = filePath;
                    OSPath = ProjectSettings.GlobalizePath(GodotPath);
                    _hashCode = OSPath.GetHashCode();
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Path type '{type}' isn't a valid PathType");
            }
            Type = type;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetFileName() => GodotPath[GodotPath.LastIndexOf('/')..];

        /// <summary>
        ///
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FilePath FromUserPath(string path) => new FilePath(path, PathType.User);

        /// <summary>
        ///
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FilePath FromResourcePath(string path) => new FilePath(path, PathType.Resource);

        /// <summary>
        ///
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FilePath FromOSPath(string path) => new FilePath(path, PathType.Native);

        /// <summary>
        ///
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FilePath FromNative(string path) => new FilePath(path, PathType.Native);

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
        public static implicit operator string(FilePath path) => path.Type switch
        {
            PathType.Native => path.OSPath,
            PathType.Resource or PathType.User => path.GodotPath,
            _ => throw new IndexOutOfRangeException(nameof(path.Type))
        };
    }
}
