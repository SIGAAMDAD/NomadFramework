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

namespace Nomad.GodotServer.Rendering {
	internal readonly unsafe struct AnimationDataDto( int* currentFrame, float* frameTimer, float* speedScale, bool* playing ) {
		public readonly int* CurrentFrame = currentFrame;
		public readonly float* FrameTimer = frameTimer;
		public readonly float* SpeedScale = speedScale;
		public readonly bool* Playing = playing;
	};
};
