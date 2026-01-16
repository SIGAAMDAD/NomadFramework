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

namespace Nomad.GodotServer.Rendering {
	/*
	===================================================================================

	RenderEntity

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	internal abstract class RenderEntity : IDisposable {
		protected readonly Rid _canvasRid;

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

			RenderingServer.CanvasItemSetDefaultTextureFilter( _canvasRid, (RenderingServer.CanvasItemTextureFilter)canvasItem.TextureFilter );
			RenderingServer.CanvasItemSetDefaultTextureRepeat( _canvasRid, (RenderingServer.CanvasItemTextureRepeat)canvasItem.TextureRepeat );
			RenderingServer.CanvasItemSetVisibilityLayer( _canvasRid, canvasItem.VisibilityLayer );
			RenderingServer.CanvasItemSetDrawBehindParent( _canvasRid, canvasItem.ShowBehindParent );
			RenderingServer.CanvasItemSetZIndex( _canvasRid, canvasItem.ZIndex );

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
		public void Dispose() {
			if ( _canvasRid.IsValid ) {
				RenderingServer.FreeRid( _canvasRid );
			}
		}

		/*
		===============
		Update
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="data"></param>
		public virtual void Update( UpdateData data ) {
			RenderingServer.CanvasItemClear( _canvasRid );
			RenderingServer.CanvasItemSetTransform( _canvasRid, data.Transform );
			RenderingServer.CanvasItemSetModulate( _canvasRid, data.Modulate );
			RenderingServer.CanvasItemSetZIndex( _canvasRid, data.ZIndex );

			RenderingServer.CanvasItemSetVisibilityLayer( _canvasRid, data.VisibilityLayer );
			RenderingServer.CanvasItemSetLightMask( _canvasRid, data.LightMask );

			RenderingServer.CanvasItemSetVisible( _canvasRid, data.Visible );
		}
	};
};
