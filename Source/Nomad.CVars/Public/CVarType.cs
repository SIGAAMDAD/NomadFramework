/*
===========================================================================
The Nomad Framework
Copyright (C) 2025 Noah Van Til

This Source Code Form is subject to the terms of the Mozilla Public
License, v2. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.

This software is provided "as is", without warranty of any kind,
express or implied, including but not limited to the warranties
of merchantability, fitness for a particular purpose and noninfringement.
===========================================================================
*/

using System;

namespace Nomad.CVars
{
    /// <summary>
    /// A CVar's internal type.
    /// </summary>
    public enum CVarType : ushort
    {
        UInt,       // 32-bit unsigned integer
        Int,        // 32-bit signed integer
        Decimal,    // 32-bit float
        String,
        Boolean,

        Count
    }

    public static class CVarTypeExtensions
    {
        /// <summary>
        /// Gets the system type from a <see cref="CVarType"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static Type GetType(this CVarType type) => type switch
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
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static CVarType GetCVarType(this Type type)
        {
            if (type == typeof(int))
            {
                return CVarType.Int;
            }
            else if (type == typeof(uint))
            {
                return CVarType.UInt;
            }
            else if (type == typeof(float))
            {
                return CVarType.Decimal;
            }
            else if (type == typeof(string))
            {
                return CVarType.String;
            }
            else if (type == typeof(bool))
            {
                return CVarType.Boolean;
            }
            else if (type.IsEnum)
            {
                return CVarType.UInt;
            }
            return CVarType.Count;
        }
    }
}
