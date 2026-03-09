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

using System.Runtime.CompilerServices;
using Nomad.Core.Compatibility.Guards;
using Nomad.Core.Exceptions;
using Nomad.Core.Util;

namespace Nomad.Core.EngineUtils.Globals
{
    /// <summary>
    ///
    /// </summary>
    public static class LocalizationService
    {
        /// <summary>
        ///
        /// </summary>
        public static ILocalizationService Instance => _instance ?? throw new SubsystemNotInitializedException();
        private static ILocalizationService? _instance;

        /// <summary>
        ///
        /// </summary>
        public static string CurrentLanguage => Instance.CurrentLanguage;

        /// <summary>
        ///
        /// </summary>
        /// <param name="instance"></param>
        internal static void Initialize(ILocalizationService instance)
        {
            ArgumentGuard.ThrowIfNull(instance);
            _instance = instance;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Translate(InternString key)
        {
            return Instance.Translate(key);
        }
    }
}
