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

namespace Nomad.Core.Engine.Assets
{
    /// <summary>
    ///
    /// </summary>
    public class EngineShaderMaterial : EngineMaterial
    {
        private readonly Shader _shader;

        /// <summary>
        ///
        /// </summary>
        /// <param name="material"></param>
        public EngineShaderMaterial(ShaderMaterial material)
            : base(material)
        {
            _shader = material.Shader;
        }
    }
}
