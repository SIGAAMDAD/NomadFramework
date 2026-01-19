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
		public Rid CanvasRid => _canvasRid;
		protected readonly Rid _canvasRid;

		public bool Visible {
			get => _visible;
			set {
				_visible = value;
				RenderingServer.CanvasItemSetVisible( _canvasRid, value );
			}
		}
		protected bool _visible = true;

		public Color Modulate {
			get => _modulate;
			set {
				_modulate = value;
				RenderingServer.CanvasItemSetModulate( _canvasRid, value );
			}
		}
		protected Color _modulate;

		public int ZIndex {
			get => _zindex;
			set {
				_zindex = value;
				RenderingServer.CanvasItemSetZIndex( _canvasRid, value );
			}
		}
		protected int _zindex;

		public uint VisibilityLayer {
			get => _visibilityLayer;
			set {
				_visibilityLayer = value;
				RenderingServer.CanvasItemSetVisibilityLayer( _canvasRid, value );
			}
		}
		protected uint _visibilityLayer;

		public int LightMask {
			get => _lightMask;
			set {
				_lightMask = value;
				RenderingServer.CanvasItemSetLightMask( _canvasRid, value );
			}
		}
		protected int _lightMask;

		public Vector2 Position {
			get => _position;
			set => _position = value;
		}
		protected Vector2 _position;

		public Vector2 Scale {
			get => _scale;
			set => _scale = value;
		}
		protected Vector2 _scale;

		public float Rotation {
			get => _rotation;
			set => _rotation = value;
		}
		protected float _rotation;

		/*
		===============
		RenderEntity
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="canvasItem"></param>
		public RenderEntity( CanvasItem canvasItem ) {
			_canvasRid = RenderingServer.CanvasItemCreate();

			var transform = canvasItem.GetTransform();
			_position = transform.Origin;
			_scale = transform.Scale;
			_rotation = transform.Rotation;

			_visibilityLayer = canvasItem.VisibilityLayer;
			_zindex = canvasItem.ZIndex;
			_visible = canvasItem.Visible;
			_modulate = canvasItem.Modulate;

			Rid parent;
			if ( canvasItem.GetParent() is Control control ) {
				parent = control.GetCanvasItem();
			} else if ( canvasItem.GetParent() is Node2D node2D ) {
				parent = node2D.GetCanvasItem();
			} else {
				throw new InvalidCastException();
			}

			RenderingServer.CanvasItemSetParent( _canvasRid, parent );
			RenderingServer.CanvasItemSetDefaultTextureFilter( _canvasRid, ( RenderingServer.CanvasItemTextureFilter )canvasItem.TextureFilter );
			RenderingServer.CanvasItemSetDefaultTextureRepeat( _canvasRid, ( RenderingServer.CanvasItemTextureRepeat )canvasItem.TextureRepeat );
			RenderingServer.CanvasItemSetVisibilityLayer( _canvasRid, _visibilityLayer );
			RenderingServer.CanvasItemSetDrawBehindParent( _canvasRid, canvasItem.ShowBehindParent );
			RenderingServer.CanvasItemSetModulate( _canvasRid, _modulate );
			RenderingServer.CanvasItemSetZIndex( _canvasRid, _zindex );
			RenderingServer.CanvasItemSetVisible( _canvasRid, _visible );
			RenderingServer.CanvasItemSetTransform( _canvasRid, transform );

			canvasItem.QueueFree();
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
			if ( _canvasRid.IsValid ) {
				RenderingServer.FreeRid( _canvasRid );
			}
			GC.SuppressFinalize( this );
		}

		/*
		===============
		Update
		===============
		*/
		/// <summary>
		/// Updates the base render data for a RenderEntity, does not draw anything.
		/// </summary>
		/// <param name="delta"></param>
		public virtual void Update( float delta ) {
			RenderingServer.CanvasItemClear( _canvasRid );
			RenderingServer.CanvasItemSetTransform( _canvasRid, new Transform2D( _rotation, _scale, 0.0f, _position ) );
		}
	};
};
