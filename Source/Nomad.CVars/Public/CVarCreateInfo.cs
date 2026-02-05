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

namespace Nomad.CVars
{
    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T">The CVar's value type.</typeparam>
    public readonly struct CVarCreateInfo<T>
    {
        /// <summary>
        /// 
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// 
        /// </summary>
        public readonly T DefaultValue;

        /// <summary>
        /// 
        /// </summary>
        public readonly string Description;

        /// <summary>
        /// 
        /// </summary>
        public readonly CVarFlags Flags;

        /// <summary>
        /// 
        /// </summary>
        public readonly Func<T, bool>? Validator;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">The name of the CVar.</param>
        /// <param name="defaultValue"></param>
        /// <param name="description"></param>
        /// <param name="flags"></param>
        /// <param name="validator"></param>
        public CVarCreateInfo(string name, T defaultValue, string description, CVarFlags flags = CVarFlags.None, Func<T, bool>? validator = null)
        {
            Name = name;
            Description = description;
            DefaultValue = defaultValue;
            Flags = flags;
            Validator = validator;
        }
    }
}
