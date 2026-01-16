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

using System.Runtime.InteropServices;

namespace Nomad.GodotServer.Rendering {
	[StructLayout( LayoutKind.Explicit, Pack = 8, Size = 24 )]
	internal struct EntityVisibility {
		[FieldOffset( 0 )] public int LightMask;
		[FieldOffset( 4 )] public uint VisibilityLayer;
		[FieldOffset( 8 )] public sbyte ZIndex;
		[FieldOffset( 9 )] public bool ShowBehindParent;
	};
};
