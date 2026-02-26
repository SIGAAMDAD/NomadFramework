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

namespace Nomad.Core.CVars
{
    /// <summary>
    /// A CVar's internal type.
    /// </summary>
    public enum CVarType : ushort
    {
        /// <summary>
        /// 
        /// </summary>
        UInt,

        /// <summary>
        /// 
        /// </summary>
        Int,

        /// <summary>
        /// 
        /// </summary>
        Decimal,

        /// <summary>
        /// 
        /// </summary>
        String,

        /// <summary>
        /// 
        /// </summary>
        Boolean,

        /// <summary>
        /// 
        /// </summary>
        Count
    }

    /// <summary>
    /// 
    /// </summary>
    public static class CVarTypeExtensions
    {
        /// <summary>
        /// Gets the system type from a <see cref="CVarType"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static Type GetSystemType(this CVarType type) => type switch
        {
            CVarType.Int => typeof(int),
            CVarType.UInt => typeof(uint),
            CVarType.Decimal => typeof(float),
            CVarType.Boolean => typeof(bool),
            CVarType.String => typeof(string),
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public static CVarType GetCVarType(this Type type)
        {
            return type switch
            {
                Type t when t == typeof(int) => CVarType.Int,
                Type t when t == typeof(uint) => CVarType.UInt,
                Type t when t == typeof(float) => CVarType.Decimal,
                Type t when t == typeof(string) => CVarType.String,
                Type t when t == typeof(bool) => CVarType.Boolean,
                Type t when t.IsEnum => CVarType.UInt,
                _ => CVarType.Count,
            };
        }
    }
}
