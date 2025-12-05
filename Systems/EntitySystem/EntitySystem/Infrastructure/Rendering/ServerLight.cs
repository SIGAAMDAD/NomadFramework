/*
===========================================================================
The Nomad AGPL Source Code
Copyright (C) 2025 Noah Van Til

The Nomad Source Code is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published
by the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

The Nomad Source Code is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with The Nomad Source Code.  If not, see <http://www.gnu.org/licenses/>.

If you have questions concerning this license or the applicable additional
terms, you may contact me via email at nyvantil@gmail.com.
===========================================================================
*/

using Godot;
using NomadCore.Interfaces.EntitySystem;
using System;

namespace NomadCore.Systems.EntitySystem.Infrastructure.Rendering {
	internal class ServerLight : ILight {
		public bool Enabled {
			get => _enabled;
			set {
				if ( _enabled == value ) {
					return;
				}
				_enabled = value;
				RenderingServer.CanvasLightSetEnabled( _lightRid, _enabled );
			}
		}
		private bool _enabled;

		public float Height {
			get => _height;
			set {
				if ( _height == value ) {
					return;
				}
				_height = value;
				RenderingServer.CanvasLightSetHeight( _lightRid, _height );
			}
		}
		private float _height;

		public bool ShadowsEnabled {
			get => _shadowsEnabled;
			set {
				if ( _shadowsEnabled == value ) {
					return;
				}
				_shadowsEnabled = value;
				RenderingServer.CanvasLightSetShadowEnabled( _lightRid, _shadowsEnabled );
			}
		}
		private bool _shadowsEnabled;

		public Light2D.ShadowFilterEnum ShadowFilterType {
			get => _shadowFilterType;
			set {
				if ( _shadowFilterType == value ) {
					return;
				}
				_shadowFilterType = value;
				RenderingServer.CanvasLightSetShadowFilter( _lightRid, (RenderingServer.CanvasLightShadowFilter)_shadowFilterType  );
			}
		}
		private Light2D.ShadowFilterEnum _shadowFilterType;

		public float ShadowFilterSmooth {
			get => _shadowFilterSmooth;
			set {
				if ( _shadowFilterSmooth == value ) {
					return;
				}
				_shadowFilterSmooth = value;
				RenderingServer.CanvasLightSetShadowSmooth( _lightRid, _shadowFilterSmooth );
			}
		}
		private float _shadowFilterSmooth;

		public float Energy {
			get => _energy;
			set {
				if ( _energy == value ) {
					return;
				}
				_energy = value;
				RenderingServer.CanvasLightSetEnergy( _lightRid, _energy );
			}
		}
		private float _energy;

		public Color Color {
			get => _color;
			set {
				if ( _color == value ) {
					return;
				}
				_color = value;
				RenderingServer.CanvasLightSetColor( _lightRid, _color );
			}
		}
		private Color _color;

		protected readonly Rid _lightRid;

		/*
		===============
		ServerLight
		===============
		*/
		public ServerLight( Light2D light, bool directional = false ) {
			_lightRid = RenderingServer.CanvasLightCreate();
			RenderingServer.CanvasLightAttachToCanvas( _lightRid, light.GetCanvas() );
			RenderingServer.CanvasLightSetBlendMode( _lightRid, (RenderingServer.CanvasLightBlendMode)light.BlendMode  );
			RenderingServer.CanvasLightSetColor( _lightRid, light.Color );
			RenderingServer.CanvasLightSetEnabled( _lightRid, light.Enabled );
			RenderingServer.CanvasLightSetEnergy( _lightRid, light.Energy );
			RenderingServer.CanvasLightSetHeight( _lightRid, light.GetHeight() );
			RenderingServer.CanvasLightSetTransform( _lightRid, light.Transform );
			RenderingServer.CanvasLightSetMode( _lightRid, directional ? RenderingServer.CanvasLightMode.Directional : RenderingServer.CanvasLightMode.Point );
			RenderingServer.CanvasLightSetShadowEnabled( _lightRid, light.ShadowEnabled );
			RenderingServer.CanvasLightSetShadowColor( _lightRid, light.ShadowColor );
			RenderingServer.CanvasLightSetShadowFilter( _lightRid, (RenderingServer.CanvasLightShadowFilter)light.ShadowFilter );
			RenderingServer.CanvasLightSetShadowSmooth( _lightRid, light.ShadowFilterSmooth );
		}

		/*
		===============
		Dispose
		===============
		*/
		public void Dispose() {
			if ( _lightRid.IsValid ) {
				RenderingServer.FreeRid( _lightRid );
			}
			GC.SuppressFinalize( this );
		}
	};
};