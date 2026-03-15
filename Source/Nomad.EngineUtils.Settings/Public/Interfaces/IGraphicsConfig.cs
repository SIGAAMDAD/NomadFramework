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
using Nomad.Core.Engine.Rendering;
using Nomad.Core.Util;

namespace Nomad.EngineUtils.Settings.Interfaces
{
    /// <summary>
    ///
    /// </summary>
    public interface IGraphicsConfig
    {
        /// <summary>
        ///
        /// </summary>
        QualitySetting LightingQuality { get; set; }

        /// <summary>
        ///
        /// </summary>
        TextureFilterMode TextureFiltering { get; set; }

        /// <summary>
        ///
        /// </summary>
        QualitySetting TextureQuality { get; set; }

        /// <summary>
        ///
        /// </summary>
        QualitySetting ShadowQuality { get; set; }

        /// <summary>
        ///
        /// </summary>
        QualitySetting EnvironmentQuality { get; set; }

        /// <summary>
        ///
        /// </summary>
        Dictionary<string, Any> CustomSettings { get; }
    }
}
