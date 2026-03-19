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

using System.Collections.Concurrent;
using Nomad.Core.Engine.Globals;
using Nomad.Core.Engine.Services;
using Nomad.Core.Util;

namespace Nomad.EngineUtils.Private.Services {
    /*
    ===================================================================================

    UnityLocalizationService

    ===================================================================================
    */
    /// <summary>
    ///
    /// </summary>
    
    internal sealed class UnityLocalizationService : ILocalizationService {
        /// <summary>
        ///
        /// </summary>
        public string CurrentLanguage => System.Globalization.CultureInfo.CurrentCulture.Name;

        private readonly ConcurrentDictionary<InternString, string> _translations = new ConcurrentDictionary<InternString, string>();

        /// <summary>
        ///
        /// </summary>
        public UnityLocalizationService() {
            LocalizationService.Initialize( this );
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string Translate( InternString key ) {
            return _translations.GetOrAdd( key, value => (string)value );
        }
    };
};
