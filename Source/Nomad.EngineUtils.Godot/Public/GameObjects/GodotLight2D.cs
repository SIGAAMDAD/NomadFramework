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
using Nomad.Core.Compatibility.Guards;
using Nomad.Core.CVars;
using Nomad.Core.EngineUtils.GameObjects;
using Nomad.CVars;

namespace Nomad.EngineUtils.GameObjects
{
    /// <summary>
    ///
    /// </summary>
    internal sealed class GodotLigh2D : ILight
    {
        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    RenderingServer.CanvasLightSetEnabled(_rid, value);
                }
            }
        }
        private bool _enabled = false;

        public System.Numerics.Vector3 Color
        {
            get => _color;
            set
            {
                if (_color != value)
                {
                    _color = value;
                    RenderingServer.CanvasLightSetColor(_rid, new Color(value.X, value.Y, value.Z));
                }
            }
        }
        private System.Numerics.Vector3 _color;

        public float Intensity
        {
            get => _intensity;
            set
            {
                if (_intensity != value)
                {
                    _intensity = value;
                    RenderingServer.CanvasLightSetEnergy(_rid, value);
                }
            }
        }
        private float _intensity;

        public float Range
        {
            get => _range;
            set
            {
                if (_range != value)
                {
                    _range = value;
                    RenderingServer.CanvasLightSetTextureScale(_rid, value);
                }
            }
        }
        private float _range;

        public bool CastShadows
        {
            get => _castShadows;
            set
            {
                if (_castShadows != value)
                {
                    _castShadows = value;
                    RenderingServer.CanvasLightSetShadowEnabled(_rid, value);
                }
            }
        }
        private bool _castShadows = false;

        private readonly Rid _rid;
        private Transform2D _transform;

        private bool _isDisposed = false;

        /// <summary>
        ///
        /// </summary>
        /// <param name="light"></param>
        /// <param name="cvarSystem"></param>
        public GodotLigh2D(PointLight2D light, ICVarSystemService cvarSystem)
        {
            ArgumentGuard.ThrowIfNull(light, nameof(light));

            _enabled = light.Enabled;
            _intensity = light.Energy;
            _range = light.TextureScale;
            _castShadows = light.ShadowEnabled;
            _transform = light.Transform;

            _rid = RenderingServer.CanvasLightCreate();
            RenderingServer.CanvasLightSetShadowEnabled(_rid, _castShadows);
            RenderingServer.CanvasLightSetShadowFilter(_rid, (RenderingServer.CanvasLightShadowFilter)cvarSystem.GetCVarOrThrow<Light2D.ShadowFilterEnum>(Constants.CVars.SHADOW_FILTER_TYPE).Value);
            RenderingServer.CanvasLightSetShadowSmooth(_rid, cvarSystem.GetCVarOrThrow<float>(Constants.CVars.SHADOW_FILTER_SMOOTH).Value);
            RenderingServer.CanvasLightSetEnergy(_rid, _range);
            RenderingServer.CanvasLightSetTransform(_rid, _transform);

            RenderingServer.CanvasLightSetShadowColor(_rid, light.ShadowColor);
            RenderingServer.CanvasLightSetBlendMode(_rid, (RenderingServer.CanvasLightBlendMode)light.BlendMode);
            RenderingServer.CanvasLightSetTexture(_rid, light.Texture.GetRid());
            RenderingServer.CanvasLightSetTextureOffset(_rid, light.Offset);

            light.QueueFree();
        }

        /// <summary>
        ///
        /// </summary>
        public void Dispose()
        {
            if (!_isDisposed)
            {
                if (_rid.IsValid)
                {
                    RenderingServer.FreeRid(_rid);
                }
            }
            GC.SuppressFinalize(this);
            _isDisposed = true;
        }
    }
}
