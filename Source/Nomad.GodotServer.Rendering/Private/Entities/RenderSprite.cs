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
using Nomad.GodotServer.Rendering.Private.ValueObjects;

namespace Nomad.GodotServer.Rendering {
	/*
	===================================================================================

	RenderSprite

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	internal sealed class RenderSprite : RenderEntity {
		private readonly Rid _textureRid;
		private readonly RefCounted _texture;

		/*
		===============
		RenderSprite
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="entityDto"></param>
		/// <param name="sprite"></param>
		public RenderSprite( EntityDataDto entityDto, Sprite2D sprite )
			: base( entityDto, sprite )
		{
			_texture = sprite.Texture;
			_textureRid = sprite.Texture.GetRid();
			_texture.Reference();
		}

		/*
		===============
		Dispose
		===============
		*/
		/// <summary>
		///
		/// </summary>
		public override void Dispose() {
			_texture.Dispose();
			base.Dispose();
		}
	};
};
