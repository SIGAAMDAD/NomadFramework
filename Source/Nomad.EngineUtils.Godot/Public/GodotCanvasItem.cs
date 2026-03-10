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
using System.Drawing;
using System.Numerics;

namespace Nomad.EngineUtils
{
    /// <summary>
    ///
    /// </summary>
    public partial class GodotControl<TControl> : GodotGameObject
        where TControl : Godot.Control
    {
        /// <summary>
        ///
        /// </summary>
        public string Name
        {
            get => control.Name;
            set => control.Name = value;
        }

        /// <summary>
        ///
        /// </summary>
        public bool Visible
        {
            get => control.Visible;
            set => control.Visible = value;
        }

        /// <summary>
        ///
        /// </summary>
        public bool Enabled
        {
            get => control.ProcessMode != ProcessModeEnum.Disabled;
            set
            {
                control.ProcessMode = value ? ProcessModeEnum.Inherit : ProcessModeEnum.Disabled;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public Vector2 Position
        {
            get => _position;
            set
            {
                if (_position != value)
                {
                    _position = value;
                    control.Position = new Godot.Vector2(value.X, value.Y);
                }
            }
        }
        private Vector2 _position;

        /// <summary>
        ///
        /// </summary>
        public Vector2 Scale
        {
            get => _scale;
            set
            {
                if (_scale != value)
                {
                    _scale = value;
                    control.Scale = new Godot.Vector2(value.X, value.Y);
                }
            }
        }
        private Vector2 _scale;

        /// <summary>
        ///
        /// </summary>
        public Color Color
        {
            get
            {
                var color = control.Modulate;
                return Color.FromArgb(color.R8, color.G8, color.B8, color.A8);
            }
            set
            {
                control.Modulate = new Godot.Color(value.R, value.G, value.B, value.A);
            }
        }

        protected readonly TControl control;

        /// <summary>
        ///
        /// </summary>
        public GodotControl()
        {
            control = (Godot.Node)this as TControl ?? throw new InvalidCastException();
        }
    }
}
