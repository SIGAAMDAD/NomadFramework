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

namespace Nomad.GodotServer.Rendering.Private.ValueObjects {
	/// <summary>
	///
	/// </summary>
	/// <param name="modulate"></param>
	/// <param name="rid"></param>
	/// <param name="zindex"></param>
	/// <param name="lightmask"></param>
	/// <param name="visibilityLayer"></param>
	/// <param name="position"></param>
	/// <param name="scale"></param>
	/// <param name="rotation"></param>
	/// <param name="visible"></param>
	internal readonly unsafe struct EntityDataDto( Color* modulate, Rid* rid, int *zindex, int *lightmask, uint* visibilityLayer, Vector2* position, Vector2* scale, float* rotation, bool* visible ) {
		public readonly Color* Modulate = modulate;
		public readonly Rid* Rid = rid;
		public readonly int* ZIndex = zindex;
		public readonly int* LightMask = lightmask;
		public readonly uint* VisibilityLayer = visibilityLayer;
		public readonly Vector2* Position = position;
		public readonly Vector2* Scale = scale;
		public readonly float* Rotation = rotation;
		public readonly bool* Visible = visible;
	};
};
