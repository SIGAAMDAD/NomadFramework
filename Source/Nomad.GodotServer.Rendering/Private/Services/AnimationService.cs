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

	AnimationService

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	internal sealed class AnimationService : IDisposable {
		private readonly struct AnimationData() {
			public readonly int[] FrameCounts = new int[ EntityService.MAX_ENTITIES ];
			public readonly bool[] Loops = new bool[ EntityService.MAX_ENTITIES ];
			public readonly Rid[][] TextureRids = new Rid[ EntityService.MAX_ENTITIES ][];
			public readonly Rect2[][] TextureRegions = new Rect2[ EntityService.MAX_ENTITIES ][];
			public readonly float[][] FrameDurations = new float[ EntityService.MAX_ENTITIES ][];
		};

		private int _animationCount;

		/*
		===============
		Dispose
		===============
		*/
		/// <summary>
		///
		/// </summary>
		public void Dispose() {
		}

		/*
		===============
		Update
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="delta"></param>
		public void Update( float delta ) {
			int count = _animationCount;
		}
	};
};
