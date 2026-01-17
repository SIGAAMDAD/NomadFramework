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
using Nomad.GodotServer.Rendering.Interfaces;
using Nomad.GodotServer.Rendering.Private.ValueObjects;

namespace Nomad.GodotServer.Rendering {
	/*
	===================================================================================

	RenderEntity

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	internal unsafe abstract class RenderEntity : IRenderEntity {
		public Rid CanvasRid => *_canvasRid;
		protected readonly Rid *_canvasRid;

		public Color Modulate {
			get => *_modulate;
			set {
				*_modulate = value;
				RenderingServer.CanvasItemSetModulate( *_canvasRid, value );
			}
		}
		private readonly Color* _modulate;

		public int ZIndex {
			get => *_zindex;
			set {
				*_zindex = value;
				RenderingServer.CanvasItemSetZIndex( *_canvasRid, value );
			}
		}
		private readonly int* _zindex;

		public uint VisibilityLayer {
			get => *_visibilityLayer;
			set {
				*_visibilityLayer = value;
				RenderingServer.CanvasItemSetVisibilityLayer( *_canvasRid, value );
			}
		}
		private readonly uint* _visibilityLayer;

		public int LightMask {
			get => *_lightMask;
			set {
				*_lightMask = value;
				RenderingServer.CanvasItemSetLightMask( *_canvasRid, value );
			}
		}
		private readonly int* _lightMask;

		public Vector2 Position {
			get => *_position;
			set => *_position = value;
		}
		private readonly Vector2* _position;

		public Vector2 Scale {
			get => *_scale;
			set => *_scale = value;
		}
		private readonly Vector2* _scale;

		public float Rotation {
			get => *_rotation;
			set => *_rotation = value;
		}
		private readonly float* _rotation;

		public bool Visible {
			get => *_visible;
			set {
				*_visible = value;
				RenderingServer.CanvasItemSetVisible( *_canvasRid, value );
			}
		}
		private readonly bool* _visible;

		/*
		===============
		RenderEntity
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="canvasItem"></param>
		public RenderEntity( EntityDataDto dto, CanvasItem canvasItem ) {
			_position = dto.Position;
			_scale = dto.Scale;
			_rotation = dto.Rotation;
			_visible = dto.Visible;
			_canvasRid = dto.Rid;
			_modulate = dto.Modulate;
			_lightMask = dto.LightMask;
			_visibilityLayer = dto.VisibilityLayer;
			_zindex = dto.ZIndex;

			*_canvasRid = RenderingServer.CanvasItemCreate();

			RenderingServer.CanvasItemSetDefaultTextureFilter( *_canvasRid, ( RenderingServer.CanvasItemTextureFilter )canvasItem.TextureFilter );
			RenderingServer.CanvasItemSetDefaultTextureRepeat( *_canvasRid, ( RenderingServer.CanvasItemTextureRepeat )canvasItem.TextureRepeat );
			RenderingServer.CanvasItemSetVisibilityLayer( *_canvasRid, canvasItem.VisibilityLayer );
			RenderingServer.CanvasItemSetDrawBehindParent( *_canvasRid, canvasItem.ShowBehindParent );
			RenderingServer.CanvasItemSetZIndex( *_canvasRid, canvasItem.ZIndex );

			canvasItem.CallDeferred( CanvasItem.MethodName.QueueFree );
		}

		/*
		===============
		Dispose
		===============
		*/
		/// <summary>
		///
		/// </summary>
		public virtual void Dispose() {
			if ( _canvasRid->IsValid ) {
				RenderingServer.FreeRid( *_canvasRid );
			}
			GC.SuppressFinalize( this );
		}
	};
};
