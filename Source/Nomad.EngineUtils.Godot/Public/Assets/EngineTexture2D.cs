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

namespace Nomad.Core.Engine.Assets
{
    /// <summary>
    ///
    /// </summary>
    public sealed class EngineTexture2D : ITexture
    {
        /// <summary>
        ///
        /// </summary>
        public string Path => _texture.ResourcePath;

        /// <summary>
        ///
        /// </summary>
        public int Width => _texture.GetWidth();

        /// <summary>
        ///
        /// </summary>
        public int Height => _texture.GetHeight();

        /// <summary>
        ///
        /// </summary>
        public ReadOnlyMemory<byte> Image => _texture.GetImage().GetData();

        private readonly Texture2D _texture;

        private bool _isDisposed = false;

        /// <summary>
        ///
        /// </summary>
        /// <param name="texture"></param>
        public EngineTexture2D(Texture2D texture)
        {
            _texture = texture;
        }

        /// <summary>
        ///
        /// </summary>
        public void Dispose()
        {
            if (!_isDisposed)
            {
                _texture?.Dispose();
            }
            GC.SuppressFinalize(this);
            _isDisposed = true;
        }
    }
}
