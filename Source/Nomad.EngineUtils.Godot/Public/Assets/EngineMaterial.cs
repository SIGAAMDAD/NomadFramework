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
using Godot;
using Nomad.Core.EngineUtils.Assets;

namespace Nomad.EngineUtils.Assets
{
    /// <summary>
    ///
    /// </summary>
    public class EngineMaterial : IMaterial
    {
        /// <summary>
        ///
        /// </summary>
        public string Path => material.ResourcePath;

        protected readonly Material material;

        private bool _isDisposed;

        /// <summary>
        ///
        /// </summary>
        /// <param name="material"></param>
        public EngineMaterial(Material material)
        {
            this.material = material;
        }

        /// <summary>
        ///
        /// </summary>
        public void Dispose()
        {
            if (!_isDisposed)
            {
                material?.Dispose();
            }
            GC.SuppressFinalize(this);
            _isDisposed = true;
        }
    }
}
