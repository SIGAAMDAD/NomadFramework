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

using System.Collections.Generic;
using Nomad.Core.Util;

namespace Nomad.Core.Engine.Services
{
    /// <summary>
    ///
    /// </summary>
    public interface IDisplayService
    {
        /// <summary>
        ///
        /// </summary>
        bool SupportsBloom { get; }

        /// <summary>
        ///
        /// </summary>
        bool SupportsTAA { get; }

        /// <summary>
        /// 
        /// </summary>
        bool SupportsFXAA { get; }

        /// <summary>
        /// 
        /// </summary>
        bool SupportsSMAA { get; }

        /// <summary>
        ///
        /// </summary>
        Dictionary<string, Any> CustomSettings { get; }
    }
}
